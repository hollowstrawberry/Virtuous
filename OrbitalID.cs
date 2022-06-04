using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    /// <summary>
    /// Used to link orbital projectiles with their corresponding items, and to manage a player's active orbitals. <para />
    /// Orbital projectile types must have unique, consecutive IDs starting at 0.
    /// </summary>
    public static class OrbitalID
    {
        public const int None = -1;

        public const int
            Sailspike     =  0,
            Facade        =  1,
            Bubble        =  2,
            SpikedBubble  =  3,
            HolyLight     =  4,
            SacDagger     =  5,
            Shuriken      =  6,
            Bullseye      =  7,
            SpiralSword   =  8,
            Fireball      =  9,
            EnergyCrystal = 10,
            LuckyBreak    = 11,
            Jungle        = 12,
            GelCube       = 13;


        /// <summary>Gives access to the properties of an orbital type whose orbital ID is the index of the array.</summary>
        public static readonly OrbitalProjectile[] Orbital = CreateOrbitalArray();


        /// <summary>Returns the projectile type for the given orbital ID.</summary>
        public static int OrbitalProjectileType(this Mod mod, int id)
        {
            return mod.Find<ModProjectile>(Orbital[id].GetType().Name).Type;
        }




        private static OrbitalProjectile[] CreateOrbitalArray()
        {
            // Creates an instance of each orbital type in this project
            var orbitals = typeof(OrbitalProjectile).Assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(OrbitalProjectile)))
                .Select(type => (OrbitalProjectile)Activator.CreateInstance(type))
                .OrderBy(orbital => orbital.OrbitalType)
                .ToArray();

            // Ensures one-to-one correspondence
            if (Enumerable.Range(0, orbitals.Length).Any(i => orbitals[i].OrbitalType != i))
            {
                throw new Exception("Virtuous: An orbital projectile has an invalid orbital ID, or the same orbital ID as another orbital. " +
                                    "Valid IDs are non-negative and consecutive.");
            }

            return orbitals;
        }
    }
}
