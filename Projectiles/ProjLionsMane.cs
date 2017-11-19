using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Projectiles
{
    class ProjLionsMane : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lion's Mane");
        }

        private const int OriginalAlpha = 200;
		private const int Updates = 2; //How many updates it makes per tick
        private const int Lifespan = 90 * Updates; //Total timeLeft in ticks of the projectile

        private float Distance { get { return projectile.ai[0]; } set { projectile.ai[0] = value; } } //Distance away from the player, stored as ai[0]
        private int Direction { get { return (int)projectile.ai[1]; } set { projectile.ai[1] = (int)value; } } //Cardinal direction it's aiming at, negative for counterclockwise. Stored as ai[1]
        private float orbitingSpeed = Tools.FullCircle / (Lifespan / 3f); //Last number is the revolutions over its Lifespan
        private float damageMultiplier = (float)Math.Pow(5.0, 1.0 / Lifespan); //First number is how much damage it gains over its Lifespan
        private float distanceMultiplier = (float)Math.Pow((1.0 + Math.Sqrt(5.0)) / 2.0, 1.0 / (Lifespan / 9.0)); //Last number is how many times over its Lifespan the distance gets multiplied by the golden ratio
        private Vector2 relativePos; //Relative position in relation to the player

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.alpha = OriginalAlpha;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
			projectile.MaxUpdates = Updates;
			projectile.timeLeft = Lifespan;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if(projectile.timeLeft == Lifespan) //First tick
            {
                orbitingSpeed *= (Direction > 0) ? +1 : -1; //Clockwise or counterclockwise rotation

                switch (Math.Abs(Direction))
                {
                    case 1: //Up
                        relativePos = Vector2.UnitY * -1;
                        break;
                    case 2: //Right
                        relativePos = Vector2.UnitX * +1;
                        break;
                    case 3: //Down
                        relativePos = Vector2.UnitY * +1;
                        break;
                    case 4: //Left
                        relativePos = Vector2.UnitX * -1;
                        break;
                    default: //Failsafe
                        relativePos = Vector2.UnitX;
                        Distance = 5;
                        break;
                }

                projectile.netUpdate = true; //Sync to multiplayer
            }
            
            if(projectile.alpha > 0)
            {
                projectile.alpha -= (int)Math.Ceiling((double)OriginalAlpha / Lifespan);
            }

            Distance = (Distance * distanceMultiplier); //Distance is increased exponentially as it goes along, and the factor is a root of the golden ratio
            relativePos = relativePos.RotatedBy(orbitingSpeed).SafeNormalize(Vector2.UnitX) * Distance; //We rotate the projectile around the user and reset its distance
            projectile.Center = player.MountedCenter + relativePos; //Puts the projectile around the player
            projectile.damage = (int)Math.Ceiling(projectile.damage * damageMultiplier); //Damage is increased exponentially as it goes along, ending at the defined final factor

            //Dust
            int dustAmount = (int)(Distance / 20f); //More dust as distance increases
            for (int i = 0; i < dustAmount; i++)
            {
                Dust newDust1 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, projectile.alpha, default(Color), 1 + Main.rand.NextFloat());
                newDust1.noGravity = true;
                newDust1.velocity *= 3.0f;
                Dust newDust2 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, projectile.alpha, default(Color), 1.5f);
                newDust2.noGravity = true;
                Dust newDust3 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/158, 0f, 0f, projectile.alpha, default(Color), 1 + Main.rand.NextFloat());
                newDust3.noGravity = true;
                newDust3.velocity *= 1.5f;
            }

            Lighting.AddLight(projectile.Center, 0.8f, 0.7f, 0.4f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust newDust3 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, projectile.alpha, default(Color), 1 + Main.rand.NextFloat()*2f);
                newDust3.noGravity = true;
                newDust3.velocity *= 1.5f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 64 - projectile.alpha / 4);
        }

    }
}
