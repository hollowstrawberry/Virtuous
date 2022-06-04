using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SpiralSword_Item : OrbitalItem
    {
        private const int ManaCost = 100;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blades of Virtue");
            Tooltip.SetDefault(
                "Ethereal swords protect you and raise all damage\nRight Click after summoning for an active attack\n" +
                "Aligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Hojas Virtud");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Las espadas etéreas te protejerán y aumentarán tu daño\nHaz Click Derecho tras invocarlas para realizar un ataque\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Клинки Доблести");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Магические клинки защищают вас и увеличивают урон\nПКМ после вызова - активная атака\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "空灵圣剑");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "空灵圣剑将保护你,并增加所有伤害\n召唤后右键可主动攻击\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.SpiralSword;
            Duration = 40 * 60;
            Amount = 8;
            Special = SpecialType.RightClick;

            Item.width = 30;
            Item.height = 30;
            Item.damage = 250;
            Item.knockBack = 5f;
            Item.mana = ManaCost; // Overwritten by CanUseItem
            Item.rare = 11;
            Item.value = Item.sellPrice(1, 0, 0, 0);
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.useStyle = 4;
            Item.useTime = 35;
            Item.useAnimation = Item.useTime;
        }


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            Item.mana = ManaCost; // So it always displays the full mana cost

            base.GetWeaponDamage(player, ref damage);
        }


        public override bool CanUseItem(Player player)
        {
            Item.mana = player.altFunctionUse == 2
                ? (int)Math.Ceiling(ManaCost / 10f) // Right click
                : ManaCost; // Left click

            return base.CanUseItem(player);
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
