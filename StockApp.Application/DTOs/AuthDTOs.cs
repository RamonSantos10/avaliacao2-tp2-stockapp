using System;

namespace StockApp.Application.DTOs
{
    public class UserLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}