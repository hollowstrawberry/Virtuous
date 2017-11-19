using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Virtuous.Items.Accessories
{

	[AutoloadEquip(EquipType.Wings)]
	public class Archangel : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Archangel");
			Tooltip.SetDefault("Sublime flight and speed!\nExtreme mobility on all surfaces\nTemporary immunity to lava");
		}

		public override void SetDefaults()
		{
			item.width = 36;
			item.height = 32;
			item.value = Item.sellPrice(1, 0, 0, 0);
            item.rare = 11;
			item.accessory = true;
			item.expert = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			VirtuousPlayer modPlayer = player.GetModPlayer<VirtuousPlayer>();
			player.wingTimeMax = 300;
			player.accRunSpeed = 16f;
			player.runAcceleration += 0.2f;
			player.iceSkate = true;
			player.waterWalk = true;
			player.fireWalk = true;
			player.lavaMax += 900;
			modPlayer.accessoryArchangel  = !hideVisual;
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
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "AstroBoots");
			recipe.AddRecipeGroup("Virtuous:CelestialWings");
			recipe.AddIngredient(ItemID.FishronWings);
			recipe.AddIngredient(ItemID.BetsyWings);
			recipe.AddIngredient(ItemID.GravityGlobe);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		
	}
}
