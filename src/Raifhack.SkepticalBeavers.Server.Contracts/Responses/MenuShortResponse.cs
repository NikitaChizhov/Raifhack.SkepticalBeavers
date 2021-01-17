using System;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Responses
{
    public class MenuShortResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}