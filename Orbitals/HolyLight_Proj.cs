using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class HolyLight_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        //Passive
        private const float DistanceAvg = 70f; //Average distance to the player
        private const float OscSpeedMax = 0.2f; //How fast it can oscillate in and out, which is to say, how far it can go before changing direction
        private const float OscAcc = OscSpeedMax / 60; //How quickly it changes oscillation speed, which is to say, how quickly it reaches the point of direction change
        private const float OrbSpeed = 1/30f * RevolutionPerSecond; //Orbiting speed
        private const float RotSpeed = -OrbSpeed; //How fast it rotates the sprite
        private bool  outwards { get { return projectile.ai[1] == 0; } set { projectile.ai[1] = value ? 0 : 1; } } //Direction it's currently oscillating in. Stores into the projectile's built-in ai[1], which is 0 by default (true in this case)
        private float distance = DistanceAvg; //Distance away from the player
        private float oscillationSpeed = -OscSpeedMax; //How quickly it's currently changing distance

        //Burst
        public  const int DyingTime = 10; //Time it spends bursting
        private const int OriginalSize = 30; //Size of the sprite
        private const int BurstSize = 120; //Size of the area where bursting causes damage

        //Main
        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)
        private Vector2 relativePos; //Relative position to the player


        public override void SetDefaults()
        {
            projectile.width = OriginalSize;
            projectile.height = OriginalSize;
            projectile.alpha = 50;
            projectile.timeLeft = 2;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true; //Doesn't interfere with other piercing damage
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.netUpdate = true; //Temporary cover for multiplayer acting strange

            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.HolyLight] && projectile.owner == Main.myPlayer) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                if (firstTick)
                {
                    firstTick = false;
                    //The projectile's desired rotation and position was passed as its velocity, so we utilize it then set it to 0 so it doesn't move
                    projectile.rotation = projectile.velocity.ToRotation();
                    relativePos = projectile.velocity.OfLength(distance);
                    projectile.velocity = Vector2.Zero;

                    projectile.Center = player.MountedCenter + relativePos;
                    for (int i = 0; i < 15; i++) //Spawns some dust
                    {
                        Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/55, 0f, 0f, /*Alpha*/50, new Color(255, 230, 100), 1.2f);
                        newDust.velocity *= 0.8f;
                        newDust.noLight = false;
                        newDust.noGravity = true;
                    }
                }

                //Passive oscillation
                if      (oscillationSpeed >= +OscSpeedMax) outwards = false; //If it has reached the outwards speed limit, begin to switch direction
                else if (oscillationSpeed <= -OscSpeedMax) outwards = true; //If it has reached the inwards speed limit, begin to switch direction
                oscillationSpeed += outwards ? +OscAcc : -OscAcc; //Accelerate in clockwise or counterclockwise direction
                distance += oscillationSpeed;

                //Makes it orbit the player
                relativePos = relativePos.RotatedBy(OrbSpeed).OfLength(distance); //Rotates the projectile around the player and resets its distance
                projectile.rotation += RotSpeed; //Rotates the projectile itself
                projectile.Center = player.MountedCenter + relativePos; //Moves the projectile to the new position around the player

                Lighting.AddLight(projectile.Center, 1.0f, 1.0f, 0.6f);
                projectile.timeLeft = 2; //Keep it from dying naturally

                if (orbitalPlayer.time == DyingTime) //Explode
                {
                    projectile.alpha = 255; //Transparent
                    projectile.damage *= 2;
                    Main.PlaySound(SoundID.Item14, projectile.Center);
                    Lighting.AddLight(projectile.Center, 2.5f, 2.5f, 1.5f);

                    //Increase the hitbox and keep its center
                    projectile.position += new Vector2(projectile.width / 2, projectile.height /2);
                    projectile.width  = BurstSize;
                    projectile.height = BurstSize;
                    projectile.position -= new Vector2(projectile.width / 2, projectile.height / 2);

                    //Spawn dust
                    const int DustAmount = 15;
                    for (int i = 0; i < DustAmount; i++)
                    {
                        Dust newDust = Dust.NewDustDirect(projectile.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, new Color(255, 230,100), /*Scale*/1.0f);
                        newDust.velocity *= 2;
                    }
                }

            }
        }

        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return false;
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(255, 230, 180, 100) * projectile.Opacity;
        }
    }
}
