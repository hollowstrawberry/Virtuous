using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    /*
     * These are used to manage the player's active orbitals, as well as controlling which items and projectiles are associated.
     * The orbitals must have individual orbital IDs, starting at 0 and being consecutive.
     */

    public static class OrbitalID
    {
        public const int None = -1;
        public const int Sailspike      =  0;
        public const int Facade         =  1;
        public const int Bubble         =  2;
        public const int SpikedBubble   =  3;
        public const int HolyLight      =  4;
        public const int SacDagger      =  5;
        public const int Shuriken       =  6;
        public const int Bullseye       =  7;
        public const int SpiralSword    =  8;
        public const int Fireball       =  9;
        public const int EnergyCrystal  = 10;
        public const int LuckyBreak     = 11;
        public const int Jungle         = 12;
        public const int GelCube        = 13;


        // Allows me to access the properties of an orbital type whose orbital ID is the index of the array
        public static readonly OrbitalProjectile[] Orbital = CreateOrbitalArray();


        public static int OrbitalProjectileType(this Mod mod, int id) // Returns the projectile type for the given orbital ID
        {
            return mod.ProjectileType(Orbital[id].GetType().Name);
        }


        private static OrbitalProjectile[] CreateOrbitalArray()
        {
            var orbitals = new List<OrbitalProjectile>(); // Create a list to manipulate before sending the final array

            // Obtain all subtypes of OrbitalProjectile
            var types = Assembly.GetAssembly(typeof(OrbitalProjectile)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(OrbitalProjectile)));

            // Make empty orbitals then sort them by ID
            orbitals.AddRange(types.Select(type => Activator.CreateInstance<OrbitalProjectile>()));
            orbitals = orbitals.OrderBy(orbital => orbital.Type).ToList();

            // Makes sure the list has a one-to-one correspondence
            if (Enumerable.Range(0, orbitals.Count).Any(i => orbitals[i].Type != i))
            {
                throw new Exception("Virtuous: An orbital projectile has an invalid orbital ID, or the same orbital ID as another orbital. " +
                                    "Valid IDs are positive and consecutive.");
            }

            return orbitals.ToArray(); // Return the final array
        }
    }
}
