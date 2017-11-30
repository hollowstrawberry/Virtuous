using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class SpiralSword_Item : OrbitalItem
    {
        private const int ManaCost = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blades of Virtue");
            Tooltip.SetDefault("Ethereal swords protect you and raise all damage\nRight Click after summoning for an active attack\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SpiralSword;
            duration = 40 * 60;
            amount = 8;
            specialFunctionType = OrbitalItem.SpecialRightClick; //Makes the orbital's special function activate with right click

            item.width = 30;
            item.height = 30;
            item.damage = 250;
            item.knockBack = 5f;
            item.mana = ManaCost; //Overwritten by CanUseItem
            item.rare = 11;
            item.value = Item.sellPrice(1, 0, 0, 0);
            item.autoReuse = true;
            item.useStyle = 4;
            item.useTime = 35;
            item.useAnimation = item.useTime;
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            item.mana = ManaCost; //So it always displays the full mana cost

            base.GetWeaponDamage(player, ref damage);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2) //Left click
            {
                item.mana = ManaCost;
            }
            else //Right click
            {
                item.mana = (int)Math.Ceiling(ManaCost / 10f);
            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
