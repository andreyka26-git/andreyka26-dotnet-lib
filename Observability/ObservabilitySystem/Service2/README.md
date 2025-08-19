# Service2 - Node.js Events API

A Node.js Express.js service that handles event persistence with PostgreSQL integration and comprehensive observability features.

## Features

- **RESTful API** for event management
- **PostgreSQL integration** with connection pooling
- **Prometheus metrics** with custom business metrics
- **Structured logging** with Winston
- **OpenTelemetry tracing** for distributed observability
- **Health checks** for monitoring
- **Request correlation** via request IDs
- **Graceful shutdown** handling

## API Endpoints

### Events Management
- `GET /api/events` - Retrieve all events (limited to 100)
- `GET /api/events/:id` - Retrieve specific event by ID
- `POST /api/events` - Create new event

### Monitoring
- `GET /api/events/health` - Health check endpoint
- `GET /metrics` - Prometheus metrics endpoint

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `PORT` | 8081 | Server port |
| `NODE_ENV` | development | Environment |
| `DB_HOST` | postgres | Database host |
| `DB_PORT` | 5432 | Database port |
| `DB_NAME` | events_db | Database name |
| `DB_USER` | postgres | Database user |
| `DB_PASSWORD` | postgres | Database password |
| `LOG_LEVEL` | info | Logging level |

## Custom Metrics

- `service2_database_query_duration_seconds` - Database query latency
- `service2_database_operations_total` - Total database operations
- `service2_http_requests_total` - Total HTTP requests
- `service2_http_request_duration_seconds` - HTTP request duration

## Development

```bash
# Install dependencies
npm install

# Run in development mode
npm run dev

# Run in production mode
npm start
```

## Docker

```bash
# Build image
docker build -t service2 .

# Run container
docker run -p 8081:8081 service2
```
