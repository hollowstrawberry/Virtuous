using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Items;
using Virtuous.Projectiles;
using Virtuous.Orbitals;
using Microsoft.Xna.Framework;

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


        public override void OnKill(NPC npc)
        {
            int dropType = ItemID.None, dropAmount = 1;

            switch (npc.type)
            {
                case NPCID.SkeletronHead:
                    dropType = Mod.Find<ModItem>(nameof(Facade_Item)).Type;
                    break;

                case NPCID.Golem:
                    dropType = Mod.Find<ModItem>(nameof(HolyLight_Item)).Type;
                    break;

                case NPCID.GiantCursedSkull:
                    if (Main.rand.NextBool(15)) dropType = Mod.Find<ModItem>(nameof(SacDagger_Item)).Type;
                    break;

                case NPCID.DukeFishron:
                    dropType = Mod.Find<ModItem>(nameof(Shuriken_Item)).Type;
                    break;

                case NPCID.MoonLordCore:
                    dropType = Mod.Find<ModItem>(nameof(TheGobbler)).Type;
                    break;

                case NPCID.PirateCaptain:
                case NPCID.PirateShip:
                    if (Main.rand.NextBool(10)) dropType = Mod.Find<ModItem>(nameof(LuckyBreak_Item)).Type;
                    break;

                case NPCID.TheDestroyer:
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                case NPCID.SkeletronPrime:
                    if (Main.rand.NextBool(6)) dropType = Mod.Find<ModItem>(nameof(EnergyCrystal_Item)).Type;
                    break;
            }

            if (dropType != ItemID.None) Item.NewItem(null, npc.Center, Vector2.Zero, dropType, dropAmount);
        }
    }
}
