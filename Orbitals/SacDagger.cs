using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class SacDagger_Item : OrbitalItem
    {
        private const int ManaCost = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Daggers");
			DisplayName.AddTranslation(GameCulture.Russian, "Жертвенные Кинжалы");
            Tooltip.SetDefault("\"Feed them\"\nThe daggers drain your life, but heal you when harming an enemy\nUse again after summoning to spin and reset duration\nAligns with either magic or melee users");
			Tooltip.AddTranslation(GameCulture.Russian, "\"Накорми их\"\nКинжалы высасывают ваше здоровье, но крадут его у врагов\nИспользуйте повторно, чтобы раскрутить и сбросить время действия\nПодходит воинам и магам");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SacDagger;
            duration = 20 * 60;
            amount = 2;
            specialFunctionType = SpecialReuse; //This makes the orbital's special function activate after using the item again

            item.width = 30;
            item.height = 30;
            item.damage = 180;
            item.knockBack = 5f;
            item.mana = ManaCost; //Overwritten by CanUseItem
            item.rare = 8;
            item.value = Item.sellPrice(0, 40, 0, 0);
            item.autoReuse = true;
            item.useStyle = 4;
            item.useTime = 16;
            item.useAnimation = item.useTime;
            item.useTurn = false;
        }

        public override bool CanUseItem(Player player)
        {
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (orbitalPlayer.active[this.type]) //If there is a dagger active
            {
                item.mana = (int)Math.Ceiling(ManaCost / 5f); 
            }
            else
            {
                item.mana = ManaCost;
            }

            return base.CanUseItem(player);
        }
    }



    public class SacDagger_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.SacDagger;
        public override int DyingTime => 30;
        public override int FadeTime => DyingTime;
        public override float BaseDistance => 105;
        public override float DyingSpeed => 30;
        public override float OscillationSpeedMax => 0.4f;
        public override float OscillationAcc => OscillationSpeedMax / 40;

        private const int SpecialSpinTime = 15; //Ticks it spends doing a half-orbit when spinning. Needs to be a divisor or multiple of 60
        private const float SpecialSpinSpeed = (30 / SpecialSpinTime) * RevolutionPerSecond; //Speed at which it will orbit while spinning


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Dagger");
			DisplayName.AddTranslation(GameCulture.Russian, "Жертвенный Кинжал");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 48;
            projectile.height = 54;
        }

        public override void PlayerEffects()
        {
            player.lifeRegenTime = 0;
            player.lifeRegen = -10;
        }

        public override void FirstTick()
        {
            base.FirstTick();
            projectile.rotation += 45.ToRadians(); //45 degrees because of the sprite
        }

        public override void SpecialFunction()
        {
            int spinDirection = specialFunctionTimer > 0 ? +1 : -1; //Positive for clockwise, negative for counterclockwise

            if (specialFunctionTimer == 0) //First tick
            {
                spinDirection = player.direction;
                projectile.netUpdate = true; //Syncs to multiplayer
            }
            else if (Math.Abs(specialFunctionTimer) == SpecialSpinTime - 1) //Last tick
            {
                orbitalPlayer.specialFunctionActive = false; //Turns off the special effect for all daggers
                projectile.netUpdate = true; //Syncs to multiplayer
            }
            
            RotatePosition(SpecialSpinSpeed * spinDirection); //Rotates the daggers
            projectile.rotation += SpecialSpinSpeed * spinDirection; //Points the sprite outwards

            specialFunctionTimer--; //Reverts the normal increase of the timer
            specialFunctionTimer += spinDirection; //Advances timer in either direction
        }

        public override void PostAll()
        {
            Lighting.AddLight(projectile.Center, 1.8f, 0f, 0f);
            base.PostAll(); //Fades
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (isDying) damage *= 3;
            else if (isDoingSpecial) damage = (int)(damage * 1.5f);
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (isDying) damage *= 3;
            else if (isDoingSpecial) damage = (int)(damage * 1.5f);
        }

        private void LifeSteal(Vector2 position, int damage) //Spawns vampire heal projectiles
        {
            Player player = Main.player[projectile.owner];

            float heal = Math.Min(damage / 30f, player.statLifeMax - player.statLife); //Caps at the life missing
            if (heal > 0) Projectile.NewProjectile(position, Vector2.Zero, ProjectileID.VampireHeal, 0, 0, projectile.owner, projectile.owner, heal);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.lifeMax > 5 && !target.immortal && !player.moonLeech)
            {
                LifeSteal(target.Center, damage);
            }
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            LifeSteal(target.Center, damage);
        }

        public override bool? CanCutTiles()
        {
            return (isDying || isDoingSpecial); //Only while actively attacking
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 0, 0, 180) * projectile.Opacity;
        }
    }
}
