using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Facade_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Facade");
            Tooltip.SetDefault("Summons barriers to protect you for a short time\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Facade;
            duration = 15 * 60;
            amount = 4;

            item.width = 30;
            item.height = 30;
            item.damage = 20;
            item.knockBack = 2.0f;
            item.mana = 30;
            item.rare = 4;
            item.value = Item.sellPrice(0, 5, 0, 0);
        }
    }
}

