using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Shuriken_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
            Tooltip.SetDefault("Shurikens defend you, raising melee speed and mana regeneration\nAligns with either magic or melee users");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 40;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 170;
            item.crit = 0;
            item.knockBack = 5f;
            item.shoot = 1;
            item.mana = 70;
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 9;
            item.value = Item.sellPrice(0, 60, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.Shuriken;
            orbitalItem.duration = 35 * 60 + Shuriken_Proj.DyingTime;
            orbitalItem.amount = 3;
        }
    }
}
