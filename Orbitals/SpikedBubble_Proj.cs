using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class SpikedBubble_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.SpikedBubble;
        public override int DyingTime => 30;
        public override int OriginalAlpha => 50;
        public override float BaseDistance => 0;
        public override float RotationSpeed => 0 * RevolutionPerSecond; //I set this to 0 for now so that the sprite doesn't look weird

        private static float ExpandedScale = 1.5f; //Size when fully expanded
        private static int ExpandedAlpha = 150; //Alpha value when fully expanded
        private static int OriginalSize = 120; //Width and height dimensions in pixels
        private static int PopTime = 10; //Part of DyingTime where it keeps its size before popping


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Bubble");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width  = OriginalSize;
            projectile.height = OriginalSize;
        }

        public override void PlayerEffects(Player player)
        {
            player.statDefense += 5;
            player.meleeDamage += 0.1f;
            player.magicDamage += 0.1f;

            Lighting.AddLight(player.Center, 0.6f, 0.4f, 0.3f);
        }

        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 50; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/16, 0f, 0f, /*Alpha*/50, new Color(255, 200, 245), 1.2f);
                newDust.velocity *= 1.5f;
                newDust.noLight = false;
            }
        }

        public override void DyingFirstTick()
        {
            projectile.damage *= 2;
        }

        public override void Dying()
        {
            Movement(); //Doesn't stop following the player

            if (projectile.timeLeft > PopTime) //Expands until PopTime then stops
            {
                //Change the apparent size
                projectile.scale += (ExpandedScale - 1) / (DyingTime - PopTime);
                //Change the hitbox size
                projectile.height = (int)(OriginalSize * projectile.scale);
                projectile.width = (int)(OriginalSize * projectile.scale);
                //Align the sprite with its new size
                drawOriginOffsetY = (projectile.height - OriginalSize) / 2;
                drawOffsetX = (projectile.width - OriginalSize) / 2;

                //Fade it
                projectile.alpha += (int)Math.Ceiling((float)(ExpandedAlpha - OriginalAlpha) / DyingTime);
            }
            else if (projectile.timeLeft == 1) //Last tick
            {
                Main.PlaySound(SoundID.Item54, projectile.Center); //Bubble pop
                Lighting.AddLight(player.Center, 1.5f, 1.0f, 1.4f);
                const int DustAmount = 50;
                for (int i = 0; i < DustAmount; i++)
                {
                    Vector2 offset = Vector2.UnitY.RotatedBy(RandomFloat(FullCircle)).OfLength(RandomInt(projectile.width / 2)); //Random rotation, random distance from the center
                    Vector2 position = projectile.Center + offset;
                    Dust newDust = Dust.NewDustDirect(position, 0, 0, /*Type*/16, 0, 0, /*Alpha*/100, new Color(255, 200, 245, 150), /*Scale*/1.2f);
                    newDust.velocity *= 2;
                }
            }
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 200, 245, 150) * projectile.Opacity;
        }

    }
}
