using System;
using System.ComponentModel.DataAnnotations;

namespace Raifhack.SkepticalBeavers.Server.Model.Snapshots
{
    internal sealed class RestaurantDb
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public MenuDb? Menu { get; set; }
    }
}