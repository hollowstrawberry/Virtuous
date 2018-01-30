using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Virtuous.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class TerraWalkers : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Walkers");
			DisplayName.AddTranslation(GameCulture.Russian, "Терра Боты");
            Tooltip.SetDefault("Extreme mobility on all surfaces\nIncreased wing time\nTemporary immunity to lava");
			Tooltip.AddTranslation(GameCulture.Russian, "Мобильность на всех поверхностях\nУвеличено время полёта\nВременная неуязвимость к лаве");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = Item.sellPrice(0, 20, 0, 0);
            item.rare = 7;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 10.0f;
            player.rocketBoots = 3;
            player.moveSpeed += 0.5f;
            player.runAcceleration += 0.03f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 420;
            player.wingTimeMax += 50;
        }

        public override void AddRecipes()
        {
            //ModRecipe recipe = new ModRecipe(mod);
            //recipe.AddIngredient(ItemID.FrostsparkBoots);
            //recipe.AddIngredient(ItemID.LavaWaders);
            //recipe.AddIngredient(ItemID.SoulofMight, 1);
            //recipe.AddIngredient(ItemID.SoulofSight, 1);
            //recipe.AddIngredient(ItemID.SoulofFright, 1);
            //recipe.AddTile(TileID.MythrilAnvil);
            //recipe.SetResult(this);
            //recipe.AddRecipe();
        }
        
    }
}
