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

        private const int Updates = 2; //How many updates it makes per tick
        private const int Lifespan = 90 * Updates; //Total timeLeft in ticks of the projectile
        private const int OriginalAlpha = 200; //Alpha value it starts with
        private const float OrbitingSpeed = Tools.FullCircle / (Lifespan / 3f); //Last number is the revolutions over its Lifespan
        private static float DistanceMultiplier = (float)Math.Pow(Tools.GoldenRatio, 1.0 / (Lifespan / 9.0)); //Last number is how many times over its Lifespan the distance gets multiplied by the golden ratio. Multiplying by the golden ratio results in a golden spiral
        private static float DamageMultiplier = (float)Math.Pow(5.0, 1.0 / Lifespan); //First number is how many times the original damage it will have at the end of its lifespan

        private float Distance { get { return projectile.ai[0]; } set { projectile.ai[0] = value; } } //Distance away from the player, stored as ai[0]
        private int Direction => (int)projectile.ai[1]; //Cardinal direction it's aiming at from 1 to 4, negative for counterclockwise. Stored as ai[1]

        private Vector2 RelativePosition //Calculated relative position in relation to the player for the current tick.
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
            projectile.alpha = 255;
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

            projectile.alpha = 255;

            projectile.damage = (int)Math.Ceiling(projectile.damage * DamageMultiplier);
            Distance *= DistanceMultiplier;
            projectile.Center = player.SpriteCenter() + RelativePosition; //Puts the projectile around the player

            //Dust
            int dustAmount = 1 + (int)(Distance / 15f); //More dust as distance increases
            for (int i = 0; i < dustAmount; i++)
            {
                Dust newDust;
                switch (Tools.RandomInt(4))
                {
                    case 0:
                        newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, projectile.alpha, default(Color), 0.5f);
                        newDust.velocity *= 3.0f;
                        break;

                    case 1:
                        newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/158, 0f, 0f, projectile.alpha, default(Color), 0.5f);
                        newDust.velocity *= 1.5f;
                        break;

                    default:
                        newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, projectile.alpha, default(Color), 0.5f);
                        break;
                }
                newDust.noGravity = true;
                newDust.scale += dustAmount / 12f; //Size gets bigger with distance as well
            }

            Lighting.AddLight(projectile.Center, 0.8f, 0.7f, 0.4f);
        }

        public override void Kill(int timeLeft)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, projectile.alpha, default(Color), Tools.RandomFloat(2f, 3f));
            //    newDust.noGravity = true;
            //}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 64 - projectile.alpha / 4);
        }

    }
}
