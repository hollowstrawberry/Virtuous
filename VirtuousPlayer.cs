using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Virtuous.Items;
using Virtuous.Projectiles;
using static Virtuous.Tools;

namespace Virtuous
{
    public class VirtuousPlayer : ModPlayer
    {

        public int[] GobblerStorage = new int[TheGobbler.StorageCapacity]; //Stores all the item types of the items sucked by the gobbler by this player

        public int titanShieldDashing = 0; //The time left and direction of the dash. 0 is inactive
        public int titanShieldCoolDown = TitanShield.CoolDown; //Time left until the dash can be used again
        public int titanShieldLastExplosion = 0; //Point in titanShieldDashing when an explosion was last spawned. 0 is unregistered

        public bool accessoryAstroBoots = false;
        public bool accessoryArchangel = false;



        ///* Global *///

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) //Held item visuals
        {
            if (player.HeldItem.type == mod.ItemType<EtherSlit>())
            {
                //I had to make a few eye-guesses for the hand position so it's centered more or less perfectly
                Vector2 handPosition = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f; //Vanilla code
                if (player.direction == -1) handPosition.X = player.bodyFrame.Width - handPosition.X - 2; //Facing left
                if (player.gravDir == -1) handPosition.Y = player.bodyFrame.Height - handPosition.Y; //Upside down
                handPosition -= new Vector2(player.bodyFrame.Width - player.width + 8, player.bodyFrame.Height - 30f) / 2f;

                Vector2 position = player.position + handPosition;

                for (int i = 0; i < 2; i++)
                {
                    Dust newDust = Dust.NewDustDirect(position, 0, 0, /*Type*/180, 2f * player.direction, 0f, /*Alpha*/150, Color.Black, 0.5f);
                    newDust.velocity = player.velocity; //Seems to follow the player around
                    newDust.noGravity = true;
                    newDust.fadeIn = 1f;
                    if (CoinFlip()) //Half the time
                    {
                        newDust.position += new Vector2(RandomFloat(-3, +3), RandomFloat(-3, +3)); //Slightly different location
                        newDust.scale += RandomFloat(); //Different size
                    }
                }
            }
            else if (player.HeldItem.type == mod.ItemType<FlurryNova>())
            {
                player.handon = (sbyte)mod.GetEquipSlot("FlurryNova", EquipType.HandsOn);
                player.handoff = (sbyte)mod.GetEquipSlot("FlurryNova", EquipType.HandsOff);
                drawInfo.handOnShader = 0;
                drawInfo.handOffShader = 0;
            }
            else if (player.HeldItem.type == mod.ItemType<TitanShield>())
            {
                float r, g, b;
                if (titanShieldDashing == 0) //Not dashing
                {
                    r = 0; g = 0.7f; b = 0.8f;
                }
                else //Dashing
                {
                    r = 0; g = 1.3f; b = 1.5f;
                }

                Lighting.AddLight(player.Center + new Vector2(10 * player.direction, 0), r, g , b);

                player.shield = (sbyte)mod.GetEquipSlot("TitanShield", EquipType.Shield);
                drawInfo.shieldShader = 0;
            }
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //if (accessoryAstroBoots)
            //{
            //    if (Math.Abs(player.velocity.Length()) > 0.1f && !player.mount.Active)
            //    {
            //        if (CoinFlip() && drawInfo.shadow == 0f)
            //        {
            //            int newDust = Dust.NewDust(player.Center, 0, 0, /*Type*/14, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, /*Alpha*/50, default(Color), /*Scale*/2.0f);
            //            Main.dust[newDust].noGravity = true;
            //            Main.playerDrawDust.Add(newDust);
            //        }
            //        fullBright = true;
            //    }
            //}
            //if (accessoryArchangel)
            //{
            //    if (Math.Abs(player.velocity.Length()) > 0.1f && !player.mount.Active)
            //    {
            //        if (CoinFlip() && drawInfo.shadow == 0f)
            //        {
            //            int newDust = Dust.NewDust(player.Center, 0, 0, /*Type*/14, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, /*Alpha*/50, default(Color), /*Scale*/2.0f);
            //            Main.dust[newDust].noGravity = true;
            //            Main.playerDrawDust.Add(newDust);
            //        }
            //        fullBright = true;
            //    }
            //}
        }



