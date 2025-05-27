using System;
using System.Collections.Generic;
using System.Linq;
using ContentRus.TenantManagement.Models;
using ContentRus.TenantManagement.Data;
using Microsoft.AspNetCore.Identity;

namespace ContentRus.TenantManagement.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public User CreateUser(string email, string password, Guid tenantId)
        {
            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                Id = _context.Users.Count() + 1,
                Email = email,
                Password = password,
                TenantId = tenantId
            };
            user.Password = hasher.HashPassword(user, password);

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public bool UpdateUserPassword(int id, string newPassword)
        {
            var user = GetUser(id);
            if (user == null) return false;

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, newPassword);

            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }

        public User? GetUser(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users;
        }

        public User? ValidateUserCredentials(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return null;

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }
    }
}