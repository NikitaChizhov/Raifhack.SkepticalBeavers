using Microsoft.EntityFrameworkCore;
using Raifhack.SkepticalBeavers.Server.Model.Snapshots;

namespace Raifhack.SkepticalBeavers.Server.Services
{
    internal sealed class SnapshotContext : DbContext
    {
        public SnapshotContext(DbContextOptions<SnapshotContext> options)
            : base(options)
        {
        }

        public DbSet<AccountDb> Accounts { get; set; }

        public DbSet<RestaurantDb> Restaurants { get; set; }

        public DbSet<MenuDb> Menus { get; set; }

        public DbSet<MenuItemDb> MenuItems { get; set; }
    }
}