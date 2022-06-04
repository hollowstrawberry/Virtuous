using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            get { return Projectile.ai[0] != 0; }
            set { Projectile.ai[0] = value ? 1 : 0; }
        }

        private NPC Target // The enemy it's stuck to, stored as ai[1]
        {
            get { return Main.npc[(int)Projectile.ai[1]]; }
            set { Projectile.ai[1] = value.whoAmI; }
        }

        private float OriginalProjRotation // Projectile rotation before being stuck, stored as localAI[0]
        {
            get { return Projectile.localAI[0]; }
            set { Projectile.localAI[0] = value; }
        }

        private float OriginalTargetRotation // Stuck target's rotation before being stuck, stored as localAI[1]
        {
            get { return Projectile.localAI[1]; }
            set { Projectile.localAI[1] = value; }
        }

        private Vector2 RelativeCenter // The relative position in relation to the target it's stuck to, stored as velocity
        {
            get { return Projectile.velocity; }
            set { Projectile.velocity = value; }
        }




        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            DisplayName.SetDefault("Summoned Sword");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Espada etérea");
        }


        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 0.6f;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifespan;
            Projectile.MaxUpdates = Updates;
            Projectile.magic = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true; // Invincibility acts per individual projectile
            Projectile.localNPCHitCooldown = Projectile.timeLeft;
        }


        public override void AI()
        {
            if (Projectile.timeLeft == Lifespan) // First tick
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 45.ToRadians(); // 45 degrees because of the sprite

                int dustAmount = 16;
                for (int i = 0; i < dustAmount; i++)
                {
                    Vector2 offset = Vector2.UnitY.RotatedBy(Tools.FullCircle * i / dustAmount) * new Vector2(1, 4); // Ellipse of dust
                    offset = offset.RotatedBy(Projectile.velocity.ToRotation()); // Rotates the ellipse to align with the projectile's rotation

                    var dust = Dust.NewDustDirect(Projectile.Center + offset, 0, 0, Type: 180, Scale: 1.5f);
                    dust.velocity = offset.OfLength(1); // Shoots outwards
                    dust.noGravity = true;
                }
            }

            if (HasHitEnemy)
            {
                if (!Target.active)
                {
                    Projectile.Kill(); // Kills the projectile if the target is dead
                    return;
                }

                Target.GetGlobalNPC<VirtuousNPC>().summonedSwordStuck++; // Applies the damage-over-time effect to the target

                // Moves and rotates the projectile to the fixed position around the target, relative to where it originally hit
                Projectile.Center = Target.SpriteCenter() - RelativeCenter.RotatedBy(Target.rotation - OriginalTargetRotation);
                Projectile.rotation = OriginalProjRotation - OriginalTargetRotation + Target.rotation;
                Projectile.position -= Projectile.velocity; //Stops velocity from affecting the projectile normally
            }

            Lighting.AddLight(Projectile.Center, 0.0f, 0.5f, 0.6f);
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Sets new projectile behavior
            Projectile.damage = 0;
            Projectile.timeLeft = Updates * ((Projectile.velocity.Y == 0) ? 150 : 90); // Horizontal shots get longer sticking duration
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            HasHitEnemy = true;

            // Stores the info to align the projectile with the target
            Target = target;
            OriginalTargetRotation = Target.rotation;
            OriginalProjRotation = Projectile.rotation;
            RelativeCenter = Target.Center - Projectile.Center;

            Projectile.netUpdate = true; // Syncs to the server just in case
        }


        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

            int dustAmount = Main.rand.Next(4, 11);
            for (int i = 1; i <= dustAmount; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, Type: 180, Alpha: 100, Scale: 2f);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;

                if (!HasHitEnemy) // Random direction, mostly aligns with the projectile's
                {
                    dust.velocity -= Projectile.velocity * 0.5f * (Main.rand.NextFloat(-1, +1));
                }
            }
        }


        public override bool PreDraw(ref Color lightColor) // Trail
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            drawOrigin += new Vector2(drawOffsetX, 0);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * 0.8f * ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);

                spriteBatch.Draw(
                    TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color,
                    Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            return true;
        }



        // Syncs local ai slots in multiplayer

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }



        public override Color? GetAlpha(Color newColor)
        {
            return new Color(100, 255, 255, 25) * Projectile.Opacity;
        }
    }
}