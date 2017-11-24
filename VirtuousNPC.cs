using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Items;
using Virtuous.Projectiles;
using Virtuous.Orbitals;
using static Virtuous.Tools;

namespace Virtuous
{
    public class VirtuousNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;


        public int fallDamage = 0; //Fall damage accumulated. 0 is inactive
        public bool startedFalling = false; //Whether the target afflicted with fall damage is in falling motion

        public int summonedSwordStuck = 0; //How many summoned swords are stuck in the target
        public int summonedSwordStuckTimer = 0; //Ticks passed since the last damage dealt by the stuck swords


        public override void ResetEffects(NPC npc)
        {
            if(summonedSwordStuck > 0)
            {
                if (Main.GameUpdateCount % 10 == 0 && npc.active && !npc.dontTakeDamage) //Every 10 ticks
                {
                    npc.StrikeNPC(ProjSummonedSword.StuckDOT * Math.Min(summonedSwordStuck/2, ProjSummonedSword.StuckMaxAmount), 0, 0, false, true); //Damages every 10 ticks, damage stacking caps at stuckMaxAmount
                }
            }
            summonedSwordStuck = 0; //Resets the effect, it gets reapplied by thr swords stuck on the target


            if (fallDamage > 0) //Fall damage effect active
            {
                if(npc.velocity.Y > 0) //While falling
                {
                    startedFalling = true;
                    if (fallDamage < 250) fallDamage += 5; //Fall damage increases
                    else if (fallDamage < 10000) fallDamage += 10;

                    if (npc.collideY) //Has hit the ground
                    {
                        npc.StrikeNPC(fallDamage, 0, 0, false, true, false); //Applies the accumulated damage
                        fallDamage = 0; //Turns off the effect
                    }
                }
                else if(startedFalling) //If it's not falling anymore, turns off the effect
                {
                    fallDamage = 0;
                }
            }
        }

        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.KingSlime)
            {
                Item.NewItem(npc.Center, mod.ItemType<Sailspike_Item>());
            }

            else if(npc.type == NPCID.SkeletronHead)
            {
                Item.NewItem(npc.Center, mod.ItemType<Facade_Item>());
            }

            else if (npc.type == NPCID.Golem)
            {
                Item.NewItem(npc.Center, mod.ItemType<HolyLight_Item>());
            }

            else if (npc.type == NPCID.GiantCursedSkull && RandomInt(15) == 0)
            {
                Item.NewItem(npc.Center, mod.ItemType<SacDagger_Item>());
            }

            else if (npc.type == NPCID.DukeFishron)
            {
                Item.NewItem(npc.Center, mod.ItemType<Shuriken_Item>());
            }

            else if(npc.type == NPCID.MoonLordCore)
            {
                Item.NewItem(npc.Center, mod.ItemType<TheGobbler>());
            }
        }
    }
}
