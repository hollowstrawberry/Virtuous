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

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Martillo Apretillo");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "\"Aplasta a tus enemigos\"");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Дробилка");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "\"На кусочки, говоришь?\"");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "粉碎战锤");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "去粉碎它,你说呢?");
        }


        public override void SetDefaults()
        {
            Item.width = 43;
            Item.height = 37;
            Item.scale = 1.1f;
            Item.useStyle = 1;
            Item.useTime = 40;
            Item.useAnimation = Item.useTime;
            Item.UseSound = SoundID.Item1;
            Item.damage = 100;
            Item.crit = 5;
            Item.knockBack = 5f;
            Item.melee = true;
            Item.autoReuse = true;
            Item.rare = 9;
            Item.value = Item.sellPrice(0, 15, 0, 0);
        }


        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            int type = Mod.Find<ModProjectile>(nameof(ProjCrusherPillar)).Type;
            int appearance = Main.rand.Next(Main.projFrames[type]);
            damage *= 2;
            knockBack = 0;

            foreach (int direction in new[] { +1, -1 })
            {
                var position = new Vector2(target.Center.X + ProjCrusherPillar.SpawnDistance * direction, target.Center.Y);
                var velocity = new Vector2(-direction * (crit ? 10 : 5), 0f);

                var proj = Projectile.NewProjectileDirect(position, velocity, type, damage, knockBack, player.whoAmI);
                var pillar = proj.ModProjectile as ProjCrusherPillar;
                pillar.Appearance = appearance;
                pillar.Crit = crit;
                proj.netUpdate = true;
            }
        }


        public override void AddRecipes()
        {
            var recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.StaffofEarth);
            recipe.AddIngredient(ItemID.ChlorophyteWarhammer);
            recipe.AddIngredient(ItemID.Marble, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
