using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SpiralSword : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.SpiralSword;
        public override int DyingTime => 30;
        public override float BaseDistance => _BaseDistance; // Set this to a constant so it can be used in other constants
        public override float DyingSpeed => 15;
        public override float OrbitingSpeed => 0.5f * Tools.RevolutionPerSecond;
        public override float RotationSpeed => OrbitingSpeed;
        public override float OscillationSpeedMax => 0.4f;
        public override float OscillationAcc => OscillationSpeedMax / 40;

        private const float DamageBoost = 0.2f; // Damage boost while the orbital is active

        private const float _BaseDistance = 95;
        private const float SpecialDistance = 300f; // Maximum distance when thrown
        private const float SpecialSpeed = (SpecialDistance - _BaseDistance) / 16;
        private const float DyingAcc = 3; // Acceleration per tick while dying


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Virtue");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Hojas Virtud");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Клинок Доблести");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "空灵圣剑");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 76;
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
            Projectile.rotation += 45.ToRadians(); // 45 degrees because of the sprite
        }


        public override void SpecialFunction()
        {
            if (SpecialFunctionTimer == 0) // First tick
            {
                Direction = Outwards;
                SetDistance(BaseDistance);
                Projectile.idStaticNPCHitCooldown = 5; // Deals damage more rapidly
            }

            // The directions in which the swords are shot out when dying can be affected by this change in speed,
            // so the amount of ticks spent at double speed should be a multiple of 15 (I think)
            float orbitSpeed = OrbitingSpeed * (SpecialFunctionTimer < 30 ? 2 : 1);

            Projectile.rotation += orbitSpeed; // Rotate sprite
            RotatePosition(orbitSpeed); // Rotate projectile relative to the player
            AddDistance(SpecialSpeed * (Direction ? +1 : -1)); // Move the sword innard or outward

            if (RelativeDistance >= SpecialDistance) // Past the maximum throw distance
            {
                Direction = Inwards;
            }
            else if (Direction == Inwards && RelativeDistance <= BaseDistance) // Back inside the passive zone
            {
                orbitalPlayer.SpecialFunctionActive = false;

                // Resets to passive behavior
                SetDistance(BaseDistance);
                OscillationSpeed = OscillationSpeedMax;
                Projectile.idStaticNPCHitCooldown = 10;
            }

            Projectile.netUpdate = true; // Sync to multiplayer
        }


        public override void Dying()
        {
            Projectile.velocity += Projectile.velocity.OfLength(DyingAcc); // Accelerates outward
            Projectile.position += Projectile.velocity;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 5;
            else if (IsDoingSpecial) damage *= 2;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 5;
            else if (IsDoingSpecial) damage *= 2;
        }


        public override bool? CanCutTiles()
        {
            return (IsDying || IsDoingSpecial); // Only while actively attacking
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(150, 255, 230, 150) * Projectile.Opacity;
        }
    }
}
