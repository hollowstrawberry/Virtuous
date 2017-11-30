using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class Facade_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Facade;
        public override int OriginalAlpha => 80; //Alpha value before fading out
        public override int FadeTime => 30; //Time it spends fading out
        public override float BaseDistance => 50;
        public override float OscillationSpeedMax => 1 / 15f * RevolutionPerSecond;
        public override float OscillationAcc => OscillationSpeedMax / 60;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Barrier");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 18;
            projectile.height = 44;
        }

        public override void PostMovement()
        {
            distance -= oscillationSpeed; //Undoes the distance oscillation
            relativePosition = relativePosition.RotatedBy(oscillationSpeed); //Applies it as angular oscillation

            projectile.rotation += oscillationSpeed; //Rotates the sprite
            projectile.Center = player.MountedCenter + relativePosition; //Keeps the projectile around the player
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(250, 233, 0, 100) * projectile.Opacity;
        }
    }
}
