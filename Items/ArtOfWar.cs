using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;
using Terraria.DataStructures;

namespace Virtuous.Items
{
    public class ArtOfWar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Art of War");
            Tooltip.SetDefault("\"Appear strong when you are, in fact, strong.\"\nWar arrows penetrate armor");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "El Arte de la Guerra");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "\"Cuando conoces el cielo y la tierra, la victoria es inagotable.\"\n" +
                "Las flechas de guerra penetran la armadura enemiga");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Искусство Войны");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "\"Ты силён, когда ты силён.\"\nСтрелы Войны пробивают броню");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "战争艺术");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "\"当你真正强大时,再去证明你的强大.\"\n战争之箭会穿透护甲");
        }


        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 5;
            Item.useAnimation = 30;
            Item.damage = 42;
            Item.crit = 10;
            Item.knockBack = 3.5f;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 16f;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 25, 0, 0);
        }


        public override bool ConsumeItem(Player player) => player.itemAnimation % 2 == 0; // Every so many uses

        public override Vector2? HoldoutOffset() => new Vector2(-2, 0);


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            // Position is off the corner of the screen and velocity points toward the mouse
            Vector2 basePosition = player.Center + new Vector2(-player.direction * (Main.screenWidth / 2), -(Main.screenHeight / 2 + 100));
            Vector2 baseVelocity = (Main.MouseWorld - basePosition).OfLength(Item.shootSpeed);

            int projAmount = Main.rand.Next(2, 6);
            for (int i = 0; i < projAmount; i++)
            {
                int newType = Main.rand.NextBool(2) ? type : Mod.Find<ModProjectile>(nameof(WarArrow)).Type; // Arrows can be replaced by the special type
                var velocityRotation = newType switch
                {
                    ProjectileID.JestersArrow => 0,
                    ProjectileID.HolyArrow => 7.ToRadians(),
                    _ => 10.ToRadians(),
                };
                Vector2 newVelocity = baseVelocity.RotatedBy(velocityRotation * -player.direction);
                Vector2 newPosition = basePosition + baseVelocity.Perpendicular(Main.rand.Next(150), Main.rand.NextBool(2)); // Random offset in either direction

                var proj = Projectile.NewProjectileDirect(null, newPosition, newVelocity, newType, damage, knockBack, player.whoAmI);
                proj.tileCollide = false;
                proj.noDropItem = true;
                proj.netUpdate = true;

                var modProj = proj.GetGlobalProjectile<VirtuousProjectile>();
                modProj.artOfWar = true;
                modProj.collidePositionY = player.position.Y;
            }

            SoundEngine.PlaySound(SoundID.Item5, basePosition);

            //if (player.itemAnimation >= item.useAnimation - item.useTime) // If I wanted to make it shoot arrows normally as well
            //{
            //    Main.PlaySound(SoundID.Item5, position);
            //    return true;
            //}

            return false;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Tsunami)
                .AddIngredient(ItemID.FragmentVortex, 10)
                .AddIngredient(ItemID.DynastyWood, 30)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}