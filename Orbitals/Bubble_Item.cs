using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Bubble_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
            Tooltip.SetDefault("\"Stay out of my personal space\"\nThe bubble repels enemies and raises defense");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.useStyle = 2;
            item.useTime = 20;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.shoot = 1;
            item.mana = 40;
            item.noMelee = true;
            item.autoReuse = false;
            item.rare = 5;
            item.value = Item.sellPrice(0, 15, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.Bubble;
            orbitalItem.duration = 20 * 60;
            orbitalItem.amount = 1;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 100);
            recipe.AddIngredient(ItemID.WaterBucket);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

