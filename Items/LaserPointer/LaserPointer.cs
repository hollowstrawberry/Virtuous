using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;
using Terraria.DataStructures;

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


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            var laser = proj.ModProjectile as ProjLaserPointer;
            laser.LaserColor = LaserColor;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 2)
                .AddIngredient(ItemID.Wire, 1)
                .AddIngredient(ColorMaterial, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
