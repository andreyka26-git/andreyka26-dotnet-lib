const express = require('express');
const promClient = require('prom-client');
const os = require('os');
const winston = require('winston');

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

// COUNTER: Monotonically increasing values
const httpRequestCounter = new promClient.Counter({
  name: 'http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status', 'partner']
});

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

// Note: Memory usage gauges are provided by default metrics (process_resident_memory_bytes, etc.)
// Removed custom memory gauge to avoid duplication

// HISTOGRAM: Distribution of observed values (better than gauge for latency)
const requestDurationHistogram = new promClient.Histogram({
  name: 'http_request_duration_seconds',
  help: 'Duration of HTTP requests in seconds',
  buckets: [0.001, 0.01, 0.1, 0.5, 1, 2, 5, 10], // 1ms, 10ms, 100ms, 500ms, 1s, 2s, 5s, 10s
  labelNames: ['method', 'route', 'partner']
});

// Removed Summary as Histogram is generally better for aggregation across instances

const partnerStatistics = {};

// Helper function to record metrics and avoid code duplication
function recordRequestMetrics(method, route, partner, status, duration) {
  requestDurationHistogram.observe({ method, route, partner }, duration);
  httpRequestCounter.inc({ method, route, status, partner });
}

// Async delay function to replace setTimeout
function delay(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

// Update active partners gauge periodically (using partnerStatistics as suggested)
setInterval(() => {
  // Update active partners gauge based on partnerStatistics
  activePartnersGauge.set(Object.keys(partnerStatistics).length);
}, 5000);


app.get('/metrics', async (req, res) => {
  res.set('Content-Type', promClient.register.contentType);
  res.end(await promClient.register.metrics());
});


app.get('/', async (req, res) => {
  const partner = req.query.partner || 'unknown';
  const startTime = Date.now();
  
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
    const duration = (Date.now() - startTime) / 1000;
    recordRequestMetrics(req.method, '/', partner, '200', duration);
    
    res.json({ 
      success: true, 
      message: `Success ${partnerStatistics[partner]}`,
      partner: partner,
      processingTime: `${duration.toFixed(3)}s`,
      type: isSlowRequest ? 'slow_request' : 'fast_request'
    });
    
  } catch (error) {
    // Error handling with proper metrics recording
    const duration = (Date.now() - startTime) / 1000;
    
    errorCounter.inc({ partner, error_type: 'processing_error' });
    logger.error('Error processing request', { partner, error: error.message });
    
    recordRequestMetrics(req.method, '/', partner, '500', duration);
    
    res.status(500).json({ 
      error: 'Internal Server Error',
      partner: partner,
      message: error.message
    });
  }
});


app.listen(port, () => {
  logger.info(`Node.js service listening on port ${port}`);
});
