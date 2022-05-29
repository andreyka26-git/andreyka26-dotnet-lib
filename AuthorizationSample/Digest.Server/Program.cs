using Digest.Server.Application;
using Digest.Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Example configuration providing an IUsernameSecretProvider (which returns the secret for a given username in plaintext)
// services.AddScoped<IUsernameSecretProvider, TrivialUsernameSecretProvider>();
// services.AddAuthentication("Digest")
//         .AddDigestAuthentication(DigestAuthenticationConfiguration.Create("VerySecret", "some-realm", 60, true, 20));

// Example configuration using IUsernameHashedSecretProvider (which returns the pre-computed MD5 hash of the secret "A1")
builder.Services.AddScoped<IUsernameHashedSecretProvider, TrivialUsernameHashedSecretProvider>();
builder.Services.AddSingleton<IHashService, HashService>();

var config = DigestAuthenticationConfiguration.Create("VerySecret", "some-realm", 60, true, 20);
builder.Services.AddAuthentication("Digest")
    .AddScheme<DigestAuthenticationOptions, DigestAuthenticationHandler>("Digest", "Digest", options => { options.Configuration = config; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
