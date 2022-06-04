using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Shuriken : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.Shuriken;
        public override int DyingTime => 30;
        public override float BaseDistance => 120;
        public override float OrbitingSpeed => 1 * Tools.RevolutionPerSecond;
        public override float RotationSpeed => -2 * OrbitingSpeed;
        public override float OscillationSpeedMax => 6.0f;
        public override float OscillationAcc => OscillationSpeedMax / 20;

        private const float DyingOrbitingSpeed = 4 * Tools.RevolutionPerSecond;
        private const float DyingRotationSpeed = -2 * DyingOrbitingSpeed;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Shuriken");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Shuriken Mágico");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Призванный Сюрикен");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "召唤飞镖");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
        }


        public override void PlayerEffects()
        {
            player.manaRegenDelayBonus++;
            player.manaRegenBonus += 25;
            player.meleeSpeed += 0.12f;
        }


        public override void DyingFirstTick()
        {
            Projectile.damage *= 3;
            RelativeDistance = BaseDistance / 2;
        }

        public override void Dying()
        {
            AddDistance(OscillationSpeedMax); // Expands outwards
            RotatePosition(DyingOrbitingSpeed); // Spins
            Projectile.rotation += DyingRotationSpeed; // Rotates the sprite as well

            if (Projectile.timeLeft == 1) // Last tick
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center); // Thump

                for (int i = 0; i < 10; i++)
                {
                    var dust = Dust.NewDustDirect(
                        Projectile.position, Projectile.width, Projectile.height,
                        /*Type*/74, 0f, 0f, /*Alpha*/150, new Color(50, 255, 100, 150), /*Scale*/1.5f);
                    dust.velocity += RelativePosition.Perpendicular(10, clockwise: false); // Tangential
                }
            }
        }


        public override void PostAll()
        {
            Lighting.AddLight(Projectile.Center, 0.0f, 1.0f, 0.2f);
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(10, 255, 50, 150) * Projectile.Opacity;
        }
    }
}
