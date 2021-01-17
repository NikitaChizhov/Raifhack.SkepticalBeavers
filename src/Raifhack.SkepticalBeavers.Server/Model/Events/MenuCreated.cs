using System;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    internal sealed class MenuCreated : EventBase
    {
        public Guid MenuId { get; set; }

        public string Name { get; set; }
    }
}