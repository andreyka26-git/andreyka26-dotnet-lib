const { NodeSDK } = require('@opentelemetry/sdk-node');
const { instrumentation } = require('./instrumentation');

// Initialize OpenTelemetry
const sdk = new NodeSDK({
  instrumentations: instrumentation,
  serviceName: 'service2',
  serviceVersion: '1.0.0',
});

sdk.start();

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const { Pool } = require('pg');
const promClient = require('prom-client');
const logger = require('./logger');
const metricsHandler = require('./metrics');

require('dotenv').config();

const app = express();
const port = process.env.PORT || 8081;

// Middleware
app.use(helmet());
app.use(cors());
app.use(express.json());

// Request tracking middleware
app.use((req, res, next) => {
  const requestId = req.headers['x-request-id'] || req.headers['traceparent'] || generateRequestId();
  req.requestId = requestId;
  res.setHeader('X-Request-Id', requestId);
  
  const startTime = Date.now();
  
  logger.info('Request started', {
    method: req.method,
    path: req.path,
    requestId: requestId,
    userAgent: req.get('User-Agent')
  });

  // Override res.end to log completion
  const originalEnd = res.end;
  res.end = function(...args) {
    const duration = Date.now() - startTime;
    
    logger.info('Request completed', {
      method: req.method,
      path: req.path,
      requestId: requestId,
      statusCode: res.statusCode,
      duration: duration
    });
    
    originalEnd.apply(this, args);
  };
  
  next();
});

// Database connection
const pool = new Pool({
  host: process.env.DB_HOST || 'postgres',
  port: process.env.DB_PORT || 5432,
  database: process.env.DB_NAME || 'events_db',
  user: process.env.DB_USER || 'postgres',
  password: process.env.DB_PASSWORD || 'postgres',
  max: 20,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 2000,
});

// Initialize database
async function initDatabase() {
  try {
    const client = await pool.connect();
    
    // Create events table if it doesn't exist
    await client.query(`
      CREATE TABLE IF NOT EXISTS events (
        id SERIAL PRIMARY KEY,
        event_type VARCHAR(100) NOT NULL,
        timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
        request_id VARCHAR(100)
      )
    `);
    
    // Insert some initial data if table is empty
    const countResult = await client.query('SELECT COUNT(*) FROM events');
    if (parseInt(countResult.rows[0].count) === 0) {
      await client.query(`
        INSERT INTO events (event_type, request_id) VALUES 
        ('SystemStart', 'init-001'),
        ('DatabaseInit', 'init-002')
      `);
      logger.info('Database initialized with sample data');
    }
    
    client.release();
    logger.info('Database connection established and tables created');
  } catch (error) {
    logger.error('Database initialization failed', { error: error.message });
    throw error;
  }
}

// Routes
app.get('/api/events', async (req, res) => {
  const requestId = req.requestId;
  const timer = metricsHandler.databaseQueryDuration.startTimer({ operation: 'select_all' });
  
  try {
    logger.info('Processing GetEvents request', { requestId });
    
    const client = await pool.connect();
    const result = await client.query(
      'SELECT id, event_type, timestamp FROM events ORDER BY timestamp DESC LIMIT 100'
    );
    client.release();
    
    timer({ status: 'success' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'select_all', status: 'success' });
    
    logger.info('Successfully retrieved events', { 
      requestId, 
      eventCount: result.rows.length 
    });
    
    res.json(result.rows);
  } catch (error) {
    timer({ status: 'error' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'select_all', status: 'error' });
    
    logger.error('Error retrieving events', { 
      requestId, 
      error: error.message 
    });
    
    res.status(500).json({ error: 'Internal server error', requestId });
  }
});

app.get('/api/events/:id', async (req, res) => {
  const requestId = req.requestId;
  const eventId = parseInt(req.params.id);
  const timer = metricsHandler.databaseQueryDuration.startTimer({ operation: 'select_by_id' });
  
  try {
    logger.info('Processing GetEvent request', { requestId, eventId });
    
    const client = await pool.connect();
    const result = await client.query(
      'SELECT id, event_type, timestamp FROM events WHERE id = $1',
      [eventId]
    );
    client.release();
    
    if (result.rows.length === 0) {
      timer({ status: 'not_found' });
      metricsHandler.databaseOperationsTotal.inc({ operation: 'select_by_id', status: 'not_found' });
      
      logger.warn('Event not found', { requestId, eventId });
      return res.status(404).json({ error: 'Event not found', requestId });
    }
    
    timer({ status: 'success' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'select_by_id', status: 'success' });
    
    logger.info('Successfully retrieved event', { requestId, eventId });
    
    res.json(result.rows[0]);
  } catch (error) {
    timer({ status: 'error' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'select_by_id', status: 'error' });
    
    logger.error('Error retrieving event', { 
      requestId, 
      eventId, 
      error: error.message 
    });
    
    res.status(500).json({ error: 'Internal server error', requestId });
  }
});

