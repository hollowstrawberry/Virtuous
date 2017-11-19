using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class SpikedBubble_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Bubble");
        }

        private bool firstTick { get { return projectile.ai[0] == 0; } set { projectile.ai[0] = value ? 0 : 1; } } //Stores into the projectile's built-in ai[0], which is 0 by default (true in this case)

        public  const float DamageBoost = 0.1f; //Damage boost while the orbital is active. Used by VirtuousPlayer and VirtuousItem
        public  const int DyingTime = 30; //Time it spends expanding before dying
        private const int PopTime = 10; //Part of dyingTime where it keeps its size before popping
        private const int OriginalSize = 120; //Width and height dimensions in pixels
        private const int OriginalAlpha = 50; //Alpha value when spawned
        private const int ExpandedAlpha = 150; //Alpha value when fully expanded
        private const float AngularSpeed = 1/60f * Tools.RevolutionPerSecond;
        private const float ExpandedScale = 1.5f; //Size when fully expanded


        public override void SetDefaults()
        {
            projectile.width = OriginalSize;
            projectile.height = OriginalSize;
            projectile.alpha = OriginalAlpha;
            projectile.timeLeft = 2;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (!orbitalPlayer.active[OrbitalID.SpikedBubble]) //Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                projectile.Center = player.MountedCenter; //Keeps it on the player
                projectile.rotation += AngularSpeed; //Makes it rotate slowly
                projectile.timeLeft = 2; //Keep it from dying naturally

                if (firstTick) //Spawns some dust
                {
                    firstTick = false;
                    for (int i = 0; i < 50; i++)
                    {
                        Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/16, 0f, 0f, /*Alpha*/50, new Color(255, 200, 245), 1.2f);
                        newDust.velocity *= 1.5f;
                        newDust.noLight = false;
                    }
                }

                if (orbitalPlayer.time <= DyingTime)
                {
                    if (orbitalPlayer.time == DyingTime) //First tick
                    {
                        projectile.damage *= 2; //Increased damage as it expands
                    }
                    if (orbitalPlayer.time > PopTime) //Expands until popTime then stops
                    {
                        //Change the apparent size
                        projectile.scale += (ExpandedScale - 1) / (DyingTime - PopTime);
                        //Change the hitbox size
                        projectile.height = (int)(OriginalSize * projectile.scale);
                        projectile.width  = (int)(OriginalSize * projectile.scale);
                        //Align the sprite with its new size
                        drawOriginOffsetY = (projectile.height - OriginalSize) / 2;
                        drawOffsetX = (projectile.width - OriginalSize) / 2;
                        //Fade it away
                        projectile.alpha += (int)Math.Ceiling((float)(ExpandedAlpha - OriginalAlpha) / DyingTime);
                    }
                    else if(orbitalPlayer.time == 1) //Last tick
                    {
                        Main.PlaySound(SoundID.Item54, projectile.Center); //Bubble pop
                        Lighting.AddLight(player.Center, 1.5f, 1.0f, 1.4f);
                        const int DustAmount = 50;
                        for (int i = 0; i < DustAmount; i++)
                        {
                            float rotation = Main.rand.NextFloat() * Tools.FullCircle; //Random rotation around the circle
                            int distance = Main.rand.Next(projectile.width / 2); //Random distance from the circle center
                            Vector2 position = projectile.Center + new Vector2(0, distance).RotatedBy(rotation);
                            Dust newDust = Dust.NewDustDirect(position, 0, 0, /*Type*/16, 0, 0, /*Alpha*/100, new Color(255, 200, 245, 150), /*Scale*/1.2f);
                            newDust.velocity *= 2;
                        }
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
            return new Color(255, 200, 245, 150) * projectile.Opacity;
        }

    }
}
