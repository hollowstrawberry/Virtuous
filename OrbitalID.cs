using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    /*
     * These get used to manage the player's orbitals, as well as controlling which items and projectiles are associated.
     * The IDs must start at 0 and be consecutive, in order for arrays that depend on them to work.
     * Adding a new orbital would require making a new ID and adding it to both the Orbital array and the GetOrbitalType switch.
     * I'm hoping to be able to make it less redundant in the future
     */

    public static class OrbitalID
    {
        public const int None = -1;
        public const int Sailspike    = 0;
        public const int Facade       = 1;
        public const int Bubble       = 2;
        public const int SpikedBubble = 3;
        public const int HolyLight    = 4;
        public const int SacDagger    = 5;
        public const int Shuriken     = 6;
        public const int Bullseye     = 7;
        public const int SpiralSword  = 8;


        public static OrbitalProjectile[] Orbital = //Contains a null static projectile instance for each orbital ID, to access its properties
        {
            new Sailspike_Proj(),
            new Facade_Proj(),
            new Bubble_Proj(),
            new SpikedBubble_Proj(),
            new HolyLight_Proj(),
            new SacDagger_Proj(),
            new Shuriken_Proj(),
            new Bullseye_Proj(),
            new SpiralSword_Proj()
        };

        public static int GetOrbitalType(Mod mod, int id) //Returns a corresponding projectile type for the given orbital ID
        {
            switch (id)
            {
                case OrbitalID.Sailspike:    return mod.ProjectileType<Sailspike_Proj>();
                case OrbitalID.Facade:       return mod.ProjectileType<Facade_Proj>();
                case OrbitalID.Bubble:       return mod.ProjectileType<Bubble_Proj>();
                case OrbitalID.SpikedBubble: return mod.ProjectileType<SpikedBubble_Proj>();
                case OrbitalID.HolyLight:    return mod.ProjectileType<HolyLight_Proj>();
                case OrbitalID.SacDagger:    return mod.ProjectileType<SacDagger_Proj>();
                case OrbitalID.Bullseye:     return mod.ProjectileType<Bullseye_Proj>();
                case OrbitalID.Shuriken:     return mod.ProjectileType<Shuriken_Proj>();
                case OrbitalID.SpiralSword:  return mod.ProjectileType<SpiralSword_Proj>();
            }

            throw new Exception("Virtuous: OrbitalID has no corresponding projectile type");
        }
    }
}
