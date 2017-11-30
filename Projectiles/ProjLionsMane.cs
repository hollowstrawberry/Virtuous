using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Projectiles
{
    class ProjLionsMane : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lion's Mane");
        }

        private const int Updates = 2; //How many updates it makes per tick
        private const int Lifespan = 90 * Updates; //Total timeLeft in ticks of the projectile
        private const int OriginalAlpha = 200;
        private const float OrbitingSpeed = FullCircle / (Lifespan / 3f); //Last number is the revolutions over its Lifespan

        private float Distance { get { return projectile.ai[0]; } set { projectile.ai[0] = value; } } //Distance away from the player, stored as ai[0]
        private int Direction => (int)projectile.ai[1]; //Cardinal direction it's aiming at from 1 to 4, negative for counterclockwise. Stored as ai[1]

        private Vector2 RelativePosition //Calculated relative position in relation to the player for the current tick
        {
            get
            {
                int direction = (Direction > 0 ? +1 : -1); //Clockwise or counterclockwise

                Vector2 startingPosition;
                switch (Math.Abs(Direction))
                {
                    case 1: //Up
                        startingPosition = Vector2.UnitY * -1;
                        break;
                    case 2: //Right
                        startingPosition = Vector2.UnitX * +1;
                        break;
                    case 3: //Down
                        startingPosition = Vector2.UnitY * +1;
                        break;
                    case 4: //Left
                        startingPosition = Vector2.UnitX * -1;
                        break;
                    default: //Failsafe
                        startingPosition = Vector2.UnitX;
                        if (Distance == 0) Distance = 5;
                        break;
                }

                //We calculate where this projectile would have to be, in relation to the player, given the time passed since it was spawned.
                //The Distance from the player is calculated separately every tick in AI(), so we just apply it as the magnitude of the vector
                //The orbitingSpeed is the rotation it gains per tick, so we multiply that by the difference between the current timeLeft and the total Lifespan to get how many ticks of rotation it needs to be moved by
                return startingPosition.RotatedBy(OrbitingSpeed * direction * (Lifespan - projectile.timeLeft + 1)).OfLength(Distance);
            }
        }


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

            if(projectile.alpha > 0)
            {
                projectile.alpha -= (int)Math.Ceiling((double)OriginalAlpha / Lifespan);
            }

            //Every tick, the distance from the player is multiplied by a root of the golden ratio.
            //This means that every certain amount of ticks, we find that the distance has increased by a factor of Phi, which is a defining factor of a golden spiral
            float DistanceMultiplier = (float)Math.Pow(GoldenRatio, 1.0/(Lifespan / 9.0)); //Last number is how many times over its Lifespan the distance gets multiplied by the golden ratio
            Distance *= DistanceMultiplier;

            //Something similar happens with the damage, increasing as it goes along and ending at the final damage multiplier
            float damageMultiplier = (float)Math.Pow(5.0, 1.0 / Lifespan); //First number is how many times the original damage it will have at the end of its lifespan
            projectile.damage = (int)Math.Ceiling(projectile.damage * damageMultiplier);

            projectile.Center = player.MountedCenter + RelativePosition; //Puts the projectile around the player

            //Dust
            if (projectile.timeLeft > 1) //Ignores the last tick
            {
                int dustAmount = (int)(Distance / 30f); //More dust as distance increases
                for (int i = 0; i < dustAmount; i++)
                {
                    Dust newDust1 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, projectile.alpha, default(Color), RandomFloat(1, 2));
                    newDust1.noGravity = true;
                    newDust1.velocity *= 3.0f;
                    Dust newDust2 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, projectile.alpha, default(Color), 1.5f);
                    newDust2.noGravity = true;
                    Dust newDust3 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/158, 0f, 0f, projectile.alpha, default(Color), RandomFloat(1, 2));
                    newDust3.noGravity = true;
                    newDust3.velocity *= 1.5f;
                }
            }

            Lighting.AddLight(projectile.Center, 0.8f, 0.7f, 0.4f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust newDust3 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, projectile.alpha, default(Color), RandomFloat(2, 3));
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
