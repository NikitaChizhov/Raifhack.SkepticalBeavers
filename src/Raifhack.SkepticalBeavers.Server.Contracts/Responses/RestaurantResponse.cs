using System;
using Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Responses
{
    public sealed class RestaurantResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Address Address { get; set; } = new Address();

        public MenuShortResponse? Menu { get; set; }
    }
}