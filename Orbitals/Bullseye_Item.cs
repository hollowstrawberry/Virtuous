using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Bullseye_Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullseye");
            Tooltip.SetDefault("\"Love at first sight\"\nShoot through the magical sight for double ranged damage\nOther ranged shots are weaker");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 30;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.shoot = 1;
            item.mana = 100;
            item.useTurn = true;
            item.noMelee = true;
            item.autoReuse = false;
            item.rare = 8;
            item.value = Item.sellPrice(0, 2, 0, 0);

            OrbitalItem moditem = item.GetGlobalItem<OrbitalItem>();
            moditem.type = OrbitalID.Bullseye;
            moditem.duration = 60 * 60;
            moditem.amount = 1;
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

