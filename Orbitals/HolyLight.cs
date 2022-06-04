using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class HolyLight : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.HolyLight;
        public override int DyingTime => 10; // Time it spends bursting
        public override float BaseDistance => 70;
        public override float OrbitingSpeed => 1 / 30f * Tools.RevolutionPerSecond;
        public override float RotationSpeed => -OrbitingSpeed;
        public override float OscillationSpeedMax => 0.2f;
        public override float OscillationAcc => OscillationSpeedMax / 60;

        private const int OriginalSize = 30; // Size of the sprite
        private const int BurstSize = 120; // Size of the area where bursting causes damage


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Luz Santa");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Святое Сияние");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "圣光");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = OriginalSize;
            Projectile.height = OriginalSize;
        }


        public override void PlayerEffects()
        {
            player.lifeRegen += 5;

            if (Main.rand.NextBool(6))
            {
                var dust = Dust.NewDustDirect(
                    player.Center + Main.rand.NextVector2().OfLength(50), 0, 0,
                    /*Type*/55, 0f, 0f, /*Alpha*/200, default(Color), /*Scale*/0.5f);
                dust.velocity = (player.Center - dust.position).OfLength(Main.rand.NextFloat(4, 5));
                dust.noGravity = true;
                dust.fadeIn = 1.3f;
            }
        }


        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 15; i++)
            {
                var dust = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    /*Type*/55, 0f, 0f, /*Alpha*/50, default(Color), Main.rand.NextFloat(1.2f, 1.5f));
                dust.velocity *= 0.8f;
                dust.noLight = false;
                dust.noGravity = true;
            }
        }


        public override bool PreMovement()
        {
            return true; // Never stops moving even while dying
        }


        public override void DyingFirstTick()
        {
            Projectile.alpha = 255; // Transparent
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center); // Explosion

            if (Main.myPlayer == Projectile.owner) Tools.ResizeProjectile(Projectile.whoAmI, BurstSize, BurstSize);

            for (int i = 0; i < 15; i++)
            {
                var dust = Dust.NewDustDirect(
                    Projectile.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, new Color(255, 230, 100), /*Scale*/1.0f);
                dust.velocity *= 2;
            }
        }


        public override void Dying() // Does nothing while fading away
        {
        }


        public override void PostAll()
        {
            if (IsDying) Lighting.AddLight(Projectile.Center, 2.0f, 2.0f, 1.2f);
            else Lighting.AddLight(Projectile.Center, 1.0f, 1.0f, 0.6f);
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 3;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 3;
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 255, 255, 50) * Projectile.Opacity;
        }
    }
}
