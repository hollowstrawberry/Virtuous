using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;
using Terraria.DataStructures;

namespace Virtuous.Items
{
    public class EtherSlit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ether Slit");
            Tooltip.SetDefault(
                "Right Click for a barrage attack\nBarrage swords have higher speed, critical chance and stick duration");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Éter");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Haz Click Derecho para un ataque concentrado de mayor potencia");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Небесная Скважина");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "ПКМ для стихийной атаки\nУ стихийных клинков повышены скорость, шанс критического удара и время застревания");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "苍穹裂痕");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "右键释放空灵剑诀\n空灵剑诀拥有更高的攻速,暴击率及持续时间");
        }


        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.shoot = Mod.Find<ModProjectile>(nameof(ProjSummonedSword)).Type;
            Item.UseSound = new Terraria.Audio.SoundStyle("Sounds/Item/Slash");
            Item.damage = 250;
            Item.knockBack = 7f;
            Item.mana = 7;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 50, 0, 0);

            // Replaced in SetUseStats
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 10;
            Item.useAnimation = Item.useTime;
            Item.shootSpeed = 16f;
            Item.crit = 15;
        }
        

        public override bool AltFunctionUse(Player player) => true;


        private void SetUseStats(Player player)
        {
            // Left Click
            if (player.altFunctionUse != 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = 10;
                Item.useAnimation = Item.useTime;
                Item.shootSpeed = 16f;
                Item.crit = 15;
            }
            // Right Click
            else
            {
                Item.useStyle = ItemUseStyleID.Thrust;
                Item.useTime = 7;
                Item.useAnimation = Item.useTime;
                Item.shootSpeed = 20f;
                Item.crit = 30;
            }
        }
        

        public override bool CanUseItem(Player player)
        {
            SetUseStats(player); // Sets stats before use

            return base.CanUseItem(player);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            SetUseStats(player); // Always displays left-click stats
            Tools.HandleAltUseAnimation(player);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 20; i++) // Makes 20 attempts at finding a projectile position that the player can reach
            {
                position = player.MountedCenter + Main.rand.NextVector2(20, 60);
                if (Collision.CanHit(player.MountedCenter, 0, 0, position, 0, 0)) break;
            }

            // Right click: Horizontal, in the direction the player is facing
            // Left click: Middlepoint between straight from the player to the cursor and straight from the sword to the cursor
            velocity = player.altFunctionUse == 2
                ? new Vector2(player.direction * Item.shootSpeed, 0)
                : Vector2.Lerp(Main.MouseWorld - player.Center, Main.MouseWorld - position, 0.5f).OfLength(Item.shootSpeed);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SkyFracture)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient(ItemID.Ectoplasm, 20)
                .AddIngredient(ItemID.BlackLens)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
