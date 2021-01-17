using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Raifhack.SkepticalBeavers.Server.Exceptions;

namespace Raifhack.SkepticalBeavers.Server.Controllers
{
    [Authorize]
    internal abstract class AuthorizedControllerBase : InternalControllerBase
    {
        protected Guid AccountId
        {
            get
            {
                var identifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    throw new ConventionViolationException($"Identity name was expected to be set for any authorized user");
                }
                if (!Guid.TryParse(identifier, out var accountId))
                {
                    throw new ConventionViolationException($"Identity name expected to be a guid, but is {identifier}");
                }

                return accountId;
            }
        }
    }
}