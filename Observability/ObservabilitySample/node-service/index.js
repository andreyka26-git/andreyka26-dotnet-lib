const express = require('express');
const promClient = require('prom-client');
const os = require('os');
const winston = require('winston');

const app = express();
const port = process.env.PORT || 3000;

// Winston logger setup
const logger = winston.createLogger({
  level: 'info',
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.json()
  ),
  transports: [new winston.transports.Console()]
});

// Prometheus metrics
const collectDefaultMetrics = promClient.collectDefaultMetrics;
collectDefaultMetrics();

const httpRequestCounter = new promClient.Counter({
  name: 'http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status']
});

const customGauge = new promClient.Gauge({
  name: 'custom_gauge',
  help: 'A custom gauge metric'
});

const customHistogram = new promClient.Histogram({
  name: 'custom_histogram',
  help: 'A custom histogram metric',
  buckets: [0.1, 0.5, 1, 2, 5]
});

// System metrics endpoint
app.get('/system', (req, res) => {
  const memoryUsage = process.memoryUsage();
  const cpuLoad = os.loadavg();
  const totalMem = os.totalmem();
  const freeMem = os.freemem();
  res.json({
    memoryUsage,
    cpuLoad,
    totalMem,
    freeMem
  });
});

// Metrics endpoint
app.get('/metrics', async (req, res) => {
  res.set('Content-Type', promClient.register.contentType);
  res.end(await promClient.register.metrics());
});

// Main endpoint
app.get('/', (req, res) => {
  const start = Date.now();
  logger.info('Info log from / endpoint');
  logger.warn('Warning log from / endpoint');
  logger.error('Error log from / endpoint');

  customGauge.set(Math.random() * 100);
  const duration = (Math.random() * 2) + 0.1;
  customHistogram.observe(duration);

  httpRequestCounter.inc({ method: req.method, route: '/', status: 200 });
  res.send('Hello from Node.js service with observability!');
});

app.listen(port, () => {
  logger.info(`Node.js service listening on port ${port}`);
});
