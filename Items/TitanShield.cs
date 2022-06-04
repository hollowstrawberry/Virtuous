using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Items
{
    [AutoloadEquip(EquipType.Shield)]
    public class TitanShield : ModItem
    {
        public const float DamageReduction = 0.2f; // This gets multiplied at the end of the player damage formula
        public const int ExplosionDelay = 10; // Ticks before consecutive explosions can occur
        public const int AoEInvincibility = 8; // How many invincibility frames enemies take when being hit by explosions
        public const int DashTime = 30; // Time spent dashing
        public const int CoolDown = 20; // Time before you can dash again


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Shield");
            Tooltip.SetDefault(
                "Use to ram your enemies\nBase damage scales with defense\n" +
                "While held: Reduces 20% of damage taken from the front");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Escudo Titán");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Atropella enemigos\nEl daño aumenta con tu defensa\n" +
                "Al sostenerlo, reduce en 20% el daño recibido por el frente");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Щит Титана");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Пробивайтесь через врагов\nБазовый урон увеличивается с защитой\n" +
                "В руках: Уменьшает получаемый урон с тыла на 20%");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "泰坦圣盾");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "撞飞你的敌人\n基础伤害与防御力成比例\n持有时:减少20%正面伤害");
        }


        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.damage = 100;
            Item.crit = 10;
            Item.knockBack = 12f;
            Item.melee = true;
            Item.rare = 10;
            Item.value = Item.sellPrice(0, 50, 0, 0);
        }


        // Most of the code is in VirtuousPlayer, as this item has no useStyle


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            // Doubles every 50 defense
            damage = (int)(Item.damage * player.meleeDamage * Math.Pow(2, player.statDefense / 50f));
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string nonEquipableText;
            if (Language.ActiveCulture == GameCulture.FromCultureName(GameCulture.CultureName.Spanish))
                nonEquipableText = "Arma no-equipable";
            else
                nonEquipableText = "Weapon, non-equipable";

            int insertIndex = tooltips.IndexOf(tooltips.FirstOrDefault(x => x.mod == "Terraria" && x.Name == "Damage"));
            tooltips.Insert(Math.Max(1, insertIndex), new TooltipLine(Mod, "NonEquipable", nonEquipableText));


            TooltipLine knockbackLine = tooltips.FirstOrDefault(x => x.mod == "Terraria" && x.Name == "Knockback");
            if (knockbackLine != null && knockbackLine.Text.ToLower().Contains("knockback"))
            {
                knockbackLine.Text = "Titanic knockback";
            }
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.PaladinsShield);
            recipe.AddIngredient(ItemID.PaladinsHammer);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
