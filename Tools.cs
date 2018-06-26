using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.Utilities;

namespace Virtuous
{
    /*
     * This class contains many tools, mostly shorthands for Terraria utils and some constant keywords.
     * They help me feel like my code is clean, understandable and more personal.
     */

    public static class Tools
    {
        // Constants

        public const float FullCircle = 2 * (float)Math.PI;
        public const float RevolutionPerSecond = FullCircle / 60;
        public const float GoldenRatio = 1.61803398875f;


        // Game objects

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


        // Changes the size of the hitbox while keeping its center
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


        // A trick to stop the bugged 1-tick delay between consecutive right-click uses of a weapon
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

        public static Vector2 Normalized(this Vector2 vector) // Shorthand for SafeNormalize
        {
            return vector.SafeNormalize(Vector2.UnitX);
        }

        public static Vector2 OfLength(this Vector2 vector, float length) // Returns a vector with the magnitude changed
        {
            return vector.Normalized() * length;
        }

        public static Vector2 Perpendicular(this Vector2 oldVector, float? length = null, bool clockwise = true) // Returns a vector perpendicular to the original
        {
            Vector2 vector = new Vector2(oldVector.Y, -oldVector.X);
            if (!clockwise) vector *= new Vector2(-1, -1);
            if (length != null) vector = vector.OfLength((float)length);
            return vector;
        }



        // String extensions

        public static string If(this string text, bool condition) // Helps with long string concatenation
        {
            return condition ? text : "";
        }

        public static bool ContainsAny(this string text, IEnumerable<string> values)
        {
            return values.All(text.Contains);
        }



        // Random extensions

        public static Vector2 NextVector2(this UnifiedRandom rand)
        {
            return Vector2.UnitY.RotatedBy(rand.NextFloat(FullCircle));
        }

        public static Vector2 NextVector2(this UnifiedRandom rand, float minLength, float maxLength)
        {
            return rand.NextVector2().OfLength(rand.NextFloat(minLength, maxLength));
        }

        public static bool OneIn(this UnifiedRandom rand, int amount) // Returns true with a 1/amount chance
        {
            return rand.Next(amount) == 0;
        }



        // Number extensions

        public static float ToRadians(this int deg) // Returns the given degrees in radians
        {
            return deg * (float)Math.PI / 180f;
        }

        public static float ToRadians(this float deg)
        {
            return deg * (float)Math.PI / 180f;
        }

        public static float ToDegrees(this float rad) // Returns the given radians in degrees
        {
            return rad * 180f / (float)Math.PI;
        }
    }
}
