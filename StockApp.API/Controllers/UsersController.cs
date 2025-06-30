using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Application.DTOs;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                // Verificar se o usuário já existe
                var existingUser = await _userRepository.GetByUsernameAsync(userRegisterDto.Username);
                if (existingUser != null)
                {
                    return BadRequest("Username already exists");
                }

                // Criar novo usuário
                var user = new User
                {
                    Username = userRegisterDto.Username,
                    Password = userRegisterDto.Password, // Em produção, deve ser hasheada
                    Role = userRegisterDto.Role
                };

                await _userRepository.CreateAsync(user);
                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during registration: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving user: {ex.Message}");
            }
        }
    }
}