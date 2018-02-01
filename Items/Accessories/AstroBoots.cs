using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Virtuous.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings, EquipType.Shoes)]
    public class AstroBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astro Boots");
            Tooltip.SetDefault("Fantastic flight and slow fall\nExtreme mobility on all surfaces\nTemporary immunity to lava");
            DisplayName.AddTranslation(GameCulture.Spanish, "Astrobotas");
            Tooltip.AddTranslation(GameCulture.Spanish, "Vuelo y velocidad excelentes\nMobilidad extrema en toda superficie\nInmunidad temporal a la lava");
            DisplayName.AddTranslation(GameCulture.Russian, "Космич ски Ботинки");
            Tooltip.AddTranslation(GameCulture.Russian, "Фантастич ский полёт и м ко п из мл ни \nМобильность на вс пов ност \nВ м нна н у звимость к лав ");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = Item.sellPrice(0, 50, 0, 0);
            item.rare = 9;
            item.accessory = true;
        }
        
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            VirtuousPlayer modPlayer = player.GetModPlayer<VirtuousPlayer>();
            player.wingTimeMax = 180;
            player.accRunSpeed = 13.0f;
            player.runAcceleration += 0.07f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax = player.lavaMax + 420;
            modPlayer.accessoryAstroBoots = !hideVisual;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 11.0f;
            acceleration *= 2.0f;
        }
        
        /*public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse) Dust.NewDust(player.position, player.width, player.height, 107, 0, 0, 0, Color.Cyan);
            base.WingUpdate(player, inUse);
            return false;
        }*/

        public override void AddRecipes()
        {
            //ModRecipe recipe = new ModRecipe(mod);
            //recipe.AddIngredient(null, "TerraWalkers");
            //recipe.AddRecipeGroup("Virtuous:Wings");
            //recipe.AddIngredient(ItemID.Ectoplasm, 20);
            //recipe.AddIngredient(ItemID.LifeFruit, 5);
            //recipe.AddIngredient(ItemID.MartianConduitPlating, 100);
            //recipe.AddTile(TileID.MythrilAnvil);
            //recipe.SetResult(this);
            //recipe.AddRecipe();
        }
        
    }
}
