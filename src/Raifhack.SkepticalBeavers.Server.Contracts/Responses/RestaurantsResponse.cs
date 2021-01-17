using System.Collections.Generic;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Responses
{
    public sealed class RestaurantsResponse
    {
        public IReadOnlyCollection<RestaurantResponse> Data { get; set; } = new RestaurantResponse[0];
    }
}
