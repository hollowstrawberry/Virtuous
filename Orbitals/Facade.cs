using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Facade : OrbitalProjectile
    {
        public override int Type => OrbitalID.Facade;
        public override int OriginalAlpha => 80;
        public override int FadeTime => 30;
        public override float BaseDistance => 50;
        public override float OscillationSpeedMax => 1 / 15f * Tools.RevolutionPerSecond;
        public override float OscillationAcc => OscillationSpeedMax / 60;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Barrier");
            DisplayName.AddTranslation(GameCulture.Spanish, "Barrera Mágica");
            DisplayName.AddTranslation(GameCulture.Russian, "Призванный Барьер");
            DisplayName.AddTranslation(GameCulture.Chinese, "召唤屏障");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 18;
            projectile.height = 44;
        }


        public override void Movement()
        {
            base.Movement();

            relativeDistance -= oscillationSpeed; // Undoes the distance oscillation
            RotatePosition(oscillationSpeed); // Applies it as angular oscillation
            projectile.rotation += oscillationSpeed; // Rotates the sprite
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(250, 233, 0, 100) * projectile.Opacity;
        }
    }
}

