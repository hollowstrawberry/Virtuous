using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Bubble : OrbitalProjectile
    {
        public override int Type => OrbitalID.Bubble;
        public override int OriginalAlpha => 120;
        public override int FadeTime => 60;
        public override float BaseDistance => 0;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
            DisplayName.AddTranslation(GameCulture.Spanish, "Burbuja");
            DisplayName.AddTranslation(GameCulture.Russian, "Пузырь");
            DisplayName.AddTranslation(GameCulture.Chinese, "泡泡");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
        }


        public override void PlayerEffects()
        {
            player.statDefense += 10;
            Lighting.AddLight(player.Center, 0.4f, 0.6f, 0.6f);
        }


        public override void FirstTick()
        {
            base.FirstTick();

            projectile.damage = 1;

            for (int i = 0; i < 40; i++)
            {
                var dust = Dust.NewDustDirect(
                    projectile.position, projectile.width, projectile.height,
                    /*Type*/16, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                dust.velocity *= 1.5f;
                dust.noLight = false;
            }
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(224, 255, 252, 150) * projectile.Opacity;
        }
    }
}

