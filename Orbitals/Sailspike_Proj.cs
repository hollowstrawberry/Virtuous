using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Sailspike_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
        }

        public  const int DyingTime = 30; //Time it spends flying outward before dying
        private const int Distance = 55; //Constant distance away from the player
        private const int OriginalAlpha = 100; //Alpha value before shooting out
        private const float ShootSpeed = 20; //Speed it starts with when shooting out
        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)
        private Vector2 relativePos; //Relative position to the player
        private float curShootSpeed = ShootSpeed; //Current speed when it's flying outward

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 14;
            projectile.alpha = OriginalAlpha;
            projectile.timeLeft = DyingTime;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.netUpdate = true; //Temporary cover for multiplayer acting strange

            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.Sailspike] && projectile.owner == Main.myPlayer) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                //Before it shoots out and dies
                if (orbitalPlayer.time > DyingTime || projectile.owner != Main.myPlayer)
                {
                    projectile.timeLeft = DyingTime; //Keep it from dying naturally

                    //Stays in front of the player
                    projectile.spriteDirection = player.direction;
                    relativePos = new Vector2(player.direction * Distance, 0);
                    projectile.Center = player.MountedCenter + relativePos;

                    if (firstTick) //Spawns some dust
                    {
                        firstTick = false;
                        for (int i = 0; i < 15; i++)
                        {
                            Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/172, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                            newDust.velocity *= 0.2f;
                            newDust.noLight = false;
                            newDust.noGravity = true;
                        }
                    }
                }
                else //timeLeft actually starts going down from dyingTime while it flies away
                {
                    if (orbitalPlayer.time == DyingTime) projectile.damage *= 2; //Sets it to deal more damage while shooting out
                    projectile.Center += relativePos.OfLength(curShootSpeed); //Projectile moves forward in the direction of its rotation
                    curShootSpeed -= ShootSpeed/20; //Slows down to a halt
                    if(projectile.timeLeft <= 20) projectile.alpha += (int)Math.Ceiling((255f-OriginalAlpha) / 20); //Fades away completely
                }

                Lighting.AddLight(projectile.Center, 0.15f, 0.5f, 1.5f);
            }
        }

        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return false;
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(28, 77, 255, 200) * projectile.Opacity;
        }

    }
}

