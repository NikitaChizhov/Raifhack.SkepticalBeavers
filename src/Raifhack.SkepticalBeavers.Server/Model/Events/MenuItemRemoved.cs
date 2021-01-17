using System;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    internal sealed class MenuItemRemoved : EventBase
    {
        public Guid MenuId { get; set; }

        public string ItemKey { get; set; }
    }
}