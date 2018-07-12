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
            get { return (int)projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        private float OriginalKnockback // Stored as ai[1]
        {
            get { return projectile.ai[1]; }
            set { projectile.ai[1] = value; }
        }



        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 16;

            DisplayName.SetDefault("Magic Lift");
            DisplayName.AddTranslation(GameCulture.Spanish, "Hechizo lanzador");
        }


        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 92;
            projectile.friendly = true;
            projectile.alpha = 50;
            projectile.timeLeft = 32;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true; // Hits once per individual projectile
            projectile.localNPCHitCooldown = Lifespan;
        }


        public override void AI()
        {
            if (projectile.timeLeft == Lifespan) // First Tick
            {
                projectile.velocity = Vector2.Zero;
                OriginalDamage = projectile.damage;
                OriginalKnockback = projectile.knockBack;
                projectile.knockBack = 0; // The knockback will be used for the vertical push instead

                if ((Main.player[projectile.owner].Center - projectile.Center).X < 0) // Facing left
                {
                    projectile.spriteDirection = -1;
                }
            }

            // The projectile only affect enemies during a part of its life
            if (projectile.timeLeft <= DamageStart && projectile.timeLeft >= DamageEnd) projectile.damage = OriginalDamage;
            else projectile.damage = 0;

            if (projectile.timeLeft % 2 == 0) projectile.frame++; // Changes animation frame every second tick

            Lighting.AddLight(projectile.Center, 0.0f, 0.5f, 0.2f);
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
            return new Color(50, 255, 100, 150) * projectile.Opacity;
        }
    }
}