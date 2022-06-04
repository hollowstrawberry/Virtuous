using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Virtuous.Dusts;

namespace Virtuous.Orbitals
{
    public class Bullseye : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.Bullseye;
        public override int FadeTime => 600;
        public override float BaseDistance => 40;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Sight");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Mira Mágica");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Призванный Прицел");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "召唤真视");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 58;
            Projectile.friendly = false; // Doesn't affect enemies
        }



        private static bool BullseyeShot(Player player) // The player is aiming in the correct direction for a boosted shot
        {
            return Math.Abs((Main.MouseWorld - player.Center).Normalized().X) > 0.990f; // Roughly straight left or right
        }


        public override void PlayerEffects()
        {
            if (BullseyeShot(player))
            {
                player.GetDamage(DamageClass.Ranged) *= 2.0f;
                player.GetDamage(DamageClass.Throwing) *= 2.0f;
            }
            else
            {
                player.GetDamage(DamageClass.Ranged) *= 0.8f;
                player.GetDamage(DamageClass.Throwing) *= 0.8f;
            }
        }


        public override void Movement()
        {
            // Stays in front of the player
            Projectile.spriteDirection = player.direction;
            SetPosition(new Vector2(player.direction * RelativeDistance, 0));

            Lighting.AddLight(new Vector2(Projectile.Center.X + 2 * player.direction, Projectile.Center.Y), 0.5f, 0.3f, 0.05f);

            // Special effect dust
            if (Main.myPlayer == Projectile.owner && BullseyeShot(player))
            {
                var dust = Dust.NewDustDirect(
                    new Vector2(Projectile.Center.X + 2 * player.direction, Projectile.Center.Y - 1),
                    0, 0, Mod.Find<ModDust>(nameof(RainbowDust)).Type, 0f, 0f, 50, new Color(255, 127, 0, 50), 1.5f);
                dust.velocity = new Vector2(player.direction * 2.5f, 0);
            }
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 130, 20, 200) * Projectile.Opacity;
        }
    }
}

