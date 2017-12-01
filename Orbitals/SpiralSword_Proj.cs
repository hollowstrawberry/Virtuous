using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Virtuous.Tools;

namespace Virtuous.Orbitals
{
    public class SpiralSword_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.SpiralSword;
        public override int DyingTime => 30;
        public override float BaseDistance => 95;
        public override float ShootSpeed => 15;
        public override float OrbitingSpeed => 0.5f * RevolutionPerSecond;
        public override float RotationSpeed => OrbitingSpeed;
        public override float OscillationSpeedMax => 0.4f;
        public override float OscillationAcc => OscillationSpeedMax / 40;

        public  const float DamageBoost = 0.2f; //Damage boost while the orbital is active. Used by OrbitalPlayer and OrbitalItem
        private const float ThrowDamageMultiplier = 1.5f; //Damage it deals when thrown
        private const float ThrowDistance = 300f; //Maximum distance when thrown
        private const float ThrowSpeed = (ThrowDistance - /*BaseDistance*/90) / 16; //Last number is how many ticks it takes to go in one direction
        private const float ShootAcc = 3; //Acceleration per tick while dying


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Virtue");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 76;
            projectile.height = 76;
        }

        public override void FirstTick()
        {
            base.FirstTick();
            projectile.rotation += 45.ToRadians(); //45 degrees because of the sprite
        }

        public override void SpecialEffect()
        {
            if (specialEffectTimer == 0) //First tick
            {
                direction = Outwards;
                distance = BaseDistance;
                projectile.damage = (int)(projectile.damage * ThrowDamageMultiplier);
                projectile.idStaticNPCHitCooldown = 5; //Deals damage more rapidly
                projectile.netUpdate = true; //Sync to multiplayer
            }

            float orbitSpeed = OrbitingSpeed * (specialEffectTimer < 30 ? 2 : 1); //Doubles the speed only the first 30 ticks of the special effect so that the final direction when dying isn't affected
            relativePosition = relativePosition.RotatedBy(orbitSpeed); //Rotates the sword around the player
            projectile.rotation += orbitSpeed; //Rotates the sprite accordingly
            distance += ThrowSpeed * (direction ? +1 : -1); //Moves inwards or outwards
            projectile.Center = player.MountedCenter + relativePosition; //Moves the sword to the defined position around the player

            specialEffectTimer++;

            if (distance >= ThrowDistance) //If it has reached the set maximum distance for the throw
            {
                direction = Inwards; //Return
            }
            else if (direction == Inwards && distance <= BaseDistance) //If it has returned to the passive zone
            {
                specialEffectActive = false;
                specialEffectTimer = 0;
                projectile.netUpdate = true; //Sync to multiplayer

                //Resets to passive behavior
                direction = Inwards;
                distance = BaseDistance;
                oscillationSpeed = OscillationSpeedMax;
                projectile.damage = (int)(projectile.damage / ThrowDamageMultiplier);
                projectile.idStaticNPCHitCooldown = 10;
            }
        }


        public override void DyingFirstTick()
        {
            projectile.damage *= 5;
            base.DyingFirstTick(); //Shoot out
        }

        public override void Dying()
        {
            projectile.velocity += projectile.velocity.OfLength(ShootAcc); //Accelerates
            projectile.position += projectile.velocity; //Re-applies velocity as it would normally be nullified for orbitals
        }


        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return specialEffectTimer > 0; //Only while actively attacking
        }

        public override Color? GetAlpha(Color newColor) //Fullbright
        {
            return new Color(150, 255, 230, 150) * projectile.Opacity;
        }
    }
}
