using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Shuriken_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
            Tooltip.SetDefault("Shurikens defend you, raising melee speed and mana regeneration\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Shuriken;
            duration = 35 * 60;
            amount = 3;

            item.width = 30;
            item.height = 30;
            item.damage = 190;
            item.knockBack = 6.2f;
            item.mana = 70;
            item.rare = 9;
            item.value = Item.sellPrice(0, 60, 0, 0);
        }
    }
}
