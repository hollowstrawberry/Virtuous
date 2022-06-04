using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.Utilities;

namespace Virtuous
{
    /// <summary>
    /// General utilities for math, game entites, randomness and vectors.
    /// </summary>
    public static class Tools
    {
        // Constants

        /// <summary></summary>
        public const float GoldenRatio = 1.61803398875f;

        /// <summary>A float alias for 2π.</summary>
        public const float FullCircle = 2 * (float)Math.PI;

        /// <summary>A rotational speed of 1/60th of a circle per tick, or one revolution per second.</summary>
        public const float RevolutionPerSecond = FullCircle / 60;




        // Game object utilities

        /// <summary>Gives the visual center position of an NPC.</summary>
        public static Vector2 SpriteCenter(this NPC npc) => npc.Center + new Vector2(0, npc.gfxOffY);

        /// <summary>Gives the visual center position of a player.</summary>
        public static Vector2 SpriteCenter(this Player player) => player.Center + new Vector2(0, player.gfxOffY);

        /// <summary>Gives the visual center position of a player or mounted player.</summary>
        public static Vector2 MountedSpriteCenter(this Player player) => player.MountedCenter + new Vector2(0, player.gfxOffY);


        /// <summary>The internal name key of an NPC.</summary>
        public static string InternalName(this NPC npc) => Lang.GetNPCName(npc.type).Key.Split('.').Last();

        /// <summary>The internal name key of an item.</summary>
        public static string InternalName(this Item item) => Lang.GetItemName(item.type).Key.Split('.').Last();

        /// <summary>The internal name key of a projectile.</summary>
        public static string InternalName(this Projectile proj) => Lang.GetProjectileName(proj.type).Key.Split('.').Last();


        /// <summary>Whether an NPC's internal name key contains any of the provided values.</summary>
        public static bool InternalNameHas(this NPC npc, params string[] values)
            => npc.InternalName().ToUpper().ContainsAny(values.Select(x => x.ToUpper()));

        /// <summary>Whether an item's internal name key contains any of the provided values.</summary>
        public static bool InternalNameHas(this Item item, params string[] values)
            => item.InternalName().ToUpper().ContainsAny(values.Select(x => x.ToUpper()));

        /// <summary>Whether a projectile's internal name key contains any of the provided values.</summary>
        public static bool InternalNameHas(this Projectile proj, params string[] values)
            => proj.InternalName().ToUpper().ContainsAny(values.Select(x => x.ToUpper()));


        /// <summary>Changes the size of a projectile's hitbox while maintaining its center.</summary>
        public static void ResizeProjectile(int projIndex, int newWidth, int newHeight, bool changeDrawPos = false)
        {
            Projectile projectile = Main.projectile[projIndex];

            if (changeDrawPos)
            {
                projectile.ModProjectile.DrawOffsetX += (newWidth - projectile.width) / 2;
                projectile.ModProjectile.DrawOriginOffsetY += (newHeight - projectile.height) / 2;
            }

            projectile.position += new Vector2(projectile.width / 2, projectile.height / 2);
            projectile.width = newWidth;
            projectile.height = newHeight;
            projectile.position -= new Vector2(projectile.width / 2, projectile.height / 2);
        }


        /// <summary>A trick to stop the bugged 1-tick delay between consecutive right-click uses of a weapon.</summary>
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




        // String extensions

        /// <summary>Helps with long text concatenation.</summary>
        public static string If(this string text, bool condition) => condition ? text : "";

        /// <summary>Whether the string contains any of the values in the given collection.</summary>
        public static bool ContainsAny(this string text, IEnumerable<string> values) => values.All(text.Contains);



        // Number extensions

        /// <summary>Returns the given value in degrees as radians.</summary>
        public static float ToRadians(this int deg) => deg * (float)Math.PI / 180f;

        /// <summary>Returns the given value in degrees as radians.</summary>
        public static float ToRadians(this float deg) => deg * (float)Math.PI / 180f;

        /// <summary>Returns the given value in radians as degrees.</summary>
        public static float ToDegrees(this float rad) => rad * 180f / (float)Math.PI;



        // Vector extensions

        /// <summary>Shorthand for <see cref="Terraria.Utils.SafeNormalize(Vector2, Vector2)"/>.</summary>
        public static Vector2 Normalized(this Vector2 vector)
        {
            return vector.SafeNormalize(Vector2.UnitX);
        }

        /// <summary>Returns a vector with the same direction as the original but with the specified length.</summary>
        public static Vector2 OfLength(this Vector2 vector, float length) 
        {
            return vector.Normalized() * length;
        }

        /// <summary>Returns a vector perpendicular to the original.</summary>
        public static Vector2 Perpendicular(this Vector2 oldVector, float? length = null, bool clockwise = true)
        {
            Vector2 vector = new Vector2(oldVector.Y, -oldVector.X);
            if (!clockwise) vector *= new Vector2(-1, -1);
            if (length.HasValue) vector = vector.OfLength(length.Value);
            return vector;
        }



        // Random extensions

        /// <summary>Produces a vector with length 1 and a random direction.</summary>
        public static Vector2 NextVector2(this UnifiedRandom rand)
        {
            return Vector2.UnitY.RotatedBy(rand.NextFloat(FullCircle));
        }

        /// <summary>Produces a vector with a random direction and a random length constrained to a range.</summary>
        public static Vector2 NextVector2(this UnifiedRandom rand, float minLength, float maxLength)
        {
            return rand.NextVector2().OfLength(rand.NextFloat(minLength, maxLength));
        }

        /// <summary>Returns a random element from a list.</summary>
        public static T Choose<T>(this UnifiedRandom rand, IList<T> items)
        {
            if (items.Count == 0) return default(T);
            if (items.Count == 1) return items[0];

            return items[rand.Next(items.Count)];
        }
    }
}
