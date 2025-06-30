using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using StockApp.Application.Interfaces;
using StockApp.Application.DTOs;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IAuthService _authService;

        public TokenController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                var token = await _authService.AuthenticateAsync(userLoginDto.Username, userLoginDto.Password);
                
                if (token == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                var tokenResponse = new TokenResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(24)
                };

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during authentication: {ex.Message}");
            }
        }
    }
}