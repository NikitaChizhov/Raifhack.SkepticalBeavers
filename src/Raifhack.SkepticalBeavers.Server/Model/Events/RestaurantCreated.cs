using System;
using Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes;

namespace Raifhack.SkepticalBeavers.Server.Model.Events
{
    internal sealed class RestaurantCreated : EventBase
    {
        public Guid RestaurantId { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public Guid? MenuId { get; set; }
    }
}