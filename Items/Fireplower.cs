using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Virtuous.Projectiles;
using static Virtuous.Tools;

namespace Virtuous.Items
{
    public class Fireplower : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireplower");
			DisplayName.AddTranslation(GameCulture.Russian, "Огнеплюв");
            Tooltip.SetDefault("50% chance to not consume gel\nLeaves burning gel on the ground");
			DisplayName.AddTranslation(GameCulture.Russian, "50% не потратить гель\nОставляет горящий гель на земле");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 22;
            item.useStyle = 5;
            item.autoReuse = true;
            item.useAnimation = 30;
            item.useTime = item.useAnimation/2;
            item.shoot = ProjectileID.Flames;
            item.useAmmo = AmmoID.Gel;
            item.UseSound = SoundID.Item34;
            item.damage = 120;
            item.knockBack = 2f;
            item.shootSpeed = 7f;
            item.noMelee = true;
            item.value = Item.sellPrice(0, 15, 0, 0);
            item.rare = 8;
            item.ranged = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ShootArc = FullCircle / 10f;
            int fireAmount = 9;
            int gelAmount = RandomInt(1, 3);

            for (int i = 0; i < fireAmount; i++)
            {
                Vector2 velocity = new Vector2(speedX, speedY).RotatedBy(ShootArc/2 * i/fireAmount * ((i % 2 == 0) ? 1 : -1));
                Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
            }

            for (int i = 0; i < gelAmount; i++)
            {
                Vector2 gelPosition = position + new Vector2(speedX, speedY).OfLength(item.width);
                if (!Collision.CanHit(player.Center, 0, 0, gelPosition, 0, 0)) gelPosition = position;
                Vector2 gelVelocity = new Vector2(speedX, speedY).RotatedBy(ShootArc * RandomFloat(-1, +1)) * RandomFloat(0, 2);

                Projectile.NewProjectile(gelPosition, gelVelocity, mod.ProjectileType<ProjGelFire>(), damage, 0, player.whoAmI);
            }

            if (OneIn(1000)) Main.NewText("Burn, baby! Burn!", Color.Orange);
            return false; //Doesn't shoot normally
        }

        public override bool ConsumeAmmo(Player player)
        {
            return CoinFlip();
        }

        /*public override Vector2? HoldoutOffset()
        {
            return new Vector2(10, 0);
        }*/

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Flamethrower);
            recipe.AddIngredient(ItemID.FireFeather);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}