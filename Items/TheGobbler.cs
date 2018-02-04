using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.GameInput;
using Terraria.ModLoader;
using Virtuous.Projectiles;


namespace Virtuous.Items
{
    public class TheGobbler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Gobbler");
            Tooltip.SetDefault(""); //Added in ModifyTooltips
        }

        //Some constants for easy use
        public const int Empty = 0;
        public const int BaseSlot = 0;
        public const int StorageCapacity = 100;
        public const int BaseDamage = 10;
        public const float BaseKnockBack = 1f;

        public override void SetDefaults()
        {
            item.width = 64;
            item.height = 42;
            item.useStyle = 5;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType<ProjGobblerItem>();
            item.damage = 100;
            item.knockBack = BaseKnockBack;
            item.shootSpeed = 12f;
            item.noMelee = true;
            item.value = Item.sellPrice(1, 0, 0, 0);
            item.rare = 11;

            //Replaced by CanUseItem
            item.useTime = 1;
            item.useAnimation = 60;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) //Right click
            {
                item.useTime = 1; //Code for the gun runs once per tick, but the animation is longer so I can control when and how to do things
                item.useAnimation = 30; //About right for the looping sound
            }
            else //Left click
            {
                item.useTime = 1;
                item.useAnimation = 20;
                if (ConsumeChance(player.GetModPlayer<VirtuousPlayer>().GobblerItem(BaseSlot)) == 0f) item.useAnimation = 15; //Exception. Namely, shoots faster with endless quiver or musket pouch
            }

            return base.CanUseItem(player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[item.owner];
            VirtuousPlayer modPlayer = player.GetModPlayer<VirtuousPlayer>();
            Item storedItem = modPlayer.GobblerItem(BaseSlot);

            int storageFilled = modPlayer.GobblerStorageFilled();
            bool hasItem = storageFilled > 0;

            //Removes the favorite text and critical chance text
            tooltips.RemoveAll(line => line.mod == "Terraria" && (line.Name.StartsWith("Favorite") || line.Name.StartsWith("CritChance")));


            foreach (TooltipLine line in tooltips)
            {
                if (line.mod == "Terraria" && line.Name == "Damage") //Replaces the damage line with capacity, shot item name and shot item damage
                {
                    if (player != null)
                    {
                        int? itemDamage = null;
                        string itemName = null;
                        if(hasItem)
                        {
                            itemDamage = ProjGobblerItem.ShotDamage(storedItem, player);
                            itemName = storedItem.Name;
                        }

                        string damageTypeText = null;
                        if      (storedItem.melee ) damageTypeText = "(melee)";
                        else if (storedItem.ranged) damageTypeText = "(ranged)";
                        else if (storedItem.magic ) damageTypeText = "(magic)";
                        else if (storedItem.summon) damageTypeText = "(summon)";
                        else if (storedItem.thrown) damageTypeText = "(thrown)";


                        line.text = $"Current capacity: {storageFilled}/{StorageCapacity}"; //Capacity

                        if (hasItem)
                        {
                            line.text += $"\nNext item: {itemName}"; //Item name, if there is one
                            if      (storedItem.ammo == AmmoID.Bullet) line.text += " (The WHOLE bullet)";
                            else if (storedItem.ammo == AmmoID.Rocket) line.text += " (...Non-ignited)";
                        }


                        line.text += "\n" + (hasItem ? ($"   Damage: {itemDamage}") : ("Variable damage")); //Item damage
                        if (hasItem) line.text += $" {damageTypeText}"; //Item damage type
                    }
                    else
                    {
                        line.text = "Current capacity: 0/100\nVariable damage"; //Default text if something went wrong
                    }
                }

                else if (line.mod == "Terraria" && line.Name == "Speed") //Replaces the speed line with shot item knockback and consume chance
                {
                    string defaultText = "Variable knockback";

                    if (player != null)
                    {
                        float knockBack = 0;
                        string knockBackText = defaultText;

                        if (hasItem)
                        {
                            knockBack = ProjGobblerItem.ShotKnockBack(storedItem, player);
                            knockBackText = $"   Knockback: {knockBack.ToString("0.0")} (";
                            
                            if      (knockBack <=  0.0f) knockBackText += "None";
                            else if (knockBack <=  1.5f) knockBackText += "Extremely weak";
                            else if (knockBack <=  3.0f) knockBackText += "Very weak";
                            else if (knockBack <=  4.0f) knockBackText += "Weak";
                            else if (knockBack <=  6.0f) knockBackText += "Average";
                            else if (knockBack <=  7.0f) knockBackText += "Strong";
                            else if (knockBack <=  9.0f) knockBackText += "Very strong";
                            else if (knockBack <= 11.0f) knockBackText += "Extremely strong";
                            else                         knockBackText += "Insane";

                            knockBackText += ")";
                        }

                        line.text = knockBackText; //Knockback

                        if (hasItem) line.text += $"\n   Preserve chance: {(100 - (int)(ConsumeChance(storedItem) * 100))}%"; //Consume chance, if any
                    }
                    else
                    {
                        line.text = defaultText; //Default text if something went wrong
                    }
                }

                else if (line.mod == "Terraria" && line.Name == "Knockback") //Replaces the knockback line with use speed, and adds the tooltip below
                {
                    string defaultText = "Average speed";
                    if(hasItem)
                    {
                        line.text = "   "; //Space to align with the other item data
                        if (ConsumeChance(storedItem) == 0f) line.text += "Fast speed"; //Exception
                        else line.text += defaultText;
                    }
                    else line.text = defaultText; //Moves forward to align with the item info

                    //At the end of the tooltip
                    line.text += $"\nRight Click to suck items from the ground, Left Click to shoot them";
                    line.text += $"\nPress Throw{" while favorited".If(!item.favorited)} to release the storage";
                    line.text += $"\nProjectile damage, properties and behavior vary on the item";
                    line.text += $"\nNon-consumable items always drop after being spent, though they may be damaged";
                    line.text += $"\n[Warning]: Experimental technology. May carry unintended and hilarious consequences";
                }
            }
        }

        public static float ConsumeChance(Item item) //The chance of the given item not being preserved when shot
        {
            float chance; //Chance to consume item depends on its rarity and other factors
            switch(item.rare)
            {
                case  2: chance = 4f / 5f; break;
                case  3: chance = 3f / 4f; break;
                case  4: chance = 2f / 3f; break;
                case  5: chance = 1f / 2f; break;
                case  6: chance = 1f / 3f; break;
                case  7: chance = 1f / 4f; break;
                case  8: chance = 1f / 5f; break;
                case  9: chance = 1f / 6f; break;
                case 10: chance = 1f / 7f; break;
                case 11: chance = 1f / 8f; break;
                default: chance = 1f; break;
            }
            if (item.questItem || item.expert || item.expertOnly) chance *= 0.5f; //Expert or quest
            if (item.accessory || item.defense > 0 || item.vanity || item.damage > 0) chance *= 0.5f; //Weapon or equipable

            if (item.type == ItemID.EndlessMusketPouch || item.type == ItemID.EndlessQuiver) chance = 0f;
            if (item.type == ItemID.Arkhalis) chance *= 0.5f;

            return chance;
        }

        public bool IsGobblableItem(Item item) //Whether the item can be sucked
        {
            return (item.active && item.type != mod.ItemType<TheGobbler>() && item.type != ItemID.CopperCoin && item.type != ItemID.SilverCoin && !ItemID.Sets.NebulaPickup[item.type]);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            VirtuousPlayer modPlayer = player.GetModPlayer<VirtuousPlayer>();

            position = player.Center + (Main.MouseWorld - player.Center).OfLength(item.width); //The nozzle
            position += position.Perpendicular(10); //Moves it up to be centered
            if (!Collision.CanHit(player.Center, 0, 0, position, 0, 0)) position = player.Center; //Resets to the player center if the nozzle is unreachable


            if (player.altFunctionUse == 2) //Right click, absorb
            {
                //A little hack to stop the bugged 1-tick delay between consecutive alternate-uses of a weapon
                if (player.itemAnimation == 1) //Resets the animation so it doesn't return to resting position
                {
                    player.itemAnimation = item.useAnimation;
                }
                if (PlayerInput.Triggers.JustReleased.MouseRight) //Stops the item use manually. Has the secondary effect of only sucking while you're actually clicking
                {
                    player.itemAnimation = 0;
                    return false;
                }

                if (player.itemAnimation == item.useAnimation - 1) Main.PlaySound(SoundID.Item22, position); //Once at the beginning of the animation

                bool sucked = false; //Whether an item has been sucked in this tick

                for(int i = 0; i < Main.maxItems; i++) //Cycles through all the items in the world
                {
                    if (Main.item[i].active && IsGobblableItem(Main.item[i])) //Finds a valid item
                    {
                        VirtuousItem targetModItem = Main.item[i].GetGlobalItem<VirtuousItem>();

                        if (Main.item[i].WithinRange(position, 250)) //Within sucking range
                        {
                            Main.item[i].velocity -= (Main.item[i].Center - position).OfLength(0.5f); //Attracts item towards the nozzle
                            targetModItem.beingGobbled = true; //So it can't be picked up

                            if (Main.item[i].WithinRange(position, 15)) //Absorb range
                            {
                                for(int slot = 0; slot < StorageCapacity; slot++) //Cycles through the storage
                                {
                                    while (slot < StorageCapacity && modPlayer.GobblerStorage[slot] == Empty && Main.item[i].stack > 0) //If it finds an empty slot, it starts adding items from the item stack into the storage
                                    {
                                        if(!sucked)
                                        {
                                            sucked = true;
                                            Main.PlaySound(SoundID.Item3, position);
                                        }

                                        modPlayer.GobblerStorage[slot] = Main.item[i].type; //Adds the item to the current storage slot
                                        Main.item[i].stack--; //Reduces the stack of the item
                                        slot++; //Moves a slot forward
                                        if (Main.item[i].stack == 0) //The stack of the item has reached 0
                                        {
                                            Main.item[i].active = false; //Kills it
                                            slot = StorageCapacity; //Breaks the for loop
                                        }
                                    }
                                }
                            }

                            if (Main.netMode == NetmodeID.MultiplayerClient) //Syncs to multiplayer
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Main.item[i].whoAmI);
                            }
                        }
                    }
                }
            }

            else //Left click, shoot
            {
                if (player.itemAnimation == item.useAnimation - 1) //Once every animation
                {
                    if (modPlayer.GobblerStorage[BaseSlot] != Empty) //if there is something in the storage
                    {
                        Item shotItem = modPlayer.GobblerItem(BaseSlot); //For easy access

                        Main.PlaySound(SoundID.Item5, position);

                        bool consume = Tools.RandomFloat() < ConsumeChance(modPlayer.GobblerItem(BaseSlot));

                        if (shotItem.ammo == AmmoID.Arrow)  //Arrows are shot just fine
                        {
                            Projectile newProj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), shotItem.shoot, ProjGobblerItem.ShotDamage(shotItem, player), ProjGobblerItem.ShotKnockBack(shotItem, player), player.whoAmI);
                            newProj.noDropItem = true; //So the arrows don't drop and make a mess...
                        }
                        else //Shooting the custom projectile that takes the form of any item
                        {
                            int itemType = modPlayer.GobblerStorage[BaseSlot];
                            if (itemType == ItemID.EndlessMusketPouch) itemType = ItemID.MusketBall; //Exception

                            Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, consume ? 1f : 0f, itemType); //Shoots the item
                            
                            if(itemType == ItemID.Arkhalis) //Ridiculous effect for ridiculous weapon
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY) * Tools.RandomFloat(), type, damage, knockBack, player.whoAmI, 0f, itemType);
                                }
                            }
                        }

                        if (consume)
                        {
                            modPlayer.GobblerStorage[BaseSlot] = Empty; //Clears the item from the storage

                            for (int slot = 0; slot < StorageCapacity; slot++) //Loops through the storage
                            {
                                //Moves the entire array one slot down
                                if (slot < StorageCapacity - 1 && modPlayer.GobblerStorage[slot + 1] != Empty)
                                {
                                    modPlayer.GobblerStorage[slot] = modPlayer.GobblerStorage[slot + 1];
                                    modPlayer.GobblerStorage[slot + 1] = Empty;
                                }
                                if (modPlayer.GobblerStorage[slot] == Empty) break;
                            }
                        }
                    }
                    else //Nothing in the storage
                    {
                        Main.PlaySound(SoundID.Item23, position);
                    }
                }
            }

            return false; //Doesn't shoot normally
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(Tools.RandomFloat(0, 2), Tools.RandomFloat(-8, -6)); //Shakes
        }
    }
}
