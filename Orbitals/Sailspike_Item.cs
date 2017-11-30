using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Sailspike_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
            Tooltip.SetDefault("Summons a spike for a short time\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Sailspike;
            duration = 5 * 60;
            amount = 1;

            item.width = 30;
            item.height = 30;
            item.damage = 15;
            item.knockBack = 1f;
            item.mana = 15;
            item.rare = 3;
            item.value = Item.sellPrice(0, 2, 0, 0);
        }
    }
}
