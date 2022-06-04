using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    public class RainBow : ModItem
    {
        // Next arrow color being shot by the bow, from 0 to 11
        private int NextColor { get; set; } = 0;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow");
            Tooltip.SetDefault("Shoot straight up to rain down on your foes");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Arco Iris");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Dispara al cielo para hacer llover destrucción sobre tus enemigos");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Радуга");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Пустите стрелу вверх для навесной атаки");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "彩虹");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "向天空射击会降临彩虹箭雨");
        }


        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 90;
            Item.useStyle = 5;
            Item.UseSound = SoundID.Item5;
            Item.damage = 300;
            Item.crit = 10;
            Item.knockBack = 6f;
            Item.shoot = Mod.Find<ModProjectile>(nameof(RainbowArrow)).Type;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 12f;
            Item.ranged = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.rare = 10;
            Item.value = Item.sellPrice(0, 50, 0, 0);

            //Replaced in CanUseItem
            Item.useTime = 30;
            Item.useAnimation = Item.useTime;
        }


        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);


        private bool ShootingWhiteArrow(Player player)
        {
            return (Main.MouseWorld - player.MountedCenter).Normalized().Y < -0.96f; // Direction is roughly straight up
        }


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            // Shoots white arrows slower
            Item.useTime = ShootingWhiteArrow(player) ? 25 : 13;
            Item.useAnimation = Item.useTime;

            base.GetWeaponDamage(player, ref damage);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Mod.Find<ModProjectile>(nameof(RainbowArrow)).Type;
            
            RainbowArrow.ArrowMode mode;
            if (ShootingWhiteArrow(player))
            {
                mode = RainbowArrow.ArrowMode.White;
            }
            else
            {
                mode = RainbowArrow.ArrowMode.Normal;
                if (NextColor < 11) NextColor++;
                else NextColor = 0;
            }

            var proj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            var arrow = proj.ModProjectile as RainbowArrow;
            arrow.Mode = mode;
            arrow.ColorId = NextColor;
            proj.netUpdate = true;
            return false;
        }
        

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.DaedalusStormbow);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.Diamond);
            recipe.AddIngredient(ItemID.Ruby);
            recipe.AddIngredient(ItemID.Emerald);
            recipe.AddIngredient(ItemID.Sapphire);
            recipe.AddIngredient(ItemID.Topaz);
            recipe.AddIngredient(ItemID.Amethyst);
            recipe.AddIngredient(ItemID.Amber);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
