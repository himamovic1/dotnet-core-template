using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppTemplate.AuthApi.Services;

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _clientFactory;

    public TokenService(
        ILogger<TokenService> logger,
        IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _config = config;
        _clientFactory = httpClientFactory;
    }


    public async Task<TokenResponse> CreateToken(Models.TokenRequest request)
    {
        var client = _clientFactory.CreateClient();
        var cache = new DiscoveryCache(_config["AuthApiUrl"]);
        var disco = await cache.GetAsync();

        if (disco.IsError)
            throw new Exception(disco.Error);

        return request.GrantType switch
        {
            "password" => await client.RequestPasswordTokenAsync(new()
            {
                Address = disco.TokenEndpoint,
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                Scope = request.Scope,
                UserName = request.Username,
                Password = request.Password
            }),
            "client_credentials" => await client.RequestClientCredentialsTokenAsync(new()
            {
                Address = disco.TokenEndpoint,
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                Scope = request.Scope,
            }),
            _ => throw new Exception("grant_type is not supported"),
        };
    }

    public async Task<TokenResponse> RefreshToken(Models.TokenRequest request)
    {
        var client = _clientFactory.CreateClient();
        var cache = new DiscoveryCache(_config["AuthApiUrl"]);
        var disco = await cache.GetAsync();

        if (disco.IsError)
            throw new Exception(disco.Error);

        return await client.RequestRefreshTokenAsync(new()
        {
            Address = disco.TokenEndpoint,
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret,
            RefreshToken = request.RefreshToken
        });
    }

    public async Task<TokenRevocationResponse> RevokeToken(Models.TokenRequest request)
    {
        var client = _clientFactory.CreateClient();
        var cache = new DiscoveryCache(_config["AuthApiUrl"]);
        var disco = await cache.GetAsync();

        if (disco.IsError)
            throw new Exception(disco.Error);

        return await client.RevokeTokenAsync(new()
        {
            Address = disco.RevocationEndpoint,
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret,
            Token = request.RefreshToken
        });
    }
}
