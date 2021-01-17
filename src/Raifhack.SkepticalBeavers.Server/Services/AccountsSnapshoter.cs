using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using Raifhack.SkepticalBeavers.Server.Exceptions;
using Raifhack.SkepticalBeavers.Server.Model.Events;
using Raifhack.SkepticalBeavers.Server.Model.Snapshots;

namespace Raifhack.SkepticalBeavers.Server.Services
{
    internal sealed class AccountsSnapshoter : IHostedService
    {
        private readonly EventStoreClient _client;

        private readonly IServiceProvider _sp;

        private readonly ILogger<AccountsSnapshoter> _logger;

        public AccountsSnapshoter(EventStoreClient client, IServiceProvider sp, ILogger<AccountsSnapshoter> logger)
        {
            _client = client;
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.SubscribeToAllAsync(Position.Start,
                HandleAsync,
                subscriptionDropped: (subscription, reason, e) =>
                {
                    _logger.LogInformation($"Subscription dropped. Reason: {reason.GetDisplayName()}");
                    if(e != null) _logger.LogError(e.ToString());
                },
                cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        [SuppressMessage("ReSharper", "MethodHasAsyncOverloadWithCancellation",
            Justification = "Using AddAsync and others is not recommended by Entity Framework docs")]
        private async Task HandleAsync(StreamSubscription subscription, ResolvedEvent @event,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(@event.OriginalStreamId, out var streamId)) return;

            using var scope = _sp.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<SnapshotContext>();

            var e = AggregateRepository.DeserializeEvent(@event);

            switch (e)
            {
                case AccountCreated accountCreated:
                {
                    if (context.Accounts.Any(acc => acc.Id == accountCreated.AccountId)) break;

                    var account = new AccountDb
                    {
                        Id = accountCreated.AccountId,
                        Name = accountCreated.Name
                    };

                    context.Accounts.Add(account);
                    break;
                }
                case MenuCreated menuCreated:
                {
                    var account = await context.Accounts
                        .Include(a => a.Menus)
                        .FirstOrDefaultAsync(a => a.Id == streamId, cancellationToken);

                    if(account == null) throw new ConventionViolationException("It should not have been possible to create a menu without an account");

                    if (account.Menus.All(m => m.Id != menuCreated.MenuId))
                    {
                        account.Menus.Add(new MenuDb { Id = menuCreated.MenuId, Name = menuCreated.Name });
                    }
                    break;
                }
                case MenuDeleted menuDeleted:
                {
                    var account = await context.Accounts
                        .Include(a => a.Menus)
                        .FirstOrDefaultAsync(a => a.Id == streamId, cancellationToken);

                    if (account == null) throw new ConventionViolationException("It should not have been possible to delete a menu without an account");

                    var menuToRemove = await account.Menus
                        .AsQueryable()
                        .FirstOrDefaultAsync(m => m.Id == menuDeleted.MenuId, cancellationToken);

                    if(menuToRemove != null)
                    {
                        context.Menus.Remove(menuToRemove);
                    }
                    break;
                }
                case MenuItemAdded menuItemAdded:
                {
                    var account = await context.Accounts
                        .Include(a => a.Menus)
                        .ThenInclude(m => m.Items)
                        .FirstOrDefaultAsync(a => a.Id == streamId, cancellationToken);
                    if (account == null) throw new ConventionViolationException("It should not have been possible to create a menu item without an account");

                    var menu = await account.Menus.AsQueryable()
                        .FirstOrDefaultAsync(m => m.Id == menuItemAdded.MenuId, cancellationToken);

                    if (menu == null) throw new ConventionViolationException("It should not have been possible to create a menu item without an menu");

                    var itemToOverwrite = await menu.Items.AsQueryable()
                        .FirstOrDefaultAsync(mi => mi.Key == menuItemAdded.Item.Key, cancellationToken);

                    if (itemToOverwrite != null)
                    {
                        menu.Items.Remove(itemToOverwrite);
                        context.MenuItems.Remove(itemToOverwrite);
                    }

                    var newItem = new MenuItemDb
                    {
                        Key = menuItemAdded.Item.Key,
                        Name = menuItemAdded.Item.Name,
                        Description = menuItemAdded.Item.Description,
                        Price = menuItemAdded.Item.Price,
                        Picture = menuItemAdded.Item.Picture
                    };

                    context.MenuItems.Add(newItem);
                    menu.Items.Add(newItem);
                    break;
                }
                case MenuItemRemoved menuItemRemoved:
                {
                    var account = await context.Accounts
                        .Include(a => a.Menus)
                        .ThenInclude(m => m.Items)
                        .FirstOrDefaultAsync(a => a.Id == streamId, cancellationToken);
                    if (account == null) throw new ConventionViolationException("It should not have been possible to create a menu item without an account");

                    var menu = await account.Menus.AsQueryable()
                        .FirstOrDefaultAsync(m => m.Id == menuItemRemoved.MenuId, cancellationToken);

                    if (menu == null) throw new ConventionViolationException("It should not have been possible to create a menu item without an menu");

                    var itemToRemove = await menu.Items.AsQueryable()
                        .FirstOrDefaultAsync(mi => mi.Key == menuItemRemoved.ItemKey, cancellationToken);

                    if (itemToRemove != null)
                    {
                        menu.Items.Remove(itemToRemove);
                        context.MenuItems.Remove(itemToRemove);
                    }
                    break;
                }
                case RestaurantCreated restaurantCreated:
                {
                    var account = await context.Accounts
                        .Include(a => a.Restaurants)
                        .FirstOrDefaultAsync(a => a.Id == streamId, cancellationToken);

                    if(account == null) throw new ConventionViolationException("It should not have been possible to create a restaurant without an account");

                    if (account.Restaurants.Any(r => r.Id == restaurantCreated.RestaurantId))
                    {
                        break;
                    }

                    var restaurant = new RestaurantDb
                    {
                        Id = restaurantCreated.RestaurantId,
                        Name = restaurantCreated.Name,
                        City = restaurantCreated.Address.City,
                        Street = restaurantCreated.Address.Street,
                        HouseNumber = restaurantCreated.Address.HouseNumber
                    };

                    if (restaurantCreated.MenuId != null)
                    {
                        restaurant.Menu = await context.Menus.FindAsync(restaurantCreated.MenuId);
                    }

                    context.Restaurants.Add(restaurant);
                    account.Restaurants.Add(restaurant);
                    break;
                }
                case RestaurantDeleted restaurantDeleted:
                {
                    var account = await context.Accounts
                        .Include(a => a.Restaurants)
                        .FirstOrDefaultAsync(a => a.Id == streamId, cancellationToken);

                    if(account == null) throw new ConventionViolationException("It should not have been possible to delete a restaurant without an account");

                    var restaurantToRemove = await account.Restaurants
                        .AsQueryable()
                        .FirstOrDefaultAsync(m => m.Id == restaurantDeleted.RestaurantId, cancellationToken);

                    context.Restaurants.Remove(restaurantToRemove);
                    break;
                }
                case RestaurantMenuSet restaurantMenuSet:
                {
                    var account = await context.Accounts
                        .Include(a => a.Restaurants)
                        .FirstOrDefaultAsync(a => a.Id == streamId, cancellationToken);

                    if(account == null) throw new ConventionViolationException("It should not have been possible to add a menu without an account");

                    var restaurant = account.Restaurants
                        .FirstOrDefault(r => r.Id == restaurantMenuSet.RestaurantId);

                    if(restaurant == null) throw new ConventionViolationException("It should not have been possible to add a menu to a non existent restaurant");

                    var menu = await context.Menus.AsQueryable()
                        .FirstOrDefaultAsync(m => m.Id == restaurantMenuSet.MenuId, cancellationToken);

                    if(menu == null) throw new ConventionViolationException("It should not have been possible to add a non existent menu");

                    restaurant.Menu = menu;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(e));
            }

            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}