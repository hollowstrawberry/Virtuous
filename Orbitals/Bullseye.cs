using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Bullseye_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullseye");
            Tooltip.SetDefault("\"Love at first sight\"\nShoot through the magical sight for double ranged damage\nOther ranged shots are weaker");
            DisplayName.AddTranslation(GameCulture.Spanish, "Disparo Certero");
            Tooltip.AddTranslation(GameCulture.Spanish, "\"Amor a primera vista\"\nDispara por la mira mágica para doble daño a distancia\nOtros daños a distancia serán más débiles");
            DisplayName.AddTranslation(GameCulture.Russian, "Бычий Глаз");
            Tooltip.AddTranslation(GameCulture.Russian, "\"Любовь с первого взгляда\"\nУдваивает дальний урон, если стрелять через прицел\nОстальные снаряды будут слабее");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Bullseye;
            duration = 60 * 60;
            amount = 1;

            item.width = 30;
            item.height = 30;
            item.mana = 100;
            item.knockBack = 2.4f;
            item.rare = 8;
            item.value = Item.sellPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RifleScope);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);
            recipe.AddIngredient(ItemID.Amber, 5);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }


    public class Bullseye_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Bullseye;
        public override int FadeTime => 120;
        public override float BaseDistance => 40;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Sight");
			DisplayName.AddTranslation(GameCulture.Russian, "Призванный Прицел");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 24;
            projectile.height = 58;
            projectile.friendly = false; //Doesn't affect enemies
        }

        private static bool BullseyeShot(Player player) //The player is aiming in the right direction for a boosted shot by the Bullseye orbital
        {
            return Math.Abs((Main.MouseWorld - player.Center).Normalized().X) > 0.990f; //Roughly straight left or right. The number guides how forgiving the treshold is
        }

        public override void PlayerEffects()
        {
            if (BullseyeShot(player))
            {
                player.rangedDamage *= 2.0f;
                player.thrownDamage *= 2.0f;
            }
            else
            {
                player.rangedDamage *= 0.8f;
                player.thrownDamage *= 0.8f;
            }
        }

        public override void Movement()
        {
            //Stays in front of the player
            projectile.spriteDirection = player.direction;
            SetPosition(new Vector2(player.direction * relativeDistance, 0));

            Lighting.AddLight(new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y), 0.5f, 0.3f, 0.05f);

            //Special effect dust
            if (Main.myPlayer == projectile.owner && BullseyeShot(player))
            {
                Vector2 position = new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y - 1);
                Dust newDust = Dust.NewDustDirect(position, 0, 0, mod.DustType<Dusts.RainbowDust>(), 0f, 0f, /*Alpha*/50, new Color(255, 127, 0, 50), /*Scale*/1.5f);
                newDust.velocity = new Vector2(player.direction * 2.5f, 0);
            }
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 130, 20, 200) * projectile.Opacity;
        }
    }
}

