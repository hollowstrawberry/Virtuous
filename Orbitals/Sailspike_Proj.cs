using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Sailspike_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Sailspike;
        public override int DyingTime => 30;
        public override int FadeTime => 20;
        public override int OriginalAlpha => 100;
        public override float BaseDistance => 55;
        public override float ShootSpeed => 20;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 30;
            projectile.height = 14;
        }


        public override void FirstTick()
        {
            base.FirstTick();
            Movement();

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/172, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                newDust.velocity *= 0.2f;
                newDust.noLight = false;
                newDust.noGravity = true;
            }
        }

        public override void Movement()
        {
            //Stays in front of the player
            projectile.spriteDirection = player.direction;
            relativePosition = new Vector2(player.direction * distance, 0);
            projectile.Center = player.MountedCenter + relativePosition;
        }

        public override void DyingFirstTick()
        {
            projectile.damage *= 2;
            base.DyingFirstTick(); //Shoots out
        }

        public override void ExtraEffects()
        {
            Lighting.AddLight(projectile.Center, 0.15f, 0.5f, 1.5f);
            base.ExtraEffects(); //Fades
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(28, 77, 255, 200) * projectile.Opacity;
        }

    }
}

