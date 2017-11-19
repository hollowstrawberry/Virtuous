using System;
using System.Linq;
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
        public const float FullCircle = 2 * (float)Math.PI;
        public const float RevolutionPerSecond = FullCircle / 60;


        public static float ToRadians(this int deg)
        {
            return (float)deg * (float)Math.PI / 180f;
        }
        public static float ToRadians(this float deg)
        {
            return deg * (float)Math.PI / 180f;
        }

        public static float ToDegrees(this float rad)
        {
            return rad * 180f / (float)Math.PI;
        }


        public static void RedundantFunc() //Catches a strange compiling error
        {
            var something = System.Linq.Enumerable.Range(1, 10);
        }
    }
}

