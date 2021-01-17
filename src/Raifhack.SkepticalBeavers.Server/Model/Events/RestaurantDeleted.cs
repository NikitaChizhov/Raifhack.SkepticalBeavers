using System;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    /// <summary>
    /// Deleted event meant to be idempotent
    /// </summary>
    internal sealed class RestaurantDeleted : EventBase
    {
        public Guid RestaurantId { get; set; }
    }
}