        ///* The Gobbler *///

        public Item GobblerItem(int slot) //Returns an item of the type stored in the gobbler storage slot provided
        {
            Item item = new Item(); //Creates a new temporal item instance equal to null
            if (GobblerStorage[slot] != ItemID.None) item.SetDefaults((int)GobblerStorage[slot]); //Initializes it if the corresponding slot in GobblerStorage is full
            return item;
        }

        public int GobblerStorageFilled() //How many slots in the gobbler storage are filled
        {
            int slot = 0;
            while (slot < TheGobbler.StorageCapacity && GobblerStorage[slot] != ItemID.None)
            {
                slot++;
            }
            return slot;
        }

        public override TagCompound Save() //Stores the gobbler storage into the savefile
        {
            return new TagCompound { { "GobblerStorage", GobblerStorage } };
        }

        public override void Load(TagCompound tag) //Withdraws the gobbler storage from the savefile
        {
            GobblerStorage = tag.GetIntArray("GobblerStorage");
        }

        public override void PreUpdate()
        {
            //The Gobbler special mechanics
            if (player.HeldItem.type == mod.ItemType<TheGobbler>())
            {
                if (player.controlThrow)
                {
                    if (GobblerStorage[TheGobbler.BaseSlot] != ItemID.None)  //if there is something in the storage
                    {
                        Main.PlaySound(SoundID.Item3, player.Center);
                        for (int slot = 0; slot < TheGobbler.StorageCapacity; slot++) //Cycles through the storage
                        {
                            if (GobblerStorage[slot] != ItemID.None)
                            {
                                Item storedItem = GobblerItem(slot); //One copy for easy access
                                int newItem = Item.NewItem(player.Center, storedItem.type, 1, false, ProjGobblerItem.IsReforgeableWeapon(storedItem) ? PrefixID.Broken : 0); //Spits it out

                                if (Main.netMode == NetmodeID.MultiplayerClient) //Syncs to multiplayer
                                {
                                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem);
                                }
                            }
                            GobblerStorage[slot] = ItemID.None; //Clears the item from the storage
                        }
                    }
                }
            }

