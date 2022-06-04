using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
            get { return (int)Projectile.localAI[0]; }
            set { Projectile.localAI[0] = value; }
        } 

        public int Appearance // How long it moves for before stopping, stored as ai[0]
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        public bool Crit // Whether the original hit was a critical hit or not, stored as ai[1]
        {
            get { return Projectile.ai[1] != 0; }
            set { Projectile.ai[1] = value ? 1 : 0; }
        }



        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;

            DisplayName.SetDefault("Crusher Pillar");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Pilar Apretillo");
        }


        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 76;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifespan;
            Projectile.friendly = true;
            Projectile.melee = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true; // Hits once per individual projectile
            Projectile.localNPCHitCooldown = Lifespan;
        }


        public override void AI()
        {
            if (Projectile.timeLeft == Lifespan) // The pojectile has just spawned
            {
                // It'll move until it reaches the edge of the opposite pillar
                MoveTime = (int)Math.Ceiling((SpawnDistance-Projectile.width/2) / Projectile.velocity.Length());
                MoveTime += 1; // Overshoot as the sprites are irregular

                if (Projectile.velocity.X < 0) Projectile.spriteDirection = -1; //Going left
                Projectile.frame = Appearance;

                Projectile.netUpdate = true; // Syncs to multiplayer just in case
            }
           
            else if (Projectile.timeLeft == Lifespan - MoveTime) // The pillars have just contacted
            {
                // Dust spawns at the contact line
                Vector2 dustoffset = new Vector2(Projectile.velocity.X > 0 ? +Projectile.width / 2 : -Projectile.width / 2, 0);

                int dustAmount = Crit ? Main.rand.Next(6, 10) : Main.rand.Next(12, 15);
                for (int i = 1; i <= dustAmount; i++)
                {
                    // More dust concentration in the center
                    dustoffset.Y = Main.rand.NextBool(3) ? 0 : Main.rand.Next(-Projectile.height/2, Projectile.height/2 + 1);
                    
                    var dust = Dust.NewDustDirect(
                        Projectile.Center + dustoffset, 0, 0, DustID.Stone, Alpha: 100, Scale: 2f);
                    dust.velocity = Projectile.velocity * Main.rand.NextFloat(-0.5f, +0.5f);
                    dust.fadeIn = 0.5f;
                    dust.noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                Projectile.velocity = Vector2.Zero;
            }

            else if(Projectile.timeLeft <= FadeTime) // Fading
            {
                Projectile.alpha += (int)Math.Ceiling(200f / FadeTime);
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
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }
    }
}