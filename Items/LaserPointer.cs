using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Virtuous.Items
{
    class LaserPointer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Pointer");
            DisplayName.AddTranslation(GameCulture.Spanish, "Puntero Láser");
            //DisplayName.AddTranslation(GameCulture.Russian, "");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 8;
            item.useStyle = 5;
            item.useTime = 1;
            item.useAnimation = 5;
            item.shoot = mod.ProjectileType<Projectiles.ProjLaserPointer>();
            item.autoReuse = true;
            item.shootSpeed = 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return null;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 2);
            recipe.AddIngredient(ItemID.Wire, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
