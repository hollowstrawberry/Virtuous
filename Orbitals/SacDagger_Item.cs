using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class SacDagger_Item : OrbitalItem
    {
        private static int ManaCost = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Daggers");
            Tooltip.SetDefault("\"Feed them\"\nThe daggers drain your life, but heal you when harming an enemy\nUse again after summoning to spin and reset duration\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SacDagger;
            duration = 20 * 60;
            amount = 2;
            specialFunctionType = OrbitalItem.SpecialReuse; //This makes the orbital's special function activate after using the item again

            item.width = 30;
            item.height = 30;
            item.damage = 180;
            item.knockBack = 5f;
            item.mana = ManaCost; //Overwritten by CanUseItem
            item.rare = 8;
            item.value = Item.sellPrice(0, 40, 0, 0);
            item.autoReuse = true;
            item.useStyle = 4;
            item.useTime = 16;
            item.useAnimation = item.useTime;
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            item.mana = ManaCost; //So it always displays the full mana cost

            base.GetWeaponDamage(player, ref damage);
        }

        public override bool CanUseItem(Player player)
        {
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (orbitalPlayer.active[this.type]) //If there is a dagger active
            {
                item.mana = (int)Math.Ceiling((decimal)(ManaCost / 5)); 
            }
            else
            {
                item.mana = ManaCost;
            }

            return base.CanUseItem(player);
        }
    }
}
