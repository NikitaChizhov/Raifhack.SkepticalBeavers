using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Raifhack.SkepticalBeavers.Server.Model.Events;

namespace Raifhack.SkepticalBeavers.Server.Model
{
    internal sealed class Menu
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<MenuItem> Items { get; set; }

        public Menu()
        {
        }

        public Menu(MenuCreated e)
        {
            Id = e.MenuId;
            Name = e.Name;
            Items = new List<MenuItem>();
        }
    }
}