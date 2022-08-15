using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;

namespace BugFixer.Application.Extensions
{
    public  static class UserExtensions
    {
        public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var identifier = claimsPrincipal.Claims.SingleOrDefault(s => s.Type == ClaimTypes.NameIdentifier);

            if (identifier == null) return 0;

            return long.Parse(identifier.Value);
        }

        public static string GetUserDisplayName(this User user)
        {
            if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
            {
                return $"{user.FirstName} {user.LastName}";
            }

            var email = user.Email.Split("@")[0];

            return email;
        }
    }
}
