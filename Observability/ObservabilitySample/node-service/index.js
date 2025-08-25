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
  let error = null;

  try {
    next();
    
  } catch (err) {
    error = err;
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

app.get('/infinite-loop', requestMetricsMiddleware, async (req, res) => {

  let arr = []
  try {
    azureService.logInfo('Starting infinite loop', { partner });
    console.log(`[${new Date().toISOString()}] Starting infinite loop for partner: ${partner}`);

    let ind = 0
    while (true) {
      arr.push(ind++);
      await delay(100);
    }

  } catch (err) {
    res.status(500).json({
      error: 'Internal Server Error',
      partner: partner,
      message: err.message
    });
  }
});

app.get('/call', requestMetricsMiddleware, async (req, res) => {
  const partner = req.query.partner || 'unknown';
  const shouldError = req.query.error === 'true';
  
  try {
    // Track partner usage
    if (!partnerStatistics[partner]) {
      partnerStatistics[partner] = 0;
    }
    partnerStatistics[partner]++;
    
    azureService.logInfo('Processing request for partner', { partner });
    console.log(`[${new Date().toISOString()}] Processing request for partner: ${partner}`);
    
    // Simulate processing time with random delay (0-2000ms for variety)
    const processingDelay = Math.random() * 2000;
    const isSlowRequest = processingDelay > 1000; // Consider >1s as slow
    
    // Only throw error if error=true query parameter is passed
    if (shouldError) {
      throw new Error('Error triggered by error=true query parameter');
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
    console.log(`[${new Date().toISOString()}] Error processing request for partner ${partner}: ${err.message}`);
    
    res.status(500).json({ 
      error: 'Internal Server Error',
      partner: partner,
      message: err.message
    });
  }
});

app.get('/nocall', requestMetricsMiddleware, async (req, res) => {
  const partner = req.query.partner || 'unknown';
  
  azureService.logInfo('Removing partner', { partner });
  console.log(`[${new Date().toISOString()}] Removing partner: ${partner}`);
  
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
});

app.listen(port, () => {
  azureService.logInfo(`Node.js service listening on port ${port}`);
  console.log(`[${new Date().toISOString()}] Node.js service listening on port ${port}`);
});
