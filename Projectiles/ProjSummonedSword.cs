using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    public class ProjSummonedSword : ModProjectile
    {
        public const int StuckDOT = 100; // Damage over time per stuck sword
        public const int StuckMaxAmount = 10; // Max stuck swords that can cause damage at the same time
        private const int Updates = 2; // Times it will update per tick instead of 1
        private const int Lifespan = 4 * 60 * Updates; // Time in ticks



        private bool HasHitEnemy // Whether it's hit an enemy and is now stuck to it, stored as ai[0]
        {
            get { return projectile.ai[0] != 0; }
            set { projectile.ai[0] = value ? 1 : 0; }
        }

        private NPC Target // The enemy it's stuck to, stored as ai[1]
        {
            get { return Main.npc[(int)projectile.ai[1]]; }
            set { projectile.ai[1] = value.whoAmI; }
        }

        private float OriginalProjRotation // Projectile rotation before being stuck, stored as localAI[0]
        {
            get { return projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        }

        private float OriginalTargetRotation // Stuck target's rotation before being stuck, stored as localAI[1]
        {
            get { return projectile.localAI[1]; }
            set { projectile.localAI[1] = value; }
        }

        private Vector2 RelativeCenter // The relative position in relation to the target it's stuck to, stored as velocity
        {
            get { return projectile.velocity; }
            set { projectile.velocity = value; }
        }




        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;

            DisplayName.SetDefault("Summoned Sword");
            DisplayName.AddTranslation(GameCulture.Spanish, "Espada etérea");
        }


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
            projectile.usesLocalNPCImmunity = true; // Invincibility acts per individual projectile
            projectile.localNPCHitCooldown = projectile.timeLeft;
        }


        public override void AI()
        {
            if (projectile.timeLeft == Lifespan) // First tick
            {
                projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians(); // 45 degrees because of the sprite

                int dustAmount = 16;
                for (int i = 0; i < dustAmount; i++)
                {
                    Vector2 offset = Vector2.UnitY.RotatedBy(Tools.FullCircle * i / dustAmount) * new Vector2(1, 4); // Ellipse of dust
                    offset = offset.RotatedBy(projectile.velocity.ToRotation()); // Rotates the ellipse to align with the projectile's rotation

                    var dust = Dust.NewDustDirect(projectile.Center + offset, 0, 0, Type: 180, Scale: 1.5f);
                    dust.velocity = offset.OfLength(1); // Shoots outwards
                    dust.noGravity = true;
                }
            }

            if (HasHitEnemy)
            {
                if (!Target.active)
                {
                    projectile.Kill(); // Kills the projectile if the target is dead
                    return;
                }

                Target.GetGlobalNPC<VirtuousNPC>().summonedSwordStuck++; // Applies the damage-over-time effect to the target

                // Moves and rotates the projectile to the fixed position around the target, relative to where it originally hit
                projectile.Center = Target.SpriteCenter() - RelativeCenter.RotatedBy(Target.rotation - OriginalTargetRotation);
                projectile.rotation = OriginalProjRotation - OriginalTargetRotation + Target.rotation;
                projectile.position -= projectile.velocity; //Stops velocity from affecting the projectile normally
            }

            Lighting.AddLight(projectile.Center, 0.0f, 0.5f, 0.6f);
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Sets new projectile behavior
            projectile.damage = 0;
            projectile.timeLeft = Updates * ((projectile.velocity.Y == 0) ? 150 : 90); // Horizontal shots get longer sticking duration
            projectile.velocity = Vector2.Zero;
            projectile.tileCollide = false;
            HasHitEnemy = true;

            // Stores the info to align the projectile with the target
            Target = target;
            OriginalTargetRotation = Target.rotation;
            OriginalProjRotation = projectile.rotation;
            RelativeCenter = Target.Center - projectile.Center;

            projectile.netUpdate = true; // Syncs to the server just in case
        }


        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item27, projectile.position);

            int dustAmount = Main.rand.Next(4, 11);
            for (int i = 1; i <= dustAmount; i++)
            {
                var dust = Dust.NewDustDirect(projectile.Center, 0, 0, Type: 180, Alpha: 100, Scale: 2f);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;

                if (!HasHitEnemy) // Random direction, mostly aligns with the projectile's
                {
                    dust.velocity -= projectile.velocity * 0.5f * (Main.rand.NextFloat(-1, +1));
                }
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) // Trail
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            drawOrigin += new Vector2(drawOffsetX, 0);

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * 0.8f * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);

                spriteBatch.Draw(
                    Main.projectileTexture[projectile.type], drawPos, null, color,
                    projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }

            return true;
        }



        // Syncs local ai slots in multiplayer

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