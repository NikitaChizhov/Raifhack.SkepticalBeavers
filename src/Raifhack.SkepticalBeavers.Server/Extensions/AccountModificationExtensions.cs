using System;
using System.Collections.Immutable;
using System.Linq;
using Raifhack.SkepticalBeavers.Server.Contracts.Requests;
using Raifhack.SkepticalBeavers.Server.Contracts.Responses;
using Raifhack.SkepticalBeavers.Server.Exceptions;
using Raifhack.SkepticalBeavers.Server.Model;
using Raifhack.SkepticalBeavers.Server.Model.Aggregates;
using Raifhack.SkepticalBeavers.Server.Model.Events;
using MenuItem = Raifhack.SkepticalBeavers.Server.Contracts.SupportingTypes.MenuItem;

namespace Raifhack.SkepticalBeavers.Server.Extensions
{
    internal static class AccountModificationExtensions
    {
        public static AccountModificationResult CreateMenu(this Account account, CreateMenuRequest request, out MenuFullResponse response)
        {
            var menuEvent = new MenuCreated
            {
                Name = request.Name,
                MenuId = Guid.NewGuid()
            };

            var result = account.ApplyWithResult(menuEvent);
            response = account.GetMenu(menuEvent.MenuId) ?? throw new ConventionViolationException(
                "Restaurant was just created, we should be able to always get it");
            return result;
        }

        public static AccountModificationResult DeleteMenu(this Account account, Guid menuId, out MenuShortResponse? response)
        {
            response = account.GetMenu(menuId);

            // even if menu was not found, record event anyway
            var menuEvent = new MenuDeleted
            {
                MenuId = menuId
            };
            return account.ApplyWithResult(menuEvent);
        }

        public static AccountModificationResult UpdateMenu(this Account account, Guid menuId, UpdateMenuRequest request, out MenuFullResponse? response)
        {
            var menu = account.Menus.FirstOrDefault(m => m.Id == menuId);
            if (menu == null)
            {
                response = null;
                return AccountModificationResult.MenuNotFound;
            }

            if (request.Delete != null)
            {
                foreach (var e in request.Delete
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .Select(key => new MenuItemRemoved {ItemKey = key, MenuId = menuId}))
                {
                    // Ideally this is the reason why there needs to be different result types for client api and internal
                    // aggregate modification api. This method is an example of a bad pattern (normally you would
                    // want to have a different result types for internal aggregate api and higher level api).
                    // But i am cutting corners and this project is about different thing, so i will allow myself to be lazy
                    account.Apply(e);
                }
            }

            if (request.Create != null)
            {
                foreach (var e in request.Create
                    .Select(item => new MenuItemAdded
                    {
                        MenuId = menuId,
                        Item = new Model.MenuItem
                        {
                            Key = item.Key,
                            Name = item.Name,
                            Description = item.Description,
                            Picture = item.Picture,
                            Price = item.Price
                        }
                    }))
                {
                    // same here
                    account.Apply(e);
                }
            }

            response = account.GetMenu(menuId);
            return AccountModificationResult.Success;
        }

        private static MenuFullResponse? GetMenu(this Account account, Guid menuId)
        {
            var menu = account.Menus.FirstOrDefault(m => m.Id == menuId);

            if (menu == null) return null;

            return new MenuFullResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Items = menu.Items.Select(i => new MenuItem
                {
                    Key = i.Key,
                    Name = i.Name,
                    Description = i.Description,
                    Picture = i.Picture,
                    Price = i.Price
                }).ToImmutableList()
            };
        }

        public static AccountModificationResult CreateRestaurant(this Account account, CreateRestaurantRequest request, out RestaurantResponse response)
        {
            var restaurantEvent = new RestaurantCreated
            {
                RestaurantId = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                MenuId = request.MenuId
            };

            var result = account.ApplyWithResult(restaurantEvent);
            response = account.GetRestaurant(restaurantEvent.RestaurantId) ?? throw new ConventionViolationException(
                "Restaurant was just created, we should be able to always get it");
            return result;
        }

        public static AccountModificationResult UpdateRestaurant(this Account account, Guid restaurantId, UpdateRestaurantRequest request, out RestaurantResponse? response)
        {
            var restaurantEvent = new RestaurantMenuSet
            {
                RestaurantId = restaurantId,
                MenuId = request.MenuId
            };

            var result = account.ApplyWithResult(restaurantEvent);
            response = account.GetRestaurant(restaurantEvent.RestaurantId);
            return result;
        }

        public static AccountModificationResult DeleteRestaurant(this Account account, Guid restaurantId, out RestaurantResponse? response)
        {
            response = account.GetRestaurant(restaurantId);
            if(response != null) response.Menu = null;

            // event if restaurant is not found, record event anyway
            var restaurantEvent = new RestaurantDeleted
            {
                RestaurantId = restaurantId
            };

            return account.ApplyWithResult(restaurantEvent);
        }

        private static RestaurantResponse? GetRestaurant(this Account account, Guid restaurantId)
        {
            var restaurant = account.Restaurants.FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant == null) return null;

            var menu = account.Menus.FirstOrDefault(m => m.Id == restaurant.MenuId);
            return new RestaurantResponse
            {
                Id = restaurant.Id,
                Address = restaurant.Address,
                Name = restaurant.Name,
                Menu = menu == null ? null : new MenuShortResponse
                {
                    Id = menu.Id,
                    Name = menu.Name
                }
            };
        }
    }
}