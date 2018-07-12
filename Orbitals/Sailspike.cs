using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Sailspike : OrbitalProjectile
    {
        public override int Type => OrbitalID.Sailspike;
        public override int DyingTime => 30;
        public override int FadeTime => 20;
        public override int OriginalAlpha => 100;
        public override float BaseDistance => 60;
        public override float DyingSpeed => 20;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
            DisplayName.AddTranslation(GameCulture.Spanish, "Picabichos");
            DisplayName.AddTranslation(GameCulture.Russian, "Парящий Шип");
            DisplayName.AddTranslation(GameCulture.Chinese, "飞梭尖刺");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 30;
            projectile.height = 14;
        }


        public override void FirstTick()
        {
            base.FirstTick();
            Movement();

            for (int i = 0; i < 15; i++)
            {
                var dust = Dust.NewDustDirect(
                    projectile.position, projectile.width, projectile.height,
                    /*Type*/172, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                dust.velocity *= 0.2f;
                dust.noLight = false;
                dust.noGravity = true;
            }
        }


        public override void Movement()
        {
            // Stays in front of the player
            projectile.spriteDirection = player.direction;
            SetPosition(new Vector2(player.direction * relativeDistance, 0));
        }


        public override void PostAll()
        {
            Lighting.AddLight(projectile.Center, 0.15f, 0.5f, 1.5f);
            base.PostAll(); // Fades
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 2;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 2;
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(28, 77, 255, 200) * projectile.Opacity;
        }
    }
}
