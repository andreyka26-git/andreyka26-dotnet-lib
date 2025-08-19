using System.Text.Json;

namespace Service1.Services;

public interface IService2Client
{
    Task<List<EventDto>> GetEventsAsync(string requestId);
    Task<int> CreateEventAsync(string eventType, string requestId);
    Task<EventDto?> GetEventByIdAsync(int id, string requestId);
}

public class Service2Client : IService2Client
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Service2Client> _logger;

    public Service2Client(HttpClient httpClient, ILogger<Service2Client> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<EventDto>> GetEventsAsync(string requestId)
    {
        _logger.LogInformation("Calling Service2 GetEvents with RequestId: {RequestId}", requestId);
        
        _httpClient.DefaultRequestHeaders.Remove("X-Request-Id");
        _httpClient.DefaultRequestHeaders.Add("X-Request-Id", requestId);

        var response = await _httpClient.GetAsync("/api/events");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var events = JsonSerializer.Deserialize<List<EventDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<EventDto>();

        _logger.LogInformation("Successfully retrieved {EventCount} events from Service2 with RequestId: {RequestId}", 
            events.Count, requestId);

        return events;
    }

    public async Task<int> CreateEventAsync(string eventType, string requestId)
    {
        _logger.LogInformation("Calling Service2 CreateEvent with RequestId: {RequestId}, EventType: {EventType}", 
            requestId, eventType);

        _httpClient.DefaultRequestHeaders.Remove("X-Request-Id");
        _httpClient.DefaultRequestHeaders.Add("X-Request-Id", requestId);

        var request = new { EventType = eventType };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/events", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateEventResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        _logger.LogInformation("Successfully created event with Id: {EventId}, RequestId: {RequestId}", 
            result?.Id, requestId);

        return result?.Id ?? 0;
    }

    public async Task<EventDto?> GetEventByIdAsync(int id, string requestId)
    {
        _logger.LogInformation("Calling Service2 GetEventById with RequestId: {RequestId}, EventId: {EventId}", 
            requestId, id);

        _httpClient.DefaultRequestHeaders.Remove("X-Request-Id");
        _httpClient.DefaultRequestHeaders.Add("X-Request-Id", requestId);

        var response = await _httpClient.GetAsync($"/api/events/{id}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var eventDto = JsonSerializer.Deserialize<EventDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        _logger.LogInformation("Successfully retrieved event with Id: {EventId}, RequestId: {RequestId}", 
            id, requestId);

        return eventDto;
    }
}

public record EventDto(int Id, string EventType, DateTime Timestamp);
public record CreateEventResponse(int Id);
