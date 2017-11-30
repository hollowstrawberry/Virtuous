using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Bullseye_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullseye");
            Tooltip.SetDefault("\"Love at first sight\"\nShoot through the magical sight for double ranged damage\nOther ranged shots are weaker");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Bullseye;
            duration = 60 * 60;
            amount = 1;

            item.width = 30;
            item.height = 30;
            item.mana = 100;
            item.knockBack = 2.4f;
            item.rare = 8;
            item.value = Item.sellPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RifleScope);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);
            recipe.AddIngredient(ItemID.Amber, 5);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

