using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class Shuriken_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Shuriken;
        public override int DyingTime => 30;
        public override float BaseDistance => 120;
        public override float OrbitingSpeed => 1 * RevolutionPerSecond;
        public override float RotationSpeed => -2 * OrbitingSpeed;
        public override float OscillationSpeedMax => 6.0f;
        public override float OscillationAcc => OscillationSpeedMax / 20;

        private const float DyingOrbitingSpeed = 4 * RevolutionPerSecond;
        private const float DyingRotationSpeed = -2 * DyingOrbitingSpeed;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Shuriken");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
        }

        public override void PlayerEffects(Player player)
        {
            player.manaRegenDelayBonus++;
            player.manaRegenBonus += 25;
            player.meleeSpeed += 0.12f;
        }

        public override void DyingFirstTick()
        {
            projectile.damage *= 3;
            distance = BaseDistance / 2;
        }

        public override void Dying()
        {
            distance += OscillationSpeedMax; //Expands out
            relativePosition = relativePosition.RotatedBy(DyingOrbitingSpeed); //Rotates the shuriken around the player
            projectile.rotation += DyingRotationSpeed; //Rotates the sprite as well
            projectile.Center = player.MountedCenter + relativePosition; //Moves the shuriken to the defined position around the player

            if (projectile.timeLeft == 1) //Last tick
            {
                Main.PlaySound(SoundID.Dig, projectile.Center); //Thump

                for (int i = 0; i < 10; i++)
                {
                    Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/74, 0f, 0f, /*Alpha*/150, new Color(50, 255, 100, 150), /*Scale*/1.5f);
                    newDust.velocity += relativePosition.Perpendicular(10, CounterClockwise); //Angular to linear velocity
                }
            }
        }

        public override void PostAll()
        {
            Lighting.AddLight(projectile.Center, 0.0f, 1.0f, 0.2f);
        }
        

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(10, 255, 50, 150) * projectile.Opacity;
        }
    }
}