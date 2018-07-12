using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    public class ProjCrusherPillar : ModProjectile
    {
        public const int SpawnDistance = 100; // How far from the target the projectile spawns. Used by the item's class
        private const int Lifespan = 40; // Total duration of the projectile
        private const int FadeTime = 20; // How long it fades away for



        private int MoveTime // How long the projectile will move for, stored as localAI[0]
        {
            get { return (int)projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        } 

        public int Appearance // How long it moves for before stopping, stored as ai[0]
        {
            get { return (int)projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        public bool Crit // Whether the original hit was a critical hit or not, stored as ai[1]
        {
            get { return projectile.ai[1] != 0; }
            set { projectile.ai[1] = value ? 1 : 0; }
        }



        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;

            DisplayName.SetDefault("Crusher Pillar");
            DisplayName.AddTranslation(GameCulture.Spanish, "Pilar Apretillo");
        }


        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 76;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.timeLeft = Lifespan;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true; // Hits once per individual projectile
            projectile.localNPCHitCooldown = Lifespan;
        }


        public override void AI()
        {
            if (projectile.timeLeft == Lifespan) // The pojectile has just spawned
            {
                // It'll move until it reaches the edge of the opposite pillar
                MoveTime = (int)Math.Ceiling((SpawnDistance-projectile.width/2) / projectile.velocity.Length());
                MoveTime += 1; // Overshoot as the sprites are irregular

                if (projectile.velocity.X < 0) projectile.spriteDirection = -1; //Going left
                projectile.frame = Appearance;

                projectile.netUpdate = true; // Syncs to multiplayer just in case
            }
           
            else if (projectile.timeLeft == Lifespan - MoveTime) // The pillars have just contacted
            {
                // Dust spawns at the contact line
                Vector2 dustoffset = new Vector2(projectile.velocity.X > 0 ? +projectile.width / 2 : -projectile.width / 2, 0);

                int dustAmount = Crit ? Main.rand.Next(6, 10) : Main.rand.Next(12, 15);
                for (int i = 1; i <= dustAmount; i++)
                {
                    // More dust concentration in the center
                    dustoffset.Y = Main.rand.OneIn(3) ? 0 : Main.rand.Next(-projectile.height/2, projectile.height/2 + 1);
                    
                    var dust = Dust.NewDustDirect(
                        projectile.Center + dustoffset, 0, 0, DustID.Stone, Alpha: 100, Scale: 2f);
                    dust.velocity = projectile.velocity * Main.rand.NextFloat(-0.5f, +0.5f);
                    dust.fadeIn = 0.5f;
                    dust.noGravity = true;
                }

                Main.PlaySound(SoundID.Dig, projectile.position);
                projectile.velocity = Vector2.Zero;
            }

            else if(projectile.timeLeft <= FadeTime) // Fading
            {
                projectile.alpha += (int)Math.Ceiling(200f / FadeTime);
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Crit) // The critical hit depends on the hammer's critical hit
            {
                damage /= 2;
                crit = true;
            }
            else
            {
                crit = false;
            }
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (Crit)
            {
                damage /= 2;
                crit = true;
            }
            else
            {
                crit = false;
            }
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
    }
}