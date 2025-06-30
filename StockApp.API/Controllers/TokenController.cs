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
                // Validação de campos nulos
                if (userLoginDto == null || string.IsNullOrEmpty(userLoginDto.Username) || string.IsNullOrEmpty(userLoginDto.Password))
                {
                    return BadRequest("Login não pode ser nulo");
                }

                var token = await _authService.AuthenticateAsync(userLoginDto.Username, userLoginDto.Password);
                
                if (token == null)
                {
                    return Unauthorized("Credenciais invalidas");
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