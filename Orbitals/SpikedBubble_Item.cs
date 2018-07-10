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

            DisplayName.AddTranslation(GameCulture.Spanish, "Burbuja Claveta");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "La burbuja aumenta tu daño y defensa\nRepele y daña enemigos\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Колючий Пузырь");
            Tooltip.AddTranslation(GameCulture.Russian,
                "Пузырь немного увеличивает урон и защиту\nВраги отталкиваются и получают урон\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "尖刺泡泡");
            Tooltip.AddTranslation(GameCulture.Chinese,
                "尖刺泡泡会略微增加伤害及防御\n碰到泡泡的敌人会受到排斥\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SpikedBubble;
            duration = 30 * 60;
            amount = 1;

            item.width = 36;
            item.height = 36;
            item.damage = 45;
            item.knockBack = 4f;
            item.mana = 60;
            item.rare = 6;
            item.value = Item.sellPrice(0, 15, 0, 0);
            item.useStyle = 2;
            item.useTime = 20;
            item.useAnimation = item.useTime;
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<Bubble_Item>());
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddIngredient(ItemID.PinkGel, 20);
            recipe.AddIngredient(ItemID.LifeFruit);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
