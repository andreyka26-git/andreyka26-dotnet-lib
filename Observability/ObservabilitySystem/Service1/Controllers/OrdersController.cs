using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Service1.Services;
using System.Diagnostics;

namespace Service1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IService2Client _service2Client;
    private readonly ILogger<OrdersController> _logger;
    
    // Custom metrics
    private static readonly Counter RequestsTotal = Metrics
        .CreateCounter("service1_requests_total", "Total number of requests", new[] { "method", "endpoint" });
    
    private static readonly Counter PartnerServiceCallsTotal = Metrics
        .CreateCounter("service1_partner_service_calls_total", "Total number of calls to partner services", new[] { "partner_service" });
    
    private static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("service1_request_duration_seconds", "Request duration in seconds", new[] { "method", "endpoint" });

    public OrdersController(IService2Client service2Client, ILogger<OrdersController> logger)
    {
        _service2Client = service2Client;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var requestId = HttpContext.TraceIdentifier;
        using var timer = RequestDuration.WithLabels("GET", "orders").NewTimer();
        RequestsTotal.WithLabels("GET", "orders").Inc();

        _logger.LogInformation("Processing GetOrders request with RequestId: {RequestId}", requestId);

        try
        {
            // Call Service2
            PartnerServiceCallsTotal.WithLabels("service2").Inc();
            var events = await _service2Client.GetEventsAsync(requestId);
            
            var orders = events.Select(e => new
            {
                Id = e.Id,
                OrderNumber = $"ORD-{e.Id:D6}",
                Description = e.EventType,
                Timestamp = e.Timestamp,
                Status = "Processed"
            }).ToList();

            _logger.LogInformation("Successfully retrieved {OrderCount} orders for RequestId: {RequestId}", 
                orders.Count, requestId);

            return Ok(new { RequestId = requestId, Orders = orders, TotalCount = orders.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GetOrders request with RequestId: {RequestId}", requestId);
            return StatusCode(500, new { RequestId = requestId, Error = "Internal server error" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var requestId = HttpContext.TraceIdentifier;
        using var timer = RequestDuration.WithLabels("POST", "orders").NewTimer();
        RequestsTotal.WithLabels("POST", "orders").Inc();

        _logger.LogInformation("Processing CreateOrder request with RequestId: {RequestId}, OrderType: {OrderType}", 
            requestId, request.OrderType);

        try
        {
            // Call Service2 to create event
            PartnerServiceCallsTotal.WithLabels("service2").Inc();
            var eventId = await _service2Client.CreateEventAsync(request.OrderType, requestId);
            
            var order = new
            {
                Id = eventId,
                OrderNumber = $"ORD-{eventId:D6}",
                OrderType = request.OrderType,
                Status = "Created",
                Timestamp = DateTime.UtcNow,
                RequestId = requestId
            };

            _logger.LogInformation("Successfully created order {OrderNumber} with RequestId: {RequestId}", 
                order.OrderNumber, requestId);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order with RequestId: {RequestId}", requestId);
            return StatusCode(500, new { RequestId = requestId, Error = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var requestId = HttpContext.TraceIdentifier;
        using var timer = RequestDuration.WithLabels("GET", "orders_by_id").NewTimer();
        RequestsTotal.WithLabels("GET", "orders_by_id").Inc();

        _logger.LogInformation("Processing GetOrderById request with RequestId: {RequestId}, OrderId: {OrderId}", 
            requestId, id);

        try
        {
            PartnerServiceCallsTotal.WithLabels("service2").Inc();
            var eventObj = await _service2Client.GetEventByIdAsync(id, requestId);
            
            if (eventObj == null)
            {
                _logger.LogWarning("Order not found with RequestId: {RequestId}, OrderId: {OrderId}", requestId, id);
                return NotFound(new { RequestId = requestId, Message = "Order not found" });
            }

            var order = new
            {
                Id = eventObj.Id,
                OrderNumber = $"ORD-{eventObj.Id:D6}",
                Description = eventObj.EventType,
                Timestamp = eventObj.Timestamp,
                Status = "Processed",
                RequestId = requestId
            };

            _logger.LogInformation("Successfully retrieved order {OrderNumber} with RequestId: {RequestId}", 
                order.OrderNumber, requestId);

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with RequestId: {RequestId}, OrderId: {OrderId}", requestId, id);
            return StatusCode(500, new { RequestId = requestId, Error = "Internal server error" });
        }
    }
}

public record CreateOrderRequest(string OrderType);
