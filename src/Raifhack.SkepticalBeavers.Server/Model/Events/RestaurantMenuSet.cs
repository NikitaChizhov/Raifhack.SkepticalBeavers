using System;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    internal sealed class RestaurantMenuSet : EventBase
    {
        public Guid RestaurantId { get; set; }

        public Guid? MenuId { get; set; }
    }
}