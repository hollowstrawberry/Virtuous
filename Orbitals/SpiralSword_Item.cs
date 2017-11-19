using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class SpiralSword_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blades of Virtue");
            Tooltip.SetDefault("Ethereal swords protect you and raise all damage\nRight Click after summoning for an active attack\nAligns with either magic or melee users");
        }


        private const int ManaCost = 100;

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 35;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 200;
            item.crit = 0;
            item.knockBack = 4f;
            item.shoot = 1;
            item.mana = ManaCost; //Overwritten by CanUseItem
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 11;
            item.value = Item.sellPrice(1, 0, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.SpiralSword;
            orbitalItem.duration = 40 * 60 + SpiralSword_Proj.DyingTime;
            orbitalItem.amount = 8;
            orbitalItem.specialFunctionType = OrbitalItem.SpecialRightClick; //Makes the orbital's special function activate with right click
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            item.mana = ManaCost; //So it always displays the full mana cost

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.GetWeaponDamage(item, player, ref damage); //Base method
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

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            return orbitalItem.CanUseItem(item, player); //Base method
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 8);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
