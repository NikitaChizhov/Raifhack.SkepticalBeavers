using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raifhack.SkepticalBeavers.Server.Model.Snapshots
{
    internal sealed class AccountDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<RestaurantDb> Restaurants { get; } = new List<RestaurantDb>();

        public ICollection<MenuDb> Menus { get; } = new List<MenuDb>();
    }
}