using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class HolyLight_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Circle of Protection");
            Tooltip.SetDefault("Holy lights surround you and increase life regeneration\nAligns with either magic or melee users");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 40;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 100;
            item.crit = 0;
            item.knockBack = 3f;
            item.shoot = 1;
            item.mana = 60;
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 8;
            item.value = Item.sellPrice(0, 40, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.HolyLight;
            orbitalItem.duration = 30 * 60 + HolyLight_Proj.DyingTime;
            orbitalItem.amount = 6;
        }
    }
}
