using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    public class LionsMane : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lion's Mane");
            Tooltip.SetDefault(
                "Damage increases exponentially as it travels\nLeft Click for clockwise, Right Click for counter-clockwise");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Melena de León");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "El daño aumenta exponencialmente con la distancia\nHaz Click Derecho para ir en sentido contrarreloj");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Львиная Грива");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Урон увеличивается пропорционально времени полёта\nЛКМ - по часовой стрелке\nПКМ - против часовой стрелки");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "雄狮鬃毛");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "随着持续时间成倍增加伤害\n左键顺时针释放,右键逆时针释放");
        }


        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = 5;
            Item.useTime = 9;
            Item.useAnimation = Item.useTime;
            Item.UseSound = SoundID.Item8;
            Item.damage = 150;
            Item.shoot = Mod.Find<ModProjectile>(nameof(ProjLionsMane)).Type;
            Item.shootSpeed = 0f;
            Item.crit = 5;
            Item.knockBack = 7f;
            Item.mana = 9;
            Item.magic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.rare = 8;
            Item.value = Item.sellPrice(0, 20, 0, 0);
        }


        public override bool AltFunctionUse(Player player) => true;


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            Tools.HandleAltUseAnimation(player);

            base.GetWeaponDamage(player, ref damage);
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float cursorAngle = (Main.MouseWorld - player.Center).ToRotation().ToDegrees();

            if (cursorAngle >= -135 && cursorAngle <= -45) position = -Vector2.UnitY; // Up
            else if (cursorAngle >= -45 && cursorAngle <= +45) position = Vector2.UnitX; // Right
            else if (cursorAngle >= +45 && cursorAngle <= +135) position = Vector2.UnitY; // Down
            else position = -Vector2.UnitX; // Left

            var proj = Projectile.NewProjectileDirect(Vector2.Zero, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            var mane = proj.ModProjectile as ProjLionsMane;
            mane.RelativePosition = position.OfLength(5);
            mane.Direction = player.altFunctionUse == 2 ? -1 : +1;
            proj.netUpdate = true;
            return false;
        }


        public override void AddRecipes()
        {
            ModRecipe recipe;

            recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.CursedFlames);
            recipe.AddIngredient(ItemID.FragmentNebula, 8);
            recipe.AddIngredient(ItemID.FragmentSolar, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(Mod);
            recipe.AddIngredient(ItemID.GoldenShower);
            recipe.AddIngredient(ItemID.FragmentNebula, 8);
            recipe.AddIngredient(ItemID.FragmentSolar, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
