using System;
using Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes;
using Raifhack.SkepticalBeavers.Server.Model.Events;

namespace Raifhack.SkepticalBeavers.Server.Model
{
    internal sealed class Restaurant
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public Guid? MenuId { get; set; }

        public Restaurant(RestaurantCreated @event)
        {
            Id = @event.RestaurantId;
            Name = @event.Name;
            Address = @event.Address;
            MenuId = @event.MenuId;
        }
    }
}