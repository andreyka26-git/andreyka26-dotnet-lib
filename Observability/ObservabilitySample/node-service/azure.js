const appInsights = require('applicationinsights');
const winston = require('winston');

class AzureService {
  constructor() {
    this.client = null;
    this.logger = winston.createLogger({
      level: 'info',
      format: winston.format.combine(
        winston.format.timestamp(),
        winston.format.json()
      ),
      transports: [new winston.transports.Console()]
    });

    this.initializeAppInsights();
  }

  initializeAppInsights() {
    const connectionString = process.env.APPLICATIONINSIGHTS_CONNECTION_STRING;
    if (connectionString) {
      appInsights.setup(connectionString)
        .setAutoCollectRequests(true)
        .setAutoCollectPerformance(true)
        .setAutoCollectExceptions(true)
        .setAutoCollectDependencies(true)
        .setAutoCollectConsole(true)
        .start();
      
      this.client = appInsights.defaultClient;
      console.log('Application Insights configured successfully');
    } else {
      console.log('Application Insights connection string not found. Set APPLICATIONINSIGHTS_CONNECTION_STRING environment variable.');
    }
  }

  trackRequest(route, partner) {
    if (this.client) {
      this.client.trackMetric({
        name: 'azure_requests_per_second',
        value: 1,
        properties: {
          partner: partner,
          endpoint: route
        }
      });

      this.client.trackTrace({
        message: `Processing request for partner: ${partner} on route: ${route}`,
        severity: appInsights.Contracts.SeverityLevel.Information,
        properties: {
          partner: partner,
          endpoint: route,
          timestamp: new Date().toISOString()
        }
      });
    }
  }

  trackLatency(route, partner, latency) {
    if (this.client) {
      this.client.trackMetric({
        name: 'azure_request_latency_ms',
        value: latency,
        properties: {
          partner: partner,
          endpoint: route
        }
      });
    }
  }

  trackError(route, partner, error, latency) {
    if (this.client) {
      this.client.trackException({
        exception: error,
        properties: {
          partner: partner,
          endpoint: route,
          latency: latency
        }
      });
      
      this.client.trackMetric({
        name: 'azure_request_errors',
        value: 1,
        properties: {
          partner: partner,
          endpoint: route,
          error_type: 'processing_error'
        }
      });
    }
  }

  logInfo(message, metadata) {
    this.logger.info(message, metadata);
  }

  logError(message, metadata) {
    this.logger.error(message, metadata);
  }
}

module.exports = new AzureService();
