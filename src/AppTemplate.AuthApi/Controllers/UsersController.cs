using AppTemplate.AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

using Api = AppTemplate.AuthApi.Models;

namespace AppTemplate.AuthApi.Controllers;

[ApiController]
[Route("api/auth/users")]
[Consumes(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersService _usersService;

    public UsersController(
        ILogger<UsersController> logger,
        IUsersService usersService)
    {
        _logger = logger;
        _usersService = usersService;
    }


    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SignUp([FromBody] Api.SignUpRequest request)
    {
        try
        {
            var result = await _usersService.RegisterUser(request);

            if (result.Succeeded) return Ok(result);
            return Conflict(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST:/signup");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
