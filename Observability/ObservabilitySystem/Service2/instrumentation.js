const { getNodeAutoInstrumentations } = require('@opentelemetry/auto-instrumentations-node');
const { JaegerExporter } = require('@opentelemetry/exporter-jaeger');
const { PrometheusExporter } = require('@opentelemetry/exporter-prometheus');

// Configure Jaeger exporter
const jaegerExporter = new JaegerExporter({
  endpoint: process.env.JAEGER_ENDPOINT || 'http://jaeger:14268/api/traces',
});

// Configure Prometheus exporter
const prometheusExporter = new PrometheusExporter({
  port: 9464, // Different port to avoid conflicts
}, () => {
  console.log('Prometheus scrape endpoint: http://localhost:9464/metrics');
});

// Auto instrumentations
const instrumentation = getNodeAutoInstrumentations({
  '@opentelemetry/instrumentation-fs': {
    enabled: false,
  },
  '@opentelemetry/instrumentation-http': {
    enabled: true,
  },
  '@opentelemetry/instrumentation-express': {
    enabled: true,
  },
  '@opentelemetry/instrumentation-pg': {
    enabled: true,
  },
});

module.exports = {
  instrumentation,
  jaegerExporter,
  prometheusExporter,
};
