using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Bubble_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
            Tooltip.SetDefault("\"Stay out of my personal space\"\nThe bubble repels enemies and raises defense");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Burbuja");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "\"Espacio personal\"\nRepele enemigos y aumenta la defensa");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Пузырь");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "\"У меня должно быть личное пространство!\"\nОтталкивает врагов и увеличивает защиту");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "泡泡");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "\"远离我的私人空间\"\n泡泡会排斥敌人,增加防御");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.Bubble;
            Duration = 20 * 60;
            Amount = 1;

            Item.width = 32;
            Item.height = 32;
            Item.mana = 40;
            Item.rare = 5;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.useStyle = 2;
            Item.useTime = 20;
            Item.useAnimation = Item.useTime;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.Gel, 100);
            recipe.AddIngredient(ItemID.WaterBucket);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
