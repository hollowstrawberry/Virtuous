using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Virtuous.Projectiles
{
    public class ProjGelFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 16;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.usesIDStaticNPCImmunity = true; //Doesn't conflict with other piercing damage
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.wet) projectile.Kill();

            int dustAmount = Main.rand.Next(1, 3);
            for (int i = 0; i < dustAmount; i++)
            {
                Dust newDust = Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.Fire, 0f, 0f, /*Alpha*/100, default(Color), Main.rand.NextFloat(1.3f, 1.8f));
                newDust.noGravity = true;
                if (i == 0) newDust.velocity.Y -= 2f;
                else newDust.velocity.Y *= 0.1f;
            }

            if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
            {
                projectile.velocity.X = projectile.velocity.X * 0.97f;
                if (Math.Abs(projectile.velocity.X) < 0.01)
                {
                    projectile.velocity.X = 0f;
                    projectile.netUpdate = true;
                }
            }
            projectile.velocity.Y += 0.2f;

            if (projectile.velocity.Y < 0.25 && projectile.velocity.Y > 0.15) projectile.velocity.X *= 0.8f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity.Y *= -0.3f;
            projectile.velocity.X *=  0.3f;
            return false; //Don't die
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 5 * 60);
        }
    }
}
