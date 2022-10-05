using IdentityModel.Client;
using System.Threading.Tasks;

using Api = AppTemplate.AuthApi.Models;

namespace AppTemplate.AuthApi.Services;

public interface ITokenService
{
    Task<TokenResponse> CreateToken(Api.TokenRequest request);
    Task<TokenResponse> RefreshToken(Api.TokenRequest request);
    Task<TokenRevocationResponse> RevokeToken(Api.TokenRequest request);
}
