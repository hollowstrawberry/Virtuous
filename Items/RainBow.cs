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

            DisplayName.AddTranslation(GameCulture.Spanish, "Arco Iris");
            Tooltip.AddTranslation(GameCulture.Spanish, "Dispara al cielo para hacer llover destrucción sobre tus enemigos");

            DisplayName.AddTranslation(GameCulture.Russian, "Радуга");
			Tooltip.AddTranslation(GameCulture.Russian, "Пустите стрелу вверх для навесной атаки");

            DisplayName.AddTranslation(GameCulture.Chinese, "彩虹");
            Tooltip.AddTranslation(GameCulture.Chinese, "向天空射击会降临彩虹箭雨");
        }


        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 90;
            item.useStyle = 5;
            item.UseSound = SoundID.Item5;
            item.damage = 300;
            item.crit = 10;
            item.knockBack = 6f;
            item.shoot = mod.ProjectileType<RainbowArrow>();
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 12f;
            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 10;
            item.value = Item.sellPrice(0, 50, 0, 0);

            //Replaced in CanUseItem
            item.useTime = 30;
            item.useAnimation = item.useTime;
        }


        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);


        private bool ShootingWhiteArrow(Player player)
        {
            return (Main.MouseWorld - player.MountedCenter).Normalized().Y < -0.96f; // Direction is roughly straight up
        }


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            // Shoots white arrows slower
            item.useTime = ShootingWhiteArrow(player) ? 25 : 13;
            item.useAnimation = item.useTime;

            base.GetWeaponDamage(player, ref damage);
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = mod.ProjectileType<RainbowArrow>();
            
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
            var arrow = proj.modProjectile as RainbowArrow;
            arrow.Mode = mode;
            arrow.ColorId = NextColor;
            return false;
        }
        

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
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
