using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raifhack.SkepticalBeavers.Server.Model.Snapshots
{
    internal sealed class MenuDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<MenuItemDb> Items { get; set; }
    }
}