using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Raifhack.SkepticalBeavers.Server.Model;

namespace Raifhack.SkepticalBeavers.Server.Services
{
    internal sealed class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>(
            new Dictionary<string, User>
            {
                {"admin", new User(DebugAccounts.Admin, "admin", "admin", UserRoles.Admin)},
                {"user", new User(DebugAccounts.User, "user", "user", UserRoles.Reader)}
            });

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public bool IsValidUserCredentials(string userName, string password, [NotNullWhen(true)] out User? user)
        {
            _logger.LogInformation($"Validating user [{userName}]");
            user = null;
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (!_users.TryGetValue(userName, out var u) || u.Password != password) return false;

            user = u;
            return true;
        }


    }
}