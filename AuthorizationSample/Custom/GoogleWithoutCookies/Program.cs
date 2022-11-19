using GoogleWithoutCookies;
using GoogleWithoutCookies.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication((Microsoft.AspNetCore.Authentication.AuthenticationOptions options) =>
    {
        options.DefaultScheme = GoogleDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    });

builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CustomGoogleOptions>, CustomOAuthPostConfigureOptions<CustomGoogleOptions, CustomGoogleHandler>>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CustomGoogleOptions>, EnsureSignInScheme<CustomGoogleOptions>>());

var state = new CustomAddSchemeHelperState(typeof(CustomGoogleHandler));
builder.Services.Configure<AuthenticationOptions>(o =>
{
    o.AddScheme(GoogleDefaults.AuthenticationScheme, scheme =>
    {
        scheme.HandlerType = state.HandlerType;
        scheme.DisplayName = GoogleDefaults.DisplayName;
    });
});

builder.Services.Configure<CustomGoogleOptions>(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = "";
    options.ClientSecret = "";
});

builder.Services.AddOptions<CustomGoogleOptions>(GoogleDefaults.AuthenticationScheme).Validate(o =>
{
    o.Validate(GoogleDefaults.AuthenticationScheme);
    return true;
});
builder.Services.AddTransient<CustomGoogleHandler>();


builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

struct CustomAddSchemeHelperState
{
    public CustomAddSchemeHelperState([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType)
    {
        HandlerType = handlerType;
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type HandlerType { get; }
}