app.post('/api/events', async (req, res) => {
  const requestId = req.requestId;
  const { eventType } = req.body;
  const timer = metricsHandler.databaseQueryDuration.startTimer({ operation: 'insert' });
  
  try {
    logger.info('Processing CreateEvent request', { requestId, eventType });
    
    if (!eventType) {
      return res.status(400).json({ error: 'eventType is required', requestId });
    }
    
    const client = await pool.connect();
    const result = await client.query(
      'INSERT INTO events (event_type, request_id) VALUES ($1, $2) RETURNING id',
      [eventType, requestId]
    );
    client.release();
    
    const newEventId = result.rows[0].id;
    
    timer({ status: 'success' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'insert', status: 'success' });
    
    logger.info('Successfully created event', { 
      requestId, 
      eventId: newEventId, 
      eventType 
    });
    
    res.status(201).json({ id: newEventId, requestId });
  } catch (error) {
    timer({ status: 'error' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'insert', status: 'error' });
    
    logger.error('Error creating event', { 
      requestId, 
      eventType, 
      error: error.message 
    });
    
    res.status(500).json({ error: 'Internal server error', requestId });
  }
});

app.get('/api/events/health', async (req, res) => {
  const requestId = req.requestId;
  const timer = metricsHandler.databaseQueryDuration.startTimer({ operation: 'health_check' });
  
  try {
    // Test database connectivity
    const client = await pool.connect();
    await client.query('SELECT 1');
    client.release();
    
    timer({ status: 'success' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'health_check', status: 'success' });
    
    logger.info('Health check successful', { requestId });
    
    res.json({
      status: 'Healthy',
      database: 'Connected',
      timestamp: new Date().toISOString(),
      requestId
    });
  } catch (error) {
    timer({ status: 'error' });
    metricsHandler.databaseOperationsTotal.inc({ operation: 'health_check', status: 'error' });
    
    logger.error('Health check failed', { 
      requestId, 
      error: error.message 
    });
    
    res.status(503).json({
      status: 'Unhealthy',
      database: 'Disconnected',
      error: error.message,
      timestamp: new Date().toISOString(),
      requestId
    });
  }
});

// Metrics endpoint
app.get('/metrics', async (req, res) => {
  try {
    res.set('Content-Type', promClient.register.contentType);
    const metrics = await promClient.register.metrics();
    res.end(metrics);
  } catch (error) {
    logger.error('Error generating metrics', { error: error.message });
    res.status(500).end('Error generating metrics');
  }
});

// Error handling middleware
app.use((error, req, res, next) => {
  logger.error('Unhandled error', {
    requestId: req.requestId,
    error: error.message,
    stack: error.stack
  });
  
  res.status(500).json({
    error: 'Internal server error',
    requestId: req.requestId
  });
});

// 404 handler
app.use('*', (req, res) => {
  logger.warn('Route not found', {
    requestId: req.requestId,
    method: req.method,
    path: req.path
  });
  
  res.status(404).json({
    error: 'Route not found',
    requestId: req.requestId
  });
});

// Utility function to generate request ID
function generateRequestId() {
  return `req-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
}

// Graceful shutdown
process.on('SIGTERM', async () => {
  logger.info('SIGTERM received, shutting down gracefully');
  await pool.end();
  process.exit(0);
});

process.on('SIGINT', async () => {
  logger.info('SIGINT received, shutting down gracefully');
  await pool.end();
  process.exit(0);
});

// Start server
async function startServer() {
  try {
    await initDatabase();
    
    app.listen(port, '0.0.0.0', () => {
      logger.info('Service2 (Events API) started', { 
        port, 
        environment: process.env.NODE_ENV || 'development' 
      });
    });
  } catch (error) {
    logger.error('Failed to start server', { error: error.message });
    process.exit(1);
  }
}

startServer();
