using AppTemplate.AuthApi.Database.Models;
using AppTemplate.AuthApi.Extensions.Mappers;
using AppTemplate.AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppTemplate.AuthApi.Services;

public class UsersService : IUsersService
{
    private readonly ILogger<UsersService> _logger;
    private readonly UserManager<AppUser> _userManager;

    public UsersService(
        ILogger<UsersService> logger,
        UserManager<AppUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }


    public async Task<IdentityResult> RegisterUser(SignUpRequest request)
    {
        var dbUser = request?.ToDatabaseModel();
        var result = await _userManager.CreateAsync(dbUser, request.Password);

        // Add roles and claims
        if (result.Succeeded)
        {
            await _userManager.AddClaimAsync(dbUser, new Claim("email", dbUser.Email));

            // define roles upon registartion
            await _userManager.AddToRolesAsync(dbUser, new[] { "admin" });
        }

        return result;
    }
}
