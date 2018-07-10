using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SpiralSwordItem : OrbitalItem
    {
        private const int ManaCost = 100;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blades of Virtue");
            Tooltip.SetDefault(
                "Ethereal swords protect you and raise all damage\nRight Click after summoning for an active attack\n" +
                "Aligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Hojas Virtud");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "Las espadas etéreas te protejerán y aumentarán tu daño\nHaz Click Derecho tras invocarlas para realizar un ataque\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Клинки Доблести");
            Tooltip.AddTranslation(GameCulture.Russian,
                "Магические клинки защищают вас и увеличивают урон\nПКМ после вызова - активная атака\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "空灵圣剑");
            Tooltip.AddTranslation(GameCulture.Chinese,
                "空灵圣剑将保护你,并增加所有伤害\n召唤后右键可主动攻击\n" +
                "更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SpiralSword;
            duration = 40 * 60;
            amount = 8;
            specialType = SpecialType.RightClick;

            item.width = 30;
            item.height = 30;
            item.damage = 250;
            item.knockBack = 5f;
            item.mana = ManaCost; // Overwritten by CanUseItem
            item.rare = 11;
            item.value = Item.sellPrice(1, 0, 0, 0);
            item.autoReuse = true;
            item.useTurn = false;
            item.useStyle = 4;
            item.useTime = 35;
            item.useAnimation = item.useTime;
        }


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            item.mana = ManaCost; // So it always displays the full mana cost

            base.GetWeaponDamage(player, ref damage);
        }


        public override bool CanUseItem(Player player)
        {
            item.mana = player.altFunctionUse == 2
                ? (int)Math.Ceiling(ManaCost / 10f) // Right click
                : ManaCost; // Left click

            return base.CanUseItem(player);
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
