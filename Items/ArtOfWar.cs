using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    public class ArtOfWar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Art of War");
            Tooltip.SetDefault("\"Appear strong when you are, in fact, strong.\"\nWar arrows penetrate armor");

            DisplayName.AddTranslation(GameCulture.Spanish, "El Arte de la Guerra");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "\"Cuando conoces el cielo y la tierra, la victoria es inagotable.\"\n" +
                "Las flechas de guerra penetran la armadura enemiga");

            DisplayName.AddTranslation(GameCulture.Russian, "Искусство Войны");
            Tooltip.AddTranslation(GameCulture.Russian, "\"Ты силён, когда ты силён.\"\nСтрелы Войны пробивают броню");

            DisplayName.AddTranslation(GameCulture.Chinese, "战争艺术");
            Tooltip.AddTranslation(GameCulture.Chinese, "\"当你真正强大时,再去证明你的强大.\"\n战争之箭会穿透护甲");
        }


        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 70;
            item.useStyle = 5;
            item.useTime = 5;
            item.useAnimation = 30;
            item.damage = 42;
            item.crit = 10;
            item.knockBack = 3.5f;
            item.shoot = 1;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 16f;
            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 10;
            item.value = Item.sellPrice(0, 25, 0, 0);
        }


        public override bool ConsumeAmmo(Player player) => player.itemAnimation % 2 == 0; // Every so many uses

        public override Vector2? HoldoutOffset() => new Vector2(-2, 0);


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Position is off the corner of the screen and velocity points toward the mouse
            Vector2 basePosition = player.Center + new Vector2(-player.direction * (Main.screenWidth / 2), -(Main.screenHeight / 2 + 100));
            Vector2 baseVelocity = (Main.MouseWorld - basePosition).OfLength(item.shootSpeed);

            int projAmount = Main.rand.Next(2, 6);
            for (int i = 0; i < projAmount; i++)
            {
                int newType = Main.rand.OneIn(2) ? type : mod.ProjectileType<WarArrow>(); // Arrows can be replaced by the special type

                float velocityRotation; // Adjustment for accuracy
                switch (newType)
                {
                    case ProjectileID.JestersArrow: velocityRotation = 0; break;
                    case ProjectileID.HolyArrow:    velocityRotation = 7.ToRadians(); break;
                    default:                        velocityRotation = 10.ToRadians(); break;
                }

                Vector2 newVelocity = baseVelocity.RotatedBy(velocityRotation * -player.direction);
                Vector2 newPosition = basePosition + baseVelocity.Perpendicular(Main.rand.Next(150), Main.rand.OneIn(2)); // Random offset in either direction

                var proj = Projectile.NewProjectileDirect(newPosition, newVelocity, newType, damage, knockBack, player.whoAmI);
                proj.tileCollide = false;
                proj.noDropItem = true;
                proj.netUpdate = true;

                var modProj = proj.GetGlobalProjectile<VirtuousProjectile>();
                modProj.artOfWar = true;
                modProj.collidePositionY = player.position.Y;
            }

            Main.PlaySound(SoundID.Item5, basePosition);

            //if (player.itemAnimation >= item.useAnimation - item.useTime) // If I wanted to make it shoot arrows normally as well
            //{
            //    Main.PlaySound(SoundID.Item5, position);
            //    return true;
            //}

            return false;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Tsunami);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);
            recipe.AddIngredient(ItemID.DynastyWood, 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}