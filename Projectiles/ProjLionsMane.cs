using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    class ProjLionsMane : ModProjectile
    {
        private const int Updates = 2; // How many updates it makes per tick
        private const int Lifespan = 90 * Updates; // Total timeLeft in ticks of the projectile
        private const float OrbitingSpeed = Tools.FullCircle / (Lifespan / 3f); // Last number is the revolutions over its Lifespan
        const float FinalDamageFactor = 5f; // How many times more damage it will have by the time it dies

        // The multipliers are applied each tick. Multiplying the distance by a root of the golden ratio results in a golden spiral 
        private static readonly float DistanceMultiplier = (float)Math.Pow(Tools.GoldenRatio, 1.0 / (Lifespan / 9.0));
        private static readonly float DamageMultiplier = (float)Math.Pow(FinalDamageFactor, 1.0 / Lifespan);



        public int Direction // +1 for clockwise, -1 for counterclockwise, stored as ai[0]
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        public Vector2 RelativePosition // Relative position from the player, stored as velocity
        {
            get { return Projectile.velocity; }
            set
            {
                Projectile.velocity = value;
                Projectile.position = Main.player[Projectile.owner].SpriteCenter() + value;
            }
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lion's Mane");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Melena de León");
        }


        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.magic = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.MaxUpdates = Updates;
            Projectile.timeLeft = Lifespan;
        }


        public override void AI()
        {
            Projectile.damage = (int)Math.Ceiling(Projectile.damage * DamageMultiplier);
            RelativePosition = RelativePosition.RotatedBy(OrbitingSpeed * Direction) * DistanceMultiplier;

            // Dust
            int dustAmount = 1 + (int)(RelativePosition.Length() / 15f); // More dust as distance increases
            for (int i = 0; i < dustAmount; i++)
            {
                Dust dust;
                switch (Main.rand.Next(4)) // Random types
                {
                    case 0:
                        dust = Dust.NewDustDirect(
                            Projectile.position, Projectile.width, Projectile.height,
                            DustID.Fire, 0f, 0f, Projectile.alpha, default(Color), 0.5f);
                        dust.velocity *= 3.0f;
                        break;

                    case 1:
                        dust = Dust.NewDustDirect(
                            Projectile.position, Projectile.width, Projectile.height,
                            158, 0f, 0f, Projectile.alpha, default(Color), 0.5f);
                        dust.velocity *= 1.5f;
                        break;

                    default:
                        dust = Dust.NewDustDirect(
                            Projectile.position, Projectile.width, Projectile.height,
                            DustID.SolarFlare, 0f, 0f, Projectile.alpha, default(Color), 0.5f);
                        break;
                }

                dust.noGravity = true;
                dust.scale += dustAmount / 12f; // Size gets bigger with distance as well
            }


            Projectile.position -= Projectile.velocity; // Undoes normal movement

            Lighting.AddLight(Projectile.Center, 0.8f, 0.7f, 0.4f);
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 64 - Projectile.alpha / 4);
        }
    }
}
