using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class BubbleItem : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
            Tooltip.SetDefault("\"Stay out of my personal space\"\nThe bubble repels enemies and raises defense");

            DisplayName.AddTranslation(GameCulture.Spanish, "Burbuja");
            Tooltip.AddTranslation(GameCulture.Spanish, "\"Espacio personal\"\nRepele enemigos y aumenta la defensa");

            DisplayName.AddTranslation(GameCulture.Russian, "Пузырь");
            Tooltip.AddTranslation(GameCulture.Russian, "\"У меня должно быть личное пространство!\"\nОтталкивает врагов и увеличивает защиту");

            DisplayName.AddTranslation(GameCulture.Chinese, "泡泡");
			Tooltip.AddTranslation(GameCulture.Chinese, "\"远离我的私人空间\"\n泡泡会排斥敌人,增加防御");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Bubble;
            duration = 20 * 60;
            amount = 1;

            item.width = 32;
            item.height = 32;
            item.mana = 40;
            item.rare = 5;
            item.value = Item.sellPrice(0, 15, 0, 0);
            item.useStyle = 2;
            item.useTime = 20;
            item.useAnimation = item.useTime;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 100);
            recipe.AddIngredient(ItemID.WaterBucket);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
