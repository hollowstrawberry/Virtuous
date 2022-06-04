using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    public class ProjFlinging : ModProjectile
    {
        private const int Lifespan = 32, DamageStart = 16, DamageEnd = 4;

        private static readonly int[] FlyingAiStyles = new[] {
            2, 5, 14, 17, 22, 23, 40, 108,
        };



        private int OriginalDamage // Stored as ai[0]
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        private float OriginalKnockback // Stored as ai[1]
        {
            get { return Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }



        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;

            DisplayName.SetDefault("Magic Lift");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Hechizo lanzador");
        }


        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 92;
            Projectile.friendly = true;
            Projectile.alpha = 50;
            Projectile.timeLeft = 32;
            Projectile.penetrate = -1;
            Projectile.magic = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true; // Hits once per individual projectile
            Projectile.localNPCHitCooldown = Lifespan;
        }


        public override void AI()
        {
            if (Projectile.timeLeft == Lifespan) // First Tick
            {
                Projectile.velocity = Vector2.Zero;
                OriginalDamage = Projectile.damage;
                OriginalKnockback = Projectile.knockBack;
                Projectile.knockBack = 0; // The knockback will be used for the vertical push instead

                if ((Main.player[Projectile.owner].Center - Projectile.Center).X < 0) // Facing left
                {
                    Projectile.spriteDirection = -1;
                }
            }

            // The projectile only affect enemies during a part of its life
            if (Projectile.timeLeft <= DamageStart && Projectile.timeLeft >= DamageEnd) Projectile.damage = OriginalDamage;
            else Projectile.damage = 0;

            if (Projectile.timeLeft % 2 == 0) Projectile.frame++; // Changes animation frame every second tick

            Lighting.AddLight(Projectile.Center, 0.0f, 0.5f, 0.2f);
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            var modTarget = target.GetGlobalNPC<VirtuousNPC>();

            if (!target.immortal && !target.boss) // Excludes dummies and bosses
            {
                if (!target.noTileCollide && target.knockBackResist != 1 && !FlyingAiStyles.Any(x => target.aiStyle == x))
                {
                    if (modTarget.fallDamage == 0) modTarget.fallDamage = 1; // Initiates the fall damage counter
                    modTarget.alreadyStartedFalling = false;
                }

                target.velocity += new Vector2(0, -OriginalKnockback); // Lifts the enemy into the air
            }
        }


        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            target.velocity += new Vector2(0, -OriginalKnockback);
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(50, 255, 100, 150) * Projectile.Opacity;
        }
    }
}