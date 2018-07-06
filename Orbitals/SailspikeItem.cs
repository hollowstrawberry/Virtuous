using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SailspikeItem : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
            Tooltip.SetDefault("Summons a spike for a short time\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Picabichos");
            Tooltip.AddTranslation(GameCulture.Spanish, "\nInvoca una pica por unos segundos\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Парящий Шип");
            Tooltip.AddTranslation(GameCulture.Russian, "Призывает временный шип\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "飞梭尖刺");
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤持续短时间的尖刺\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Sailspike;
            duration = 5 * 60;
            amount = 1;

            item.width = 30;
            item.height = 30;
            item.damage = 15;
            item.knockBack = 1f;
            item.mana = 15;
            item.rare = 3;
            item.value = Item.sellPrice(0, 2, 0, 0);
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 15);
            recipe.AddIngredient(ItemID.Silk, 2);
            recipe.AddIngredient(ItemID.Star, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
