using System;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Requests
{
    public sealed class UpdateRestaurantRequest
    {
        public Guid? MenuId { get; set; }
    }
}