using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace Virtuous.Projectiles
{
    public class ProjFist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flurry Nova");
            Main.projFrames[projectile.type] = 4;
        }

        private const int ShootSpeed   =  5; //Defined speed of the projectile
        private const int Lifespan     = 20; //Total projectile duration
        private const int MoveTime     = 12; //How long it moves at constant speed for
        private const int SlowdownTime =  5; //How long it takes to slow down to a stop
        private const int FadeTime     =  5; //How long it fades away for at the end of its Lifespan

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
            projectile.usesLocalNPCImmunity = true; //Hits once per individual projectile
            projectile.localNPCHitCooldown = Lifespan;
        }
        
        public override void AI()
        {
            //Projectile has spawned
            if (projectile.timeLeft == Lifespan)
            {
                if (projectile.velocity.X < 0) projectile.spriteDirection = -1; //Facing left
                projectile.velocity = projectile.velocity.OfLength(ShootSpeed) + Main.player[projectile.owner].velocity; //Sets the defined speed, follows the player's movement

                const float DustAmount = 16f;
                for (int i = 0; i < DustAmount; i++) //We create 16 dusts in an ellipse
                {
                    Vector2 rotation = Vector2.UnitY.RotatedBy(Tools.FullCircle * i / DustAmount); //Divides a circle into a set amount of points and picks the current one in the loop
                    rotation *= new Vector2(1, 4); // Multiplies the point by a vertical squish factor so the circle becomes an ellipse
                    rotation = rotation.RotatedBy(projectile.rotation); //Rotates the resulting ellipse point to align with the projectile's rotation
                    Vector2 position = projectile.Center + rotation; //The final position of the dust
                    position.X -= 10 * Main.player[projectile.owner].direction; //This line needs to change if needed to work with multidirectional fists

                    Dust newDust = Dust.NewDustDirect(position, 0, 0, /*Type*/127, 0f, 0f, /*Alpha*/0, new Color(255, 255, 0), /*Scale*/2.0f);
                    newDust.velocity = rotation.Normalized(); //Shoots outwards
                    newDust.noGravity = true;
                }
            }
            //Projectile has passed movetime and it's now slowdowntime
            else if (projectile.timeLeft < Lifespan-MoveTime && projectile.timeLeft >= Lifespan-(MoveTime+SlowdownTime))
            {
                float oldspeed = projectile.velocity.Length();
                projectile.velocity *= (oldspeed - ShootSpeed/SlowdownTime) / oldspeed; //Slows to a halt over slowdowntime
            }

            if(projectile.timeLeft % 2 == 0) //Every second tick
            {
                if (projectile.frame < 3) projectile.frame++;
                else projectile.frame = 0;
            }

            //Projectile fades away
            if (projectile.timeLeft <= FadeTime)
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