using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace AppTemplate.AuthApi.Configuration;

public class IdentityConfiguration
{
    public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    };

    public static IEnumerable<ApiResource> GetResourceApis() => new List<ApiResource>
    {
        new ApiResource(name: "<ApiName>", displayName: "Klika Resource API")
        {
            Scopes = new List<string>() { "<ApiName>" }
        }
    };

    public static IEnumerable<ApiScope> GetApiScopes() => new[]
    {
        new ApiScope(name: "<ApiName>",   displayName: "Klika Resource Api Access")
    };

    public static IEnumerable<Client> GetClients(IConfiguration configuration) => new List<Client>
    {
        new Client()
        {
            ClientId = "mobile",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret(configuration["mobile_client_secret"].Sha256()) },
            AllowedScopes = new List<string>
            {
                "<ApiName>",
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.OfflineAccess
            },
            AllowOfflineAccess = true,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
        },
        new Client()
        {
            ClientId = "web",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret(configuration["web_client_secret"].Sha256()) },
            AllowedScopes = new List<string>
            {
                "<ApiName>",
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.OfflineAccess
            },
            AllowOfflineAccess = true,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
        }
    };
}
