using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;

namespace Oidc.AuthorizationClient.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    [HttpGet("~/")]
    public ActionResult Index() => View("Home");

    [Authorize, HttpPost("~/first")]
    public async Task<ActionResult> First(CancellationToken cancellationToken)
    {
        var token = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectParameterNames.AccessToken);
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("The access token cannot be found in the authentication ticket. " +
                                                "Make sure that SaveTokens is set to true in the OIDC options.");
        }

        using var client = _httpClientFactory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7002/api/message");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        return View("Home", model: await response.Content.ReadAsStringAsync());
    }

    [Authorize, HttpPost("~/second")]
    public async Task<ActionResult> Second(CancellationToken cancellationToken)
    {
        var token = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectParameterNames.AccessToken);
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("The access token cannot be found in the authentication ticket. " +
                                                "Make sure that SaveTokens is set to true in the OIDC options.");
        }

        using var client = _httpClientFactory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7003/api/message");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        return View("Home", model: await response.Content.ReadAsStringAsync());
    }
}
