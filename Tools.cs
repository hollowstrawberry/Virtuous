using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Virtuous
{
    /*
     * This class contains many tools, mostly shorthands for Terraria utils and some constant keywords.
     * They help me feel like my code is clean, understandable and more personal.
     */

    public static class Tools
    {
        //Constants

        public const float FullCircle = 2 * (float)Math.PI;
        public const float RevolutionPerSecond = FullCircle / 60;
        public const float GoldenRatio = 1.61803398875f;

        public const bool Clockwise = true;
        public const bool CounterClockwise = false;
        public const bool Outwards = true;
        public const bool Inwards = false;


        //Objects

        public static void ResizeProjectile(int projIndex, int newWidth, int newHeight)
        {
            Projectile projectile = Main.projectile[projIndex];

            projectile.position += new Vector2(projectile.width / 2, projectile.height / 2);
            projectile.width = newWidth;
            projectile.height = newHeight;
            projectile.position -= new Vector2(projectile.width / 2, projectile.height / 2);
        }


        //Vectors

        public static Vector2 Normalized(this Vector2 vector) //Shorthand for Terraria's SafeNormalize
        {
            return vector.SafeNormalize(Vector2.UnitX);
        }

        public static Vector2 OfLength(this Vector2 vector, float length) //Returns a vector of the same direction but with the provided absolute length
        {
            return vector.Normalized() * length;
        }

        public static Vector2 Perpendicular(this Vector2 oldVector, float? length = null, bool clockwise = true) //Returns a vector perpendicular to the original
        {
            Vector2 vector = new Vector2(oldVector.Y, -oldVector.X);
            if (!clockwise) vector *= new Vector2(-1, -1);
            if (length != null) vector = vector.OfLength((float)length);
            return vector;
        }


        //Random

        public static int RandomInt(int min, int max) //Inclusive min and max
        {
            return Main.rand.Next(min, max + 1);
        }
        public static int RandomInt(int max) //Exclusive max
        {
            return Main.rand.Next(max);
        }

        public static float RandomFloat(float min, float max) //Inclusive min and max
        {
            return (float)Main.rand.NextDouble() * (max - min) + min;
        }
        public static float RandomFloat(float max = 1)
        {
            return (float)Main.rand.NextDouble() * max;
        }

        public static bool OneIn(int integer) //Returns true with a 1/<integer> chance
        {
            return RandomInt(integer) == 0;
        }

        public static bool CoinFlip() //Returns true half the time
        {
            return Main.rand.NextDouble() < 0.5;
        }


        //Circles

        public static float ToRadians(this int deg) //Returns the given degrees in radians
        {
            return (float)deg * (float)Math.PI / 180f;
        }
        public static float ToRadians(this float deg)
        {
            return deg * (float)Math.PI / 180f;
        }

        public static float ToDegrees(this float rad) //Returns the given radians in degrees
        {
            return rad * 180f / (float)Math.PI;
        }
    }
}
