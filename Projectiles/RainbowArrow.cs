using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Dusts;
using static Virtuous.Tools;

namespace Virtuous.Projectiles
{
    public class RainbowArrow : ModProjectile
    {
        private const int Lifespan = 6 * 60; //Total duration of the projetile
        private const int RainDelay = 50; //Time it takes for a special arrow effect to activate

        private int arrowMode //ai[0] defines the arrow mode
        {
            get
            {
                return (int)projectile.ai[0];
            }
        }
        public const int Normal = 0; //Behave like regular arrows. The bow will shoot them faster
        public const int White = 1; //Are shot upward, don't affect enemies, and cause rain shots to spawn after a short time
        public const int Rain = 2; //Spawned by white shots, fall from above and have no knockback

        private Color projectileColor //ai[1] defines the arrow color
        {
            get
            {
                if(arrowMode == White) return new Color(255, 255, 255, 0);

                int alpha = 120;
                switch ((int)projectile.ai[1])
                {
                    case  0: return new Color(000, 050, 255, alpha); //Blue
                    case  1: return new Color(000, 125, 255, alpha); //Sky
                    case  2: return new Color(000, 255, 255, alpha); //Cyan
                    case  3: return new Color(000, 255, 110, alpha); //Aquamarine
                    case  4: return new Color(000, 255, 000, alpha); //Green
                    case  5: return new Color(110, 255, 000, alpha); //Lime
                    case  6: return new Color(240, 240, 000, alpha); //Yellow
                    case  7: return new Color(255, 100, 000, alpha); //Orange
                    case  8: return new Color(255, 000, 000, alpha); //Red
                    case  9: return new Color(255, 000, 150, alpha); //Fuchsia
                    case 10: return new Color(150, 000, 255, alpha); //Purple
                    case 11: return new Color(090, 020, 255, alpha); //Violet
                    default: return Color.Black;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.friendly = true;
            projectile.arrow = true;
            projectile.alpha = 0;
            projectile.timeLeft = Lifespan;
            projectile.ranged = true;
        }

        private void SpawnRainbowDust(bool Burst=false)
        {
			int type=0;
			float scale=0;
			Vector2 velocity = Vector2.Zero; //Changed if we want the dust to have a special velocity

			switch(arrowMode)
			{
				case Normal:
					type = 16; //Cloud
					if(Burst) scale = 1.2f;
                    else scale = 0.8f;
					break;

				case White:
					type = 16; //Cloud
                    scale = 1.3f;
					break;

				case Rain:
                    type = mod.DustType<RainbowDust>();
                    scale = Burst ? 1.5f : 1.2f; //Bigger size if it's a burst of dust
					if (!Burst) velocity = new Vector2(RandomFloat(-0.25f, +0.25f), projectile.velocity.Length()/2); //X is random, Y follows the arrow
                    break;
			}

            int dustAmount = Burst ? 10 : 1;
            for (int i = 0; i < dustAmount; i++)
            {
                Dust newDust = Dust.NewDustDirect(projectile.Center, 0, 0, type, 0f, 0f, /*Alpha*/0, projectileColor, scale);
			    if (velocity != Vector2.Zero) newDust.velocity = velocity; //If a special velocity was defined, apply it
                if (arrowMode == Rain) newDust.position.Y -= projectile.height/2; //Trails behind
            }
        }


        public override void AI()
        {
            //On projectile spawn
            if(projectile.timeLeft == Lifespan) 
            {
                switch(arrowMode)
                {
                    case White:
                        projectile.scale *= 1.4f;
                        projectile.friendly = false; //Doesn't affect enemies
                        break;

                    case Rain:
                        SpawnRainbowDust(true);
                        break;
                }

                projectile.rotation = projectile.velocity.ToRotation() + 90.ToRadians(); //Rotates the sprite according to its velocity. Adds 90 degrees because of the sprite
            }

            //On every tick
            SpawnRainbowDust();
            float l = 0.7f; //Light intensity
            Lighting.AddLight(projectile.Center, l*projectileColor.R/255, l*projectileColor.G/255, l*projectileColor.B/255);

            //Special arrow effect
            if(arrowMode == White && projectile.timeLeft == Lifespan - RainDelay && projectile.owner == Main.myPlayer) 
            {
                const int ArrowAmount = 30;
                float arrowSpacing = projectile.width * 6f;
                float nextColor = RandomInt(12); //Starts the rainbow of arrows at a random color
                for(int i = 1; i <= ArrowAmount; i++)
                {
                    Vector2 position = projectile.Center;
                    position.X += -ArrowAmount*arrowSpacing/2 - arrowSpacing/2 + i*arrowSpacing; //Makes a row of arrows spaced evenly according to the arrow amount and the defined space between them
                    position.Y += 120; //Distance below the original arrow
                    if (!Main.tile[(int)position.X/16,(int)position.Y/16].active() || !Main.tile[(int)position.X/16,(int)position.Y/16].nactive()) //Only spawns if it's open space
                    {
                        Projectile.NewProjectile(position, Vector2.UnitY * projectile.velocity.Length(), mod.ProjectileType<RainbowArrow>(), projectile.damage, 0, projectile.owner, Rain, nextColor);
                    }
                    //Cycles through colors as the rainbow progresses
                    if (nextColor < 11) nextColor++;
                    else nextColor = 0;
                }
                projectile.Kill();
            }

        }

        public override void Kill(int timeLeft)
        {
            SpawnRainbowDust(true);
        }

        public override Color? GetAlpha(Color newColor) //Sets the custom color
        {
            return projectileColor;
        }

    }
}