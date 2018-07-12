using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace Virtuous.Projectiles
{
    enum LaserColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        White,
        Orange
    }


    class ProjLaserPointer : ModProjectile
    {

        private static readonly int[] EyeNPCs = new[] {
            NPCID.Spazmatism, NPCID.Retinazer, NPCID.ServantofCthulhu,
        }.Select(x => (int)x).ToArray(); // Please tell me why these were shorts in the first place



        public LaserColor LaserColor // Stored as ai[0]
        {
            get { return (LaserColor)(int)projectile.ai[0]; }
            set { projectile.ai[0] = (int)value; }
        }

        private Color RgbColor
        {
            get
            {
                switch (LaserColor)
                {
                    default:                return Color.Red;
                    case LaserColor.Green:  return new Color(0, 255, 0);
                    case LaserColor.Blue:   return Color.Blue;
                    case LaserColor.Yellow: return Color.Yellow;
                    case LaserColor.Purple: return new Color(230, 0, 255);
                    case LaserColor.White:  return Color.White;
                    case LaserColor.Orange: return Color.OrangeRed;
                }
            }
        }



        public override void SetDefaults()
        {
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.width = 2;
            projectile.height = 2;
            projectile.netImportant = true;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 endPoint = GetEndPoint();

            Texture2D texture = Main.projectileTexture[projectile.type]; // The dot which composes the line
            float drawRotation = projectile.velocity.ToRotation(); //Direction of the line
            Vector2 drawOrigin = new Vector2(projectile.width / 2, projectile.height / 2);

            Rectangle drawRect = new Rectangle( // Where the line will be from corner to corner
                (int)Math.Round(projectile.Center.X - Main.screenPosition.X),
                (int)Math.Round(projectile.Center.Y - Main.screenPosition.Y),
                (int)Math.Round((endPoint - projectile.Center).Length()),
                projectile.width);

            spriteBatch.Draw(texture, drawRect, null, RgbColor, drawRotation, Vector2.Zero, SpriteEffects.None, 0);

            OtherFunEffects(endPoint);

            return false;
        }


        private Vector2 GetEndPoint()
        {
            Vector2 endPoint = projectile.Center;

            // We scan along a line to see how far the laser can go before hitting something
            while ((endPoint - projectile.Center).Length() < Main.screenWidth
                   && Collision.CanHit(projectile.Center, 1, 1, endPoint, 1, 1))
            {
                endPoint += projectile.velocity.OfLength(1);

                foreach (var npc in Main.npc) // Returns if it collides with an entity
                {
                    if (npc.active && npc.Hitbox.Contains((int)endPoint.X, (int)endPoint.Y))
                    {
                        if (EyeNPCs.Contains(npc.type) || npc.InternalNameHas("eye"))
                        {
                            npc.StrikeNPC(10, 0, 0); // Hurts eyes
                        }

                        return endPoint;
                    }
                }

                foreach (var player in Main.player)
                {
                    if (player.active && player.whoAmI != projectile.owner
                        && player.Hitbox.Contains((int)endPoint.X, (int)endPoint.Y))
                    {
                        return endPoint;
                    }
                }
            }

            return endPoint;
        }


        private void OtherFunEffects(Vector2 endPoint)
        {
            if (Main.myPlayer == projectile.owner)
            {
                var cat = Main.projectile.FirstOrDefault(x => x.active && x.type == ProjectileID.BlackCat);
                if (cat != null) // Cat follows the laser
                {
                    cat.velocity += (endPoint - Main.projectile[i].position).OfLength(1);
                }
            }
        }
    }
}
