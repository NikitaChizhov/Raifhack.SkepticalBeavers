using System;

namespace Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes
{
    public sealed class MenuItem
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri Picture { get; set; }

        public Decimal Price { get; set; }
    }
}