            if (Main.GameUpdateCount % 60 == 0 && player.altFunctionUse != 2) //Every second, if the player is not right clicking
            {
                for (int i = 0; i < Main.maxItems; i++) //Cycles through all the items in the world
                {
                    if (Main.item[i].active && Main.item[i].WithinRange(player.Center, 300))
                    {
                        Main.item[i].GetGlobalItem<VirtuousItem>().beingGobbled = false; //Makes the items able to be picked up again
                    }
                }
            }
        }



        ///* Titan Shield *///

        public void TitanShieldDash() //Executes the dash (ramming)
        {
            int dashDirection = titanShieldDashing > 0 ? +1 : -1; //Negative is left

            player.vortexStealthActive = false;

            //Hits NPCs
            Rectangle playerRect = new Rectangle((int)(player.position.X + player.velocity.X * 0.5 - 4.0), (int)(player.position.Y + player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8); //Expanded player hitbox
            foreach (NPC npc in Main.npc)
            {
                //Finds a suitable target that collides with the player
                if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.immune[player.whoAmI] <= 0)
                {
                    if (playerRect.Intersects(npc.getRect()) && (npc.noTileCollide || player.CanHit(npc)))
                    {
                        TitanShield titanShield = (TitanShield)player.inventory[player.selectedItem].modItem; //Gets the held item
                        int damage = titanShield.GetDamage(player);
                        float knockBack = titanShield.item.knockBack;
                        bool crit = RandomInt(100) < (titanShield.item.crit + player.meleeCrit);

                        //Damages the enemy
                        player.ApplyDamageToNPC(npc, damage, knockBack, player.direction, crit);
                        npc.immune[player.whoAmI] = 5;

                        //Area of effect
                        if (titanShieldLastExplosion == 0 || titanShieldLastExplosion - Math.Abs(titanShieldDashing) >= TitanShield.ExplosionDelay) //Cooldown before consecutive explosions
                        {
                            titanShieldLastExplosion = Math.Abs(titanShieldDashing);
                            Main.PlaySound(SoundID.Item14, npc.Center);
                            Projectile newProj = Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, mod.ProjectileType<ProjTitanAOE>(), damage, knockBack / 2, player.whoAmI, /*ai[0]*/crit ? 1 : 0);
                        }

                        //Prevents player from taking damage
                        player.immune = true;
                        player.immuneNoBlink = true;
                        player.immuneTime = Math.Min(10, Math.Abs(titanShieldDashing)); //Immune time caps at the time left

                    }
                }
            }

            //Dust
            for (int i = 0; i < 5; i++)
            {
                Color color = CoinFlip() ? new Color(255, 255, 255) : new Color(255, 255, 255, 0f);
                Dust newDust = Dust.NewDustDirect(player.position + new Vector2(RandomFloat(-5, +5), RandomFloat(-5, +5)), player.width, player.height, /*Type*/16, 0f, 0f, /*Alpha*/0, color, /*Scale*/RandomFloat(1f, 2f));
                newDust.velocity *= 0.2f;
                newDust.noGravity = true;
                newDust.fadeIn = 0.5f;
            }

            if (Math.Abs(titanShieldDashing) > TitanShield.DashTime - 8) //First 8 ticks
            {
                player.velocity.X = 16.0f * dashDirection; //Maintains top speed in the direction of the dash
            }
            else if (dashDirection == +1 && player.velocity.X <= 0 || dashDirection == -1 && player.velocity.X >= 0) //Player has changed direction
            {
                titanShieldDashing = 0; //Ends the dash
                return;
            }

            titanShieldDashing -= dashDirection; //Goes toward 0
            titanShieldCoolDown = TitanShield.CoolDown; //Resets cooldown to the max until the dash is over
        }

        public override void PostUpdate()
        {
            //Titan shield mechanics
            if (player.HeldItem.type == mod.ItemType<TitanShield>()) //Holding the item
            {
                if (player.controlUseItem && titanShieldCoolDown == 0) //Clicking
                {
                    player.direction = (Main.MouseWorld - player.Center).X > 0 ? +1 : -1;
                    titanShieldDashing = TitanShield.DashTime * player.direction;
                    Main.PlaySound(SoundID.Item15, player.Center);
                }

                if (titanShieldDashing == 0) titanShieldLastExplosion = 0; //Resets explosion cooldown
                else TitanShieldDash(); //Executes the dash
            }
            else
            {
                titanShieldDashing = 0; //Resets dash
                titanShieldLastExplosion = 0;
            }
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            //Titan shield held: Reduces damage if the hit comes from the front
            if (player.HeldItem.type == mod.ItemType<TitanShield>() && hitDirection != player.direction)
            {
                damage = (int)(damage * (1 - TitanShield.DamageReduction));
                int dustAmount = RandomInt(15, 20);
                for (int i = 0; i < dustAmount; i++)
                {
                    Dust newDust = Dust.NewDustDirect(player.Center + new Vector2(10 * player.direction, 0), 0, 0, /*Type*/180, 0f, 0f, /*Alpha*/100, default(Color), /*Scale*/RandomFloat(1, 2.5f));
                    newDust.noGravity = true;
                }
            }
            return true;
        }

        public override void ResetEffects()
        {
            if (titanShieldCoolDown > 0) titanShieldCoolDown--;
        }
    }
}
