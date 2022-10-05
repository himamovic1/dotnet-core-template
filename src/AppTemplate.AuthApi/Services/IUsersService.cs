using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

using Api = AppTemplate.AuthApi.Models;

namespace AppTemplate.AuthApi.Services;

public interface IUsersService
{
    Task<IdentityResult> RegisterUser(Api.SignUpRequest request);
}
