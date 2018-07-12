using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Items
{
    class SpotterGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spotter");
            Tooltip.SetDefault("\"They won't see it coming\"");

            DisplayName.AddTranslation(GameCulture.Spanish, "Pistola Señuelo");
            Tooltip.AddTranslation(GameCulture.Spanish, "\"No lo verán venir\"");

            // TODO: Missing Russian translation
            //DisplayName.AddTranslation(GameCulture.Russian, "");
            //Tooltip.AddTranslation(GameCulture.Russian, "");

            DisplayName.AddTranslation(GameCulture.Chinese, "零点");
            Tooltip.AddTranslation(GameCulture.Chinese, "它们将看不到未来");
        }


        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 24;
            item.useStyle = 5;
            item.useTime = 15;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item40;
            item.damage = 150;
            item.crit = 15;
            item.knockBack = 2.5f;
            item.shoot = 1;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 12f;
            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 8;
            item.value = Item.sellPrice(0, 15, 0, 0);
        }


        public override Vector2? HoldoutOffset() => new Vector2(5, 0);


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var proj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            proj.GetGlobalProjectile<VirtuousProjectile>().spotter = true; // Projectile can spawn a Crosshair

            return false;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.Nanites, 50);
            recipe.AddIngredient(ItemID.Ectoplasm, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
