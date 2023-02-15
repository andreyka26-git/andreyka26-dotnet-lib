using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:7000/connect/authorize"),
                TokenUrl = new Uri("https://localhost:7000/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "api://29a02307-5a1b-460c-85ba-9e9abb75e48d/Read.WeatherForecast", "Reads the Weather forecast" }
                }
            }
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();