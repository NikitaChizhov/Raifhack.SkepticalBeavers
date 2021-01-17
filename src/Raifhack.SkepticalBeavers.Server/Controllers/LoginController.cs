using System;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raifhack.SkepticalBeavers.Server.Contracts;
using Raifhack.SkepticalBeavers.Server.Contracts.Requests;
using Raifhack.SkepticalBeavers.Server.Contracts.Responses;
using Raifhack.SkepticalBeavers.Server.Services;
using Raifhack.SkepticalBeavers.Server.Utils;

namespace Raifhack.SkepticalBeavers.Server.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    internal sealed class LoginController : InternalControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IJwtAuthManager _jwtAuthManager;

        private readonly IUserService _userService;

        public LoginController(IJwtAuthManager jwtAuthManager, ILogger<LoginController> logger, IUserService userService)
        {
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost(Routes.Login, Name = nameof(Login))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [Consumes(HttpContentTypes.MultipartFormData, HttpContentTypes.ApplicationJson)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!_userService.IsValidUserCredentials(request.UserName, request.Password, out var user) || user == null)
            {
                return Unauthorized();
            }

            var claims = new []
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString("N")),
            };
            var token = _jwtAuthManager.GenerateToken(request.UserName, claims, DateTime.UtcNow);
            _logger.LogInformation($"User [{request.UserName}] logged in the system.");

            return Ok(new LoginResponse
            {
                UserName = request.UserName,
                AccessToken = token
            });
        }
    }
}