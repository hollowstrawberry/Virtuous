using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Graphics;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Projectiles
{
    class ProjLaserPointer : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
            projectile.width = 2;
            projectile.height = 2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Texture2D texture = Main.projectileTexture[projectile.type];
            //Vector2 drawPosition = projectile.Center;
            //Color drawColor = Color.White;
            //float drawRotation = projectile.velocity.ToRotation();
            //Vector2 drawOrigin = new Vector2(projectile.width / 2, projectile.height / 2);

            ////Draws a straight line composed of strips
            //while ((drawPosition - projectile.Center).Length() < Main.screenWidth && Collision.CanHit(projectile.Center, 0, 0, drawPosition, 0, 0))
            //{
            //    spriteBatch.Draw(texture, drawPosition - Main.screenPosition, null, drawColor * projectile.Opacity, drawRotation, drawOrigin, projectile.scale, SpriteEffects.None, 0);
            //    drawPosition += projectile.velocity.OfLength(projectile.width);
            //}

            Vector2 endPoint = projectile.Center;

            while ((endPoint - projectile.Center).Length() < Main.screenWidth && Collision.CanHit(projectile.Center, 1, 1, endPoint, 1, 1))
            {
                endPoint += projectile.velocity.Normalized(); //We scan along a line to see how far the laser can go before hitting something
            }

            Texture2D texture = Main.projectileTexture[projectile.type]; //The dot which composes the line
            Rectangle drawRect = new Rectangle((int)Math.Round(projectile.Center.X - Main.screenPosition.X), (int)Math.Round(projectile.Center.Y - Main.screenPosition.Y), (int)Math.Round((endPoint - projectile.Center).Length()), projectile.width); //Where the line will be from corner to corner
            Color drawLight = Color.White; //Fullbright
            float drawRotation = projectile.velocity.ToRotation(); //Direction of the line
            Vector2 drawOrigin = new Vector2(projectile.width / 2, projectile.height / 2);

            spriteBatch.Draw(texture, drawRect, null, drawLight, drawRotation, Vector2.Zero, SpriteEffects.None, 0);



            //I'll make it damage the eye of cthulhu in a very stupid way
            if (Main.myPlayer == projectile.owner)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.EyeofCthulhu)
                    {
                        if (Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1) && Math.Abs((Main.npc[i].Center - projectile.Center).ToRotation() - (endPoint - projectile.Center).ToRotation()) < 0.1f)
                        {
                            Main.npc[i].StrikeNPC(10, 0, 0);
                        }
                    }
                }
            }

            return false; //Don't draw normally
        }
    }
}
