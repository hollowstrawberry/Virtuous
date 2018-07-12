using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Virtuous.Dusts;

namespace Virtuous.Orbitals
{
    public class Bullseye : OrbitalProjectile
    {
        public override int Type => OrbitalID.Bullseye;
        public override int FadeTime => 120;
        public override float BaseDistance => 40;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Sight");
            DisplayName.AddTranslation(GameCulture.Spanish, "Mira Mágica");
            DisplayName.AddTranslation(GameCulture.Russian, "Призванный Прицел");
            DisplayName.AddTranslation(GameCulture.Chinese, "召唤真视");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 24;
            projectile.height = 58;
            projectile.friendly = false; // Doesn't affect enemies
        }



        private static bool BullseyeShot(Player player) // The player is aiming in the correct direction for a boosted shot
        {
            return Math.Abs((Main.MouseWorld - player.Center).Normalized().X) > 0.990f; // Roughly straight left or right
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
            // Stays in front of the player
            projectile.spriteDirection = player.direction;
            SetPosition(new Vector2(player.direction * relativeDistance, 0));

            Lighting.AddLight(new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y), 0.5f, 0.3f, 0.05f);

            // Special effect dust
            if (Main.myPlayer == projectile.owner && BullseyeShot(player))
            {
                var dust = Dust.NewDustDirect(
                    new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y - 1),
                    0, 0, mod.DustType<RainbowDust>(), 0f, 0f, 50, new Color(255, 127, 0, 50), 1.5f);
                dust.velocity = new Vector2(player.direction * 2.5f, 0);
            }
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 130, 20, 200) * projectile.Opacity;
        }
    }
}

