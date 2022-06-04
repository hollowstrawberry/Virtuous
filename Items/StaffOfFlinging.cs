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
            Item.staff[Item.type] = true;

            DisplayName.SetDefault("Staff of Flinging");
            Tooltip.SetDefault("Ground-born enemies will take damage from the fall");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Bastón del Lanzamiento");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Los enemigos terrestres reciben daño por caída");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Посох Броска");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Наземные враги получают урон от падения");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "抛掷法杖");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "地面上的敌人扔上天,使它们摔死");
        }


        public override void SetDefaults()
        {
            Item.width  = 64;
            Item.height = 64;
            Item.useStyle = 5;
            Item.useTime = 20;
            Item.useAnimation = Item.useTime;
            Item.shoot = Mod.Find<ModProjectile>(nameof(ProjFlinging)).Type;
            Item.UseSound = SoundID.Item8;
            Item.damage = 60;
            Item.crit = 15;
            Item.knockBack = 15f;
            Item.mana = 15;
            Item.shootSpeed = 1;
            Item.magic = true;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.rare = 7;
            Item.value = Item.sellPrice(0, 10, 0, 0);
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = tooltips.FirstOrDefault(x => x.mod == "Terraria" && x.Name == "Knockback");
            if (line != null)
            {
                if (Language.ActiveCulture == GameCulture.FromCultureName(GameCulture.CultureName.Spanish))
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
            var recipe = new ModRecipe(Mod);
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
