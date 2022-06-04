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
            Main.projFrames[Projectile.type] = 4;

            DisplayName.SetDefault("Flurry Nova");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Golpe Nova");
        }


        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 30;
            Projectile.alpha = 10;
            Projectile.timeLeft = Lifespan;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.melee = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true; // Hits once per individual projectile
            Projectile.localNPCHitCooldown = Lifespan;
        }
        

        public override void AI()
        {
            if (Projectile.timeLeft == Lifespan) // Projectile just spawned
            {
                if (Projectile.velocity.X < 0) Projectile.spriteDirection = -1; // Facing left
                Projectile.velocity = Projectile.velocity.OfLength(ShootSpeed) + Main.player[Projectile.owner].velocity; // Follows player

                const float dustAmount = 16f;
                for (int i = 0; i < dustAmount; i++)
                {
                    Vector2 offset = Vector2.UnitY.RotatedBy(Tools.FullCircle * i / dustAmount) * new Vector2(1, 4); // Ellipse of dust
                    offset = offset.RotatedBy(Projectile.rotation); // Rotates the resulting ellipse to align with the projectile's rotation

                    Vector2 position = Projectile.Center + offset;
                    position.X -= 10 * Main.player[Projectile.owner].direction; // This line needs to change if needed to work with multidirectional fists

                    var dust = Dust.NewDustDirect(position, 0, 0, 127, 0f, 0f, newColor: new Color(255, 255, 0), Scale: 2.0f);
                    dust.velocity = offset.Normalized(); // Shoots outwards
                    dust.noGravity = true;
                }
            }
            else if (Projectile.timeLeft < Lifespan - MoveTime && Projectile.timeLeft >= Lifespan - (MoveTime + SlowdownTime)) // Slow down
            {
                float oldspeed = Projectile.velocity.Length();
                Projectile.velocity *= (oldspeed - ShootSpeed/SlowdownTime) / oldspeed;
            }


            if(Projectile.timeLeft % 2 == 0) // Animation
            {
                if (Projectile.frame < 3) Projectile.frame++;
                else Projectile.frame = 0;
            }

            if (Projectile.timeLeft <= FadeTime) // Fade away
            {
                Projectile.alpha += 200/FadeTime;
            }
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 180 - Projectile.alpha);
        }
    }
}