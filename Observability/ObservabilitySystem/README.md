# Observability Demo System

A comprehensive microservices observability demo showcasing metrics, logging, tracing, and alerting best practices using .NET, React, and a full observability stack.

## 🏗️ System Architecture

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Client1   │───▶│  Service1   │───▶│  Service2   │───▶│ PostgreSQL  │
│   (React)   │    │  (Orders)   │    │  (Events)   │    │ (Database)  │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                            │                │                     │
                            ▼                ▼                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                    Observability Stack                                   │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐       │
│  │ Prometheus  │ │   Grafana   │ │    Loki     │ │ Alertmanager│       │
│  │ (Metrics)   │ │(Dashboards) │ │   (Logs)    │ │  (Alerts)   │       │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘       │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐       │
│  │ Node Exp.   │ │  cAdvisor   │ │  Promtail   │ │Postgres Exp.│       │
│  │(Host Metrics)│ │(Container)  │ │(Log Ship.)  │ │(DB Metrics) │       │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘       │
└─────────────────────────────────────────────────────────────────────────┘
```

### Components

**Application Services:**
- **Client1** (React): Frontend application with order management UI
- **Service1** (Orders API): .NET 9 business logic service that handles orders and calls Service2
- **Service2** (Events API): Node.js data persistence service with PostgreSQL integration

**Database:**
- **PostgreSQL**: Stores event data with health monitoring

**Observability Stack:**
- **Prometheus**: Metrics collection and storage
- **Grafana**: Visualization dashboards
- **Loki**: Log aggregation
- **Promtail**: Log shipping
- **Alertmanager**: Alert handling and routing
- **Node Exporter**: Host system metrics
- **cAdvisor**: Container metrics
- **Postgres Exporter**: Database metrics

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- At least 4GB RAM available for containers
- Ports 3000, 3001, 8080, 8081, 5432, 9090, 9093, 3100 available

### Running the System

1. **Clone and navigate to the project:**
   ```bash
   cd c:\Projects\home\andreyka26-dotnet-lib\Observability\ObservabilitySystem
   ```

2. **Start all services:**
   ```bash
   docker-compose up -d
   ```

3. **Wait for all services to start (2-3 minutes):**
   ```bash
   docker-compose ps
   ```

4. **Access the applications:**
   - **Demo Application**: http://localhost:3001
   - **Grafana Dashboards**: http://localhost:3000 (admin/admin)
   - **Prometheus**: http://localhost:9090
   - **Alertmanager**: http://localhost:9093

### First Steps

1. **Create some test data:**
   - Go to http://localhost:3001
   - Create several orders with different types (Electronics, Books, Clothing)
   - Refresh the orders list multiple times

2. **View observability data:**
   - Open Grafana at http://localhost:3000
   - Login with admin/admin
   - Explore the pre-configured dashboards
   - Check Prometheus targets at http://localhost:9090/targets

## 📊 Observability Features

### 1. Metrics (Prometheus + Grafana)

**Service-Level Metrics:**
- Request latency (histograms)
- Request rate and error rate
- Custom business metrics:
  - `service1_requests_total`: Total requests to Service1
  - `service1_partner_service_calls_total`: Calls from Service1 to Service2
  - `service2_database_operations_total`: Database operations in Service2
  - `service2_database_query_duration_seconds`: Database query latency

**Infrastructure Metrics:**
- CPU, Memory, Disk usage (Node Exporter)
- Container metrics (cAdvisor)
- PostgreSQL metrics (Postgres Exporter)

**Endpoints:**
- Service1 metrics: http://localhost:8080/metrics
- Service2 metrics: http://localhost:8081/metrics
- Prometheus UI: http://localhost:9090

### 2. Logging (Loki + Promtail + Grafana)

**Features:**
- Centralized log aggregation
- Request ID tracing across all services
- Structured logging with context
- Service identification in logs

**Log Flow:**
1. Applications write structured logs to stdout
2. Docker captures container logs
3. Promtail ships logs to Loki
4. Grafana queries logs from Loki

**Sample Log Queries:**
```
{container_name="observability-service1"} |= "RequestId"
{container_name=~"observability-service.*"} |= "Error"
```

### 3. Tracing (OpenTelemetry)

**Implementation:**
- Request correlation via Request IDs
- Cross-service tracing headers
- Activity correlation in logs
- Distributed trace correlation

**Trace Flow:**
1. Client generates request ID
2. Service1 receives and logs request ID
3. Service1 passes request ID to Service2
4. Service2 uses same request ID for database operations
5. All logs include the same request ID for correlation

### 4. Alerting (Prometheus Alertmanager)

**Pre-configured Alerts:**
- High request latency (>500ms)
- High error rate (>10%)
- Service down
- High CPU usage (>80%)
- High memory usage (>85%)
- Database connection failures

**Alert Configuration:**
- Rules defined in `observability/prometheus/alert_rules.yml`
- Alertmanager config in `observability/alertmanager/alertmanager.yml`
- Alerts visible in Grafana and Alertmanager UI

## 🛠️ Development

### Project Structure
```
ObservabilitySystem/
├── Service1/                 # Orders API (.NET 9)
│   ├── Controllers/          # API endpoints
│   ├── Services/            # HTTP clients
│   ├── Middleware/          # Request tracking
│   └── Dockerfile
├── Service2/                 # Events API (Node.js)
│   ├── server.js            # Main application
│   ├── logger.js            # Winston logging
│   ├── metrics.js           # Prometheus metrics
│   ├── instrumentation.js   # OpenTelemetry setup
│   └── Dockerfile
├── Client1/                  # React frontend
│   ├── src/                 # React components
│   ├── public/              # Static assets
│   └── Dockerfile
├── observability/           # Observability configs
│   ├── prometheus/          # Prometheus config
│   ├── grafana/            # Grafana provisioning
│   ├── loki/               # Loki config
│   ├── promtail/           # Promtail config
│   └── alertmanager/       # Alertmanager config
└── docker-compose.yml      # Main orchestration
```

### Building Individual Services

**Service1:**
```bash
cd Service1
dotnet build
dotnet run
```

**Service2 (Node.js):**
```bash
cd Service2
npm install
npm start
```

**Client1:**
```bash
cd Client1
npm install
npm start
```

### Adding Custom Metrics

**In .NET Services:**
```csharp
private static readonly Counter MyCustomMetric = Metrics
    .CreateCounter("my_custom_metric_total", "Description", new[] { "label1" });

