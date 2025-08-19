const promClient = require('prom-client');

// Create a Registry
const register = new promClient.Registry();

// Add default metrics
promClient.collectDefaultMetrics({
  register,
  prefix: 'service2_',
});

// Custom metrics for database operations
const databaseQueryDuration = new promClient.Histogram({
  name: 'service2_database_query_duration_seconds',
  help: 'Database query duration in seconds',
  labelNames: ['operation', 'status'],
  buckets: [0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1, 2, 5]
});

const databaseOperationsTotal = new promClient.Counter({
  name: 'service2_database_operations_total',
  help: 'Total number of database operations',
  labelNames: ['operation', 'status']
});

const httpRequestsTotal = new promClient.Counter({
  name: 'service2_http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status_code']
});

const httpRequestDuration = new promClient.Histogram({
  name: 'service2_http_request_duration_seconds',
  help: 'HTTP request duration in seconds',
  labelNames: ['method', 'route', 'status_code'],
  buckets: [0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1, 2, 5]
});

// Register custom metrics
register.registerMetric(databaseQueryDuration);
register.registerMetric(databaseOperationsTotal);
register.registerMetric(httpRequestsTotal);
register.registerMetric(httpRequestDuration);

// Override the default register
promClient.register = register;

module.exports = {
  register,
  databaseQueryDuration,
  databaseOperationsTotal,
  httpRequestsTotal,
  httpRequestDuration
};
