using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Virtuous.Projectiles;

namespace Virtuous.Items.LaserPointer
{
    class LaserPointerWhite : ModItem
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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, ProjLaserPointer.White);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 2);
            recipe.AddIngredient(ItemID.Wire, 1);
            recipe.AddIngredient(ItemID.DiamondGemsparkBlock, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
