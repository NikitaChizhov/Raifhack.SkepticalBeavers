using System;

namespace Raifhack.SkepticalBeavers.Server.Model
{
    internal sealed class User
    {
        public User(Guid accountId, string name, string password, string role)
        {
            Name = name;
            Password = password;
            Role = role;
            AccountId = accountId;
        }

        public Guid AccountId { get; }

        public string Name { get; }

        public string Password { get; }

        public string Role { get; }
    }
}