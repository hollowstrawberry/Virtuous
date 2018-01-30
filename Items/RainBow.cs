using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Projectiles;
using static Virtuous.Tools;

namespace Virtuous.Items
{
    public class RainBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow");
			DisplayName.AddTranslation(GameCulture.Russian, "Радуга");
            Tooltip.SetDefault("Shoot straight up to rain down on your foes");
			Tooltip.AddTranslation(GameCulture.Russian, "Пустите стрелу вверх для навесной атаки");
        }

        private int nextColor = 0; //Next arrow color being shot by the bow, from 0 to 11

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

            //Replaced by CanUseItem
            item.useTime = 30;
            item.useAnimation = item.useTime;
        }

        private bool IsWhiteArrow(Player player)
        {
            return (Main.MouseWorld - player.MountedCenter).Normalized().Y < -0.96f; //Direction is roughly straight up
        }

        public override bool CanUseItem(Player player)
        {
            //Detects a special white shot
            if(IsWhiteArrow(player))
            {
                item.useTime = 25; //Shoots slower
            }
            else
            {
                item.useTime = 13;
            }
            item.useAnimation = item.useTime;

            return base.CanUseItem(player); //Proceeds to check normally
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.mod == "Terraria" && line.Name == "UseSpeed")
                {
                    if (line.text.Contains("speed")) line.text = "Very fast speed"; //Only changes English, keeps the normal shot's speed in the tooltip
                }
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = mod.ProjectileType<RainbowArrow>(); //Replaces any arrows with the unique arrows
            
            int arrowMode;
            if (IsWhiteArrow(player))
            {
                arrowMode = RainbowArrow.White;
            }
            else
            {
                arrowMode = RainbowArrow.Normal;
                if (nextColor < 11) nextColor++;
                else nextColor = 0;
            }

            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, arrowMode, nextColor);
                
            return false; //Doesn't shoot normally
        }
        
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
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
