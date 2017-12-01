using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
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
        private const float SpecialDamageMultiplier = 1.5f; //Damage it deals while spinning


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Dagger");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 48;
            projectile.height = 54;
        }

        public override void PlayerEffects(Player player)
        {
            player.lifeRegenTime = 0;
            player.lifeRegen = -10;
        }

        public override void FirstTick()
        {
            base.FirstTick();
            projectile.rotation += 45.ToRadians(); //45 degrees because of the sprite
        }

        public override void SpecialEffect()
        {
            int spinDirection = specialEffectTimer > 0 ? +1 : -1; //Positive for clockwise, negative for counterclockwise

            if (specialEffectTimer == 0) //First tick
            {
                spinDirection = player.direction;
                projectile.damage = (int)(projectile.damage * SpecialDamageMultiplier); //Higher damage when spinning
                projectile.netUpdate = true; //Syncs to multiplayer
            }
            else if (Math.Abs(specialEffectTimer) == SpecialSpinTime - 1) //Last tick
            {
                specialEffectTimer = -spinDirection; //Gets set to 0 at the end of the method
                specialEffectActive = false; //Turns off the special effect for all daggers
                projectile.damage = (int)(projectile.damage / SpecialDamageMultiplier); //Returns the damage to its original
                projectile.netUpdate = true; //Syncs to multiplayer
            }

            relativePosition = relativePosition.RotatedBy(SpecialSpinSpeed * spinDirection); //Rotates the daggers
            projectile.rotation += SpecialSpinSpeed * spinDirection; //Points the sprite outwards
            projectile.Center = player.MountedCenter + relativePosition; //Keeps the orbital on the player

            specialEffectTimer += spinDirection;
        }

        public override void DyingFirstTick()
        {
            projectile.damage *= 3;
            base.DyingFirstTick(); //Shoots out
        }

        public override void PostAll()
        {
            Lighting.AddLight(projectile.Center, 1.8f, 0f, 0f);
            base.PostAll(); //Fades
        }


        private void LifeSteal(Vector2 position, int damage) //Spawns vampire heal projectiles
        {
            Player player = Main.player[projectile.owner];

            float heal = Math.Min(damage / 30f, player.statLifeMax - player.statLife); //Caps at the life missing
            if (heal > 0 /*&& player.lifeSteal > 0*/) 
            {
                //player.lifeSteal -= heal; //Limits how much you can heal at once
                Projectile.NewProjectile(position, Vector2.Zero, ProjectileID.VampireHeal, 0, 0, projectile.owner, projectile.owner, heal);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.lifeMax > 5 && !Main.player[projectile.owner].moonLeech && !target.immortal)
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
            return orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOn]; //Only while spinning
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(255, 0, 0, 180) * projectile.Opacity;
        }
    }
}

