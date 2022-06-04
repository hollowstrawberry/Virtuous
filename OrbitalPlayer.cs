using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    /// <summary>
    /// Stores the active orbital data for a given player.
    /// </summary>
    public class OrbitalPlayer : ModPlayer
    {
        /// <summary>Time left, in ticks, of the current orbital summon. 0 is inactive.</summary>
        public int time;

        /// <summary>Whether an orbital at a given index (orbital ID) is active for the current player.</summary>
        public bool[] active = new bool[OrbitalID.Orbital.Length];

        /// <summary>Deducted from total damage so that orbitals don't boost their own damage.</summary>
        public float damageBuffFromOrbitals;

        /// <summary>Multiplier for orbital duration.</summary>
        public float durationMultiplier = 1f;

        /// <summary>Multiplier for orbital damage.</summary>
        public float damageMultiplier = 1f;

        /// <summary>Whether the player is wearing the accessory that gives orbitals infinite duration.</summary>
        public bool accessoryPermanent;


        private bool specialFunction;
        private bool specialFunctionTurnOff;

        /// <summary>Starts or ends the active orbital's special effect.</summary>
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



        /// <summary>The final duration of an orbital, all effects taken into account.</summary>
        public int ModifiedOrbitalTime(OrbitalItem item)
        {
            return (int)(item.Duration * durationMultiplier + OrbitalID.Orbital[item.OrbitalType].DyingTime);
        }


        /// <summary>Resets all active orbital data for the player.</summary>
        public void ResetOrbitals()
        {
            time = 0;
            specialFunction = false;
            specialFunctionTurnOff = false;
            active = new bool[active.Length]; // All to false
        }


        public override void PostUpdateEquips()
        {
            for (int type = 0; type < OrbitalID.Orbital.Length; type++)
            {
                if (!active[type]) continue;

                var orbital = Main.projectile
                    .Where(proj => proj.active && proj.owner == Player.whoAmI)
                    .Select(proj => proj.ModProjectile as OrbitalProjectile)
                    .FirstOrDefault(orb => orb != null && orb.OrbitalType == type);

                orbital?.PlayerEffects();
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
