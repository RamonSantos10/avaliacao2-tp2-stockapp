using System;
using System.Threading.Tasks;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            
            if (user == null || user.Password != password)
            {
                return null;
            }

            return $"token_{user.Username}_{DateTime.UtcNow.Ticks}";
        }
    }
}