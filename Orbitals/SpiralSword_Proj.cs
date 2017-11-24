using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class SpiralSword_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Virtue");
        }

        //Passive
        public  const float DamageBoost = 0.2f; //Damage boost while the orbital is active. Used by VirtuousPlayer and VirtuousItem
        private const float DistanceAvg = 90f; //Average distance to the player when passive
        private const float OscSpeedMax = 0.4f; //How fast it can move in and out, which is to say, how far it can go before changing direction
        private const float OscAcc = OscSpeedMax / 40; //How quickly it changes oscillation speed, which is to say, how quickly it reaches the point of direction change

        //Right-click
        private const float ThrowDistance = 300f; //Maximum distance when thrown
        private const float ThrowSpeed = ThrowDistance / 20; //How quickly it moves when thrown
        private int rightClickTimer = 0; //How long it's been since the right click effect activated. Controls the angular speed increase when thrown

        //Shooting out
        public  const int DyingTime = 30; //Time it spends flying outward before dying
        private const float ShootAcc = 3; //Acceleration when it's flying outward
        private float shootSpeed = 15; //Speed when it's flying outward

        //Main
        private const float AngularSpeed = 1/2f * RevolutionPerSecond; //How fast it orbits around the player
        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)
        private bool outwards { get { return projectile.ai[1] == 0; } set { projectile.ai[1] = value ? 0 : 1; } } //Direction it's currently moving in (away or into the player). Stores into the projectile's built-in ai[1], which is 0 by default (true in this case)
        private Vector2 relativePos; //Relative position to the player
        private float distance = DistanceAvg; //Distance away from the player
        private float distanceSpeed = -OscSpeedMax; //How quickly it's changing distance
        private int originalDamage; //Stores the original damage so it can be changed later
        

        public override void SetDefaults()
        {
            projectile.width = 76;
            projectile.height = 76;
            projectile.alpha = 50;
            projectile.timeLeft = DyingTime;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10; //Replaced when right-clicking
        }

        public override void AI()
        {
			projectile.netUpdate = true; //Temporary cover for multiplayer acting strange

            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.SpiralSword] && projectile.owner == Main.myPlayer) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                if (firstTick)
                {
                    //The projectile's desired rotation was passed as its velocity, so we utilize it then set it to 0 so it doesn't move
                    projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians(); //45 degrees because of the sprite
                    relativePos = projectile.velocity.OfLength(distance);
                    projectile.velocity = Vector2.Zero;
                    originalDamage = projectile.damage;
                    firstTick = false;
                }

                //Before it shoots out and dies
                if (orbitalPlayer.time > DyingTime || projectile.owner != Main.myPlayer)
                {
                    projectile.timeLeft = DyingTime; //Keep it from dying naturally

                    //Alters the distance from the player
                    if (orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOn]) //Active attack: They are quickly thrown outwards
                    {
                        projectile.netUpdate = true; //Syncs to multiplayer
                        if (rightClickTimer == 0) outwards = true; //Sets the starting direction

                        projectile.damage = originalDamage * 2; //Deals more damage when thrown
                        projectile.idStaticNPCHitCooldown = 5; //Deals damage more rapidly
                        distanceSpeed = ThrowSpeed; //How quickly the distance changes
                        if (!outwards) distanceSpeed *= -1; //Moves in reverse direction if returning
                        rightClickTimer++;

                        if (distance >= ThrowDistance) //If it has reached the set maximum distance for the throw
                        {
                            outwards = false; //Return
                        }
                        else if (!outwards && distance <= DistanceAvg) //If it has returned to the passive zone
                        {
                            orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOff] = true; //Initiate shutdown of the right click effect
                            rightClickTimer = 0;

                            //Resets to passive oscillation behavior
                            distance = DistanceAvg;
                            distanceSpeed = -OscSpeedMax;
                            projectile.damage = originalDamage;
                            projectile.idStaticNPCHitCooldown = 10;
                        }

                    }
                    else //Passive oscillation
                    {
                        if      (distanceSpeed >= +OscSpeedMax) outwards = false; //If it has reached the outwards speed limit, begin to switch direction
                        else if (distanceSpeed <= -OscSpeedMax) outwards = true; //If it has reached the inwards speed limit, begin to switch direction
                        distanceSpeed += outwards ? +OscAcc : -OscAcc; //Accelerate innards or outwards
                    }
                    distance += distanceSpeed; //Finally applies the change in distance

                    //Makes them orbit the player
                    float newAngSpeed = AngularSpeed; //Actual angular speed used
                    if (rightClickTimer != 0 && rightClickTimer <= 30) newAngSpeed *= 2; //Double speed the first 30 ticks of shooting out
                    relativePos = relativePos.RotatedBy(newAngSpeed).OfLength(distance); //Rotates the sword around the player and resets its distance
                    projectile.rotation += newAngSpeed; //Rotates the sprite accordingly
                    projectile.Center = player.MountedCenter + relativePos; //Moves the sword to the defined position around the player

                }
                else //timeLeft actually starts going down from dyingTime while it flies away
                {
                    projectile.damage = originalDamage * 5; //Deals more damage when shooting out
                    projectile.Center += relativePos.OfLength(shootSpeed); //Projectile moves forward in the direction of its rotation
                    shootSpeed += ShootAcc; //Accelerate
                }
            }
        }

        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return rightClickTimer > 0; //Only while actively attacking
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(150, 255, 230, 150) * projectile.Opacity;
        }
    }
}
