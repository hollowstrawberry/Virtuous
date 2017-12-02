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

        private bool[] specialFunction = new bool[2];
        private const int On = 0; //specialFunction[0] controls whether the special is active or not
        private const int SafeTurnOff = 1; //specialFunction[1] safely signals the player to turn off the special itself in ResetEffects. Without this protection, orbitals could desync and interfere with one another in their behavior

        public bool specialFunctionActive
        {
            get { return specialFunction[On]; }

            set
            {
                if (value) //Set to true
                {
                    if (!specialFunction[SafeTurnOff]) specialFunction[On] = true;
                }
                else //Set to false
                {
                    specialFunction[SafeTurnOff] = true;
                }
            }
        }

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
            if (specialFunction[SafeTurnOff]) specialFunction = new bool[2]; //Shuts down special effect
        }

        public override void UpdateDead()
        {
            ResetOrbitals();
        }
    }
}
