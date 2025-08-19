# Quick Start Guide

## Prerequisites
- Docker Desktop installed and running
- At least 4GB of available RAM
- Ports 3000, 3001, 8080, 8081, 5432, 9090, 9093, 3100 must be available

## Start the Demo

```bash
docker-compose up -d
```

## Access the System

1. **Demo Application**: http://localhost:3001
   - Create orders and see the system in action

2. **Grafana Dashboards**: http://localhost:3000
   - Username: admin
   - Password: admin

3. **Prometheus**: http://localhost:9090
   - View metrics and targets

4. **Service Metrics**:
   - Service1: http://localhost:8080/metrics
   - Service2: http://localhost:8081/metrics

## Stop the Demo
```bash
docker-compose down -v
```

## Troubleshooting
- If services fail to start, check that all required ports are available
- Increase Docker Desktop memory allocation if needed
- Wait 2-3 minutes for all services to fully initialize
