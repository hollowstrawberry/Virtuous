using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    public class StaffOfFlinging : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;

            DisplayName.SetDefault("Staff of Flinging");
            Tooltip.SetDefault("Ground-born enemies will take damage from the fall");

            DisplayName.AddTranslation(GameCulture.Spanish, "Bastón del Lanzamiento");
            Tooltip.AddTranslation(GameCulture.Spanish, "Los enemigos terrestres reciben daño por caída");

            DisplayName.AddTranslation(GameCulture.Russian, "Посох Броска");
            Tooltip.AddTranslation(GameCulture.Russian, "Наземные враги получают урон от падения");

            DisplayName.AddTranslation(GameCulture.Chinese, "抛掷法杖");
            Tooltip.AddTranslation(GameCulture.Chinese, "地面上的敌人扔上天,使它们摔死");
        }


        public override void SetDefaults()
        {
            item.width  = 64;
            item.height = 64;
            item.useStyle = 5;
            item.useTime = 20;
            item.useAnimation = item.useTime;
            item.shoot = mod.ProjectileType<ProjFlinging>();
            item.UseSound = SoundID.Item8;
            item.damage = 60;
            item.crit = 15;
            item.knockBack = 15f;
            item.mana = 15;
            item.shootSpeed = 1;
            item.magic = true;
            item.noMelee = true;
            item.autoReuse = false;
            item.rare = 7;
            item.value = Item.sellPrice(0, 10, 0, 0);
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = tooltips.FirstOrDefault(x => x.mod == "Terraria" && x.Name == "Knockback");
            if (line != null)
            {
                if (Language.ActiveCulture == GameCulture.Spanish)
                    line.text = "Retroceso Espacial";
                else
                    line.text ="Flying Knockback";
            }
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            return false;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtRod);
            recipe.AddIngredient(ItemID.GiantHarpyFeather);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
