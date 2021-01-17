using System;
using Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Requests
{
    public sealed class CreateRestaurantRequest
    {
        public string Name { get; set; }

        public Address Address { get; set; }

        public Guid? MenuId { get; set; }
    }
}