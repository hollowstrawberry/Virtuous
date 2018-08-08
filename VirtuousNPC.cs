using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Items;
using Virtuous.Projectiles;

namespace Virtuous
{
    /// <summary>
    /// Custom data given to all NPCs by this mod.
    /// Includes fall damage mechanics and the damage over time induced by <see cref="ProjSummonedSword"/>.
    /// </summary>
    public class VirtuousNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;


        /// <summary>Fall damage accumulated. 0 is inactive.</summary>
        public int fallDamage = 0;

        /// <summary>Whether the target afflicted with fall damage is in falling motion.</summary>
        public bool alreadyStartedFalling = false;

        /// <summary>How many <see cref="ProjSummonedSword"/>s are stuck to this NPC.</summary>
        public int summonedSwordStuck = 0;



        public override void ResetEffects(NPC npc)
        {
            if (summonedSwordStuck > 0)
            {
                if (npc.active && !npc.dontTakeDamage && Main.GameUpdateCount % 10 == 0)
                {
                    npc.StrikeNPC(
                        ProjSummonedSword.StuckDOT * Math.Min(summonedSwordStuck/2, ProjSummonedSword.StuckMaxAmount),
                        0, 0, false, true);
                }
            }

            summonedSwordStuck = 0; // Effect will get reapplied by the swords stuck on the target


            if (fallDamage > 0) // Fall damage effect active
            {
                if (npc.velocity.Y > 0) // While falling
                {
                    alreadyStartedFalling = true;

                    if (fallDamage < 250) fallDamage += 5;
                    else if (fallDamage < 10000) fallDamage += 10;

                    if (npc.collideY) // Has hit the ground
                    {
                        npc.StrikeNPC(fallDamage, 0, 0, false, true, false);
                        fallDamage = 0;
                    }
                }
                else if (alreadyStartedFalling) // Was falling but not anymore
                {
                    fallDamage = 0;
                }
            }
        }


        public override void NPCLoot(NPC npc)
        {
            int dropType = ItemID.None, dropAmount = 1;

            switch (npc.type)
            {
                case NPCID.SkeletronHead:
                    dropType = mod.ItemType<Orbitals.Facade_Item>();
                    break;

                case NPCID.Golem:
                    dropType = mod.ItemType<Orbitals.HolyLight_Item>();
                    break;

                case NPCID.GiantCursedSkull:
                    if (Main.rand.OneIn(15)) dropType = mod.ItemType<Orbitals.SacDagger_Item>();
                    break;

                case NPCID.DukeFishron:
                    dropType = mod.ItemType<Orbitals.Shuriken_Item>();
                    break;

                case NPCID.MoonLordCore:
                    dropType = mod.ItemType<TheGobbler>();
                    break;

                case NPCID.PirateCaptain:
                case NPCID.PirateShip:
                    if (Main.rand.OneIn(10)) dropType = mod.ItemType<Orbitals.LuckyBreak_Item>();
                    break;

                case NPCID.TheDestroyer:
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                case NPCID.SkeletronPrime:
                    if (Main.rand.OneIn(6)) dropType = mod.ItemType<Orbitals.EnergyCrystal_Item>();
                    break;
            }

            if (dropType != ItemID.None) Item.NewItem(npc.Center, dropType, dropAmount);
        }
    }
}
