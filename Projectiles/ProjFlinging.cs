using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Projectiles
{
    public class ProjFlinging : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Lift");
            Main.projFrames[projectile.type] = 16;
        }

        private int originalDamage { get { return (int)projectile.ai[0]; } set { projectile.ai[0] = (int)value; } } //Full damage of the projectile to change it later, stored as the projectile's native ai[0]
        private float originalKnockback { get { return projectile.ai[1]; } set { projectile.ai[1] = value; } } //Full knockback of the projectile to change it later, stored as the projectile's native ai[1]

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
            projectile.usesLocalNPCImmunity = true; //Hits once per individual projectile
            projectile.localNPCHitCooldown = 32;
        }

        public override void AI()
        {
            if(projectile.timeLeft == 32) //First Tick
            {
                projectile.velocity = Vector2.Zero;
                originalDamage = projectile.damage; //Stores the damage so it can be changed later
                originalKnockback = projectile.knockBack; //Stores the knockback and sets it to 0, as it will be used for the vertical push instead
                projectile.knockBack = 0;

                if((Main.player[projectile.owner].Center - projectile.Center).X < 0) //Facing left
                {
                    projectile.spriteDirection = -1;
                }
            }

            if (projectile.timeLeft <= 16 && projectile.timeLeft >= 4) //The projectile will only affect enemies for a fraction of its life
            {
                projectile.damage = originalDamage;
            }
            else
            {
                projectile.damage = 0;
            }

            if (projectile.timeLeft % 2 == 0) projectile.frame++; //Changes frame every second tick

            Lighting.AddLight(projectile.Center, 0.0f, 0.5f, 0.2f);
        }


        private bool IsFlying(NPC target)
        {
            if (   target.aiStyle ==  2 || target.aiStyle ==  5 || target.aiStyle == 14 || target.aiStyle == 17 || target.aiStyle == 22
                || target.aiStyle == 22 || target.aiStyle == 23 || target.aiStyle == 40 || target.aiStyle == 108
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            VirtuousNPC modtarget = target.GetGlobalNPC<VirtuousNPC>();

            if (!target.immortal && !target.boss) //Excludes dummies and bosses
            {
                if (!target.noTileCollide && target.knockBackResist != 1 && !IsFlying(target)) //If it's applicable for fall damage
                {
                    modtarget.fallDamage = 1; //Initiates the fall damage counter
                    modtarget.alreadyStartedFalling = false; //Resets any previous fall damage data
                }

                target.velocity += new Vector2(0, -originalKnockback); //Lifts the enemy into the air
            }
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            target.velocity += new Vector2(0, -originalKnockback);
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(50, 255, 100, 150) * projectile.Opacity;
        }

    }
}