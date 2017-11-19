using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class SacDagger_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Daggers");
            Tooltip.SetDefault("\"Feed them\"\nThe daggers drain your life, but heal you when harming an enemy\nUse again after summoning to spin and reset duration\nAligns with either magic or melee users");
        }

        private const int ManaCost = 50;

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 16;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 180;
            item.crit = 0;
            item.knockBack = 5f;
            item.shoot = 1;
            item.mana = ManaCost; //Overwritten by CanUseItem
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 8;
            item.value = Item.sellPrice(0, 40, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.SacDagger;
            orbitalItem.duration = 20 * 60 + SacDagger_Proj.DyingTime;
            orbitalItem.amount = 2;
            orbitalItem.specialFunctionType = OrbitalItem.SpecialReuse; //This makes the orbital's special function activate after using the item again
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            item.mana = ManaCost; //So it always displays the full mana cost

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.GetWeaponDamage(item, player, ref damage); //Base method
        }

        public override bool CanUseItem(Player player)
        {
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();
            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();

            if (orbitalPlayer.active[orbitalItem.type]) //If there is a dagger active
            {
                item.mana = (int)Math.Ceiling((decimal)(ManaCost / 5)); 
            }
            else
            {
                item.mana = ManaCost;
            }

            return orbitalItem.CanUseItem(item, player); //Base method
        }
    }
}
