using System;
using System.ComponentModel.DataAnnotations;

namespace Raifhack.SkepticalBeavers.Server.Model
{
    internal sealed class MenuItem
    {
        [Key]
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri Picture { get; set; }

        public Decimal Price { get; set; }
    }
}