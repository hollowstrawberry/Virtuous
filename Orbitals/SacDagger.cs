using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SacDagger : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.SacDagger;
        public override int DyingTime => 30;
        public override int FadeTime => DyingTime;
        public override float BaseDistance => 105;
        public override float DyingSpeed => 30;
        public override float OscillationSpeedMax => 0.4f;
        public override float OscillationAcc => OscillationSpeedMax / 40;

        private const int SpecialSpinTime = 15; // Ticks it spends doing a half-orbit when spinning. Needs to be a multiple of 15
        private const float SpecialSpinSpeed = (30 / SpecialSpinTime) * Tools.RevolutionPerSecond; // Orbit speed while spinning


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Dagger");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Daga de Sacrificio");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Жертвенный Кинжал");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "牺牲匕首");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 54;
        }



        private void LifeSteal(Vector2 position, int damage) // Spawns vampire heal projectiles
        {
            Player player = Main.player[Projectile.owner];

            float heal = Math.Min(damage / 30f, player.statLifeMax - player.statLife); // Caps at the life missing
            if (heal > 0)
            {
                Projectile.NewProjectile(
                    position, Vector2.Zero, ProjectileID.VampireHeal, 0, 0, Projectile.owner, Projectile.owner, heal);
            }
        }



        public override void PlayerEffects()
        {
            player.lifeRegenTime = 0;
            player.lifeRegen = -10;
        }


        public override void FirstTick()
        {
            base.FirstTick();
            Projectile.rotation += 45.ToRadians(); // 45 degrees because of the sprite
        }


        public override void SpecialFunction()
        {
            int spinDirection = SpecialFunctionTimer >= 0 ? +1 : -1; // Positive for clockwise, negative for counterclockwise

            if (SpecialFunctionTimer == 0) // First tick, sets direction
            {
                spinDirection = player.direction;
            }
            else if (Math.Abs(SpecialFunctionTimer) == SpecialSpinTime - 1) // Last tick
            {
                orbitalPlayer.SpecialFunctionActive = false;
            }
            
            RotatePosition(SpecialSpinSpeed * spinDirection); // Rotate around the player
            Projectile.rotation += SpecialSpinSpeed * spinDirection; // Rotate sprite

            SpecialFunctionTimer--; // Undoes the normal increase of the timer
            SpecialFunctionTimer += spinDirection; // Advances timer in either direction

            Projectile.netUpdate = true;
        }


        public override void PostAll()
        {
            Lighting.AddLight(Projectile.Center, 1.8f, 0f, 0f);
            base.PostAll(); // Fades
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 3;
            else if (IsDoingSpecial) damage = (int)(damage * 1.5f);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 3;
            else if (IsDoingSpecial) damage = (int)(damage * 1.5f);
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
            return (IsDying || IsDoingSpecial); // Only while actively attacking
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 0, 0, 180) * Projectile.Opacity;
        }
    }
}
