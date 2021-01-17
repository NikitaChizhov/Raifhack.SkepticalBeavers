using System.Collections.Generic;
using Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Responses
{
    public sealed class MenuFullResponse : MenuShortResponse
    {
        public IReadOnlyCollection<MenuItem> Items { get; set; } = new MenuItem[0];
    }
}