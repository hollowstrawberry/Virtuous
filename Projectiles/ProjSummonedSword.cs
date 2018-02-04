using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


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
        private const int Updates = 2; //Times it will update per tick instead of 1
        private const int Lifespan = 4 * 60 * Updates; //Time in ticks

        private bool HasHitEnemy { get{ return projectile.ai[0] == 1; } set{ projectile.ai[0] = value ? 1 : 0; } } //Whether it's hit an enemy and is now stuck to it
        private NPC Target { get{ return Main.npc[(int)projectile.ai[1]]; } set{ projectile.ai[1] = value.whoAmI; } } //The enemy it's stuck to
        private Vector2 relativeCenter { get{ return projectile.velocity; } set{ projectile.velocity = value; } } //The relative position in relation to the target it's stuck to
        private float originalProjRotation { get{ return projectile.localAI[0]; } set{ projectile.localAI[0] = value; } } //Projectile rotation before being stuck
        private float originalTargetRotation { get{ return projectile.localAI[1]; } set{ projectile.localAI[1] = value; } } //Stuck target's rotation before being stuck


        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.scale = 0.6f;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.timeLeft = Lifespan;
            projectile.MaxUpdates = Updates;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.usesLocalNPCImmunity = true; //Invincibility per individual projectile
            projectile.localNPCHitCooldown = projectile.timeLeft;
        }

        public override void AI()
        {
            if (projectile.timeLeft == Lifespan) //First tick
            {
                projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians(); //45 degrees because of the sprite

                int dustAmount = 16;
                for (int i = 0; i < dustAmount; i++) //We create 16 dusts in an ellipse
                {
                    Vector2 rotation = Vector2.UnitY.RotatedBy(Tools.FullCircle * i / dustAmount); //A circle of radius 1 is divided into the set amount of points, focusing on the current point in the loop
                    rotation *= new Vector2(1, 4); //Multiplies the points by a vertical squish factor so the circle becomes an ellipse
                    rotation = rotation.RotatedBy(projectile.velocity.ToRotation()); //Rotates the resulting ellipse to align with the projectile's rotation

                    Dust newDust = Dust.NewDustDirect(projectile.Center + rotation, 0, 0, /*Type*/180, 0f, 0f, /*Alpha*/0, default(Color), /*Scale*/1.5f);
                    newDust.velocity = rotation.OfLength(1); //Shoots outwards
                    newDust.noGravity = true;
                }
            }

            if (HasHitEnemy) //Is stuck to an enemy
            {
                if (!Target.active) projectile.Kill(); //Kills the projectile if the target is dead
                Target.GetGlobalNPC<VirtuousNPC>().summonedSwordStuck++; //Applies the damage-over-time effect to the target

                projectile.Center = Target.SpriteCenter() - relativeCenter.RotatedBy(Target.rotation - originalTargetRotation); //Moves the projectile to the fixed position around the target, relative to where it originally hit
                projectile.rotation = originalProjRotation - originalTargetRotation + Target.rotation; //Rotates the sprite accordingly
                projectile.position -= projectile.velocity; //Stops velocity from affecting the projectile normally
            }

            Lighting.AddLight(projectile.Center, 0.0f, 0.5f, 0.6f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //Sets new projectile behavior
            projectile.damage = 0;
            projectile.timeLeft = Updates * ((projectile.velocity.Y == 0) ? 150 : 90); //Horizontal shots get longer sticking duration
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

            int dustAmount = Tools.RandomInt(4,10);
            for (int i = 1; i <= dustAmount; i++)
            {
                Dust newDust = Dust.NewDustDirect(projectile.Center, 0, 0, /*Type*/180, 0f, 0f, /*Alpha*/100, default(Color), /*Scale*/2f);
                if (!HasHitEnemy) newDust.velocity -= projectile.velocity*0.5f * (Tools.RandomFloat(-1, +1)); //Random direction, mostly aligns with the projectile's
                newDust.fadeIn = 0.5f;
                newDust.noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) //Trail
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * 0.8f * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }

            return true;
        }

        //Syncs local ai slots in multiplayer
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(100, 255, 255, 25) * projectile.Opacity;
        }

    }
}