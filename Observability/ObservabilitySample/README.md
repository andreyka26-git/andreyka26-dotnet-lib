# Observability Sample

Complete observability stack with Grafana, Loki, Promtail, and Prometheus.

## Quick Start

1. **Start the full stack:**
   ```powershell
   .\Start-ObservabilityStack.ps1
   ```

2. **Run load tests:**
   ```powershell
   cd load-test
   .\Run-LoadTest.ps1
   ```

## What's Included

- **Node.js Service** with Express, Prometheus metrics, and structured logging
- **Grafana Dashboard** with auto-import and responsive metrics
- **Loki Log Aggregation** filtering only application logs
- **Prometheus Metrics** collection with alerting rules
- **Cross-Platform Scripts** for Windows, Mac, and Linux
- **K6 Load Testing** with 50 RPS capability

## Key Features

- ✅ **Container ID Management**: Template-based system survives container recreation
- ✅ **Cross-Platform**: PowerShell (Windows) and bash (Mac/Linux) scripts
- ✅ **Responsive Metrics**: Rate-based calculations for real-time feedback
- ✅ **Log Filtering**: Only application logs, no system noise
- ✅ **Auto-Import**: Dashboard loads automatically on Grafana startup
- ✅ **Load Testing**: Comprehensive K6 framework with unique partner generation

## Access Points

- **Grafana**: http://localhost:3001 (admin/admin)
- **Node Service**: http://localhost:3000
- **Prometheus**: http://localhost:9090
- **Loki**: http://localhost:3100

## Load Testing

See `load-test/README.md` for detailed load testing instructions.

The load test generates:
- 50 requests per second
- Unique partner IDs for each request
- 80% success / 20% failure ratio
- ~3000 total requests in 60 seconds
