# Azure Application Insights Integration Summary

## What Was Added

### 1. New Endpoint: `/azure/call`
- **Route**: `GET /azure/call?partner={partner_name}`
- **Purpose**: Demonstrates Application Insights integration for metrics and logging
- **Features**:
  - Request per second counter tracking
  - Latency measurement and reporting
  - Error tracking and logging
  - Custom properties (partner, endpoint)

### 2. Application Insights SDK Integration
- **Package**: `applicationinsights@^2.9.5`
- **Auto-collection**: Requests, performance, exceptions, dependencies, console logs
- **Custom telemetry**: Metrics and traces with custom properties

### 3. Metrics Tracked
| Metric Name | Type | Description | Properties |
|-------------|------|-------------|------------|
| `azure_requests_per_second` | Counter | Counts requests to endpoint | partner, endpoint |
| `azure_request_latency_ms` | Metric | Response time in milliseconds | partner, endpoint |
| `azure_request_errors` | Counter | Error count | partner, endpoint, error_type |

### 4. Logs Sent
- **Information logs**: Successful request processing
- **Exception logs**: Error details with stack traces
- **Custom properties**: Partner, endpoint, timestamp, latency

### 5. Configuration Files Updated
- `package.json`: Added Application Insights dependency
- `index.js`: Added Application Insights configuration and new endpoint
- `Dockerfile`: Added environment variable placeholder
- `docker-compose.yml`: Added Application Insights connection string environment
- `.env`: Created for easy configuration

### 6. Documentation Created
- `APPLICATION_INSIGHTS_SETUP.md`: Complete setup instructions
- `test-azure-endpoint.ps1`: PowerShell test script

## How to Use

### Without Application Insights (Local Testing)
```bash
# The endpoint works without Application Insights configured
curl "http://localhost:3000/azure/call?partner=test-partner"
```

### With Application Insights (Full Monitoring)
1. Get your Application Insights connection string from Azure Portal
2. Set it in the `.env` file:
   ```
   APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=...;IngestionEndpoint=...
   ```
3. Rebuild and restart:
   ```bash
   docker compose up -d --build
   ```
4. Make requests and view data in Azure Portal

## Monitoring Capabilities

### Azure Portal Views
1. **Metrics**: Custom metrics with partner and endpoint filtering
2. **Logs**: KQL queries for detailed log analysis
3. **Dashboards**: Visual representation of request patterns
4. **Alerts**: Automated notifications on thresholds

### Sample KQL Queries
```kql
// View all Azure endpoint requests
traces
| where customDimensions.endpoint == "/azure/call"
| order by timestamp desc

// View errors by partner
exceptions
| where customDimensions.partner != ""
| summarize count() by tostring(customDimensions.partner)
```

## Testing
Run the included test script to generate sample data:
```powershell
./test-azure-endpoint.ps1
```

This implementation provides comprehensive monitoring capabilities while maintaining compatibility with your existing Prometheus/Grafana observability stack.
