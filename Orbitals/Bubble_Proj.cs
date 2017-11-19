using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Bubble_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)
        private const int FadeTime = 60; //Time it spends fading away
        private const int OriginalAlpha = 120; //Alpha value before fading away

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.alpha = OriginalAlpha;
            projectile.timeLeft = 2;
            projectile.damage = 1;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true; //Doesn't interfere with other piercing damage
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.Bubble]) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                projectile.Center = player.MountedCenter; //Keep it on the player
                projectile.timeLeft = 2; //Keep it from dying naturally

                if (firstTick) //Spawns some dust
                {
                    firstTick = false;

                    projectile.damage = 1;

                    for (int i = 0; i < 40; i++)
                    {
                        Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/16, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                        newDust.velocity *= 1.5f;
                        newDust.noLight = false;
                    }
                }

                if (orbitalPlayer.time <= FadeTime)
                {
                    projectile.alpha += (int)Math.Ceiling((255f-OriginalAlpha) / FadeTime); //Fades away completely over fadeTime
                }
                else
                {
                    projectile.alpha = OriginalAlpha;
                }
            }
        }

        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return false;
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(224, 255, 252, 150) * projectile.Opacity;
        }

    }
}
