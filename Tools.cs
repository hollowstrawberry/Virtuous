using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.Utilities;

namespace Virtuous
{
    /// <summary>Utilities that help make my code cleaner</summary>
    public static class Tools
    {
        // Constants

        public const float GoldenRatio = 1.61803398875f;
        public const float FullCircle = 2 * (float)Math.PI; // Really just an alias that is also a float
        public const float RevolutionPerSecond = FullCircle / 60; // 60 game ticks = 1 second


        // Game object utilities

        public static Vector2 SpriteCenter(this NPC npc) => npc.Center + new Vector2(0, npc.gfxOffY);
        public static Vector2 SpriteCenter(this Player player) => player.Center + new Vector2(0, player.gfxOffY);
        public static Vector2 MountedSpriteCenter(this Player player) => player.MountedCenter + new Vector2(0, player.gfxOffY);


        public static string InternalName(this NPC npc) => Lang.GetNPCName(npc.type).Key.Split('.').Last();
        public static string InternalName(this Item item) => Lang.GetItemName(item.type).Key.Split('.').Last();
        public static string InternalName(this Projectile proj) => Lang.GetProjectileName(proj.type).Key.Split('.').Last();


        public static bool InternalNameHas(this NPC npc, params string[] values)
            => npc.InternalName().ToUpper().ContainsAny(values.Select(x => x.ToUpper()));

        public static bool InternalNameHas(this Item item, params string[] values)
            => item.InternalName().ToUpper().ContainsAny(values.Select(x => x.ToUpper()));

        public static bool InternalNameHas(this Projectile proj, params string[] values)
            => proj.InternalName().ToUpper().ContainsAny(values.Select(x => x.ToUpper()));


        /// <summary>Changes the size of the hitbox while keeping its center</summary>
        public static void ResizeProjectile(int projIndex, int newWidth, int newHeight, bool changeDrawPos = false)
        {
            Projectile projectile = Main.projectile[projIndex];

            if (changeDrawPos)
            {
                projectile.modProjectile.drawOffsetX += (newWidth - projectile.width) / 2;
                projectile.modProjectile.drawOriginOffsetY += (newHeight - projectile.height) / 2;
            }

            projectile.position += new Vector2(projectile.width / 2, projectile.height / 2);
            projectile.width = newWidth;
            projectile.height = newHeight;
            projectile.position -= new Vector2(projectile.width / 2, projectile.height / 2);
        }


        /// <summary>A trick to stop the bugged 1-tick delay between consecutive right-click uses of a weapon</summary>
        public static void HandleAltUseAnimation(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (PlayerInput.Triggers.JustReleased.MouseRight) // Stops the animation manually
                {
                    player.itemAnimation = 0;
                }
                else if (player.itemAnimation == 1) // Doesn't let the hand return to resting position
                {
                    player.altFunctionUse = 1;
                    player.controlUseItem = true;
                }
            }
        }




        // Vector extensions

        /// <summary>Shorthand for SafeNormalize</summary>
        public static Vector2 Normalized(this Vector2 vector)
        {
            return vector.SafeNormalize(Vector2.UnitX);
        }

        /// <summary>Returns a vector with the magnitude changed</summary>
        public static Vector2 OfLength(this Vector2 vector, float length) 
        {
            return vector.Normalized() * length;
        }

        ///<summary>Returns a vector perpendicular to the original</summary>
        public static Vector2 Perpendicular(this Vector2 oldVector, float? length = null, bool clockwise = true)
        {
            Vector2 vector = new Vector2(oldVector.Y, -oldVector.X);
            if (!clockwise) vector *= new Vector2(-1, -1);
            if (length != null) vector = vector.OfLength((float)length);
            return vector;
        }



        // String extensions

        ///<summary>Helps with long string concatenation</summary>
        public static string If(this string text, bool condition)
        {
            return condition ? text : "";
        }

        ///<summary>Whether the string contains any of the values in the given collection</summary>
        public static bool ContainsAny(this string text, IEnumerable<string> values)
        {
            return values.All(text.Contains);
        }



        // Random extensions

        /// <summary>Produces a vector with random direction and magnitude 1</summary>
        public static Vector2 NextVector2(this UnifiedRandom rand)
        {
            return Vector2.UnitY.RotatedBy(rand.NextFloat(FullCircle));
        }

        /// <summary>Produces a vector with random direction and random length within a range</summary>
        public static Vector2 NextVector2(this UnifiedRandom rand, float minLength, float maxLength)
        {
            return rand.NextVector2().OfLength(rand.NextFloat(minLength, maxLength));
        }

        /// <summary>Returns true with a 1/amount chance </summary>
        public static bool OneIn(this UnifiedRandom rand, int amount)
        {
            return rand.Next(amount) == 0;
        }

        /// <summary>Returns a random element in a list</summary>
        public static T Choose<T>(this UnifiedRandom rand, IList<T> items)
        {
            if (items.Count == 0) return default(T);
            if (items.Count == 1) return items[0];

            return items[rand.Next(items.Count)];
        }



        // Number extensions

        /// <summary>Returns the given value in degrees as radians</summary>
        public static float ToRadians(this int deg)
        {
            return deg * (float)Math.PI / 180f;
        }

        /// <summary>Returns the given value in degrees as radians</summary>
        public static float ToRadians(this float deg)
        {
            return deg * (float)Math.PI / 180f;
        }

        /// <summary>Returns the given value in radians as degrees</summary>
        public static float ToDegrees(this float rad)
        {
            return rad * 180f / (float)Math.PI;
        }
    }
}
