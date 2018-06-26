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
        public int time; // Time left, in ticks, of an orbital summon. 0 is inactive
        public bool[] active = new bool[OrbitalID.Orbital.Length];
        public float damageBuffFromOrbitals; // Set every tick by active orbitals
        public float durationMultiplier = 1f;
        public float damageMultiplier = 1f;
        public bool accessoryPermanent;

        private bool specialFunction;
        private bool specialFunctionTurnOff;

        public bool SpecialFunctionActive // Turning off is handled by the player itself so that orbitals don't desync
        {
            get { return specialFunction; }

            set
            {
                if (value) // When set to true
                {
                    if (!specialFunctionTurnOff) specialFunction = true;
                }
                else // When set to false
                {
                    specialFunctionTurnOff = true;
                }
            }
        }



        public int ModifiedOrbitalTime(OrbitalItem item)
        {
            return (int)(item.duration * durationMultiplier + OrbitalID.Orbital[item.type].DyingTime);
        }


        public void ResetOrbitals() // Resets all active orbital data for the player
        {
            time = 0;
            specialFunction = false;
            specialFunctionTurnOff = false;
            active = new bool[active.Length]; // All to false
        }


        public override void PostUpdateEquips() // Applies orbital buffs
        {
            for (int type = 0; type < OrbitalID.Orbital.Length; type++)
            {
                if (active[type])
                {
                    OrbitalProjectile.FindFirst(mod, player, type)?.PlayerEffects();
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

            if (specialFunctionTurnOff)
            {
                specialFunction = false;
                specialFunctionTurnOff = false;
            }
        }


        public override void UpdateDead()
        {
            ResetOrbitals();
        }
    }
}
