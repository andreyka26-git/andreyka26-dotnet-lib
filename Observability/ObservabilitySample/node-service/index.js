const express = require('express');
const promClient = require('prom-client');
const os = require('os');
const winston = require('winston');
const appInsights = require('applicationinsights');

// Configure Application Insights
const connectionString = process.env.APPLICATIONINSIGHTS_CONNECTION_STRING;
if (connectionString) {
  appInsights.setup(connectionString)
    .setAutoCollectRequests(true)
    .setAutoCollectPerformance(true)
    .setAutoCollectExceptions(true)
    .setAutoCollectDependencies(true)
    .setAutoCollectConsole(true)
    .start();
  
  console.log('Application Insights configured successfully');
} else {
  console.log('Application Insights connection string not found. Set APPLICATIONINSIGHTS_CONNECTION_STRING environment variable.');
}

const app = express();
const port = process.env.PORT || 3000;

const logger = winston.createLogger({
  level: 'info',
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.json()
  ),
  transports: [new winston.transports.Console()]
});

const collectDefaultMetrics = promClient.collectDefaultMetrics;
collectDefaultMetrics();

// COUNTER: Monotonically increasing values, cumulative
const httpRequestCounter = new promClient.Counter({
  name: 'http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status', 'partner']
});

// COUNTER: Monotonically increasing values, cumulative
const errorCounter = new promClient.Counter({
  name: 'application_errors_total',
  help: 'Total number of application errors',
  labelNames: ['partner', 'error_type']
});

// GAUGE: Values that can go up and down
const activePartnersGauge = new promClient.Gauge({
  name: 'active_partners_current',
  help: 'Current number of active partners',
});

// GAUGE: Synthetic heartbeat to ensure continuous data
const heartbeatGauge = new promClient.Gauge({
  name: 'service_heartbeat',
  help: 'Service heartbeat timestamp',
});

const requestDurationHistogram = new promClient.Histogram({
  name: 'http_request_duration_seconds',
  help: 'Duration of HTTP requests in seconds',
  // 1ms, 10ms, 100ms, 500ms, 1s, 2s, 5s, 10s
  buckets: [0.001, 0.01, 0.1, 0.5, 1, 2, 5, 10], 
  labelNames: ['method', 'route', 'partner']
});

const partnerStatistics = {};

function recordRequestMetrics(method, route, partner, status, duration) {
  requestDurationHistogram.observe({ method, route, partner }, duration);
  httpRequestCounter.inc({ method, route, status, partner });
}

