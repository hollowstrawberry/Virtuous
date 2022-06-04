using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    public class ProjGelFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flaming gel");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Gel ardiente");
        }


        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true; // Doesn't conflict with other piercing damage
            Projectile.idStaticNPCHitCooldown = 10;
        }


        public override void AI()
        {
            if (Projectile.wet) Projectile.Kill();

            int dustAmount = Main.rand.Next(1, 3);
            for (int i = 0; i < dustAmount; i++)
            {
                var dust = Dust.NewDustDirect(
                    Projectile.Center, Projectile.width, Projectile.height, DustID.Fire,
                    Alpha: 100, Scale: Main.rand.NextFloat(1.3f, 1.8f));
                dust.noGravity = true;

                if (i == 0) dust.velocity.Y -= 2f;
                else dust.velocity.Y *= 0.1f;
            }

            if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f) // Slows down on the ground
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                if (Math.Abs(Projectile.velocity.X) < 0.01)
                {
                    Projectile.velocity.X = 0f;
                }
            }

            Projectile.velocity.Y += 0.2f; // Gravity

            if (Projectile.velocity.Y < 0.25 && Projectile.velocity.Y > 0.15) // No idea why this is here
            {
                Projectile.velocity.X *= 0.8f;
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.Y *= -0.3f; // Bounce
            Projectile.velocity.X *=  0.3f;
            return false; // Don't die
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 5 * 60);
        }
    }
}
