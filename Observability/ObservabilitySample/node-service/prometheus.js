const promClient = require('prom-client');

// Initialize default metrics collection
const collectDefaultMetrics = promClient.collectDefaultMetrics;
collectDefaultMetrics();

// COUNTER: Monotonically increasing values, cumulative
const httpRequestCounter = new promClient.Counter({
  name: 'http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status']
});

// COUNTER: Monotonically increasing values, cumulative
const errorCounter = new promClient.Counter({
  name: 'application_errors_total',
  help: 'Total number of application errors',
  labelNames: ['error_type']
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
  labelNames: ['method', 'route']
});

class PrometheusService {
  constructor() {
    // Start heartbeat interval
    setInterval(() => {
      heartbeatGauge.set(Date.now() / 1000);
    }, 5000);
  }

  recordRequest(method, route, partner, status, duration) {
    requestDurationHistogram.observe({ method, route }, duration);
    httpRequestCounter.inc({ method, route, status });
  }

  recordError(partner, errorType) {
    errorCounter.inc({ error_type: errorType });
  }

  updateActivePartners(count) {
    activePartnersGauge.set(count);
  }

  async getMetrics() {
    return await promClient.register.metrics();
  }

  getContentType() {
    return promClient.register.contentType;
  }
}

module.exports = new PrometheusService();