function delay(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

setInterval(() => {
  activePartnersGauge.set(Object.keys(partnerStatistics).length);
  // Update heartbeat to current timestamp to ensure continuous data
  heartbeatGauge.set(Date.now() / 1000);
}, 5000);


app.get('/metrics', async (req, res) => {
  res.set('Content-Type', promClient.register.contentType);
  res.end(await promClient.register.metrics());
});

app.get('/azure/call', async (req, res) => {
  const partner = req.query.partner || 'unknown';
  const startTime = Date.now();
  let status = 200;
  
  try {
    // Custom telemetry for Application Insights
    const client = appInsights.defaultClient;
    
    // Track requests per second counter
    if (client) {
      client.trackMetric({
        name: 'azure_requests_per_second',
        value: 1,
        properties: {
          partner: partner,
          endpoint: '/azure/call'
        }
      });
    }
    
    // Log to Application Insights
    if (client) {
      client.trackTrace({
        message: `Processing Azure call request for partner: ${partner}`,
        severity: appInsights.Contracts.SeverityLevel.Information,
        properties: {
          partner: partner,
          endpoint: '/azure/call',
          timestamp: new Date().toISOString()
        }
      });
    }
    
    // Simulate processing time (100-1500ms)
    const processingDelay = 100 + Math.random() * 1400;
    await delay(processingDelay);
    
    // Simulate 3% error rate
    if (Math.random() < 0.03) {
      throw new Error('Azure service temporarily unavailable');
    }
    
    const responseTime = Date.now() - startTime;
    
    // Track latency to Application Insights
    if (client) {
      client.trackMetric({
        name: 'azure_request_latency_ms',
        value: responseTime,
        properties: {
          partner: partner,
          endpoint: '/azure/call'
        }
      });
    }
    
    res.json({
      success: true,
      message: `Azure call processed successfully for ${partner}`,
      partner: partner,
      latency: `${responseTime}ms`,
      timestamp: new Date().toISOString()
    });
    
  } catch (err) {
    status = 500;
    const responseTime = Date.now() - startTime;
    
    // Track error metrics and logs
    const client = appInsights.defaultClient;
    if (client) {
      client.trackException({
        exception: err,
        properties: {
          partner: partner,
          endpoint: '/azure/call',
          latency: responseTime
        }
      });
      
      client.trackMetric({
        name: 'azure_request_errors',
        value: 1,
        properties: {
          partner: partner,
          endpoint: '/azure/call',
          error_type: 'processing_error'
        }
      });
    }
    
    res.status(500).json({
      error: 'Azure service error',
      partner: partner,
      message: err.message,
      timestamp: new Date().toISOString()
    });
  }
});


app.get('/call', async (req, res) => {
  const partner = req.query.partner || 'unknown';
  const startTime = Date.now();
  let status = '200';
  let error = null;
  
  try {
    // Track partner usage
    if (!partnerStatistics[partner]) {
      partnerStatistics[partner] = 0;
    }
    partnerStatistics[partner]++;
    
    logger.info('Processing request for partner', { partner });
    
    // Simulate processing time with random delay (0-2000ms for variety)
    const processingDelay = Math.random() * 2000;
    const isSlowRequest = processingDelay > 1000; // Consider >1s as slow
    
    // Simulate 5% error rate - throw actual error
    if (Math.random() < 0.05) {
      throw new Error('Random processing error occurred');
    }
    
    // Simulate processing delay
    await delay(processingDelay);
    
    // Success response
    res.json({ 
      success: true, 
      message: `Success ${partnerStatistics[partner]}`,
      partner: partner,
      processingTime: `${((Date.now() - startTime) / 1000).toFixed(3)}s`,
      type: isSlowRequest ? 'slow_request' : 'fast_request'
    });
    
  } catch (err) {
    // Error handling
    error = err;
    status = '500';
    
    errorCounter.inc({ partner, error_type: 'processing_error' });
    logger.error('Error processing request', { partner, error: err.message });
    
    res.status(500).json({ 
      error: 'Internal Server Error',
      partner: partner,
      message: err.message
    });
  } finally {
    // Always record metrics regardless of success or failure
    const duration = (Date.now() - startTime) / 1000;
    recordRequestMetrics(req.method, '/call', partner, status, duration);
  }
});

app.get('/nocall', async (req, res) => {
  const partner = req.query.partner || 'unknown';
  const startTime = Date.now();
  let status = '200';
  let error = null;
  
  try {
    logger.info('Removing partner', { partner });
    
    // Remove partner from statistics
    if (partnerStatistics[partner]) {
      delete partnerStatistics[partner];
      
      res.json({ 
        success: true, 
        message: `Partner ${partner} removed successfully`,
        partner: partner
      });
    } else {
      res.status(404).json({ 
        success: false, 
        message: `Partner ${partner} not found`,
        partner: partner
      });
      status = '404';
    }
    
  } catch (err) {
    // Error handling
    error = err;
    status = '500';
    
    errorCounter.inc({ partner, error_type: 'partner_removal_error' });
    logger.error('Error removing partner', { partner, error: err.message });
    
    res.status(500).json({ 
      error: 'Internal Server Error',
      partner: partner,
      message: err.message
    });
  } finally {
    // Always record metrics regardless of success or failure
    const duration = (Date.now() - startTime) / 1000;
    recordRequestMetrics(req.method, '/nocall', partner, status, duration);
  }
});


app.listen(port, () => {
  logger.info(`Node.js service listening on port ${port}`);
});
