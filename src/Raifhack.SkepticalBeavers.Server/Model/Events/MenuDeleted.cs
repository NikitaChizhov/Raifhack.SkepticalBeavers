using System;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    internal sealed class MenuDeleted : EventBase
    {
        public Guid MenuId { get; set; }
    }
}