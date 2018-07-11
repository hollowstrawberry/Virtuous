using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    public struct GobblerStoredItem
    {
        public int type;
        public byte prefix;

        public GobblerStoredItem(int type, byte prefix)
        {
            this.type = type;
            this.prefix = prefix;
        }

        public Item MakeItem()
        {
            var item = new Item();
            item.SetDefaults(type);
            item.prefix = prefix;
            return item;
        }
    }


    public class TheGobbler : ModItem
    {
        public const int StorageCapacity = 100;
        public const int BaseDamage = 10;
        public const float BaseKnockBack = 1f;

        private const int AttractRange = 250;
        private const int StoreRange = 15;

        private static readonly string[] TooltipsToRemove = new[] {
            "Favorite",
            "Damage",
            "CritChance",
            "Speed",
            "Knockback",
        };




        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault(""); // Added in ModifyTooltips

            DisplayName.SetDefault("The Gobbler");
            DisplayName.AddTranslation(GameCulture.Spanish, "Engullidor");
            //DisplayName.AddTranslation(GameCulture.Russian, "");
            DisplayName.AddTranslation(GameCulture.Chinese, "吞噬者");
        }


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

            // Replaced in CanUseItem
            item.useTime = 1;
            item.useAnimation = 60;
        }


        public override bool AltFunctionUse(Player player) => true;


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(Main.rand.NextFloat(0, 2), Main.rand.NextFloat(-8, -6)); // Shakes
        }


        public override bool CanUseItem(Player player)
        {
            // Right click
            if (player.altFunctionUse == 2)
            {
                item.useTime = 1; // Shooting code runs every tick, but the animation is longer so I can control what's happening
                item.useAnimation = 30; // About right for the looping sound
            }
            // Left click
            else
            {
                item.useTime = 1;
                item.useAnimation = 20;

                var modPlayer = player.GetModPlayer<VirtuousPlayer>();
                if (modPlayer.GobblerStorage.Count > 0 && ConsumeChance(modPlayer.GobblerStorage.First().MakeItem()) == 0f)
                {
                    item.useAnimation = 15; // An exception: Shoots faster with endless quiver or musket pouch
                }
            }

            return base.CanUseItem(player);
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string descriptionText;
            if (Language.ActiveCulture == GameCulture.Spanish)
            {
                descriptionText =
                    $"Click Derecho para succionar objetos en el suelo, Click Izquierdo para dispararlos\n" +
                    $"Presiona Tirar Objeto{" (con el arma en favoritos)".If(!item.favorited)} para vaciar el arma\n" +
                    $"El daño, propiedades y comportamiento del proyectil dependerá según el objeto que dispares\n" +
                    $"Los objetos no-consumibles siempre serán recuperados una vez disparados";
            }
            else
            {
                descriptionText =
                    $"Right Click to suck items from the ground, Left Click to shoot them\n" +
                    $"Press Throw{" while favorited".If(!item.favorited)} to release the storage\n" +
                    $"Projectile damage, properties and behavior vary on the item\n" +
                    $"Non-consumable items are never lost and will drop after use";
            }


            var player = Main.player[item.owner];
            var modPlayer = player.GetModPlayer<VirtuousPlayer>();

            Item storedItem = modPlayer.GobblerStorage.Count > 0 
                ? modPlayer.GobblerStorage.First().MakeItem()
                : null;


            var nextItemTooltip = new StringBuilder();

            if (storedItem == null)
            {
                nextItemTooltip.Append("Variable damage\nVariable knockback");
            }
            else
            {
                int damage = ProjGobblerItem.ShotDamage(storedItem, player);
                float knockBack = ProjGobblerItem.ShotKnockBack(storedItem, player);
                int preserveChance = 100 - (int)(ConsumeChance(storedItem) * 100);


                nextItemTooltip.Append($"Next item: {storedItem.Name}");
                if      (storedItem.ammo == AmmoID.Bullet) nextItemTooltip.Append(" (The WHOLE bullet)");
                else if (storedItem.ammo == AmmoID.Rocket) nextItemTooltip.Append(" (...Non-ignited)");

                nextItemTooltip.Append($"\n   Damage: {damage} ");
                if      (storedItem.melee)  nextItemTooltip.Append("(melee)");
                else if (storedItem.ranged) nextItemTooltip.Append("(ranged)");
                else if (storedItem.magic)  nextItemTooltip.Append("(magic)");
                else if (storedItem.summon) nextItemTooltip.Append("(summon)");
                else if (storedItem.thrown) nextItemTooltip.Append("(thrown)");

                nextItemTooltip.Append($"\n   Knockback: {knockBack.ToString("0.0")} ");
                if      (knockBack <= 0.0f)  nextItemTooltip.Append("(None)");
                else if (knockBack <= 1.5f)  nextItemTooltip.Append("(Extremely weak)");
                else if (knockBack <= 3.0f)  nextItemTooltip.Append("(Very weak)");
                else if (knockBack <= 4.0f)  nextItemTooltip.Append("(Weak)");
                else if (knockBack <= 6.0f)  nextItemTooltip.Append("(Average)");
                else if (knockBack <= 7.0f)  nextItemTooltip.Append("(Strong)");
                else if (knockBack <= 9.0f)  nextItemTooltip.Append("(Very strong)");
                else if (knockBack <= 11.0f) nextItemTooltip.Append("(Extremely strong)");
                else                         nextItemTooltip.Append("(Insane)");

                nextItemTooltip.Append($"\n   Preserve chance: {preserveChance}%");
            }


            var customTooltips = new List<TooltipLine>();

            string capacityTooltip = $"Current capacity: {modPlayer.GobblerStorage.Count}/{StorageCapacity}";
            customTooltips.Add(new TooltipLine(mod, "GobblerCapacity", capacityTooltip));
            customTooltips.Add(new TooltipLine(mod, "GobblerItem", nextItemTooltip.ToString()));
            customTooltips.Add(new TooltipLine(mod, "GobblerTooltip", descriptionText));

            tooltips.RemoveAll(line => line.mod == "Terraria" && TooltipsToRemove.Any(x => line.Name == x));
            tooltips.InsertRange(1, customTooltips);
        }


        public static float ConsumeChance(Item item) // The chance of the given item not being preserved when shot
        {
            if (item.type == ItemID.EndlessMusketPouch || item.type == ItemID.EndlessQuiver) return 0f;

            float chance; // Chance to consume item depends on its rarity and other factors

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

            if (item.questItem || item.expert || item.expertOnly) chance *= 0.5f; // Expert or quest
            if (item.accessory || item.defense > 0 || item.vanity || item.damage > 0) chance *= 0.5f; // Weapon or equipable
            if (item.type == ItemID.Arkhalis) chance *= 0.5f;

            return chance;
        }


        public bool IsGobblableItem(Item item) // Whether the item can be sucked
        {
            return item.active && item.type != mod.ItemType<TheGobbler>()
                && item.type != ItemID.CopperCoin && item.type != ItemID.SilverCoin
                && !ItemID.Sets.NebulaPickup[item.type];
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var modPlayer = player.GetModPlayer<VirtuousPlayer>();

            position = player.Center + (Main.MouseWorld - player.Center).OfLength(item.width); // Tip of the nozzle
            position += position.Perpendicular(10); // Moves it up to be centered

            if (!Collision.CanHit(player.Center, 0, 0, position, 0, 0)) position = player.Center; // So it doesn't shoot through walls


            // Right click, absorb
            if (player.altFunctionUse == 2)
            {
                if (player.itemAnimation == 1) // Resets the animation so it doesn't return to resting position
                {
                    player.itemAnimation = item.useAnimation;
                }

                if (PlayerInput.Triggers.JustReleased.MouseRight) // Stops the item use immediately
                {
                    player.itemAnimation = 0;

                    foreach (var item in Main.item.Where(x => x.active))
                    {
                        item.GetGlobalItem<VirtuousItem>().beingGobbled = false; // Makes items able to be picked up again
                    }

                    return false;
                }


                if (player.itemAnimation == item.useAnimation - 1)
                {
                    Main.PlaySound(SoundID.Item22, position); // Once at the beginning of the animation
                }


                bool sucked = false; // Whether an item has been sucked in this tick

                foreach (Item item in Main.item)
                {
                    if (IsGobblableItem(item))
                    {
                        if (item.WithinRange(position, AttractRange))
                        {
                            item.GetGlobalItem<VirtuousItem>().beingGobbled = true; // So it can't be picked up
                            item.velocity -= (item.Center - position).OfLength(0.5f); // Drawn toward the nozzle

                            if (item.WithinRange(position, StoreRange))
                            {
                                while (item.stack > 0 && modPlayer.GobblerStorage.Count < StorageCapacity)
                                {
                                    if (!sucked)
                                    {
                                        sucked = true;
                                        Main.PlaySound(SoundID.Item3, position);
                                    }

                                    modPlayer.GobblerStorage.Add(new GobblerStoredItem(item.type, item.prefix));

                                    if (--item.stack == 0)
                                    {
                                        item.active = false;
                                    }
                                }
                            }

                            if (Main.netMode == NetmodeID.MultiplayerClient) // Syncs to multiplayer
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
                            }
                        }
                    }
                }
            }
            // Left click, shoot
            else
            {
                if (player.itemAnimation == item.useAnimation - 1) // Once every animation
                {
                    if (modPlayer.GobblerStorage.Count == 0)
                    {
                        Main.PlaySound(SoundID.Item23, position);
                    }
                    else
                    {
                        Main.PlaySound(SoundID.Item5, position);

                        var gobblerItem = modPlayer.GobblerStorage.First();
                        var item = gobblerItem.MakeItem();
                        bool consume = Main.rand.NextFloat() < ConsumeChance(item);

                        if (item.ammo == AmmoID.Arrow) // Arrows are shot just fine
                        {
                            var proj = Projectile.NewProjectileDirect(
                                position, new Vector2(speedX, speedY), item.shoot,
                                ProjGobblerItem.ShotDamage(item, player), ProjGobblerItem.ShotKnockBack(item, player),
                                player.whoAmI);
                            proj.noDropItem = true;
                        }
                        else // Shooting the custom projectile that takes the form of any item
                        {
                            if (gobblerItem.type == ItemID.EndlessMusketPouch) gobblerItem.type = ItemID.MusketBall; // Exception

                            var proj = Projectile.NewProjectileDirect(
                                position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                            var gobblerProj = proj.modProjectile as ProjGobblerItem;
                            gobblerProj.Consumed = consume;
                            gobblerProj.GobblerItem = gobblerItem;


                            if (gobblerItem.type == ItemID.Arkhalis) // Ridiculous effect for ridiculous weapon
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    proj = Projectile.NewProjectileDirect(
                                        position, new Vector2(speedX, speedY) * Main.rand.NextFloat(),
                                        type, damage, knockBack, player.whoAmI);
                                    gobblerProj = proj.modProjectile as ProjGobblerItem;
                                    gobblerProj.Consumed = false;
                                    gobblerProj.GobblerItem = gobblerItem;
                                }
                            }
                        }

                        if (consume) modPlayer.GobblerStorage.RemoveAt(0);
                    }
                }
            }

            return false;
        }
    }
}
