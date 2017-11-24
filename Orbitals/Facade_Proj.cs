using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class Facade_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Barrier");
        }

        //Passive
        private const float Distance = 50; //Constant distance away from the player
        private const float AngularSpeedMax = 1/15f * RevolutionPerSecond;
        private const float AngularAcc = AngularSpeedMax / 60; //How quickly it changes speed, which is to say, how quickly it reaches the point of direction change
        private bool  clockwise { get { return projectile.ai[1] == 0; } set { projectile.ai[1] = value ? 0 : 1; } } //Direction the barrier is currently orbiting in. Stores into the projectile's built-in ai[1], which is 0 by default (true in this case)
        private float angularSpeed = AngularSpeedMax; //Current speed at which it orbits

        //Fading
        private const int FadeTime = 30; //Time it spends fading out
        private const int OriginalAlpha = 80; //Alpha value before fading out

        //Main
        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)
        private Vector2 relativePos; //Relative position to the player


        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 44;
            projectile.alpha = OriginalAlpha;
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
            //if (projectile.owner != Main.myPlayer) return; //Only runs AI for the client
			projectile.netUpdate = true;
			
            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.Facade] && projectile.owner == Main.myPlayer) //Keep it alive only while the summon is active
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
                    relativePos = projectile.velocity.OfLength(Distance);
                    projectile.velocity = Vector2.Zero;;
                }

                projectile.timeLeft = 2; //Keep it from dying naturally

                //Makes it oscillate back and forth around the player
                if      (angularSpeed >= +AngularSpeedMax) clockwise = false; //If it has reached the clockwise speed limit, begin to switch direction
                else if (angularSpeed <= -AngularSpeedMax) clockwise = true; //If it has reached the counterclockwise speed limit, begin to switch direction
                angularSpeed += clockwise ? +AngularAcc : -AngularAcc; //Accelerate in clockwise or counterclockwise direction
                relativePos = relativePos.RotatedBy(angularSpeed); //Moves the projectile in respect to the player accordingly
                projectile.rotation += angularSpeed; //Rotates the sprite accordingly
                projectile.Center = player.MountedCenter + relativePos; //Keeps the projectile around the player

                if(orbitalPlayer.time <= FadeTime && projectile.owner == Main.myPlayer)
                {
                    projectile.alpha += (int)Math.Ceiling((255f - OriginalAlpha) / FadeTime); //Fades away over fadeTime
                }
                else
                {
                    projectile.alpha = OriginalAlpha; //Restores the opacity if the item is reused during fading
                }

            }
        }

        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return false;
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(250, 233, 0, 100) * projectile.Opacity;
        }

    }
}
