using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class SpikedBubble_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Bubble");
            Tooltip.SetDefault("The bubble slightly raises damage and defense\nAligns with either magic or melee users");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 36;
            item.useStyle = 2;
            item.useTime = 20;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.damage = 45;
            item.knockBack = 2.5f;
            item.shoot = 1;
            item.mana = 60;
            item.noMelee = true;
            item.autoReuse = false;
            item.rare = 6;
            item.value = Item.sellPrice(0, 15, 0, 0);

            OrbitalItem orbitalItem = item.GetGlobalItem<OrbitalItem>();
            orbitalItem.type = OrbitalID.SpikedBubble;
            orbitalItem.duration = 30 * 60 + SpikedBubble_Proj.DyingTime;
            orbitalItem.amount = 1;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
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