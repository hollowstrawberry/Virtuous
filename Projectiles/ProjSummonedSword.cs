using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Projectiles
{
    public class ProjSummonedSword : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Sword");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }


        public const int StuckDOT = 100; //Damage over time per stuck sword
        public const int StuckMaxAmount = 10; //Max stuck swords that can cause damage at the same time

        private bool firstTick = true;
        private bool HasHitEnemy { get { return projectile.ai[0] == 1; } set { projectile.ai[0] = value ? 1 : 0; } } //Whether it's hit an enemy and is now stuck to it
        private NPC Target { get { return Main.npc[(int)projectile.ai[1]]; } set { projectile.ai[1] = value.whoAmI; } } //The enemy it's stuck to
        private Vector2 relativeCenter; //The relative position in relation to the target it's stuck to
        private float originalProjRotation;
        private float originalTargetRotation;

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.scale = 0.6f;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.timeLeft = 4 * 60;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.MaxUpdates = 2; //Updates twice instead of once per tick
            projectile.timeLeft *= projectile.MaxUpdates;
            projectile.usesLocalNPCImmunity = true; //Invincibility per individual projectile
            projectile.localNPCHitCooldown = projectile.timeLeft;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.0f, 0.5f, 0.6f);
            if(HasHitEnemy)
            {
                projectile.Center = Target.Center - relativeCenter.RotatedBy(Target.rotation - originalTargetRotation); //Moves the projectile to the fixed position around the target, relative to where it originally hit
                projectile.rotation = originalProjRotation - originalTargetRotation + Target.rotation; //Rotates the sprite accordingly

                if (!Target.active) projectile.Kill(); //Kills the projectile if the target is dead
                Target.GetGlobalNPC<VirtuousNPC>().summonedSwordStuck++; //Applies the damage over time effect to the target
                projectile.netUpdate = true; //Syncs to the server
            }

            if (firstTick)
            {
                projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians(); //45 degrees because of the sprite

                const int DustAmount = 16;
                for (int i = 0; i < DustAmount; i++) //We create 16 dusts in an ellipse
                {
                    Vector2 rotation = Vector2.UnitY.RotatedBy(FullCircle * i/DustAmount); //Divides a circle into 16 points and picks the current one in the loop
                    rotation *= new Vector2(1,4); // Multiplies the points by a vertical squish factor so the circle becomes an ellipse
                    rotation = rotation.RotatedBy(projectile.velocity.ToRotation()); //Rotates the resulting ellipse to align with the projectile's rotation
                    
                    Dust newDust = Dust.NewDustDirect(projectile.Center + rotation, 0, 0, /*Type*/180, 0f, 0f, /*Alpha*/0, default(Color), /*Scale*/1.5f);
                    newDust.velocity = rotation.Normalized(); //Shoots outwards
                    newDust.noGravity = true;
                }
                firstTick = false;
                projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //Sets new projectile behavior
            projectile.damage = 0;
            projectile.timeLeft = projectile.MaxUpdates * ((projectile.velocity.Y == 0) ? 150 : 90); //Horizontal shots get longer sticking duration
            projectile.velocity = Vector2.Zero;
            projectile.tileCollide = false;
            HasHitEnemy = true;

            //Stores the info to align the projectile with the target
            Target = target;
            originalTargetRotation = Target.rotation;
            originalProjRotation = projectile.rotation;
            relativeCenter = Target.Center - projectile.Center;
            projectile.netUpdate = true; //Syncs to the server
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item27, projectile.position);

            int dustAmount = RandomInt(4,10);
            for (int i = 1; i <= dustAmount; i++)
            {
                Dust newDust = Dust.NewDustDirect(projectile.Center, 0, 0, /*Type*/180, 0f, 0f, /*Alpha*/100, default(Color), /*Scale*/2f);
                newDust.velocity -= projectile.velocity*0.5f * (RandomFloat(-1, +1)); //Random direction, mostly aligns with the projectile's
                newDust.fadeIn = 0.5f;
                newDust.noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!HasHitEnemy)
            {
                Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                    Color color = projectile.GetAlpha(lightColor) * 0.8f * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                    spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(100, 255, 255, 25) * projectile.Opacity;
        }

    }
}