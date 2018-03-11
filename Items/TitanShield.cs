using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    [AutoloadEquip(EquipType.Shield)]
    public class TitanShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Shield");
            Tooltip.SetDefault("Use to ram your enemies\nBase damage scales with defense\nWhile held: Reduces 20% of damage taken from the front");
            DisplayName.AddTranslation(GameCulture.Spanish, "Escudo Titán");
            Tooltip.AddTranslation(GameCulture.Spanish, "Atropella enemigos\nEl daño aumenta con tu defensa\nAl sostenerlo, reduce en 20% el daño recibido por el frente");
            DisplayName.AddTranslation(GameCulture.Russian, "Щит Титана");
            Tooltip.AddTranslation(GameCulture.Russian, "Пробивайтесь через врагов\nБазовый урон увеличивается с защитой\nВ руках: Уменьшает получаемый урон с тыла на 20%");
        }

        public const float DamageReduction = 0.2f; //This gets applied multiplicatevely at the end of the damage formula
        public const int   ExplosionDelay = 10; //Ticks before consecutive explosions can ocurr
        public const int   AoEInvincibility = 8; //How many invincibility frames enemies take when being hit by explosions
        public const int   DashTime = 30; //Time spent dashing
        public const int   CoolDown = 20; //Time before you can dash again

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 42;
            item.damage = 100;
            item.crit = 10;
            item.knockBack = 12f;
            item.melee = true;
            item.rare = 10;
            item.value = Item.sellPrice(0, 50, 0, 0);
        }

        //Most of the code is in VirtuousPlayer, as this weapon has no proper useStyle.

        public int GetDamage(Player player)
        {
            return (int)(item.damage * player.meleeDamage * Math.Pow(2, player.statDefense / 50f)); //Doubles every 50 defense
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            damage = GetDamage(player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.mod == "Terraria" && line.Name == "Damage")
                {
                    if (Language.ActiveCulture == GameCulture.Spanish) line.text = $"Arma no-equipable\n{line.text}";
                    else line.text = $"Weapon, non-equipable\n{line.text}"; //Adds a comment above damage
                }

                if (line.mod == "Terraria" && line.Name == "Knockback")
                {
                    if (line.text.Contains("knockback")) line.text = "Titanic knockback"; //Only changes English
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PaladinsShield);
            recipe.AddIngredient(ItemID.PaladinsHammer);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
