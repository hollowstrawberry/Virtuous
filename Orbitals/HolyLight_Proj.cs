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

        public override void PlayerEffects(Player player)
        {
            player.lifeRegen += 3;

            if (OneIn(6))
            {
                Dust newDust = Dust.NewDustDirect(player.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, default(Color), /*Scale*/0.5f);
                newDust.velocity = new Vector2(RandomFloat(-1, +1), RandomFloat(-1, +1)).OfLength(RandomFloat(4, 6)); //Random direction, random magnitude
                newDust.position -= newDust.velocity.OfLength(50f); //Sets the distance in a position where it will move towards the player
                newDust.velocity += player.velocity; //Follows the player somewhat
                newDust.noGravity = true;
                newDust.fadeIn = 1.3f;
            }
        }

        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/55, 0f, 0f, /*Alpha*/50, default(Color), RandomFloat(1.2f, 1.5f));
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
            Main.PlaySound(SoundID.Item14, projectile.Center); //Explosion

            ResizeProjectile(projectile.whoAmI, BurstSize, BurstSize);

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, new Color(255, 230, 100), /*Scale*/1.0f);
                newDust.velocity *= 2;
            }
        }
        
        public override void Dying()
        {
            Movement(); //Even for the split second it dies for, it doesn't stop moving
            Lighting.AddLight(projectile.Center, 2.0f, 2.0f, 1.2f);
        }


        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(255, 230, 180, 100) * projectile.Opacity;
        }
    }
}
