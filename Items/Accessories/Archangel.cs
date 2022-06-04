using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class Archangel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archangel");
            Tooltip.SetDefault(
                "TEST ITEM\nSublime flight and speed!\nExtreme mobility on all surfaces\nTemporary immunity to lava");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Arcángel");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "TEST ITEM\nVuelo y velocidad sublimes\nMobilidad extrema en toda superficie\nInmunidad temoral a la lava");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Архангел");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "TEST ITEM\nандиозный полёт и ско ость!\nМобильность на вс пов ност\nВ м нна н у звимость к лав");
        }


        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.value = Item.sellPrice(1, 0, 0, 0);
            Item.rare = 11;
            Item.accessory = true;
            Item.expert = true;
        }
        

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<VirtuousPlayer>();
            modPlayer.accessoryArchangel = !hideVisual;

            player.wingTimeMax = 300;
            player.accRunSpeed = 16f;
            player.runAcceleration += 0.2f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 900;
        }


        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }


        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 15.0f;
            acceleration *= 3.0f;
        }


        /*public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse) Dust.NewDust(player.position, player.width, player.height, 107, 0, 0, 0, Color.Green);
            base.WingUpdate(player, inUse);
            return false;
        }*/


        public override void AddRecipes()
        {
            //var recipe = new ModRecipe(mod);
            //recipe.AddIngredient(null, "AstroBoots");
            //recipe.AddRecipeGroup("Virtuous:CelestialWings");
            //recipe.AddIngredient(ItemID.FishronWings);
            //recipe.AddIngredient(ItemID.BetsyWings);
            //recipe.AddIngredient(ItemID.GravityGlobe);
            //recipe.AddTile(TileID.LunarCraftingStation);
            //recipe.SetResult(this);
            //recipe.AddRecipe();
        }
    }
}
