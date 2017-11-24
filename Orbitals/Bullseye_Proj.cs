using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Dusts;

namespace Virtuous.Orbitals
{
    public class Bullseye_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Sight");
        }


        public  const float GapTreshold = 0.990f; //How forgiving it is with the damage boost zone. Used by OrbitalPlayer
        private const int Distance = 40; //Constant distance away from the player
        private const int FadeTime = 120; //Time it spends fading out
        private const int OriginalAlpha = 50; //Alpha value before fading out
        private Vector2 relativePos { get { return new Vector2(projectile.ai[0], projectile.ai[1]); } set { projectile.ai[0] = value.X; projectile.ai[1] = value.Y; } } //Relative position to the player. Stored as the projectile's native ai[0] and ai[1]

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 58;
            projectile.alpha = OriginalAlpha;
            projectile.timeLeft = 2;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.netUpdate = true; //Temporary cover for multiplayer acting strange

            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.Bullseye] && projectile.owner == Main.myPlayer) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                projectile.timeLeft = 2; //Keep it from dying naturally

                //Stays in front of the player
                projectile.spriteDirection = player.direction;
                relativePos = new Vector2(player.direction * Distance, 0);
                projectile.Center = player.Center + relativePos;

                Lighting.AddLight(new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y), 0.5f, 0.3f, 0.05f);

                if (orbitalPlayer.BullseyeShot())
                {
                    Vector2 position = new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y - 1);
                    Dust newDust = Dust.NewDustDirect(position, 0, 0, mod.DustType<RainbowDust>(), 0f, 0f, /*Alpha*/50, new Color(255, 127, 0, 50), /*Scale*/1.5f);
                    newDust.velocity = new Vector2(player.direction * 2.5f, 0);
                }

                if (orbitalPlayer.time <= FadeTime)
                {
                    projectile.alpha += (int)Math.Ceiling((255f - OriginalAlpha) / FadeTime); //Fades away completely over dyingTime
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
            return new Color(255, 130, 20, 200) * projectile.Opacity;
        }
    }
}