// Usage
MyCustomMetric.WithLabels("value1").Inc();
```

### Adding Custom Alerts

**Edit `observability/prometheus/alert_rules.yml`:**
```yaml
- alert: MyCustomAlert
  expr: my_custom_metric_total > 100
  for: 1m
  labels:
    severity: warning
  annotations:
    summary: "Custom metric threshold exceeded"
```

## 🎯 Demo Scenarios

### 1. Normal Operations
1. Access the React app at http://localhost:3001
2. Create several orders of different types
3. Observe metrics in Grafana showing normal operation
4. Check logs in Grafana log explorer

### 2. Load Testing
1. Use browser dev tools or curl to make multiple requests
2. Observe latency metrics increasing
3. Watch for any error rate increases
4. Monitor infrastructure metrics

### 3. Error Simulation
1. Stop Service2: `docker-compose stop service2`
2. Try to create orders in the UI
3. Observe error metrics and alerts firing
4. Check Alertmanager for alert notifications

### 4. Database Issues
1. Stop PostgreSQL: `docker-compose stop postgres`
2. Observe database connectivity alerts
3. Watch Service2 health endpoint failures
4. See error propagation through the system

## 📋 Useful Commands

**View all services status:**
```bash
docker-compose ps
```

**View service logs:**
```bash
docker-compose logs -f service1
docker-compose logs -f service2
```

**Restart specific service:**
```bash
docker-compose restart service1
```

**Scale services:**
```bash
docker-compose up -d --scale service1=2
```

**Clean up:**
```bash
docker-compose down -v  # Removes volumes too
```

## 🔧 Configuration

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `DATA_SOURCE_NAME`: PostgreSQL connection for exporter
- `GF_SECURITY_ADMIN_PASSWORD`: Grafana admin password

### Port Mapping
| Service | Internal | External | Purpose |
|---------|----------|----------|---------|
| Client1 | 3001 | 3001 | React app |
| Grafana | 3000 | 3000 | Dashboards |
| Loki | 3100 | 3100 | Log aggregation |
| Service1 | 8080 | 8080 | Orders API |
| Service2 | 8081 | 8081 | Events API |
| cAdvisor | 8080 | 8082 | Container metrics |
| Prometheus | 9090 | 9090 | Metrics |
| Alertmanager | 9093 | 9093 | Alerts |
| Node Exporter | 9100 | 9100 | Host metrics |
| Postgres Exporter | 9187 | 9187 | DB metrics |
| PostgreSQL | 5432 | 5432 | Database |

## 🎓 Learning Objectives

This demo demonstrates:

1. **Microservices Communication**: HTTP-based service-to-service calls with proper error handling
2. **Distributed Tracing**: Request correlation across service boundaries
3. **Metrics Collection**: Custom and standard metrics with Prometheus
4. **Log Aggregation**: Centralized logging with structured data
5. **Alerting**: Proactive monitoring with threshold-based alerts
6. **Dashboard Design**: Effective visualization of system health
7. **Infrastructure Monitoring**: Container and host-level observability
8. **Database Monitoring**: Application and infrastructure database metrics

## 🐛 Troubleshooting

**Services not starting:**
1. Check port conflicts: `netstat -tulpn | grep :3000`
2. Increase Docker memory allocation
3. Check Docker logs: `docker-compose logs service-name`

**Metrics not appearing:**
1. Verify service /metrics endpoints are accessible
2. Check Prometheus targets page
3. Confirm service registration in prometheus.yml

**Logs not showing:**
1. Verify Promtail configuration
2. Check Loki connectivity
3. Confirm log driver configuration in docker-compose.yml

**Alerts not firing:**
1. Check alert rule syntax in alert_rules.yml
2. Verify Alertmanager configuration
3. Confirm Prometheus can reach Alertmanager

## 📚 Further Reading

- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [Loki Documentation](https://grafana.com/docs/loki/latest/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [.NET Observability Best Practices](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/)
