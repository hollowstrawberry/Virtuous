using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    class Fireball_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Tooltip.SetDefault(
                "Burns nearby enemies\nRight-Click after summoning for a fire burst\n" +
                "Aligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Bola de Fuego");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Quema a los enemigos cercanos\nHaz Click Derecho tras invocarla para una pequeña explosión\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Огненный Шар");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Поджигает ближайших врагов\nПКМ после вызова для всплеска огня\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "火球术");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "使附近的敌人燃烧\n右键使火球爆炸\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.Fireball;
            Duration = 25 * 60;
            Amount = 1;
            Special = SpecialType.RightClick;

            Item.width = 30;
            Item.height = 30;
            Item.damage = 50;
            Item.knockBack = 3f;
            Item.mana = 20;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = 5;
            Item.useTime = 50;
            Item.useAnimation = Item.useTime;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.TatteredCloth, 1);
            recipe.AddTile(TileID.Books);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
