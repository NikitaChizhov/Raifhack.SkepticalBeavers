using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Raifhack.SkepticalBeavers.Server.Contracts;
using Raifhack.SkepticalBeavers.Server.Contracts.Requests;
using Raifhack.SkepticalBeavers.Server.Contracts.Responses;
using Raifhack.SkepticalBeavers.Server.Exceptions;
using Raifhack.SkepticalBeavers.Server.Extensions;
using Raifhack.SkepticalBeavers.Server.Model;
using Raifhack.SkepticalBeavers.Server.Model.Aggregates;
using Raifhack.SkepticalBeavers.Server.Model.Events;
using Raifhack.SkepticalBeavers.Server.Services;

namespace Raifhack.SkepticalBeavers.Server.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    internal sealed class AccountCommandsController : AuthorizedControllerBase
    {
        private readonly AggregateRepository _repository;

        /// <inheritdoc />
        public AccountCommandsController(AggregateRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("/account")]
        public async Task<IActionResult> CreateAccount()
        {
            var account = await _repository.LoadAsync<Account>(AccountId);
            if (account.Version == -1)
            {
                await _repository.SaveAsync(new Account(new AccountCreated
                {
                    Name = "account",
                    AccountId = DebugAccounts.Admin
                }));
                return Ok();
            }

            return NoContent();
        }

        [HttpPost(Routes.Menus, Name = nameof(CreateMenu))]
        [ProducesResponseType(typeof(MenuFullResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuRequest request)
        {
            var account = await _repository.LoadAsync<Account>(AccountId);
            var result = account.CreateMenu(request, out var response);
            await _repository.SaveAsync(account);

            var routeValues = new RouteValueDictionary
            {
                [Routes.MenuWithId.ParamNames.Id] = response?.Id
            };
            return MaybeCreatedAt(result,
                nameof(AccountQueriesController.GetMenu),
                nameof(AccountQueriesController).Replace("Controller", ""),
                routeValues,
                response);
        }

        [HttpPut(Routes.MenuWithId.Route, Name = nameof(UpdateMenu))]
        [ProducesResponseType(typeof(MenuFullResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMenu([FromRoute(Name = Routes.MenuWithId.ParamNames.Id)] Guid menuId,
            [FromBody] UpdateMenuRequest request)
        {
            var account = await _repository.LoadAsync<Account>(AccountId);
            var result = account.UpdateMenu(menuId, request, out var response);
            await _repository.SaveAsync(account);
            return MaybeOk(result, response);
        }

        [HttpDelete(Routes.MenuWithId.Route, Name = nameof(DeleteMenu))]
        public async Task<IActionResult> DeleteMenu([FromRoute(Name = Routes.MenuWithId.ParamNames.Id)] Guid menuId)
        {
            var account = await _repository.LoadAsync<Account>(AccountId);
            var result = account.DeleteMenu(menuId, out var response);
            await _repository.SaveAsync(account);
            return MaybeOk(result, response);
        }

        [HttpPost(Routes.Restaurants, Name = nameof(CreateRestaurant))]
        [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantRequest request)
        {
            var account = await _repository.LoadAsync<Account>(AccountId);
            var result = account.CreateRestaurant(request, out var response);
            await _repository.SaveAsync(account);

            var routeValues = new RouteValueDictionary
            {
                [Routes.RestaurantWithId.ParamNames.Id] = response?.Id
            };
            return MaybeCreatedAt(result,
                nameof(AccountQueriesController.GetRestaurant),
                nameof(AccountQueriesController).Replace("Controller", ""), routeValues,
                response);
        }

        [HttpPut(Routes.RestaurantWithId.Route, Name = nameof(UpdateRestaurant))]
        [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRestaurant([FromRoute(Name = Routes.RestaurantWithId.ParamNames.Id)] Guid restaurantId,
            UpdateRestaurantRequest request)
        {
            var account = await _repository.LoadAsync<Account>(AccountId);
            var result = account.UpdateRestaurant(restaurantId, request, out var response);
            await _repository.SaveAsync(account);

            return MaybeOk(result, response);
        }

        [HttpDelete(Routes.RestaurantWithId.Route, Name = nameof(DeleteRestaurant))]
        [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRestaurant([FromRoute(Name = Routes.RestaurantWithId.ParamNames.Id)] Guid restaurantId)
        {
            var account = await _repository.LoadAsync<Account>(AccountId);
            var result = account.DeleteRestaurant(restaurantId, out var response);
            await _repository.SaveAsync(account);

            return MaybeOk(result, response);
        }

        private IActionResult MaybeOk(AccountModificationResult result, object? value)
        {
            return result switch
            {
                AccountModificationResult.Success => Ok(value),
                _ => MapFailure(result)
            };
        }

        private IActionResult MaybeCreatedAt(AccountModificationResult result, string actionName, string controllerName,
            RouteValueDictionary routeValues, object? value)
        {
            return result switch
            {
                AccountModificationResult.Success => CreatedAtAction(actionName, controllerName, routeValues, value),
                _ => MapFailure(result)
            };
        }

        private IActionResult MapFailure(AccountModificationResult result)
        {
            return result switch
            {
                AccountModificationResult.Success or
                    AccountModificationResult.AccountAlreadyExists or
                    AccountModificationResult.RestaurantAlreadyExists or
                    AccountModificationResult.MenuAlreadyExists => throw new ConventionViolationException(
                        "This should never be reached."),
                AccountModificationResult.RestaurantNotFound => NotFound("Restaurant was not found."),
                AccountModificationResult.MenuNotFound => NotFound("Menu was not found"),
                AccountModificationResult.ItemNotFound => NotFound("Item was not found"),
                AccountModificationResult.ItemKeyAlreadyExists => Conflict("Item with this key already exists"),
                _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
            };
        }
    }
}