using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Projectiles;
using static Virtuous.Tools;

namespace Virtuous.Items
{
    public class EtherSlit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ether Slit");
            Tooltip.SetDefault("Right Click for a barrage attack, with higher speed, critical chance and stick duration");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.shoot = mod.ProjectileType<ProjSummonedSword>();
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Slash");
            item.damage = 250;
            item.knockBack = 7f;
            item.mana = 7;
            item.magic = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.rare = 10;
            item.value = Item.sellPrice(0, 50, 0, 0);

            //Replaced by CanUseItem
            item.useStyle = 5;
            item.useTime = 10;
            item.useAnimation = item.useTime;
            item.shootSpeed = 16f;
            item.crit = 15;
        }
        
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        
        public override bool CanUseItem(Player player)
        {
            //Left Click
            if (player.altFunctionUse != 2)
            {
                item.useStyle = 5;
                item.useTime = 10;
                item.useAnimation = item.useTime;
                item.shootSpeed = 16f;
                item.crit = 15;
            }
            //Right Click
            else
            {
                item.useStyle = 3;
                item.useTime = 7;
                item.useAnimation = item.useTime;
                item.shootSpeed = 20f;
                item.crit = 30;
            }

            return base.CanUseItem(player);
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            //So it always displays the left-click values when not using the weapon
            CanUseItem(player);

            //A little hack to stop the bugged 1-tick delay between consecutive right-click uses of a weapon
            if (player.altFunctionUse == 2)
            {
                if (player.itemAnimation == 1) //Resets the animation so it doesn't let the hand return to resting position
                {
                    player.itemAnimation = item.useAnimation;
                    Main.PlaySound(item.UseSound, player.Center);
                }
                if (PlayerInput.Triggers.JustReleased.MouseRight) //Stops the animation manually
                {
                    player.itemAnimation = 0;
                }
            }


            base.GetWeaponDamage(player, ref damage);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 center = player.MountedCenter; //Player center
            int distanceMin = 20; //Projectile spawnpoint's minimum distance to the player
            int distanceMax = 60; //Projectile spawnpoint's maximum distance to the player

            for (int i = 0; i < 20; i++) //Makes 20 attempts at creating a projectile that the player can reach. Gives up otherwise.
            {
                position = center + (RandomFloat(FullCircle).ToRotationVector2() * RandomInt(distanceMin, distanceMax)); //Random rotation, random distance from the player

                if (Collision.CanHit(center, 0, 0, position + (position - center).OfLength(8f), 0, 0)) break;
            }
            
            Vector2 direction = player.altFunctionUse!=2 ? new Vector2(Main.mouseX + Main.screenPosition.X - center.X, Main.mouseY + Main.screenPosition.Y - center.Y) : new Vector2(player.direction, 0); //The direction to shoot at. In the direction of the mouse with left click, in the direction the player is facing with right click.
            Vector2 straightVelocity =  direction.OfLength(item.shootSpeed); //Swords shoot parallel to each other in the set direction
            Vector2 cursorVelocity   = (Main.MouseWorld - position).SafeNormalize(straightVelocity) * item.shootSpeed; //Swords shoot directly at the mouse
            Vector2 finalVelocity    = player.altFunctionUse!=2 ? Vector2.Lerp(cursorVelocity, straightVelocity, 0.4f) : straightVelocity; //Middlepoint of both with left click, straight with right click

            Projectile.NewProjectile(position, finalVelocity, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SkyFracture);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddIngredient(ItemID.BlackLens);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
