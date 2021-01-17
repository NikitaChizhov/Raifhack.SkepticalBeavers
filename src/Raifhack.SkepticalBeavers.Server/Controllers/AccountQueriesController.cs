using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raifhack.SkepticalBeavers.Server.Contracts;
using Raifhack.SkepticalBeavers.Server.Contracts.Responses;
using Raifhack.SkepticalBeavers.Server.Services;

namespace Raifhack.SkepticalBeavers.Server.Controllers
{
    internal sealed class AccountQueriesController : AuthorizedControllerBase
    {
        private readonly SnapshotContext _context;

        /// <inheritdoc />
        public AccountQueriesController(SnapshotContext context)
        {
            _context = context;
        }

        [HttpGet(Routes.Menus, Name = nameof(GetMenus))]
        [ProducesResponseType(typeof(MenusResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMenus()
        {
            var account = await _context.Accounts
                .Include(a => a.Menus)
                .ThenInclude(m => m.Items)
                .FirstOrDefaultAsync(a => a.Id == AccountId);

            return account?.Menus == null ? (IActionResult) NotFound() : Ok(account.Menus);
        }

        [HttpGet(Routes.MenuWithId.Route, Name = nameof(GetMenu))]
        [ProducesResponseType(typeof(MenuFullResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMenu([FromRoute(Name = Routes.MenuWithId.ParamNames.Id)] Guid menuId)
        {
            var account = await _context.Accounts
                .Include(a => a.Menus)
                .ThenInclude(m => m.Items)
                .FirstOrDefaultAsync(a => a.Id == AccountId);

            return account?.Menus == null ? (IActionResult) NotFound() : Ok(account.Menus.FirstOrDefault(m => m.Id == menuId));
        }

        [HttpGet(Routes.Restaurants, Name = nameof(GetRestaurants))]
        [ProducesResponseType(typeof(RestaurantsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRestaurants()
        {
            var account = await _context.Accounts
                .Include(a => a.Restaurants)
                .ThenInclude(r => r.Menu)
                .FirstOrDefaultAsync(a => a.Id == AccountId);

            return account?.Restaurants == null ? (IActionResult) NotFound() : Ok(account.Restaurants);
        }

        [HttpGet(Routes.RestaurantWithId.Route, Name = nameof(GetRestaurant))]
        [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRestaurant([FromRoute(Name = Routes.RestaurantWithId.ParamNames.Id)] Guid restaurantId)
        {
            var account = await _context.Accounts
                .Include(a => a.Restaurants)
                .ThenInclude(r => r.Menu)
                .FirstOrDefaultAsync(a => a.Id == AccountId);

            return account?.Restaurants == null ? (IActionResult) NotFound() : Ok(account.Restaurants.FirstOrDefault(r => r.Id == restaurantId));
        }
    }
}