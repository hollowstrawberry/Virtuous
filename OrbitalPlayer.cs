using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Orbitals;
using static Virtuous.Tools;

namespace Virtuous
{
    public class OrbitalPlayer : ModPlayer
    {
        public int time = 0; //Time left, in ticks, of an orbital summon. 0 is inactive
        public bool[] active = new bool[OrbitalID.Orbital.Length];

        public bool[] specialFunction = new bool[2]; //[0] calls for the right-click effect to activate, [1] calls for it to shut down
        public const int SpecialOn = 0;
        public const int SpecialOff = 1;

        public bool accessoryTimeBoost = false;
        public bool accessoryDmgBoost = false;


        public int ModifiedOrbitalTime(OrbitalItem orbitalItem) //Returns the final duration after any boosts applicable
        {
            return (int)((orbitalItem.duration) * (accessoryTimeBoost ? 1.5 : 1)); // More damage with the conditon active
        }

        public void ResetOrbitals() //Resets all orbital data on the player, killing any orbitals active in the process
        {
            time = 0;
            specialFunction = new bool[2]; //Sets all to false
            active = new bool[OrbitalID.Orbital.Length]; //Sets all to false
        }


        public override void PostUpdateEquips()
        {
            for (int type = 0; type < OrbitalID.Orbital.Length; type++)
            {
                if (active[type]) OrbitalID.Orbital[type].PlayerEffects(player);
            }
        }
        
        public override void ResetEffects()
        {
            if (time > 0) time--;
            if (time <= 0) ResetOrbitals();
            accessoryTimeBoost = false;
            accessoryDmgBoost  = false;

            if (specialFunction[SpecialOff]) specialFunction = new bool[2]; //Shuts down special effect
        }

        public override void UpdateDead()
        {
            ResetOrbitals();
        }
    }
}
