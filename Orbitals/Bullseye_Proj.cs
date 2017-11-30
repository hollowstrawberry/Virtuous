using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Dusts;

namespace Virtuous.Orbitals
{
    public class Bullseye_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Bullseye;
        public override int FadeTime => 120;
        public override float BaseDistance => 40;

        public static float GapTreshold = 0.990f; //How forgiving it is with the damage boost zone. Used by OrbitalPlayer


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Sight");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 24;
            projectile.height = 58;
            projectile.friendly = false; //Doesn't affect enemies
        }

        public override void Movement()
        {
            //Stays in front of the player
            projectile.spriteDirection = player.direction;
            relativePosition = new Vector2(player.direction * distance, 0);
            projectile.Center = player.Center + relativePosition;

            Lighting.AddLight(new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y), 0.5f, 0.3f, 0.05f);

            //Special effect dust
            if (orbitalPlayer.BullseyeShot())
            {
                Vector2 position = new Vector2(projectile.Center.X + 2 * player.direction, projectile.Center.Y - 1);
                Dust newDust = Dust.NewDustDirect(position, 0, 0, mod.DustType<RainbowDust>(), 0f, 0f, /*Alpha*/50, new Color(255, 127, 0, 50), /*Scale*/1.5f);
                newDust.velocity = new Vector2(player.direction * 2.5f, 0);
            }
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(255, 130, 20, 200) * projectile.Opacity;
        }
    }
}