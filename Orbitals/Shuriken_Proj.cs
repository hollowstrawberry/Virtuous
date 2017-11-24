using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class Shuriken_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Shuriken");
        }


        //Passive
        private const float DistanceAvg = 120f; //Average distance to the player when passive
        private const float OscSpeedMax = 6.0f; //How fast it can oscillate in and out, which is to say, how far it can go before changing direction
        private const float OscAcc = OscSpeedMax / 20; //How quickly it changes oscillation speed, which is to say, how quickly it reaches the point of direction change
        private const float OrbSpeed = 1 * RevolutionPerSecond; //Orbiting speed
        private bool outwards { get { return projectile.ai[1] == 0; } set { projectile.ai[1] = value ? 0 : 1; } } //Direction it's currently moving in (away or into the player). Stores into the projectile's built-in ai[1], which is 0 by default (true in this case)

        //Final spin
        public  const int DyingTime = 30; //Time it spends doing the final spin
        private const float DyingOrbSpeed = 4 * RevolutionPerSecond; //Final spin speed

        //Main
        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)
        private Vector2 relativePos; //Relative position to the player
        private float distance = DistanceAvg; //Distance away from the player
        private float oscillationSpeed = -OscSpeedMax; //How quickly it's changing distance


        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.alpha = 50;
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
            //if (projectile.owner != Main.myPlayer) return; //Only runs AI for the client
			projectile.netUpdate = true;

            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.Shuriken] && projectile.owner == Main.myPlayer) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                if (firstTick)
                {
                    firstTick = false;
                    //The projectile's desired rotation was passed as its velocity, so we utilize it then set it to 0 so it doesn't move
                    projectile.rotation = projectile.velocity.ToRotation();
                    relativePos = projectile.velocity.OfLength(distance);
                    projectile.velocity = Vector2.Zero;

                    //projectile.Center = player.MountedCenter + relativePos;
                    //for (int i = 0; i < 15; i++) //Spawns some dust
                    //{
                    //    Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/74, 0f, 0f, /*Alpha*/50, new Color(50, 255, 100, 150), 1.3f);
                    //    newDust.velocity *= 0.5f;
                    //    newDust.noLight = false;
                    //    newDust.noGravity = true;
                    //}
                }

                //Before it spins and dies
                if (orbitalPlayer.time > DyingTime || projectile.owner != Main.myPlayer)
                {
                    projectile.timeLeft = DyingTime; //Keep it from dying naturally

                    //Passive oscillation
                    if      (oscillationSpeed >= +OscSpeedMax) outwards = false; //If it has reached the outwards speed limit, begin to switch direction
                    else if (oscillationSpeed <= -OscSpeedMax) outwards = true; //If it has reached the inwards speed limit, begin to switch direction
                    oscillationSpeed += outwards ? +OscAcc : -OscAcc; //Accelerate innards or outwards
                    distance += oscillationSpeed; //Applies the change in distance
                }
                else //timeLeft actually starts going down from dyingTime while it spins
                {
                    if (orbitalPlayer.time == DyingTime) //First tick
                    {
                        projectile.damage *= 3;
                    }
                    else if (orbitalPlayer.time == 2) //Last tick, dunno why 1 doesn't work
                    {
                        Main.PlaySound(SoundID.Dig, projectile.Center); //Thump

                        const int DustAmount = 10;
                        for(int i = 0; i < DustAmount; i++)
                        {
                            Dust.NewDust(projectile.position, projectile.width, projectile.height, /*Type*/74, 0f, 0f, /*Alpha*/150, new Color(50, 255, 100, 150), /*Scale*/1.5f);
                        }
                    }
                }

                //Moves it around the player
                float newOrbitingSpeed = OrbSpeed; //Actual orbiting speed used
                if (orbitalPlayer.time <= DyingTime) newOrbitingSpeed = DyingOrbSpeed; //Changes it if it's the final spin
                //Makes them orbit the player
                relativePos = relativePos.RotatedBy(newOrbitingSpeed).OfLength(distance); //Rotates the shuriken around the player and resets its distance
                projectile.rotation += -newOrbitingSpeed * 2.0f; //Rotates the sprite as well
                projectile.Center = player.MountedCenter + relativePos; //Moves the shuriken to the defined position around the player

                Lighting.AddLight(projectile.Center, 0.0f, 1.0f, 0.2f);
            }
        }

        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return false;
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(10, 255, 50, 150) * projectile.Opacity;
        }
    }
}