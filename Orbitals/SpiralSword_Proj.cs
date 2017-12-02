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
        public override float BaseDistance => _BaseDistance; //Set this to a constant so it can be used in other constants
        public override float DyingSpeed => 15;
        public override float OrbitingSpeed => 0.5f * RevolutionPerSecond;
        public override float RotationSpeed => OrbitingSpeed;
        public override float OscillationSpeedMax => 0.4f;
        public override float OscillationAcc => OscillationSpeedMax / 40;

        private const float _BaseDistance = 92;
        private const float SpecialDistance = 300f; //Maximum distance when thrown
        private const float SpecialSpeed = (SpecialDistance - _BaseDistance) / 16; //Last number is how many ticks it takes to go in one direction
        private const float SpecialDmgMult = 1.5f; //Damage it deals when thrown
        private const float DyingAcc = 3; //Acceleration per tick while dying
        private const float DamageBoost = 0.2f; //Damage boost while the orbital is active


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Virtue");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 76;
            projectile.height = 76;
        }

        public override void PlayerEffects(Player player)
        {
            player.meleeDamage  += DamageBoost;
            player.rangedDamage += DamageBoost;
            player.magicDamage  += DamageBoost;
            player.minionDamage += DamageBoost;
            player.thrownDamage += DamageBoost;

            Lighting.AddLight(player.Center, 0.5f, 1.5f, 3.0f);
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
                projectile.damage = (int)(projectile.damage * SpecialDmgMult);
                projectile.idStaticNPCHitCooldown = 5; //Deals damage more rapidly
                projectile.netUpdate = true; //Sync to multiplayer
            }

            float orbitSpeed = OrbitingSpeed * (specialEffectTimer < 30 ? 2 : 1); //Doubles the speed only the first 30 ticks of the special effect so that the final direction when dying isn't affected
            relativePosition = relativePosition.RotatedBy(orbitSpeed); //Rotates the sword around the player
            projectile.rotation += orbitSpeed; //Rotates the sprite accordingly
            distance += SpecialSpeed * (direction ? +1 : -1); //Moves inwards or outwards
            projectile.Center = player.MountedCenter + relativePosition; //Moves the sword to the defined position around the player

            if (distance >= SpecialDistance) //If it has reached the set maximum distance for the throw
            {
                direction = Inwards; //Return
            }
            else if (direction == Inwards && distance <= BaseDistance) //If it has returned to the passive zone
            {
                orbitalPlayer.specialFunctionActive = false;
                projectile.netUpdate = true; //Sync to multiplayer

                //Resets to passive behavior
                distance = BaseDistance;
                oscillationSpeed = OscillationSpeedMax;
                projectile.damage = (int)(projectile.damage / SpecialDmgMult);
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
            projectile.velocity += projectile.velocity.OfLength(DyingAcc); //Accelerates
            projectile.position += projectile.velocity; //Re-applies velocity as it would normally be nullified for orbitals
        }


        public override bool? CanCutTiles() //So it doesn't become a lawnmower
        {
            return (isDying || doSpecialEffect); //Only while actively attacking
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(150, 255, 230, 150) * projectile.Opacity;
        }
    }
}
