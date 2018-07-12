using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Virtuous.Items;
using Virtuous.Projectiles;

namespace Virtuous
{
    public class VirtuousPlayer : ModPlayer
    {
        const string DeprecatedGobblerStorageKey = "GobblerStorage";
        const string GobblerItemTypesKey = "GobblerStorageTypes";
        const string GobblerItemPrefixesKey = "GobblerStoragePrefixes";


        // Stores the types of items sucked by the gobbler
        public List<GobblerStoredItem> GobblerStorage = new List<GobblerStoredItem>(TheGobbler.StorageCapacity);

        public int titanShieldDashing = 0; // Time left and direction of the dash. 0 is inactive
        public int titanShieldCoolDown = TitanShield.CoolDown; // Time left until the dash can be used again
        public int titanShieldLastExplosion = 0; // Point in titanShieldDashing when an explosion was last spawned. 0 is unregistered

        public bool accessoryAstroBoots = false;
        public bool accessoryArchangel = false;




        // General

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) // Held item visuals
        {
            if (player.HeldItem.type == mod.ItemType<EtherSlit>())
            {
                // I had to make a few eye-guesses for the hand position so that it's centered more or less perfectly
                Vector2 handPosition = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f; //Vanilla code
                if (player.direction == -1) handPosition.X = player.bodyFrame.Width - handPosition.X - 2; //Facing left
                if (player.gravDir == -1) handPosition.Y = player.bodyFrame.Height - handPosition.Y; //Upside down
                handPosition -= new Vector2(player.bodyFrame.Width - player.width + 8, player.bodyFrame.Height - 30f) / 2f;

                Vector2 position = player.position + handPosition;

                for (int i = 0; i < 2; i++)
                {
                    var dust = Dust.NewDustDirect(
                        position, 0, 0, /*Type*/180, 2f * player.direction, 0f, /*Alpha*/150, Color.Black, 0.5f);
                    dust.velocity = player.velocity; // So that it seems to follow the player
                    dust.noGravity = true;
                    dust.fadeIn = 1f;

                    if (Main.rand.OneIn(2))
                    {
                        dust.position += new Vector2(Main.rand.NextFloat(-3, +3), Main.rand.NextFloat(-3, +3));
                        dust.scale += Main.rand.NextFloat();
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
                if (titanShieldDashing == 0) // Not dashing
                {
                    r = 0; g = 0.7f; b = 0.8f;
                }
                else // Dashing
                {
                    r = 0; g = 1.3f; b = 1.5f;
                }

                Lighting.AddLight(player.Center + new Vector2(10 * player.direction, 0), r, g, b);

                player.shield = (sbyte)mod.GetEquipSlot("TitanShield", EquipType.Shield);
                drawInfo.shieldShader = 0;
            }
        }




        // The Gobbler

        public override void PreUpdate()
        {
            if (player.HeldItem.type == mod.ItemType<TheGobbler>())
            {
                if (player.controlThrow && GobblerStorage.Count > 0) // Release storage
                {
                    Main.PlaySound(SoundID.Item3, player.Center);
                    foreach (var storedItem in GobblerStorage)
                    {
                        int itemIndex = Item.NewItem(player.Center, storedItem.type, prefixGiven: storedItem.prefix);
                        if (Main.netMode == NetmodeID.MultiplayerClient) // Syncs to multiplayer
                        {
                            NetMessage.SendData(MessageID.SyncItem, number: itemIndex);
                        }
                    }

                    GobblerStorage.Clear();
                }
            }
        }


        public override TagCompound Save()
        {
            return new TagCompound {
                { GobblerItemTypesKey, GobblerStorage.Select(x => x.type).ToArray() },
                { GobblerItemPrefixesKey, GobblerStorage.Select(x => x.prefix).ToArray() }
            };
        }


        public override void Load(TagCompound tag) // Grabs the gobbler storage from the savefile
        {
            GobblerStorage.Clear();

            int[] itemTypes;
            if (tag.ContainsKey(GobblerItemTypesKey))
            {
                itemTypes = tag.GetIntArray(GobblerItemTypesKey);
            }
            else if (tag.ContainsKey(DeprecatedGobblerStorageKey)) // Backwards compatibility
            {
                itemTypes = tag.GetIntArray(DeprecatedGobblerStorageKey).Where(x => x != ItemID.None).ToArray();
            }
            else itemTypes = new int[0];

            if (itemTypes.Length == 0) return; // Nothing to add

            byte[] itemPrefixes;
            if (tag.ContainsKey(GobblerItemPrefixesKey)) itemPrefixes = tag.GetByteArray(GobblerItemPrefixesKey);
            else itemPrefixes = new byte[itemTypes.Length];

            var items = Enumerable.Range(0, itemTypes.Length).Select(i => new GobblerStoredItem(itemTypes[i], itemPrefixes[i]));
            GobblerStorage.AddRange(items);
        }




        // Titan Shield

        public void TitanShieldDash()
        {
            int dashDirection = titanShieldDashing > 0 ? +1 : -1; // Negative is left

            player.vortexStealthActive = false;

            var playerRect = new Rectangle(
                (int)(player.position.X + player.velocity.X * 0.5f - 4),
                (int)(player.position.Y + player.velocity.Y * 0.5f - 4),
                player.width + 8,
                player.height + 8);

            // All NPCs colliding with the expanded player hitbox
            foreach (NPC npc in Main.npc.Where(x => x.active && !x.dontTakeDamage && !x.friendly && x.immune[player.whoAmI] <= 0
                                                    && playerRect.Intersects(x.getRect()) && (x.noTileCollide || player.CanHit(x))))
            {
                var titanShield = (TitanShield)player.inventory[player.selectedItem].modItem; // Gets the held item
                if (titanShield == null) return; // Shouldn't happen

                int damage = 0; titanShield.GetWeaponDamage(player, ref damage);
                float knockBack = titanShield.item.knockBack;
                bool crit = Main.rand.Next(100) < (titanShield.item.crit + player.meleeCrit);

                // Damages the enemy
                player.ApplyDamageToNPC(npc, damage, knockBack, player.direction, crit);
                npc.immune[player.whoAmI] = 5;

                // Area of effect, with cooldown
                if (titanShieldLastExplosion == 0 || titanShieldLastExplosion - Math.Abs(titanShieldDashing) >= TitanShield.ExplosionDelay)
                {
                    titanShieldLastExplosion = Math.Abs(titanShieldDashing);
                    Main.PlaySound(SoundID.Item14, npc.Center);
                    var proj = Projectile.NewProjectileDirect(
                        npc.Center, Vector2.Zero, mod.ProjectileType<ProjTitanAOE>(), damage, knockBack / 2, player.whoAmI);
                    var modProj = proj.modProjectile as ProjTitanAOE;
                    modProj.Crit = crit;
                }

                // Prevents player from taking damage
                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime = Math.Min(10, Math.Abs(titanShieldDashing)); // Immune time caps at the time left
            }

            // Dust
            for (int i = 0; i < 5; i++)
            {
                Color color = Main.rand.OneIn(2) ? new Color(255, 255, 255) : new Color(255, 255, 255, 0f);
                var dust = Dust.NewDustDirect(
                    player.position + new Vector2(Main.rand.NextFloat(-5, +5), Main.rand.NextFloat(-5, +5)), player.width, player.height,
                    /*Type*/16, 0f, 0f, /*Alpha*/0, color, /*Scale*/Main.rand.NextFloat(1, 2));
                dust.velocity *= 0.2f;
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }

            if (Math.Abs(titanShieldDashing) > TitanShield.DashTime - 8)
            {
                player.velocity.X = 16.0f * dashDirection; // First 8 ticks maintains top speed in the direction of the dash
            }
            else if (dashDirection == +1 && player.velocity.X <= 0 || dashDirection == -1 && player.velocity.X >= 0)
            {
                titanShieldDashing = 0; // Player changes direction, ends the dash
                return;
            }

            titanShieldDashing -= dashDirection; // Goes toward 0
            titanShieldCoolDown = TitanShield.CoolDown; // Resets cooldown to the max until the dash is over
        }


        public override void PostUpdate()
        {
            // Titan shield mechanics

            if (player.HeldItem.type == mod.ItemType<TitanShield>()) // Holding the item
            {
                if (player.controlUseItem && titanShieldCoolDown == 0) // Clicking
                {
                    player.direction = (Main.MouseWorld - player.Center).X > 0 ? +1 : -1;
                    titanShieldDashing = TitanShield.DashTime * player.direction;
                    Main.PlaySound(SoundID.Item15, player.Center);
                }

                if (titanShieldDashing == 0) titanShieldLastExplosion = 0; // Resets explosion cooldown
                else TitanShieldDash(); // Executes the dash
            }
            else
            {
                titanShieldDashing = 0; // Resets dash
                titanShieldLastExplosion = 0;
            }
        }


        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            // Titan shield held: Reduces damage if the hit comes from the front
            if (player.HeldItem.type == mod.ItemType<TitanShield>() && hitDirection != player.direction)
            {
                damage = (int)(damage * (1 - TitanShield.DamageReduction));
                int dustAmount = Main.rand.Next(15, 21);
                for (int i = 0; i < dustAmount; i++)
                {
                    var dust = Dust.NewDustDirect(
                        player.Center + new Vector2(10 * player.direction, 0), 0, 0, /*Type*/180, 0f, 0f, /*Alpha*/100,
                        default(Color), /*Scale*/Main.rand.NextFloat(1, 2.5f));
                    dust.noGravity = true;
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
