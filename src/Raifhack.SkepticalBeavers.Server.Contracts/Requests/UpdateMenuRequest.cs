using System.Collections.Generic;
using Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Requests
{
    public sealed class UpdateMenuRequest
    {
        /// <summary>
        /// List of keys to delete from the menu
        /// </summary>
        public List<string>? Delete { get; set; }

        /// <summary>
        /// Items to add to the menu
        /// </summary>
        public List<MenuItem>? Create { get; set; }
    }
}