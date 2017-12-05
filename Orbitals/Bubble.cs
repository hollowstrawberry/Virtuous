using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Bubble_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
            Tooltip.SetDefault("\"Stay out of my personal space\"\nThe bubble repels enemies and raises defense");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Bubble;
            duration = 20 * 60;
            amount = 1;

            item.width = 32;
            item.height = 32;
            item.mana = 40;
            item.rare = 5;
            item.value = Item.sellPrice(0, 15, 0, 0);
            item.useStyle = 2;
            item.useTime = 20;
            item.useAnimation = item.useTime;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 100);
            recipe.AddIngredient(ItemID.WaterBucket);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }


    public class Bubble_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Bubble;
        public override int OriginalAlpha => 120;
        public override int FadeTime => 60;
        public override float BaseDistance => 0;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
        }

        public override void PlayerEffects()
        {
            player.statDefense += 10;
            Lighting.AddLight(player.Center, 0.4f, 0.6f, 0.6f);
        }

        public override void FirstTick()
        {
            base.FirstTick();

            projectile.damage = 1;

            for (int i = 0; i < 40; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/16, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                newDust.velocity *= 1.5f;
                newDust.noLight = false;
            }
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(224, 255, 252, 150) * projectile.Opacity;
        }
    }
}

