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
using Virtuous.Utils;
using Virtuous.Projectiles;

namespace Virtuous
{
    /// <summary>
    /// Custom data and effects given to a player by this mod.
    /// Includes <see cref="TheGobbler"/> storage, held item effects, etc.
    /// For orbital-related data and effects use <see cref="OrbitalPlayer"/> intead.
    /// </summary>
    public class VirtuousPlayer : ModPlayer
    {
        const string DeprecatedGobblerStorageKey = "GobblerStorage";
        const string GobblerItemTypesKey = "GobblerStorageTypes";
        const string GobblerItemPrefixesKey = "GobblerStoragePrefixes";


        /// <summary>Stores all items sucked by the gobbler, reduced to their type and prefix.</summary>
        public List<GobblerStoredItem> GobblerStorage = new List<GobblerStoredItem>(TheGobbler.StorageCapacity);

        /// <summary>Time left and direction of the dash. 0 is inactive.</summary>
        public int titanShieldDashing = 0;

        /// <summary>Time left until the dash can be used again.</summary>
        public int titanShieldCoolDown = TitanShield.CoolDown;

        /// <summary>Point in titanShieldDashing when an explosion was last spawned. 0 is none.</summary>
        public int titanShieldLastExplosion = 0;

        public bool accessoryAstroBoots = false;
        public bool accessoryArchangel = false;




        // General

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) // Held item visuals
        {
            if (player.HeldItem.type == mod.ItemType<EtherSlit>())
            {
                // I had to make a few eye-guesses for the hand position so that it's centered more or less perfectly
                Vector2 handPosition = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f; // Vanilla code
                if (player.direction == -1) handPosition.X = player.bodyFrame.Width - handPosition.X - 2; // Facing left
                if (player.gravDir == -1) handPosition.Y = player.bodyFrame.Height - handPosition.Y; // Upside down
                handPosition -= new Vector2(player.bodyFrame.Width - player.width + 8, player.bodyFrame.Height - 30f) / 2f;

                Vector2 position = player.position + handPosition;

                for (int i = 0; i < 2; i++)
                {
                    var dust = Dust.NewDustDirect(
                        position, 0, 0, Type: 180, SpeedX: 2f * player.direction, Alpha: 150, newColor: Color.Black, Scale: 0.5f);
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
                    var items = new Dictionary<GobblerStoredItem, int>();
                    foreach (var storedItem in GobblerStorage)
                    {
                        if (!items.ContainsKey(storedItem)) items[storedItem] = 0;
                        items[storedItem]++;
                    }

                    foreach (var pair in items)
                    {
                        var storedItem = pair.Key;
                        int amount = pair.Value;
                        int maxStack = storedItem.MakeItem().maxStack;
                        while (amount > 0)
                        {
                            int stackSize = Math.Min(amount, maxStack);
                            amount -= stackSize;

                            int itemIndex = Item.NewItem(player.Center, storedItem.type, stackSize, false, storedItem.prefix);
                            if (Main.netMode == NetmodeID.MultiplayerClient) // Syncs to multiplayer
                            {
                                NetMessage.SendData(MessageID.SyncItem, number: itemIndex);
                            }
                        }

                    }

                    Main.PlaySound(SoundID.Item3, player.Center);
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

            int[] itemTypes = new int[0];
            if (tag.ContainsKey(GobblerItemTypesKey))
            {
                itemTypes = tag.GetIntArray(GobblerItemTypesKey);
            }
            else if (tag.ContainsKey(DeprecatedGobblerStorageKey)) // Backwards compatibility
            {
                itemTypes = tag.GetIntArray(DeprecatedGobblerStorageKey).Where(x => x != ItemID.None).ToArray();
            }

            if (itemTypes.Length == 0) return; // Nothing to add

            byte[] itemPrefixes = new byte[itemTypes.Length];
            if (tag.ContainsKey(GobblerItemPrefixesKey))
            {
                itemPrefixes = tag.GetByteArray(GobblerItemPrefixesKey);
            }

            var items = Enumerable.Range(0, itemTypes.Length).Select(i => new GobblerStoredItem(itemTypes[i], itemPrefixes[i]));
            GobblerStorage.AddRange(items);
        }




        // Titan Shield

        public void TitanShieldDash(TitanShield shield)
        {
            int dashDirection = titanShieldDashing > 0 ? +1 : -1; // Negative is left

            player.vortexStealthActive = false;

            var playerRect = new Rectangle(
                (int)(player.position.X + player.velocity.X * 0.5f - 4),
                (int)(player.position.Y + player.velocity.Y * 0.5f - 4),
                player.width + 8,
                player.height + 8);

            var npcs = Main.npc
                .Where(x => x.active && !x.dontTakeDamage && !x.friendly && x.immune[player.whoAmI] <= 0)
                .Where(x => playerRect.Intersects(x.getRect()) && (x.noTileCollide || player.CanHit(x)));

            foreach (NPC npc in npcs)
            {
                int damage = 0; shield.GetWeaponDamage(player, ref damage);
                float knockBack = shield.item.knockBack;
                bool crit = Main.rand.Next(100) < (shield.item.crit + player.meleeCrit);

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
            // Holding the titan shield

            var shield = player.HeldItem?.modItem as TitanShield;
            if (shield == null)
            {
                if (player.controlUseItem && titanShieldCoolDown == 0) // Clicking
                {
                    player.direction = (Main.MouseWorld - player.Center).X > 0 ? +1 : -1;
                    titanShieldDashing = TitanShield.DashTime * player.direction;
                    Main.PlaySound(SoundID.Item15, player.Center);
                }

                if (titanShieldDashing == 0) titanShieldLastExplosion = 0; // Reset cooldown
                else TitanShieldDash(shield); // Execute dash
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
            if (hitDirection != player.direction && player.HeldItem?.type == mod.ItemType<TitanShield>())
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
