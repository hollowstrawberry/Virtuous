using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    public class CrusherMace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crusher Mace");
            Tooltip.SetDefault("\"To shreds, you say?\"");

            DisplayName.AddTranslation(GameCulture.Spanish, "Martillo Apretillo");
            Tooltip.AddTranslation(GameCulture.Spanish, "\"Aplasta a tus enemigos\"");

            DisplayName.AddTranslation(GameCulture.Russian, "Дробилка");
            Tooltip.AddTranslation(GameCulture.Russian, "\"На кусочки, говоришь?\"");

            DisplayName.AddTranslation(GameCulture.Chinese, "粉碎战锤");
            Tooltip.AddTranslation(GameCulture.Chinese, "去粉碎它,你说呢?");
        }


        public override void SetDefaults()
        {
            item.width = 43;
            item.height = 37;
            item.scale = 1.1f;
            item.useStyle = 1;
            item.useTime = 40;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item1;
            item.damage = 100;
            item.crit = 5;
            item.knockBack = 5f;
            item.melee = true;
            item.autoReuse = true;
            item.rare = 9;
            item.value = Item.sellPrice(0, 15, 0, 0);
        }


        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            int type = mod.ProjectileType<ProjCrusherPillar>();
            int appearance = Main.rand.Next(Main.projFrames[type]);
            damage *= 2;
            knockBack = 0;

            foreach (int direction in new[] { +1, -1 })
            {
                var position = new Vector2(target.Center.X + ProjCrusherPillar.SpawnDistance * direction, target.Center.Y);
                var velocity = new Vector2(-direction * (crit ? 10 : 5), 0f);

                var proj = Projectile.NewProjectileDirect(position, velocity, type, damage, knockBack, player.whoAmI);
                var pillar = proj.modProjectile as ProjCrusherPillar;
                pillar.Appearance = appearance;
                pillar.Crit = crit;
            }
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.StaffofEarth);
            recipe.AddIngredient(ItemID.ChlorophyteWarhammer);
            recipe.AddIngredient(ItemID.Marble, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
