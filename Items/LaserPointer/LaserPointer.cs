using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items.LaserPointer
{
    class LaserPointer : ModItem
    {
        protected virtual LaserColor LaserColor => LaserColor.Red;
        protected virtual short ColorMaterial => ItemID.RubyGemsparkBlock;



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Pointer");
            DisplayName.AddTranslation(GameCulture.Spanish, "Puntero Láser");
            DisplayName.AddTranslation(GameCulture.Russian, "Лазерный Указатель");
            //DisplayName.AddTranslation(GameCulture.Chinese, "");
        }


        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 8;
            item.useStyle = 5;
            item.useTime = 1;
            item.useAnimation = 5;
            item.shoot = mod.ProjectileType<ProjLaserPointer>();
            item.autoReuse = true;
            item.shootSpeed = 1;
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var proj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            var laser = proj.modProjectile as ProjLaserPointer;
            laser.LaserColor = LaserColor;
            return false;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 2);
            recipe.AddIngredient(ItemID.Wire, 1);
            recipe.AddIngredient(ColorMaterial, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
