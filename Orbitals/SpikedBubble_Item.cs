using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SpikedBubble_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Bubble");
            Tooltip.SetDefault(
                "The bubble slightly raises damage and defense\n" +
                "Enemies are repelled and damaged\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Burbuja Claveta");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "La burbuja aumenta tu daño y defensa\nRepele y daña enemigos\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Колючий Пузырь");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Пузырь немного увеличивает урон и защиту\nВраги отталкиваются и получают урон\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "尖刺泡泡");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "尖刺泡泡会略微增加伤害及防御\n碰到泡泡的敌人会受到排斥\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.SpikedBubble;
            Duration = 30 * 60;
            Amount = 1;

            Item.width = 36;
            Item.height = 36;
            Item.damage = 45;
            Item.knockBack = 4f;
            Item.mana = 60;
            Item.rare = 6;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.useStyle = 2;
            Item.useTime = 20;
            Item.useAnimation = Item.useTime;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(Mod.Find<ModItem><Bubble_Item>().Type);
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddIngredient(ItemID.PinkGel, 20);
            recipe.AddIngredient(ItemID.LifeFruit);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
