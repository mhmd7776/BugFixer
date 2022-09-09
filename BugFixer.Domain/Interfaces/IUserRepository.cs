using BugFixer.Domain.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsExistsUserByEmail(string email);

        Task CreateUser(User user);

        Task UpdateUser(User user);

        Task<User> GetUserByEmail(string email);

        Task<User> GetUserByActivationCode(string activationCode);

        Task<User?> GetUserById(long userId);

        IQueryable<User> GetAllUsers();

        Task Save();
    }
}
