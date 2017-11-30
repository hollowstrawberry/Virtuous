using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class HolyLight_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Circle of Protection");
            Tooltip.SetDefault("Holy lights surround you and increase life regeneration\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.HolyLight;
            duration = 30 * 60;
            amount = 6;

            item.width = 30;
            item.height = 30;
            item.damage = 100;
            item.knockBack = 3f;
            item.mana = 60;
            item.rare = 8;
            item.value = Item.sellPrice(0, 40, 0, 0);
        }
    }
}
