using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Sailspike_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
            Tooltip.SetDefault("Summons a spike for a short time\nAligns with either magic or melee users");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 30;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 15;
            item.knockBack = 1f;
            item.shoot = 1;
            item.mana = 15;
            item.useTurn = true;
            item.noMelee = true;
            item.autoReuse = false;
            item.rare = 3;
            item.value = Item.sellPrice(0, 2, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.Sailspike;
            orbitalItem.duration = 5 * 60 + Sailspike_Proj.DyingTime;
            orbitalItem.amount = 1;
        }
    }
}
