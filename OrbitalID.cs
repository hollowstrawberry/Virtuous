using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    /*
     * These get used to manage the player's active orbitals, as well as controlling which items and projectiles are associated.
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
        public const int Jungle         = 11;
        public const int LuckyBreak     = 12;
        public const int GelCube        = 13;


        public static OrbitalProjectile[] Orbital = CreateOrbitalArray(); //Allows me to access the properties of an orbital type whose orbital ID is the index


        public static int OrbitalProjectileType(this Mod mod, int id) //Returns the projectile type for the given orbital ID
        {
            return mod.ProjectileType(Orbital[id].GetType().Name);
        }


        private static OrbitalProjectile[] CreateOrbitalArray()
        {
            List<OrbitalProjectile> orbitals = new List<OrbitalProjectile>(); //Create a list to manipulate before sending the final array

            //Obtain all subtypes of OrbitalProjectile
            foreach (Type type in Assembly.GetAssembly(typeof(OrbitalProjectile)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(OrbitalProjectile))))
            {
                orbitals.Add((OrbitalProjectile)Activator.CreateInstance(type));
            }

            orbitals = orbitals.OrderBy(orbital => orbital.Type).ToList(); //Sorts the orbitals by Orbital ID

            //Makes sure the list has a one-to-one correspondence
            for (int i = 0; i < orbitals.Count; i++)
            {
                if (orbitals[i].Type != i) throw new Exception("Virtuous: An orbital projectile has an invalid orbital ID, or the same orbital ID as another orbital. Valid IDs must start at 0 and be consecutive. The ID that caused the error is: " + orbitals[i].Type.ToString());
            }

            //Return the final array
            return orbitals.ToArray();
        }
    }
}
