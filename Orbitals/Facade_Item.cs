using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Facade_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Facade");
            Tooltip.SetDefault("Summons barriers to protect you for a short time\nAligns with either magic or melee users");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 30;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 20;
            item.knockBack = 2.0f;
            item.shoot = 1;
            item.mana = 30;
            item.useTurn = true;
            item.noMelee = true;
            item.autoReuse = false;
            item.rare = 4;
            item.value = Item.sellPrice(0, 5, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.Facade;
            orbitalItem.duration = 15 * 60;
            orbitalItem.amount = 4;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

