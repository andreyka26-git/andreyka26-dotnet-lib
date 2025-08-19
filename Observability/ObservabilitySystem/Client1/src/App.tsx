import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

interface Order {
  id: number;
  orderNumber: string;
  description: string;
  timestamp: string;
  status: string;
}

interface ApiResponse {
  requestId: string;
  orders: Order[];
  totalCount: number;
}

interface CreateOrderRequest {
  orderType: string;
}

const App: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [newOrderType, setNewOrderType] = useState('');
  const [requestId, setRequestId] = useState<string>('');

  const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:8080';

  const fetchOrders = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await axios.get<ApiResponse>(`${API_BASE_URL}/api/orders`);
      setOrders(response.data.orders);
      setRequestId(response.data.requestId);
    } catch (err) {
      setError('Failed to fetch orders');
      console.error('Error fetching orders:', err);
    } finally {
      setLoading(false);
    }
  };

  const createOrder = async () => {
    if (!newOrderType.trim()) {
      setError('Order type is required');
      return;
    }

    setLoading(true);
    setError('');
    try {
      const request: CreateOrderRequest = { orderType: newOrderType };
      await axios.post(`${API_BASE_URL}/api/orders`, request);
      setNewOrderType('');
      await fetchOrders(); // Refresh the list
    } catch (err) {
      setError('Failed to create order');
      console.error('Error creating order:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <h1>Observability Demo System</h1>
        <p>Client → Service1 → Service2 → PostgreSQL</p>
        {requestId && (
          <p className="request-id">Last Request ID: {requestId}</p>
        )}
      </header>

      <main className="App-main">
        <section className="create-order-section">
          <h2>Create New Order</h2>
          <div className="create-order-form">
            <input
              type="text"
              value={newOrderType}
              onChange={(e) => setNewOrderType(e.target.value)}
              placeholder="Enter order type (e.g., Electronics, Books, Clothing)"
              className="order-type-input"
            />
            <button
              onClick={createOrder}
              disabled={loading}
              className="create-button"
            >
              {loading ? 'Creating...' : 'Create Order'}
            </button>
          </div>
        </section>

        <section className="orders-section">
          <div className="orders-header">
            <h2>Orders ({orders.length})</h2>
            <button
              onClick={fetchOrders}
              disabled={loading}
              className="refresh-button"
            >
              {loading ? 'Loading...' : 'Refresh'}
            </button>
          </div>

          {error && <div className="error-message">{error}</div>}

          <div className="orders-grid">
            {orders.length === 0 && !loading ? (
              <div className="no-orders">No orders found. Create one to get started!</div>
            ) : (
              orders.map((order) => (
                <div key={order.id} className="order-card">
                  <h3>{order.orderNumber}</h3>
                  <p><strong>Type:</strong> {order.description}</p>
                  <p><strong>Status:</strong> {order.status}</p>
                  <p><strong>Created:</strong> {new Date(order.timestamp).toLocaleString()}</p>
                </div>
              ))
            )}
          </div>
        </section>

        <section className="metrics-section">
          <h2>Observability Links</h2>
          <div className="metrics-links">
            <a href="http://localhost:3000" target="_blank" rel="noopener noreferrer">
              📊 Grafana Dashboard
            </a>
            <a href="http://localhost:9090" target="_blank" rel="noopener noreferrer">
              📈 Prometheus Metrics
            </a>
            <a href="http://localhost:8080/metrics" target="_blank" rel="noopener noreferrer">
              🔍 Service1 Metrics
            </a>
            <a href="http://localhost:8081/metrics" target="_blank" rel="noopener noreferrer">
              🔍 Service2 Metrics
            </a>
          </div>
        </section>
      </main>
    </div>
  );
};

export default App;
