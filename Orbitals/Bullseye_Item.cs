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

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Disparo Certero");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "\"Amor a primera vista\"\nDispara por la mira mágica para doble daño a distancia\n" +
                "Otros daños a distancia serán más débiles");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Бычий Глаз");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "\"Любовь с первого взгляда\"\nУдваивает дальний урон, если стрелять через прицел\n" +
                "Остальные снаряды будут слабее");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "魔法靶眼");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
	    	    "\"一见钟情\"\n过魔法标靶会获得两倍远程伤害\n从其他方向射击伤害将会削弱");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.Bullseye;
            Duration = 60 * 60;
            Amount = 1;

            Item.width = 30;
            Item.height = 30;
            Item.mana = 100;
            Item.knockBack = 2.4f;
            Item.rare = 8;
            Item.value = Item.sellPrice(0, 2, 0, 0);
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.RifleScope);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);
            recipe.AddIngredient(ItemID.Amber, 5);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
