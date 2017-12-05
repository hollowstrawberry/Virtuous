using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class SpikedBubble_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Bubble");
            Tooltip.SetDefault("The bubble slightly raises damage and defense\nEnemies are repelled and damaged\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SpikedBubble;
            duration = 30 * 60;
            amount = 1;

            item.width = 36;
            item.height = 36;
            item.damage = 45;
            item.knockBack = 4f;
            item.mana = 60;
            item.rare = 6;
            item.value = Item.sellPrice(0, 15, 0, 0);
            item.useStyle = 2;
            item.useTime = 20;
            item.useAnimation = item.useTime;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<Bubble_Item>());
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddIngredient(ItemID.PinkGel, 20);
            recipe.AddIngredient(ItemID.LifeFruit);
            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }


    public class SpikedBubble_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.SpikedBubble;
        public override int DyingTime => 30;
        public override float BaseDistance => 0;
        public override float RotationSpeed => 0 * RevolutionPerSecond; //I set this to 0 for now so that the sprite doesn't look weird

        private const float ExpandedScale = 1.5f; //Size when fully expanded
        private const int ExpandedAlpha = 150; //Alpha value when fully expanded
        private const int OriginalSize = 120; //Width and height dimensions in pixels
        private const int PopTime = 10; //Part of DyingTime where it keeps its size before popping
        private const float DamageBoost = 0.1f; //Damage boost while the orbital is active


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Bubble");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = OriginalSize;
            projectile.height = OriginalSize;
        }

        public override void PlayerEffects()
        {
            player.statDefense += 5;
            player.meleeDamage += DamageBoost;
            player.magicDamage += DamageBoost;
            orbitalPlayer.damageBuffFromOrbitals += DamageBoost;

            Lighting.AddLight(player.Center, 0.6f, 0.4f, 0.3f);
        }

        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 50; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/16, 0f, 0f, /*Alpha*/50, new Color(255, 200, 245), 1.2f);
                newDust.velocity *= 1.5f;
                newDust.noLight = false;
            }
        }

        public override bool PreMovement()
        {
            return true; //Move even while dying
        }

        public override void Dying()
        {
            if (projectile.timeLeft >= PopTime) //Expands until PopTime then stops
            {
                //Change the apparent size
                projectile.scale += (ExpandedScale - 1f) / (DyingTime - PopTime);
                //Change the hitbox size
                projectile.height = (int)(OriginalSize * projectile.scale);
                projectile.width  = (int)(OriginalSize * projectile.scale);
                //Align the sprite with its new size
                drawOriginOffsetY = (projectile.height - OriginalSize) / 2;
                drawOffsetX = (projectile.width - OriginalSize) / 2;

                projectile.alpha += (int)Math.Ceiling((float)(ExpandedAlpha - OriginalAlpha) / DyingTime); //Fade
            }

            else if (projectile.timeLeft == 1) //Last tick
            {
                Main.PlaySound(SoundID.Item54, projectile.Center); //Bubble pop
                Lighting.AddLight(player.Center, 1.5f, 1.0f, 1.4f);
                const int DustAmount = 50;
                for (int i = 0; i < DustAmount; i++)
                {
                    Vector2 position = projectile.Center + Vector2.UnitY.RotatedBy(RandomFloat(FullCircle)).OfLength(RandomInt(projectile.width / 2)); //Random rotation, random distance from the center
                    Dust newDust = Dust.NewDustDirect(position, 0, 0, /*Type*/16, 0, 0, /*Alpha*/100, new Color(255, 200, 245, 150), /*Scale*/1.2f);
                    newDust.velocity *= 2;
                }
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (isDying) damage *= 2;
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (isDying) damage *= 2;
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 200, 245, 150) * projectile.Opacity;
        }

    }
}