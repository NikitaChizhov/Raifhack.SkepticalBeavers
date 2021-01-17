using System;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    internal sealed class AccountCreated : EventBase
    {
        public Guid AccountId { get; set; }

        public string Name { get; set; }
    }
}