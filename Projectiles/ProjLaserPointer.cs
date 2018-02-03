using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace Virtuous.Projectiles
{
    class ProjLaserPointer : ModProjectile
    {
        private static readonly List<int> eyeNPCs = new List<int>(new int[] { NPCID.ServantofCthulhu, NPCID.CataractEye, NPCID.CataractEye2, NPCID.DemonEye, NPCID.DemonEye2, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship, NPCID.DialatedEye, NPCID.DialatedEye2, NPCID.EyeofCthulhu, NPCID.Eyezor, NPCID.GreenEye, NPCID.GreenEye2, NPCID.PurpleEye, NPCID.PurpleEye2, NPCID.SleepyEye, NPCID.SleepyEye2, NPCID.WallofFleshEye, NPCID.WanderingEye });

        public override void SetDefaults()
        {
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.width = 2;
            projectile.height = 2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 endPoint = GetEndPoint();

            Texture2D texture = Main.projectileTexture[projectile.type]; //The dot which composes the line
            Rectangle drawRect = new Rectangle((int)Math.Round(projectile.Center.X - Main.screenPosition.X), (int)Math.Round(projectile.Center.Y - Main.screenPosition.Y), (int)Math.Round((endPoint - projectile.Center).Length()), projectile.width); //Where the line will be from corner to corner
            Color drawLight = Color.White; //Fullbright
            float drawRotation = projectile.velocity.ToRotation(); //Direction of the line
            Vector2 drawOrigin = new Vector2(projectile.width / 2, projectile.height / 2);

            spriteBatch.Draw(texture, drawRect, null, drawLight, drawRotation, Vector2.Zero, SpriteEffects.None, 0);

            OtherFunEffects(endPoint);

            return false; //Don't draw normally
        }

        private Vector2 GetEndPoint()
        {
            Vector2 endPoint = projectile.Center;

            //We scan along a line to see how far the laser can go before hitting something
            while ((endPoint - projectile.Center).Length() < Main.screenWidth && Collision.CanHit(projectile.Center, 1, 1, endPoint, 1, 1))
            {
                endPoint += projectile.velocity.Normalized();

                for (int i = 0; i < Main.maxNPCs; i++) //Returns if it collides with an entity
                {
                    if (Main.npc[i].active && Main.npc[i].Hitbox.Contains((int)endPoint.X, (int)endPoint.Y))
                    {
                        if (eyeNPCs.Contains(Main.npc[i].type)) Main.npc[i].StrikeNPC(10, 0, 0); //Hurts eyes
                        return endPoint;
                    }
                }
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && Main.player[i].whoAmI != projectile.owner && Main.player[i].Hitbox.Contains((int)endPoint.X, (int)endPoint.Y)) return endPoint;
                }
            }

            return endPoint;
        }

        private void OtherFunEffects(Vector2 endPoint)
        {
            if (Main.myPlayer == projectile.owner)
            {
                //Cat follows the laser
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.BlackCat)
                    {
                        Main.projectile[i].velocity += (endPoint - Main.projectile[i].position).OfLength(1);
                    }
                }
            }
        }
    }
}
