using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    public class ProjFist : ModProjectile
    {
        private const int ShootSpeed = 5; // Defined speed of the projectile
        private const int Lifespan = 20; // Total projectile duration
        private const int MoveTime = 12; // How long it moves at constant speed for
        private const int SlowdownTime = 5; // How long it takes to slow down to a stop
        private const int FadeTime = 5; // How long it fades away for at the end of its Lifespan



        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;

            DisplayName.SetDefault("Flurry Nova");
            DisplayName.AddTranslation(GameCulture.Spanish, "Golpe Nova");
        }


        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 30;
            projectile.alpha = 10;
            projectile.timeLeft = Lifespan;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true; // Hits once per individual projectile
            projectile.localNPCHitCooldown = Lifespan;
        }
        

        public override void AI()
        {
            if (projectile.timeLeft == Lifespan) // Projectile just spawned
            {
                if (projectile.velocity.X < 0) projectile.spriteDirection = -1; // Facing left
                projectile.velocity = projectile.velocity.OfLength(ShootSpeed) + Main.player[projectile.owner].velocity; // Follows player

                const float dustAmount = 16f;
                for (int i = 0; i < dustAmount; i++)
                {
                    Vector2 offset = Vector2.UnitY.RotatedBy(Tools.FullCircle * i / dustAmount) * new Vector2(1, 4); // Ellipse of dust
                    offset = offset.RotatedBy(projectile.rotation); // Rotates the resulting ellipse to align with the projectile's rotation

                    Vector2 position = projectile.Center + offset;
                    position.X -= 10 * Main.player[projectile.owner].direction; // This line needs to change if needed to work with multidirectional fists

                    var dust = Dust.NewDustDirect(position, 0, 0, 127, 0f, 0f, newColor: new Color(255, 255, 0), Scale: 2.0f);
                    dust.velocity = offset.Normalized(); // Shoots outwards
                    dust.noGravity = true;
                }
            }
            else if (projectile.timeLeft < Lifespan - MoveTime && projectile.timeLeft >= Lifespan - (MoveTime + SlowdownTime)) // Slow down
            {
                float oldspeed = projectile.velocity.Length();
                projectile.velocity *= (oldspeed - ShootSpeed/SlowdownTime) / oldspeed;
            }


            if(projectile.timeLeft % 2 == 0) // Animation
            {
                if (projectile.frame < 3) projectile.frame++;
                else projectile.frame = 0;
            }

            if (projectile.timeLeft <= FadeTime) // Fade away
            {
                projectile.alpha += 200/FadeTime;
            }
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 180 - projectile.alpha);
        }
    }
}