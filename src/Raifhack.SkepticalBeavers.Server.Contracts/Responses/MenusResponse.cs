using System.Collections.Generic;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Responses
{
    public sealed class MenusResponse
    {
        public IReadOnlyCollection<MenuShortResponse> Menus { get; set; } = new MenuShortResponse[0];
    }
}