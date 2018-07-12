using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    class Fireball : OrbitalProjectile
    {
        public override int Type => OrbitalID.Fireball;
        public override int DyingTime => 60;
        public override int FadeTime => DyingTime;
        public override float BaseDistance => 90;
        public override float OrbitingSpeed => 0.5f * Tools.RevolutionPerSecond;
        public override float RotationSpeed => OrbitingSpeed;

        private const int OriginalSize = 30;
        private const int BurstSize = OriginalSize * 4;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            DisplayName.AddTranslation(GameCulture.Spanish, "Bola de Fuego");
            DisplayName.AddTranslation(GameCulture.Chinese, "火球");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = OriginalSize;
            projectile.height = OriginalSize;
        }



        private void MakeDust()
        {
            for (int i = 0; i < 7; i++) 
            {
                var dust = Dust.NewDustDirect(
                    projectile.position, projectile.width, projectile.height,
                    Main.rand.Choose(new int[] { DustID.Fire, DustID.SolarFlare, 158 }), 0f, 0f, projectile.alpha, default(Color), 1f);

                dust.noGravity = true;
                if (dust.type == DustID.SolarFlare) dust.scale = 1.5f;
                else dust.velocity *= 2;
            }
        }



        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 15; i++)
            {
                var dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare);
                dust.velocity *= 2;
            }
        }


        public override bool PreMovement()
        {
            return true; // Always move even during special effect or death
        }

        public override void Movement()
        {
            base.Movement();
            MakeDust();
        }


        public override void SpecialFunction()
        {
            if (specialFunctionTimer == 0) // First tick
            {
                Tools.ResizeProjectile(projectile.whoAmI, BurstSize, BurstSize, true);
                Main.PlaySound(SoundID.Item14, projectile.Center);

                MakeDust();

                for (int i = 0; i < 6; i++) // Extra dust
                {
                    var dust = Dust.NewDustDirect(
                        projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f,
                        projectile.alpha, default(Color), 4f);
                    dust.noGravity = true;
                }

                projectile.Damage(); // Damage enemies instantly
            }
            else if (specialFunctionTimer >= 5) // Last tick
            {
                Tools.ResizeProjectile(projectile.whoAmI, OriginalSize, OriginalSize, true);
                orbitalPlayer.SpecialFunctionActive = false;
            }
        }


        public override void DyingFirstTick()
        {
        }

        public override void Dying()
        {
            // Accelerates throughout DyingTime
            float rotationMult = 7f * (DyingTime - projectile.timeLeft) / DyingTime;
            RotatePosition(OrbitingSpeed * rotationMult);
            projectile.rotation += RotationSpeed * rotationMult;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying || IsDoingSpecial) damage *= 2;
            target.AddBuff(BuffID.OnFire, 5 * 60);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying || IsDoingSpecial) damage *= 2;
            target.AddBuff(BuffID.OnFire, 5 * 60);
        }
    }
}
