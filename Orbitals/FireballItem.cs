using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    class FireballItem : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Tooltip.SetDefault(
                "Burn nearby enemies\nRight-Click after summoning for a fire burst\n" +
                "Aligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Bola de Fuego");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "Quema a los enemigos cercanos\nHaz Click Derecho tras invocarla para una pequeña explosión\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Огненный Шар");
            Tooltip.AddTranslation(GameCulture.Russian,
                "Поджигает ближайших врагов\nПКМ после вызова для всплеска огня\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "火球术");
			Tooltip.AddTranslation(GameCulture.Chinese,
                "使附近的敌人燃烧\n右键使火球爆炸\n" +
                "更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Fireball;
            duration = 20 * 60;
            amount = 1;
            specialType = SpecialType.RightClick;

            item.width = 30;
            item.height = 30;
            item.damage = 50;
            item.knockBack = 3f;
            item.mana = 20;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = 5;
            item.useTime = 50;
            item.useAnimation = item.useTime;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.TatteredCloth, 1);
            recipe.AddTile(TileID.Books);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
