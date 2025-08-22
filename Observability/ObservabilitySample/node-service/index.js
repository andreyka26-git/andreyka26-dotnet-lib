const express = require('express');
const prometheusService = require('./prometheus');
const azureService = require('./azure');

const app = express();
const port = process.env.PORT || 3000;

const partnerStatistics = {};

function delay(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

// Update active partners count every 5 seconds
setInterval(() => {
  prometheusService.updateActivePartners(Object.keys(partnerStatistics).length);
}, 5000);

// Request middleware for metrics and logging
function requestMetricsMiddleware(req, res, next) {
  const startTime = Date.now();
  const partner = req.query.partner || 'unknown';
  const route = req.route ? req.route.path : req.path;
  let status = '200';
  let error = null;

  try {
    next();
    
  } catch (err) {
    error = err;
    status = '500';
    throw err;
  } finally {
    // This will run after the route handler completes
    res.on('finish', () => {
      const duration = (Date.now() - startTime) / 1000;
      const finalStatus = res.statusCode.toString();
      const latency = Date.now() - startTime;
      
      // Report to azure
      azureService.trackRequest(route, partner);

      // Report to Prometheus
      prometheusService.recordRequest(req.method, route, partner, finalStatus, duration);
      
      // Report to Azure
      azureService.trackLatency(route, partner, latency);
      
      if (error || res.statusCode >= 400) {
        prometheusService.recordError(partner, 'processing_error');
        if (error) {
          azureService.trackError(route, partner, error, latency);
        }
      }
    });
  }
}

app.get('/metrics', async (req, res) => {
  res.set('Content-Type', prometheusService.getContentType());
  res.end(await prometheusService.getMetrics());
});

app.get('/call', requestMetricsMiddleware, async (req, res) => {
  const partner = req.query.partner || 'unknown';
  
  try {
    // Track partner usage
    if (!partnerStatistics[partner]) {
      partnerStatistics[partner] = 0;
    }
    partnerStatistics[partner]++;
    
    azureService.logInfo('Processing request for partner', { partner });
    
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
      processingTime: `${(processingDelay / 1000).toFixed(3)}s`,
      type: isSlowRequest ? 'slow_request' : 'fast_request'
    });
    
  } catch (err) {
    azureService.logError('Error processing request', { partner, error: err.message });
    
    res.status(500).json({ 
      error: 'Internal Server Error',
      partner: partner,
      message: err.message
    });
  }
});

app.get('/nocall', requestMetricsMiddleware, async (req, res) => {
  const partner = req.query.partner || 'unknown';
  
  try {
    azureService.logInfo('Removing partner', { partner });
    
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
    }
    
  } catch (err) {
    azureService.logError('Error removing partner', { partner, error: err.message });
    
    res.status(500).json({ 
      error: 'Internal Server Error',
      partner: partner,
      message: err.message
    });
  }
});

app.get('/error', requestMetricsMiddleware, async (req, res) => {
  const partner = req.query.partner || 'unknown';
  
  try {
    azureService.logInfo('Intentionally triggering error for testing', { partner });
    
    // Always throw an error for testing purposes
    throw new Error('This endpoint always throws an error for testing metrics and monitoring');
    
  } catch (err) {
    azureService.logError('Intentional error endpoint triggered', { partner, error: err.message });
    
    res.status(500).json({ 
      error: 'Intentional Server Error',
      partner: partner,
      message: err.message,
      timestamp: new Date().toISOString()
    });
  }
});


app.listen(port, () => {
  azureService.logInfo(`Node.js service listening on port ${port}`);
});
