using System;
using System.ComponentModel.DataAnnotations;

namespace Raifhack.SkepticalBeavers.Server.Model.Snapshots
{
    internal sealed class MenuItemDb
    {
        [Key]
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri Picture { get; set; }

        public Decimal Price { get; set; }
    }
}