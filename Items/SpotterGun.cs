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

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Pistola Señuelo");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "\"No lo verán venir\"");

            // TODO: Missing Russian translation
            //DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "");
            //Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "零点");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "它们将看不到未来");
        }


        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 24;
            Item.useStyle = 5;
            Item.useTime = 15;
            Item.useAnimation = Item.useTime;
            Item.UseSound = SoundID.Item40;
            Item.damage = 150;
            Item.crit = 15;
            Item.knockBack = 2.5f;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 12f;
            Item.ranged = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.rare = 8;
            Item.value = Item.sellPrice(0, 15, 0, 0);
        }


        public override Vector2? HoldoutOffset() => new Vector2(5, 0);


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var proj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            proj.GetGlobalProjectile<VirtuousProjectile>().spotter = true; // Projectile can spawn a Crosshair
            proj.netUpdate = true;

            return false;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.Nanites, 50);
            recipe.AddIngredient(ItemID.Ectoplasm, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
