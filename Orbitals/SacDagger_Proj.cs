using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class SacDagger_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Dagger");
        }

        //Passive
        private const float DistanceAvg = 105f; //Average distance to the player when passive
        private const float OscSpeedMax = 0.4f; //How fast it can move in and out, which is to say, how far it can go before changing direction
        private const float OscAcc = OscSpeedMax / 40; //How quickly it changes oscillation speed, which is to say, how quickly it reaches the point of direction change
        private float oscillationSpeed = OscSpeedMax; //How fast it's currently oscillating
        private bool  outwards { get { return projectile.ai[1] == 0; } set { projectile.ai[1] = value ? 0 : 1; } } //Direction it's currently moving in (away or into the player). Stores into the projectile's built-in ai[1], which is 0 by default (true in this case)
        private float distance = DistanceAvg; //Current distance away from the player

        //Right-click
        private const float AngularSpeed = 2 * RevolutionPerSecond; //Spinning speed
        private float curAngularSpeed = AngularSpeed; //Current spinning speed
        private int rightClickTimer = 0; //How long it's been since the right click effect activated

        //Shooting out
        public  const int DyingTime = 30; //Time it spends flying outward before dying
        private const int OriginalAlpha = 50; //Alpha value before flying outward
        private const float ShootSpeed = 30; //Speed it starts with when flying outward
        private float curShootSpeed = ShootSpeed; //Current speed when it's flying outward

        //Main
        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)
        private Vector2 relativePos; //Relative position to the player
        private int originalDamage; //Stores the original damage so it can be changed later


        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 54;
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
            //if (projectile.owner != Main.myPlayer) return; //Only runs AI for the client
			projectile.netUpdate = true;

            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.SacDagger] && projectile.owner == Main.myPlayer) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                if (firstTick)
                {
                    firstTick = false;
                    //The projectile's desired rotation was passed as its velocity, so we utilize it then set it to 0 so it doesn't move
                    projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians(); //45 degrees because of the sprite
                    relativePos = projectile.velocity.OfLength(distance);
                    projectile.velocity = Vector2.Zero;
                    originalDamage = projectile.damage;

                }

                //Before it shoots out and dies
                if (orbitalPlayer.time > DyingTime || projectile.owner != Main.myPlayer)
                {
                    projectile.timeLeft = DyingTime; //Keep it from dying naturally

                    //Alters the distance from the player
                    if (!orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOn])
                    {
                        if (oscillationSpeed >= OscSpeedMax) outwards = false; //If it has reached the outwards speed limit, begin to switch direction
                        else if (oscillationSpeed <= -OscSpeedMax) outwards = true; //If it has reached the inwards speed limit, begin to switch direction
                        oscillationSpeed += outwards ? +OscAcc : -OscAcc; //Accelerate innards or outwards
                        distance += oscillationSpeed; //Apply the change in distance
                    }

                    //Right click effect of switching positions
                    if (orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOn])
                    {
                        if (rightClickTimer == 15) //Magic number for a half-rotation
                        {
                            orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOff] = true; //Initiate shutdown of the right click effect
                            rightClickTimer = 0;
                            projectile.damage = originalDamage;
                            projectile.netUpdate = true; //Syncs to multiplayer
                        }
                        else
                        {
                            if (rightClickTimer == 0) //Sets the spinning direction on the first tick
                            {
                                projectile.damage = (int)(originalDamage * 1.2); //Higher damage when spinning
                                curAngularSpeed = player.direction * AngularSpeed; //Spinning direction changes on player orientation
                                projectile.netUpdate = true; //Syncs to multiplayer
                            }
                            relativePos = relativePos.RotatedBy(curAngularSpeed); //Rotates the dagger around the player
                            projectile.rotation += curAngularSpeed; //Rotates the sprite accordingly
                            rightClickTimer++;
                        }
                    }

                    relativePos = relativePos.OfLength(distance); //Resets the distance
                    projectile.Center = player.MountedCenter + relativePos; //Moves the sword to the defined position around the player

                }
                else //timeLeft actually starts going down from dyingTime while it flies away
                {
                    if (orbitalPlayer.time == DyingTime) projectile.damage *= 3; //Deals more damage when shooting out
                    projectile.Center += relativePos.OfLength(curShootSpeed); //Projectile moves forward in the direction of its rotation
                    curShootSpeed -= ShootSpeed / DyingTime; //Slows down to a halt over dyingTime
                    projectile.alpha += (int)Math.Ceiling((255f - OriginalAlpha) / DyingTime); //Fades away over dyingTime
                }

                Lighting.AddLight(projectile.Center, 1.8f, 0f, 0f);

            }
        }


        private void LifeSteal(Vector2 position) //Spawns vampire heal projectiles depending on damage dealt and player's lifesteal status
        {
            float heal = projectile.damage / 50f;
            if (heal != 0 && Main.player[projectile.owner].lifeSteal > 0)
            {
                Main.player[projectile.owner].lifeSteal -= heal;
                Projectile.NewProjectile(position, Vector2.Zero, ProjectileID.VampireHeal, 0, 0, projectile.owner, projectile.owner, heal);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.lifeMax > 5 && !Main.player[projectile.owner].moonLeech && !target.immortal)
            {
                LifeSteal(target.Center);
            }
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            LifeSteal(target.Center);
        }


        public override bool? CanCutTiles()
        {
            return rightClickTimer > 0; //Only while spinning
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(255, 0, 0, 180) * projectile.Opacity;
        }
    }
}

