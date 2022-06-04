using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
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
            get { return (LaserColor)(int)Projectile.ai[0]; }
            set { Projectile.ai[0] = (int)value; }
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
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.netImportant = true;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 endPoint = GetEndPoint();

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value; // The dot which composes the line
            float drawRotation = Projectile.velocity.ToRotation(); //Direction of the line
            Vector2 drawOrigin = new Vector2(Projectile.width / 2, Projectile.height / 2);

            Rectangle drawRect = new Rectangle( // Where the line will be from corner to corner
                (int)Math.Round(Projectile.Center.X - Main.screenPosition.X),
                (int)Math.Round(Projectile.Center.Y - Main.screenPosition.Y),
                (int)Math.Round((endPoint - Projectile.Center).Length()),
                Projectile.width);

            spriteBatch.Draw(texture, drawRect, null, RgbColor, drawRotation, Vector2.Zero, SpriteEffects.None, 0);

            OtherFunEffects(endPoint);

            return false;
        }


        private Vector2 GetEndPoint()
        {
            Vector2 endPoint = Projectile.Center;

            // We scan along a line to see how far the laser can go before hitting something
            while ((endPoint - Projectile.Center).Length() < Main.screenWidth
                   && Collision.CanHit(Projectile.Center, 1, 1, endPoint, 1, 1))
            {
                endPoint += Projectile.velocity.OfLength(1);

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
                    if (player.active && player.whoAmI != Projectile.owner
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
            if (Main.myPlayer == Projectile.owner)
            {
                var cat = Main.projectile.FirstOrDefault(x => x.active && x.type == ProjectileID.BlackCat);
                if (cat != null) // Cat follows the laser
                {
                    cat.velocity += (endPoint - cat.position).OfLength(1);
                }
            }
        }
    }
}
