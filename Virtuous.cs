using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous
{
    class Virtuous : Mod
    {
        public Virtuous()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup wingsgroup = new RecipeGroup(() => "Any Wings", new int[]
            {
                ItemID.DemonWings,
                ItemID.AngelWings,
                ItemID.RedsWings,
                ItemID.ButterflyWings,
                ItemID.FairyWings,
                ItemID.HarpyWings,
                ItemID.BoneWings,
                ItemID.FlameWings,
                ItemID.FrozenWings,
                ItemID.GhostWings,
                ItemID.SteampunkWings,
                ItemID.LeafWings,
                ItemID.BatWings,
                ItemID.BeeWings,
                ItemID.DTownsWings,
                ItemID.WillsWings,
                ItemID.CrownosWings,
                ItemID.CenxsWings,
                ItemID.TatteredFairyWings,
                ItemID.SpookyWings,
                ItemID.FestiveWings,
                ItemID.BeetleWings,
                ItemID.FinWings,
                ItemID.FishronWings,
                ItemID.MothronWings,
                ItemID.WingsSolar,
                ItemID.WingsVortex,
                ItemID.WingsNebula,
                ItemID.WingsStardust,
                ItemID.Yoraiz0rWings,
                ItemID.JimsWings,
                ItemID.SkiphsWings,
                ItemID.LokisWings,
                ItemID.BetsyWings,
                ItemID.ArkhalisWings,
                ItemID.LeinforsWings
            });
            RecipeGroup.RegisterGroup("Virtuous:Wings", wingsgroup);

            RecipeGroup celestialwingsgroup = new RecipeGroup(() => "Any Celestial Wings", new int[]
            {
                ItemID.WingsSolar,
                ItemID.WingsVortex,
                ItemID.WingsNebula,
                ItemID.WingsStardust,
            });
            RecipeGroup.RegisterGroup("Virtuous:CelestialWings", celestialwingsgroup);
        }
    }

    public static class OrbitalID //These get used to monitor the player's active orbitals, and in OrbitalItem to set the projectile to shoot
    {
        public const int None = -1;
        public const int TestOrbital = 0;
        public const int Sailspike = 1;
        public const int Facade = 2;
        public const int Bubble = 3;
        public const int SpikedBubble = 4;
        public const int HolyLight = 5;
        public const int SacDagger = 6;
        public const int Bullseye = 7;
        public const int Shuriken = 8;
        public const int SpiralSword = 9;
    }

    public static class Tools
    {
        //This class contains many tools, mostly shorthands for Terraria utils, that help me feel like my code is cleaner, understandable and more personal.


        //Constants

        public const float GoldenRatio = 1.61803398875f;
        public const float FullCircle = 2 * (float)Math.PI;
        public const float RevolutionPerSecond = FullCircle / 60;
        public const bool Clockwise = true;
        public const bool CounterClockwise = false;


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




        public static void DontCrashMyGame() //The mere presence of this magical method stops a compiling error
        {
            var thanks = System.Linq.Enumerable.Range(1, 10);
        }
    }
}

