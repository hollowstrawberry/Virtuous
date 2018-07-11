using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    [AutoloadEquip(EquipType.HandsOn, EquipType.HandsOff)]
    public class FlurryNova : ModItem
    {
        private const int BasePosX = 22;

        private static readonly Vector2[] FistPositions = new[] {
            new Vector2(BasePosX + 17,   0),
            new Vector2(BasePosX + 14, +10),
            new Vector2(BasePosX +  8, +20),
            new Vector2(BasePosX +  4, +30),
            new Vector2(BasePosX +  0, +40),
            new Vector2(BasePosX + 14, -10),
            new Vector2(BasePosX +  8, -20),
            new Vector2(BasePosX +  4, -30),
            new Vector2(BasePosX +  0, -40)
        };




        // Stores the index of the previous fist position, so it won't be used twice in a row
        private int PreviousFist { get; set; } = -1; 


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flurry Nova");
            Tooltip.SetDefault("\"Over 9000 punches per hour\"\nRight Click for a boring punch");

            DisplayName.AddTranslation(GameCulture.Spanish, "Golpe Nova");
            Tooltip.AddTranslation(GameCulture.Spanish, "\"Más de 9000 golpes por hora\"\nHaz Click Derecho para un golpe normal");

            DisplayName.AddTranslation(GameCulture.Russian, "Шквал");
            Tooltip.AddTranslation(GameCulture.Russian, "\"9000 ударов в час\"\nПКМ для обычного удара");

            DisplayName.AddTranslation(GameCulture.Chinese, "疾风新星");
            Tooltip.AddTranslation(GameCulture.Chinese, "\"每小时拳击可超过9000次\"\n右键出\"石头\"");
        }


        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 36;
            item.UseSound = SoundID.Item1;
            item.crit = 20;
            item.knockBack = 3f;
            item.melee = true;
            item.autoReuse = true;
            item.rare = 9;
            item.value = Item.sellPrice(0, 20, 0, 0);
            
            // Replaced in SetUseStats
            item.useStyle = 1;
            item.useTime = 5;
            item.useAnimation = item.useTime;
            item.damage = 300;
            item.shoot = mod.ProjectileType<ProjFist>();
            item.noMelee = true;
            item.noUseGraphic = true;
        }


        public override bool AltFunctionUse(Player player) => true;
        

        private void SetUseStats(Player player)
        {
            //Left Click
            if (player.altFunctionUse != 2)
            {
                item.useStyle = 1;
                item.useTime = 6;
                item.useAnimation = item.useTime;
                item.damage = 300;
                item.shoot = mod.ProjectileType<ProjFist>();
                item.noMelee = true;
                item.noUseGraphic = true;
            }
            //Right Click
            else
            {
                item.useStyle = 3;
                item.useTime = 10;
                item.useAnimation = item.useTime;
                item.shoot = 0;
                item.damage = 900;
                item.noMelee = false;
                item.noUseGraphic = false;
            }
        }


        public override bool CanUseItem(Player player)
        {
            SetUseStats(player); // Sets stats before use

            // Equivalent to autoReuse being false, as that flag is bugged with alternate use
            if (player.altFunctionUse == 2 && !PlayerInput.Triggers.JustPressed.MouseRight) return false;

            return base.CanUseItem(player);
        }


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            SetUseStats(player); // Always displays the left-click values

            base.GetWeaponDamage(player, ref damage);
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 center = player.MountedCenter;
            Vector2 velocity = new Vector2(player.direction, 0); //Fist movement direction

            int nextFist = -1;
            for (int i = 0; i < 20; i++) // Makes x attempts at creating a projectile that the player can reach
            {
                do { nextFist = Main.rand.Next(FistPositions.Length); }
                while (nextFist == PreviousFist); // Can't match the position of the previous one

                var offset = FistPositions[nextFist];
                offset.X *= player.direction;
                position = center + offset;

                if (Collision.CanHit(center, 0, 0, position, 0, 0)) break;
            }

            PreviousFist = nextFist;
            Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
            return false;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.KOCannon);
            recipe.AddIngredient(ItemID.FragmentSolar, 8);
            recipe.AddIngredient(ItemID.FragmentStardust, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
