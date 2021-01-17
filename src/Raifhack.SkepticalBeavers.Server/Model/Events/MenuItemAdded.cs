using System;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    internal sealed class MenuItemAdded : EventBase
    {
        public Guid MenuId { get; set; }

        public MenuItem Item { get; set; }
    }
}