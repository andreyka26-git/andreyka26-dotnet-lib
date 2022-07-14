using CsOpenGenericsInRuntime;
using CsOpenGenericsInRuntime.Demos;
using CsOpenGenericsInRuntime.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IService<FirstServiceRequest>, FirstServiceImplementation>();
builder.Services.AddTransient<IService<SecondServiceRequest>, SecondServiceImplementation>();

builder.Services.AddTransient<ReflectionDemo>();
builder.Services.AddTransient<MediatorDesignDemo>();

var app = builder.Build();

// Configure the HTTP request pipeline.

var demo = app.Services.GetRequiredService<MediatorDesignDemo>();
await demo.RunAsync();

app.UseHttpsRedirection();

app.Run();
