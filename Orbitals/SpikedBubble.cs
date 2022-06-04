using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SpikedBubble : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.SpikedBubble;
        public override int DyingTime => 30;
        public override float BaseDistance => 0;
        public override float RotationSpeed => 0; // At first I wanted this to rotate but it made the sprite look weird

        private const float DamageBoost = 0.1f; // Damage boost while the orbital is active

        private const float ExpandedScale = 1.5f; // Size when fully expanded
        private const int ExpandedAlpha = 150; // Alpha value when fully expanded
        private const int OriginalSize = 120; // Width and height dimensions in pixels
        private const int PopTime = 10; // Part of DyingTime where it keeps its size before popping


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Bubble");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Burbuja Claveta");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Колючий Пузырь");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "尖刺泡泡");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = OriginalSize;
            Projectile.height = OriginalSize;
        }


        public override void PlayerEffects()
        {
            player.statDefense += 5;
            player.meleeDamage += DamageBoost;
            player.magicDamage += DamageBoost;
            orbitalPlayer.damageBuffFromOrbitals += DamageBoost;

            Lighting.AddLight(player.Center, 0.6f, 0.4f, 0.3f);
        }


        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 50; i++)
            {
                var dust = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    /*Type*/16, 0f, 0f, /*Alpha*/50, new Color(255, 200, 245), 1.2f);
                dust.velocity *= 1.5f;
                dust.noLight = false;
            }
        }


        public override bool PreMovement()
        {
            return true; // Move even while dying
        }


        public override void Dying()
        {
            if (Projectile.timeLeft >= PopTime) // Expands until PopTime then stops
            {
                // Change the apparent size
                Projectile.scale += (ExpandedScale - 1f) / (DyingTime - PopTime);
                // Change the hitbox size
                Projectile.height = (int)(OriginalSize * Projectile.scale);
                Projectile.width  = (int)(OriginalSize * Projectile.scale);
                // Align the sprite with its new size
                drawOriginOffsetY = (Projectile.height - OriginalSize) / 2;
                drawOffsetX = (Projectile.width - OriginalSize) / 2;

                Projectile.alpha += (int)Math.Ceiling((float)(ExpandedAlpha - OriginalAlpha) / DyingTime); // Fade
            }
            else if (Projectile.timeLeft == 1) // Last tick
            {
                SoundEngine.PlaySound(SoundID.Item54, Projectile.Center); // Pop
                Lighting.AddLight(player.Center, 1.5f, 1.0f, 1.4f);

                for (int i = 0; i < 50; i++)
                {
                    var dust = Dust.NewDustDirect(
                        Projectile.Center + Main.rand.NextVector2(0, Projectile.width / 2), // Random point in the bubble
                        0, 0, /*Type*/16, 0, 0, /*Alpha*/100, new Color(255, 200, 245, 150), /*Scale*/1.2f);
                    dust.velocity *= 2;
                }
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 2;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 2;
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 200, 245, 150) * Projectile.Opacity;
        }
    }
}