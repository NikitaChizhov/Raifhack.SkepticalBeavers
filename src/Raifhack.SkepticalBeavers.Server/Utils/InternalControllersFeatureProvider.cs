using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Raifhack.SkepticalBeavers.Server.Controllers;

namespace Raifhack.SkepticalBeavers.Server.Utils
{
    internal sealed class InternalControllersFeatureProvider : ControllerFeatureProvider
    {
        /// <inheritdoc />
        protected override bool IsController(TypeInfo typeInfo)
        {
            var isInternalController = !typeInfo.IsAbstract && typeof(InternalControllerBase).IsAssignableFrom(typeInfo);
            return isInternalController || base.IsController(typeInfo);
        }
    }
}