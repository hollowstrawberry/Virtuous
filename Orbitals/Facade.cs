using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Facade : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.Facade;
        public override int OriginalAlpha => 80;
        public override int FadeTime => 30;
        public override float BaseDistance => 50;
        public override float OscillationSpeedMax => 1 / 15f * Tools.RevolutionPerSecond;
        public override float OscillationAcc => OscillationSpeedMax / 60;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Barrier");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Barrera Mágica");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Призванный Барьер");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "召唤屏障");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 44;
        }


        public override void Movement()
        {
            base.Movement();

            RelativeDistance -= OscillationSpeed; // Undoes the distance oscillation
            RotatePosition(OscillationSpeed); // Applies it as angular oscillation
            Projectile.rotation += OscillationSpeed; // Rotates the sprite
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(250, 233, 0, 100) * Projectile.Opacity;
        }
    }
}

