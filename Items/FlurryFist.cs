using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Projectiles;

namespace Virtuous.Items
{
    [AutoloadEquip(EquipType.HandsOn, EquipType.HandsOff)]
    public class FlurryFist : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flurry Nova");
            Tooltip.SetDefault("\"Over 9000 punches per hour\"\nRight Click for a boring punch");
        }

        private int previousFist = -1; //The position ID of the previous fist shot, so it won't be used twice in a row

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 36;
            item.UseSound = SoundID.Item1;
            item.crit = 20;
            item.knockBack = 2f;
            item.melee = true;
            item.rare = 9;
            item.value = Item.sellPrice(0, 20, 0, 0);
            
            //Replaced by CanUseItem
            item.useStyle = 1;
            item.useTime = 5;
            item.useAnimation = item.useTime;
            item.damage = 300;
            item.shoot = mod.ProjectileType<ProjFist>();
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
		
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {//Left Click
                item.useStyle = 1;
                item.useTime = 6;
				item.useAnimation = item.useTime;
                item.damage = 300;
                item.shoot = mod.ProjectileType<ProjFist>();
                item.autoReuse = true;
                item.noMelee = true;
                item.noUseGraphic = true;
            }
            else
            {//Right Click
                item.useStyle = 3;
                item.useTime = 10;
				item.useAnimation = item.useTime;
                item.shoot = 0;
                item.damage = 600;
                item.autoReuse = false;
                item.noMelee = false;
                item.noUseGraphic = false;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            const int D = 22; //Fist spawnpoint's minimum horizontal distance to the player
            Vector2 center = player.MountedCenter;
            Vector2 offset = default(Vector2); //Fist spawnpoint relative to the center of the player
            Vector2 velocity = new Vector2(player.direction, 0); //Fist movement direction

            int nextFist = -1;
            for (int i = 0; i < 20; i++) //Makes x attempts at creating a projectile that the player can reach. Gives up otherwise.
			{
                do {nextFist = Main.rand.Next(9);
                } while (nextFist == previousFist); //Can't match the position of the previous one

                switch (nextFist) //One of 9 different spawn points
				{
					case 0: offset = new Vector2(D+17,  0); break;
					case 1: offset = new Vector2(D+14,+10); break;
					case 2: offset = new Vector2(D+ 8,+20); break;
					case 3: offset = new Vector2(D+ 4,+30); break;
					case 4: offset = new Vector2(D+ 0,+40); break;
                    case 5: offset = new Vector2(D+14,-10); break;
                    case 6: offset = new Vector2(D+ 8,-20); break;
                    case 7: offset = new Vector2(D+ 4,-30); break;
                    case 8: offset = new Vector2(D+ 0,-40); break;
                }
				offset.X *= player.direction;

				position = center + offset; //Final position of the to-be-spawned fist
                if (Collision.CanHit(center, 0, 0, position, 0, 0)) break;
            }

            previousFist = nextFist;
            Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FragmentSolar, 8);
            recipe.AddIngredient(ItemID.FragmentStardust, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
