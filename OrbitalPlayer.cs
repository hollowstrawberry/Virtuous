using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Orbitals;


namespace Virtuous
{
    public class OrbitalPlayer : ModPlayer
    {
        public int time = 0; //Time left, in ticks, of an orbital summon. 0 is inactive
        public bool[] active = new bool[OrbitalID.Orbital.Length];

        private bool[] specialFunction = new bool[2];
        private const int IsOn = 0; //specialFunction[0] controls whether the special is active or not
        private const int SafeTurnOff = 1; //specialFunction[1] safely signals the player to itself turn off the special in ResetEffects. Without this protection, orbitals could desync and interfere with one another in their behavior
        public bool specialFunctionActive
        {
            get { return specialFunction[IsOn]; }

            set
            {
                if (value) //Set to true
                {
                    if (!specialFunction[SafeTurnOff]) specialFunction[IsOn] = true;
                }
                else //Set to false
                {
                    specialFunction[SafeTurnOff] = true;
                }
            }
        }

        public float damageBuffFromOrbitals = 0f; //Increased every tick by orbitals that don't want their damage buff to increase other orbitals' damage

        public float durationMultiplier = 1f;
        public float damageMultiplier = 1f;
        public bool accessoryPermanent = false; //Orbitals last forever


        public int ModifiedOrbitalTime(OrbitalItem item)
        {
            return (int)(item.duration * durationMultiplier + OrbitalID.Orbital[item.type].DyingTime);
        }

        public void ResetOrbitals() //Resets all active orbital data on the player, killing orbitals in the process
        {
            time = 0;
            specialFunction = new bool[2]; //Sets all to false
            active = new bool[active.Length]; //Sets all to false
        }


        public override void PostUpdateEquips() //Applies orbital buffs
        {
            for (int type = 0; type < OrbitalID.Orbital.Length; type++)
            {
                if (active[type])
                {
                    OrbitalProjectile orbital = OrbitalProjectile.FindFirstOrbital(mod, player, type);
                    if (orbital != null) orbital.PlayerEffects();
                }
            }
        }
        
        public override void ResetEffects()
        {
            if (time > 0) time--;
            if (time <= 0) ResetOrbitals();

            damageBuffFromOrbitals  = 0f;

            durationMultiplier = 1f;
            damageMultiplier   = 1f;
            accessoryPermanent = false;

            if (specialFunction[SafeTurnOff]) specialFunction = new bool[2]; //Shuts down special effect
        }

        public override void UpdateDead()
        {
            ResetOrbitals();
        }
    }
}
