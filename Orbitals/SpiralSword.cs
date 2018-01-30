using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class SpiralSword_Item : OrbitalItem
    {
        private const int ManaCost = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blades of Virtue");
			DisplayName.AddTranslation(GameCulture.Russian, "Клинки Доблести");
            Tooltip.SetDefault("Ethereal swords protect you and raise all damage\nRight Click after summoning for an active attack\nAligns with either magic or melee users");
			Tooltip.AddTranslation(GameCulture.Russian, "Магические клинки защищают вас и увеличивают урон\nПКМ после вызова - активная атака\nПодходит воинам и магам");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SpiralSword;
            duration = 40 * 60;
            amount = 8;
            specialFunctionType = SpecialRightClick; //Makes the orbital's special function activate with right click

            item.width = 30;
            item.height = 30;
            item.damage = 250;
            item.knockBack = 5f;
            item.mana = ManaCost; //Overwritten by CanUseItem
            item.rare = 11;
            item.value = Item.sellPrice(1, 0, 0, 0);
            item.autoReuse = true;
            item.useTurn = false;
            item.useStyle = 4;
            item.useTime = 35;
            item.useAnimation = item.useTime;
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            item.mana = ManaCost; //So it always displays the full mana cost

            base.GetWeaponDamage(player, ref damage);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2) //Left click
            {
                item.mana = ManaCost;
            }
            else //Right click
            {
                item.mana = (int)Math.Ceiling(ManaCost / 10f);
            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }



    public class SpiralSword_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.SpiralSword;
        public override int DyingTime => 30;
        public override float BaseDistance => _BaseDistance; //Set this to a constant so it can be used in other constants
        public override float DyingSpeed => 15;
        public override float OrbitingSpeed => 0.5f * RevolutionPerSecond;
        public override float RotationSpeed => OrbitingSpeed;
        public override float OscillationSpeedMax => 0.4f;
        public override float OscillationAcc => OscillationSpeedMax / 40;

        private const float _BaseDistance = 95;
        private const float SpecialDistance = 300f; //Maximum distance when thrown
        private const float SpecialSpeed = (SpecialDistance - _BaseDistance) / 16; //Last number is how many ticks it takes to go in one direction
        private const float DyingAcc = 3; //Acceleration per tick while dying
        private const float DamageBoost = 0.2f; //Damage boost while the orbital is active


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Virtue");
			DisplayName.AddTranslation(GameCulture.Russian, "Клинок Доблести");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 76;
            projectile.height = 76;
        }

        public override void PlayerEffects()
        {
            player.meleeDamage  += DamageBoost;
            player.rangedDamage += DamageBoost;
            player.magicDamage  += DamageBoost;
            player.minionDamage += DamageBoost;
            player.thrownDamage += DamageBoost;
            orbitalPlayer.damageBuffFromOrbitals += DamageBoost;

            Lighting.AddLight(player.Center, 0.5f, 1.5f, 3.0f);
        }

        public override void FirstTick()
        {
            base.FirstTick();
            projectile.rotation += 45.ToRadians(); //45 degrees because of the sprite
        }

        public override void SpecialFunction()
        {
            if (specialFunctionTimer == 0) //First tick
            {
                direction = Outwards;
                SetDistance(BaseDistance);
                projectile.idStaticNPCHitCooldown = 5; //Deals damage more rapidly
                projectile.netUpdate = true; //Sync to multiplayer
            }

            float orbitSpeed = OrbitingSpeed * (specialFunctionTimer < 30 ? 2 : 1); //Doubles the speed only the first 30 ticks of the special effect so that the final direction when dying isn't affected
            RotatePosition(orbitSpeed); //Moves the sword around the player
            projectile.rotation += orbitSpeed; //Rotates the sprite accordingly
            AddDistance(SpecialSpeed * (direction ? +1 : -1)); //Moves the sword innard or outward

            if (relativeDistance >= SpecialDistance) //If it has reached the set maximum distance for the throw
            {
                direction = Inwards; //Return
            }
            else if (direction == Inwards && relativeDistance <= BaseDistance) //If it has returned to the passive zone
            {
                orbitalPlayer.specialFunctionActive = false;
                projectile.netUpdate = true; //Sync to multiplayer

                //Resets to passive behavior
                SetDistance(BaseDistance);
                oscillationSpeed = OscillationSpeedMax;
                projectile.idStaticNPCHitCooldown = 10;
            }
        }

        public override void Dying()
        {
            projectile.velocity += projectile.velocity.OfLength(DyingAcc); //Accelerates
            projectile.position += projectile.velocity; //Re-applies velocity as it would normally be nullified for orbitals
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (isDying) damage *= 5;
            else if (isDoingSpecial) damage *= 2;
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (isDying) damage *= 5;
            else if (isDoingSpecial) damage *= 2;
        }

        public override bool? CanCutTiles()
        {
            return (isDying || isDoingSpecial); //Only while actively attacking
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(150, 255, 230, 150) * projectile.Opacity;
        }
    }
}
