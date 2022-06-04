using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
        public List<GobblerStoredItem> GobblerStorage = new(TheGobbler.StorageCapacity);

        /// <summary>Time left and direction of the dash. 0 is inactive.</summary>
        public int titanShieldDashing = 0;

        /// <summary>Time left until the dash can be used again.</summary>
        public int titanShieldCoolDown = TitanShield.CoolDown;

        /// <summary>Point in titanShieldDashing when an explosion was last spawned. 0 is none.</summary>
        public int titanShieldLastExplosion = 0;

        public bool accessoryAstroBoots = false;
        public bool accessoryArchangel = false;




        // General

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) // Held item visuals
        {
            if (Player.HeldItem.type == Mod.Find<ModItem>(nameof(EtherSlit)).Type)
            {
                // I had to make a few eye-guesses for the hand position so that it's centered more or less perfectly
                Vector2 handPosition = Main.OffsetsPlayerOnhand[Player.bodyFrame.Y / 56] * 2f; // Vanilla code
                if (Player.direction == -1) handPosition.X = Player.bodyFrame.Width - handPosition.X - 2; // Facing left
                if (Player.gravDir == -1) handPosition.Y = Player.bodyFrame.Height - handPosition.Y; // Upside down
                handPosition -= new Vector2(Player.bodyFrame.Width - Player.width + 8, Player.bodyFrame.Height - 30f) / 2f;

                Vector2 position = Player.position + handPosition;

                for (int i = 0; i < 2; i++)
                {
                    var dust = Dust.NewDustDirect(
                        position, 0, 0, Type: DustID.DungeonSpirit, SpeedX: 2f * Player.direction, Alpha: 150, newColor: Color.Black, Scale: 0.5f);
                    dust.velocity = Player.velocity; // So that it seems to follow the player
                    dust.noGravity = true;
                    dust.fadeIn = 1f;

                    if (Main.rand.NextBool(2))
                    {
                        dust.position += new Vector2(Main.rand.NextFloat(-3, +3), Main.rand.NextFloat(-3, +3));
                        dust.scale += Main.rand.NextFloat();
                    }
                }
            }

            else if (Player.HeldItem.type == Mod.Find<ModItem>(nameof(FlurryNova)).Type)
            {
                Player.handon = (sbyte)EquipLoader.GetEquipSlot(Mod, "FlurryNova", EquipType.HandsOn);
                Player.handoff = (sbyte)EquipLoader.GetEquipSlot(Mod, "FlurryNova", EquipType.HandsOff);
                //drawInfo.HandOnShader = 0;
                //drawInfo.HandOffShader = 0;
            }

            else if (Player.HeldItem.type == Mod.Find<ModItem>(nameof(TitanShield)).Type)
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

                Lighting.AddLight(Player.Center + new Vector2(10 * Player.direction, 0), r, g, b);

                Player.shield = (sbyte)EquipLoader.GetEquipSlot(Mod, "TitanShield", EquipType.Shield);
                //drawInfo.shieldShader = 0;
            }
        }




        // The Gobbler

        public override void PreUpdate()
        {
            if (Player.HeldItem.type == Mod.Find<ModItem>(nameof(TheGobbler)).Type)
            {
                if (Player.controlThrow && GobblerStorage.Count > 0) // Release storage
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

                            int itemIndex = Item.NewItem(null, Player.Center, storedItem.type, stackSize, false, storedItem.prefix);
                            if (Main.netMode == NetmodeID.MultiplayerClient) // Syncs to multiplayer
                            {
                                NetMessage.SendData(MessageID.SyncItem, number: itemIndex);
                            }
                        }

                    }

                    SoundEngine.PlaySound(SoundID.Item3, Player.Center);
                    GobblerStorage.Clear();
                }
            }
        }


        public override void SaveData(TagCompound tag)
        {
            tag = new TagCompound {
                { GobblerItemTypesKey, GobblerStorage.Select(x => x.type).ToArray() },
                { GobblerItemPrefixesKey, GobblerStorage.Select(x => x.prefix).ToArray() }
            };
        }


        public override void LoadData(TagCompound tag) // Grabs the gobbler storage from the savefile
        {
            GobblerStorage.Clear();

            var itemTypes = Array.Empty<int>();
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

            Player.vortexStealthActive = false;

            var playerRect = new Rectangle(
                (int)(Player.position.X + Player.velocity.X * 0.5f - 4),
                (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4),
                Player.width + 8,
                Player.height + 8);

            var npcs = Main.npc
                .Where(x => x.active && !x.dontTakeDamage && !x.friendly && x.immune[Player.whoAmI] <= 0)
                .Where(x => playerRect.Intersects(x.getRect()) && (x.noTileCollide || Player.CanHit(x)));

            foreach (NPC npc in npcs)
            {
                int damage = 0; shield.GetWeaponDamage(Player, ref damage);
                float knockBack = shield.Item.knockBack;
                bool crit = Main.rand.Next(100) < (shield.Item.crit + Player.GetCritChance<MeleeDamageClass>());

                // Damages the enemy
                Player.ApplyDamageToNPC(npc, damage, knockBack, Player.direction, crit);
                npc.immune[Player.whoAmI] = 5;

                // Area of effect, with cooldown
                if (titanShieldLastExplosion == 0 || titanShieldLastExplosion - Math.Abs(titanShieldDashing) >= TitanShield.ExplosionDelay)
                {
                    titanShieldLastExplosion = Math.Abs(titanShieldDashing);
                    SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                    var proj = Projectile.NewProjectileDirect(null,
                        npc.Center, Vector2.Zero, Mod.Find<ModProjectile>(nameof(ProjTitanAOE)).Type, damage, knockBack / 2, Player.whoAmI);
                    var modProj = proj.ModProjectile as ProjTitanAOE;
                    modProj.Crit = crit;
                }

                // Prevents player from taking damage
                Player.immune = true;
                Player.immuneNoBlink = true;
                Player.immuneTime = Math.Min(10, Math.Abs(titanShieldDashing)); // Immune time caps at the time left
            }

            // Dust
            for (int i = 0; i < 5; i++)
            {
                Color color = Main.rand.NextBool(2) ? new Color(255, 255, 255) : new Color(255, 255, 255, 0f);
                var dust = Dust.NewDustDirect(
                    Player.position + new Vector2(Main.rand.NextFloat(-5, +5), Main.rand.NextFloat(-5, +5)), Player.width, Player.height,
                    DustID.Cloud, 0f, 0f, /*Alpha*/0, color, /*Scale*/Main.rand.NextFloat(1, 2));
                dust.velocity *= 0.2f;
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }

            if (Math.Abs(titanShieldDashing) > TitanShield.DashTime - 8)
            {
                Player.velocity.X = 16.0f * dashDirection; // First 8 ticks maintains top speed in the direction of the dash
            }
            else if (dashDirection == +1 && Player.velocity.X <= 0 || dashDirection == -1 && Player.velocity.X >= 0)
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

            var shield = Player.HeldItem?.ModItem as TitanShield;
            if (shield == null)
            {
                if (Player.controlUseItem && titanShieldCoolDown == 0) // Clicking
                {
                    Player.direction = (Main.MouseWorld - Player.Center).X > 0 ? +1 : -1;
                    titanShieldDashing = TitanShield.DashTime * Player.direction;
                    SoundEngine.PlaySound(SoundID.Item15, Player.Center);
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
            if (hitDirection != Player.direction && Player.HeldItem?.type == Mod.Find<ModItem>(nameof(TitanShield)).Type)
            {
                damage = (int)(damage * (1 - TitanShield.DamageReduction));
                int dustAmount = Main.rand.Next(15, 21);
                for (int i = 0; i < dustAmount; i++)
                {
                    var dust = Dust.NewDustDirect(
                        Player.Center + new Vector2(10 * Player.direction, 0), 0, 0, DustID.DungeonSpirit, 0f, 0f, /*Alpha*/100,
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
