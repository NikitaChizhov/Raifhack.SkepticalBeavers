using System;
using System.Collections.Generic;
using System.Linq;
using Raifhack.SkepticalBeavers.Server.Model.Events;

namespace Raifhack.SkepticalBeavers.Server.Model.Aggregates
{
    internal sealed class Account : AggregateBase<AccountModificationResult>
    {
        public string Name { get; set; }

        public IList<Restaurant> Restaurants { get; } = new List<Restaurant>();

        public IList<Menu> Menus { get; } = new List<Menu>();

        public Account()
        {
        }

        public Account(AccountCreated e)
        {
            Apply(e);
        }

        /// <inheritdoc />
        protected override AccountModificationResult ModifyStateWithResult(EventBase @event)
        {
            return @event switch
            {
                AccountCreated e => OnCreated(e),
                RestaurantCreated e => OnRestaurantCreated(e),
                RestaurantDeleted e => OnRestaurantDeleted(e),
                MenuCreated e => OnMenuCreated(e),
                MenuDeleted e => OnMenuDeleted(e),
                MenuItemAdded e => OnMenuItemAdded(e),
                MenuItemRemoved e => OnMenuItemRemoved(e),
                RestaurantMenuSet e => OnRestaurantMenuSet(e),
                _ => throw new ArgumentOutOfRangeException(nameof(@event))
            };
        }

        private AccountModificationResult OnCreated(AccountCreated e)
        {
            if (Version != -1) return AccountModificationResult.AccountAlreadyExists;

            Id = e.AccountId;
            Name = e.Name;
            return Success;
        }

        private AccountModificationResult OnRestaurantCreated(RestaurantCreated e)
        {
            if (Restaurants.Any(r => r.Id == e.RestaurantId)) return AccountModificationResult.RestaurantAlreadyExists;

            Restaurants.Add(new Restaurant(e));
            return Success;
        }

        private AccountModificationResult OnRestaurantDeleted(RestaurantDeleted e)
        {
            var toDelete = Restaurants.FirstOrDefault(r => r.Id == e.RestaurantId);
            if (toDelete == null) return RestaurantNotFound;

            Restaurants.Remove(toDelete);
            return Success;
        }

        private AccountModificationResult OnMenuCreated(MenuCreated e)
        {
            if (Menus.Any(m => m.Id == e.MenuId)) return AccountModificationResult.MenuAlreadyExists;

            Menus.Add(new Menu(e));
            return Success;
        }

        private AccountModificationResult OnMenuDeleted(MenuDeleted e)
        {
            var toDelete = Menus.FirstOrDefault(m => m.Id == e.MenuId);
            if (toDelete == null) return MenuNotFound;

            Menus.Remove(toDelete);
            return Success;
        }

        private AccountModificationResult OnMenuItemAdded(MenuItemAdded e)
        {
            var menuToUpdate = Menus.FirstOrDefault(m => m.Id == e.MenuId);
            if (menuToUpdate == null) return MenuNotFound;

            if (menuToUpdate.Items.Any(i => i.Key == e.Item.Key)) return AccountModificationResult.ItemKeyAlreadyExists;

            menuToUpdate.Items.Add(e.Item);
            return Success;
        }

        private AccountModificationResult OnMenuItemRemoved(MenuItemRemoved e)
        {
            var menuToUpdate = Menus.FirstOrDefault(m => m.Id == e.MenuId);
            if (menuToUpdate == null) return MenuNotFound;

            var itemToRemove = menuToUpdate.Items.FirstOrDefault(i => i.Key == e.ItemKey);
            if (itemToRemove == null) return AccountModificationResult.ItemNotFound;

            menuToUpdate.Items.Remove(itemToRemove);
            return Success;
        }

        private AccountModificationResult OnRestaurantMenuSet(RestaurantMenuSet e)
        {
            if (e.MenuId.HasValue && Menus.All(m => m.Id != e.MenuId.Value)) return MenuNotFound;

            var toUpdate = Restaurants.FirstOrDefault(r => r.Id == e.RestaurantId);
            if (toUpdate == null) return RestaurantNotFound;

            toUpdate.MenuId = e.MenuId;
            return Success;
        }

        private static AccountModificationResult Success => AccountModificationResult.Success;
        private static AccountModificationResult RestaurantNotFound => AccountModificationResult.RestaurantNotFound;
        private static AccountModificationResult MenuNotFound => AccountModificationResult.MenuNotFound;
    }
}