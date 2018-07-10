using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Bullseye_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullseye");
            Tooltip.SetDefault(
                "\"Love at first sight\"\nShoot through the magical sight for double ranged damage\n" +
                "Other ranged shots are weaker");

            DisplayName.AddTranslation(GameCulture.Spanish, "Disparo Certero");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "\"Amor a primera vista\"\nDispara por la mira mágica para doble daño a distancia\n" +
                "Otros daños a distancia serán más débiles");

            DisplayName.AddTranslation(GameCulture.Russian, "Бычий Глаз");
            Tooltip.AddTranslation(GameCulture.Russian,
                "\"Любовь с первого взгляда\"\nУдваивает дальний урон, если стрелять через прицел\n" +
                "Остальные снаряды будут слабее");

            DisplayName.AddTranslation(GameCulture.Chinese, "魔法靶眼");
            Tooltip.AddTranslation(GameCulture.Chinese,
	    	    "\"一见钟情\"\n过魔法标靶会获得两倍远程伤害\n从其他方向射击伤害将会削弱");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Bullseye;
            duration = 60 * 60;
            amount = 1;

            item.width = 30;
            item.height = 30;
            item.mana = 100;
            item.knockBack = 2.4f;
            item.rare = 8;
            item.value = Item.sellPrice(0, 2, 0, 0);
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RifleScope);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);
            recipe.AddIngredient(ItemID.Amber, 5);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
