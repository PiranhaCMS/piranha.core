using System;
using System.Collections.Generic;
using System.Linq;
using ContentRus.TenantManagement.Models;

namespace ContentRus.TenantManagement.Services
{
    public class UserService
    {
        private readonly List<User> _users = new();

        public User CreateUser(string email, string password, Guid tenantId)
        {
            var user = new User
            {
                Id = _users.Count + 1,
                Email = email,
                Password = password,
                TenantId = tenantId
            };

            _users.Add(user);
            return user;
        }

        public bool UpdateUserPassword(int id, string newPassword)
        {
            var user = GetUser(id);
            if (user == null) return false;

            user.Password = newPassword;
            return true;
        }

        public User? GetUser(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _users;
        }

        public User? ValidateUserCredentials(string email, string password)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.Password != password)
            {
                return null;
            }
            return user;
        }
    }
}