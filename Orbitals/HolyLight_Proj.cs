using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class HolyLight_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.HolyLight;
        public override int DyingTime => 10; //Time it spends bursting
        public override float BaseDistance => 70;
        public override float OrbitingSpeed => 1 / 30f * RevolutionPerSecond;
        public override float RotationSpeed => -OrbitingSpeed;
        public override float OscillationSpeedMax => 0.2f; 
        public override float OscillationAcc => OscillationSpeedMax / 60;

        private const int OriginalSize = 30; //Size of the sprite
        private const int BurstSize = 120; //Size of the area where bursting causes damage


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = OriginalSize;
            projectile.height = OriginalSize;
        }

        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/55, 0f, 0f, /*Alpha*/50, new Color(255, 230, 100), 1.2f);
                newDust.velocity *= 0.8f;
                newDust.noLight = false;
                newDust.noGravity = true;
            }
        }

        public override void PostMovement()
        {
            Lighting.AddLight(projectile.Center, 1.0f, 1.0f, 0.6f);
        }

        public override void DyingFirstTick()
        {
            projectile.damage *= 3;
            projectile.alpha = 255; //Transparent
            Main.PlaySound(SoundID.Item14, projectile.Center);

            ResizeProjectile(projectile.whoAmI, BurstSize, BurstSize);

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, new Color(255, 230, 100), /*Scale*/1.0f);
                newDust.velocity *= 2;
            }
        }
        
        public override void Dying()
        {
            projectile.position -= projectile.velocity; //Reverses the effect of velocity so it doesn't move
            Lighting.AddLight(projectile.Center, 2.0f, 2.0f, 1.2f);
        }


        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(255, 230, 180, 100) * projectile.Opacity;
        }
    }
}
