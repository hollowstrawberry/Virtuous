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
        public bool[] active = new bool[10];

        public bool[] specialFunction = new bool[2]; //[0] calls for the right-click effect to activate, [1] calls for it to shut down
        public const int SpecialOn = 0;
        public const int SpecialOff = 1;

        public bool accessoryTimeBoost = false;
        public bool accessoryDmgBoost = false;

        public int ModifiedOrbitalTime(Item item) //Sets the final duration after any boosts applicable
        {
            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            return (int)((orbitalItem.duration) * (accessoryTimeBoost ? 1.5 : 1)); // More damage with the conditon active
        }

        public void ResetOrbitals() //Resets all orbital data on the player, killing any orbitals active in the process
        {
            time = 0;
            specialFunction = new bool[2]; //Sets all to false
            active = new bool[10]; //Sets all to false
        }

        public bool BullseyeShot() //The player is aiming in the right direction for a boosted shot by the Bullseye orbital
        {
            return Math.Abs((Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX).X) > Bullseye_Proj.GapTreshold; //Roughly straight left or right
        }

        public override void PostUpdateEquips()
        {
            //Orbital buffs
            if (active[OrbitalID.Bubble])
            {
                player.statDefense += 10;
            }
            else if (active[OrbitalID.SpikedBubble])
            {
                player.statDefense += 5;
                player.meleeDamage += SpikedBubble_Proj.DamageBoost;
                player.magicDamage += SpikedBubble_Proj.DamageBoost;
            }
            else if (active[OrbitalID.HolyLight])
            {
                player.lifeRegen += 3;
            }
            else if (active[OrbitalID.SacDagger])
            {
                player.lifeRegenTime = 0;
                player.lifeRegen = -10;
            }
            else if (active[OrbitalID.Bullseye])
            {
                if(BullseyeShot())
                {
                    player.rangedDamage *= 2.0f;
                    player.thrownDamage *= 2.0f;
                }
                else
                {
                    player.rangedDamage *= 0.8f;
                    player.thrownDamage *= 0.8f;
                }
            }
            else if (active[OrbitalID.Shuriken])
            {
                player.manaRegenDelayBonus++;
                player.manaRegenBonus += 25;
                player.meleeSpeed += 0.12f;
            }
            else if(active[OrbitalID.SpiralSword])
            {
                player.meleeDamage  += SpiralSword_Proj.DamageBoost;
                player.magicDamage  += SpiralSword_Proj.DamageBoost;
                player.rangedDamage += SpiralSword_Proj.DamageBoost;
                player.thrownDamage += SpiralSword_Proj.DamageBoost;
            }
        }
        
        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //Orbital visuals
            if(active[OrbitalID.Bubble])
            {
                Lighting.AddLight(player.Center, 0.4f, 0.6f, 0.6f);
            }
            else if (active[OrbitalID.SpikedBubble])
            {
                Lighting.AddLight(player.Center, 0.6f, 0.4f, 0.3f);
            }
            else if(active[OrbitalID.HolyLight] && time % 10 == 0) //Every x ticks
            {
                Dust newDust = Dust.NewDustDirect(player.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, default(Color), /*Scale*/0.5f);
                Vector2 velocity = new Vector2(Main.rand.NextFloat() * 2 - 1, Main.rand.NextFloat() * 2 - 1).SafeNormalize(Vector2.UnitX) * (Main.rand.NextFloat() * 2 + 4);
                Vector2 offset = velocity.SafeNormalize(Vector2.UnitX) * 50f; //Distance from the player
                newDust.velocity = velocity;
                newDust.position -= offset;
                newDust.noGravity = true;
                newDust.fadeIn = 1.3f;
            }
            else if(active[OrbitalID.SpiralSword])
            {
                Lighting.AddLight(player.Center, 0.5f, 1.5f, 3.0f);
            }
        }

        public override void ResetEffects()
        {
            if (time > 0) time--;
            if (time == 0) ResetOrbitals();
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
