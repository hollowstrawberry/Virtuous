﻿using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings, EquipType.Shoes)]
    public class AstroBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astro Boots");
            Tooltip.SetDefault(
                "TEST ITEM\nFantastic flight and slow fall\nExtreme mobility on all surfaces\nTemporary immunity to lava");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Astrobotas");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "TEST ITEM\nVuelo y velocidad excelentes\nMobilidad extrema en toda superficie\nInmunidad temporal a la lava");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Космич ски Ботинки");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "TEST ITEM\nФантастич ский полёт и м ко п из мл ни \nМобильность на вс пов ност \nВ м нна н у звимость к лав ");
        }


        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 50, 0, 0);
            Item.rare = 9;
            Item.accessory = true;
        }
        

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<VirtuousPlayer>();
            modPlayer.accessoryAstroBoots = !hideVisual;

            player.wingTimeMax = 180;
            player.accRunSpeed = 13.0f;
            player.runAcceleration += 0.07f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax = player.lavaMax + 420;
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
            //var recipe = new ModRecipe(mod);
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
