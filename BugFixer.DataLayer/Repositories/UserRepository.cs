using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.DataLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region Ctor

        private readonly BugFixerDbContext _context;

        public UserRepository(BugFixerDbContext context)
        {
            _context = context;
        }

        #endregion

        public async Task<bool> IsExistsUserByEmail(string email)
        {
            return await _context.Users.AnyAsync(s => s.Email == email);
        }

        public async Task CreateUser(User user)
        {
            await _context.AddAsync(user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(s => s.Email.Equals(email));
        }

        public async Task<User> GetUserByActivationCode(string activationCode)
        {
            return await _context.Users.FirstOrDefaultAsync(s => s.EmailActivationCode.Equals(activationCode));
        }

        public async Task<User?> GetUserById(long userId)
        {
            return await _context.Users.FirstOrDefaultAsync(s => !s.IsDelete && s.Id == userId);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            _context.Update(user);
        }

        public IQueryable<User> GetAllUsers()
        {
            return _context.Users.Where(s => !s.IsDelete).AsQueryable();
        }
    }
}
