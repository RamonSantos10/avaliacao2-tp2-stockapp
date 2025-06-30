using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;

namespace StockApp.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static List<User> _users = new List<User>();
        private static int _nextId = 1;

        public async Task<User> GetByIdAsync(int id)
        {
            await Task.Delay(10); // Simula operação assíncrona
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            await Task.Delay(10); // Simula operação assíncrona
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public async Task CreateAsync(User user)
        {
            await Task.Delay(10); // Simula operação assíncrona
            user.Id = _nextId++;
            _users.Add(user);
        }
    }
}