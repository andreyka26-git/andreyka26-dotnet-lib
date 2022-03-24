using Oidc.AuthorizationServerWithOpenIddict.Data;
using Oidc.AuthorizationServerWithOpenIddict.Encryption;
using OpenIddict.Abstractions;
using System.Globalization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Oidc.AuthorizationServerWithOpenIddict;

public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEncryptor _encryptor;

    public Worker(IServiceProvider serviceProvider, IEncryptor encryptor)
    {
        _serviceProvider = serviceProvider;
        _encryptor = encryptor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AuthorizationContext>();
        await context.Database.EnsureCreatedAsync();
        await CreateCertificatesIfNotExistAsync(cancellationToken);

        await RegisterApplicationsAsync(scope.ServiceProvider, cancellationToken);
        await RegisterScopesAsync(scope.ServiceProvider);
    }

    private async Task CreateCertificatesIfNotExistAsync(CancellationToken cancellationToken)
    {
        // add behavior for checking expiration date
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "certificates");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (Directory.GetFiles(dir).Length == 0)
        {
            var (encFileName, encContent) = _encryptor.GenerateEncryptionCertificate();
            var (sigFileName, sigContent) = _encryptor.GenerateSigningCertificate();

            var encPath = Path.Combine(dir, encFileName);
            await File.WriteAllBytesAsync(encPath, encContent, cancellationToken);

            var sigPath = Path.Combine(dir, sigFileName);
            await File.WriteAllBytesAsync(sigPath, sigContent);
        }
    }
    
    private static async Task RegisterApplicationsAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

        // web client
        var client = await manager.FindByClientIdAsync("mvc");

        if (client != null)
        {
            await manager.DeleteAsync(client, cancellationToken);
        }

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "mvc",
            ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
            ConsentType = ConsentTypes.Explicit,
            DisplayName = "MVC client application",
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7001/signout-callback-oidc")
            },
            RedirectUris =
            {
                new Uri("https://localhost:7001/signin-oidc")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                Permissions.Prefixes.Scope + "api1",
                Permissions.Prefixes.Scope + "api2"
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        });

        //There is no need to register resource_server_1 because it uses symetric key for the token validation
        // resource server

        //if (await manager.FindByClientIdAsync("resource_server_1") == null)
        //{
        //    var descriptor = new OpenIddictApplicationDescriptor
        //    {
        //        ClientId = "resource_server_1",
        //        ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342",
        //        Permissions =
        //                {
        //                    Permissions.Endpoints.Introspection
        //                }
        //    };

        //    await manager.CreateAsync(descriptor);
        //}

        if (await manager.FindByClientIdAsync("resource_server_2") == null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "resource_server_2",
                ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342",
                Permissions =
                {
                    Permissions.Endpoints.Introspection
                }
            };

            await manager.CreateAsync(descriptor);
        }
    }

    private static async Task RegisterScopesAsync(IServiceProvider provider)
    {
        var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

        // symetric key validation
        if (await manager.FindByNameAsync("api1") is null)
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api1",
                Resources =
                {
                    "resource_server_1"
                }
            });
        }

        //introspection validation
        if (await manager.FindByNameAsync("api2") is null)
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api2",
                Resources =
                {
                    "resource_server_2"
                }
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
