using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppTemplate.AuthApi.Services.Identity;

public class IdentityProfileService : IProfileService
{

    private readonly ILogger<IdentityProfileService> _logger;

    public IdentityProfileService(ILogger<IdentityProfileService> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// This method is called whenever claims about the user are requested (e.g. during
    //  token creation or via the userinfo endpoint)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        try
        {
            var identity = (ClaimsIdentity)context.Subject.Identity;

            //Add claims to issued access token
            context.IssuedClaims.AddRange(identity.Claims);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetProfileDataAsync));
            throw;
        }
    }

    /// <summary>
    /// This method gets called whenever identity server needs to determine if the user
    //  is valid or active (e.g. if the user's account has been deactivated since they
    //  logged in). (e.g. during token issuance or validation).
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task IsActiveAsync(IsActiveContext context)
    {
        await Task.CompletedTask;
    }
}
