using AppTemplate.AuthApi.Services;
using IdentityModel.Client;
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
[Route("api/auth/token")]
[Consumes(MediaTypeNames.Application.Json)]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenService _authService;

    public TokenController(
        ILogger<TokenController> logger,
        ITokenService authService)
    {
        _logger = logger;
        _authService = authService;
    }


    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateToken([FromBody] Api.TokenRequest request)
    {
        try
        {
            var result = await _authService.CreateToken(request);

            if (!result.IsError) return Ok(result.Json);
            return Conflict(result.Json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST:/token");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] Api.TokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshToken(request);

            if (!result.IsError) return Ok(result.Json);
            return Conflict(result.Json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST:/refresh");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TokenRevocationResponse>> RevokeToken([FromBody] Api.TokenRequest request)
    {
        try
        {
            var result = await _authService.RevokeToken(request);

            if (!result.IsError) return Ok();
            return Conflict(result.Json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST:/revoke");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
