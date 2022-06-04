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
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Puntero Láser");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Лазерный Указатель");
            //DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "");
        }


        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 8;
            Item.useStyle = 5;
            Item.useTime = 1;
            Item.useAnimation = 5;
            Item.shoot = Mod.Find<ModProjectile>(nameof(ProjLaserPointer)).Type;
            Item.autoReuse = true;
            Item.shootSpeed = 1;
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var proj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            var laser = proj.ModProjectile as ProjLaserPointer;
            laser.LaserColor = LaserColor;
            return false;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddRecipeGroup("IronBar", 2);
            recipe.AddIngredient(ItemID.Wire, 1);
            recipe.AddIngredient(ColorMaterial, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
