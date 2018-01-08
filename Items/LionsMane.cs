using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Projectiles;
using static Virtuous.Tools;

namespace Virtuous.Items
{
    public class LionsMane : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lion's Mane");
			DisplayName.AddTranslation(GameCulture.Russian, "Львиная Грива");
            Tooltip.SetDefault("Damage increases exponentially as it travels\nLeft Click for clockwise, Right Click for counter-clockwise");
			Tooltip.AddTranslation(GameCulture.Russian, "Урон увеличивается пропорционально времени полёта\nЛКМ - по часовой стрелке\nПКМ - против часовой стрелки");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useStyle = 5;
            item.useTime = 9;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 150;
            item.shoot = mod.ProjectileType<ProjLionsMane>();
            item.shootSpeed = 0f;
            item.crit = 5;
            item.knockBack = 7f;
            item.mana = 9;
            item.magic = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.channel = true;
            item.rare = 8;
            item.value = Item.sellPrice(0, 20, 0, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            HandleAltUseAnimation(player, item); //A trick to stop the bugged 1-tick delay between consecutive right-click uses of a weapon

            base.GetWeaponDamage(player, ref damage);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            const int distance = 5;
            int direction;

            float cursorAngle = (Main.MouseWorld - player.Center).ToRotation();
            if      (cursorAngle >= -135.ToRadians() && cursorAngle <=  -45.ToRadians()) direction = 1; //Up
            else if (cursorAngle >=  -45.ToRadians() && cursorAngle <=  +45.ToRadians()) direction = 2; //Right
            else if (cursorAngle >=  +45.ToRadians() && cursorAngle <= +135.ToRadians()) direction = 3; //Down
            else direction = 4; //Left

            if (player.altFunctionUse == 2) direction *= -1; //Right click, counter-clockwise

            Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI, /*ai[0] and ai[1]*/ distance, direction);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe;

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CursedFlames);
            recipe.AddIngredient(ItemID.FragmentNebula, 8);
            recipe.AddIngredient(ItemID.FragmentSolar, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldenShower);
            recipe.AddIngredient(ItemID.FragmentNebula, 8);
            recipe.AddIngredient(ItemID.FragmentSolar, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
