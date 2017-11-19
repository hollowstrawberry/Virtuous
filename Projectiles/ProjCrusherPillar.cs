using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Projectiles
{
    public class ProjCrusherPillar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crusher Pillar");
        }

        public  const int TargetDistance = 100; //How far from the target the projectile spawns. Used by the item class
        private const int Lifespan = 40; //Total duration of the projectile
        private const int FadeTime = 20; //How long it fades away for
        private int moveTime { get { return (int)projectile.ai[0]; } set { projectile.ai[0] = value; } } //How long it moves for before stopping. Stored as the projectile's native ai[0]
        private bool crit { get { return projectile.ai[0] > 0; } } //Whether the original hit was a critical hit or not, passed as ai[1]

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 90;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.timeLeft = Lifespan;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true; //Hits once per individual projectile
            projectile.localNPCHitCooldown = Lifespan;
        }

        public override void AI()
        {

            if (projectile.timeLeft == Lifespan) //If projectile has just spawned
            {
                moveTime = (int)Math.Ceiling((TargetDistance-projectile.width/2) / projectile.velocity.Length()); //It'll move until its edge reaches the edge of the opposite pillar
            }
            else if (projectile.timeLeft == Lifespan - moveTime) //Movetime is over, and the pillars have contacted
            {
                int dustAmount = crit ? Main.rand.Next(6,9+1) : Main.rand.Next(12,14+1); //More dust if it's a crit
                Vector2 dustoffset; //How far from the projectile center the dust should spawn
                dustoffset.X = projectile.velocity.X>0 ? +projectile.width/2 : -projectile.width/2; //To the left or right of the pillar depending on where it's coming from
                for (int i = 1; i <= dustAmount; i++)
                {
                    dustoffset.Y = Main.rand.Next(3)==0 ? 0 : Main.rand.Next(-projectile.height/2, projectile.height/2+1); //Chance of the dust appearing in the center or somewhere else along the two pillars' contact line
                    
                    Dust newDust = Dust.NewDustDirect(projectile.Center+dustoffset, 0, 0, DustID.Dirt, 0f, 0f, /*Alpha*/100, default(Color), /*Scale*/2f);
                    newDust.velocity = projectile.velocity*0.5f * (Main.rand.NextFloat()*2f - 1f); //Same direction as pillar before stopping
                    newDust.fadeIn = 0.5f;
                    newDust.noGravity = true;
                }

                Main.PlaySound(SoundID.Dig, projectile.position);
                projectile.velocity = Vector2.Zero; //Pillar stops
            }
            else if(projectile.timeLeft <= FadeTime) //Fadetime has begun
            {
                projectile.alpha += (int)Math.Ceiling(200f / FadeTime); //Spreads the fading over fadetime
            }

        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (crit)
            {
                damage /= 2;
                crit = true;
            }
            else
            {
                crit = false;
            }
        }

    }
}