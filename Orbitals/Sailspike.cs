using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Sailspike : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.Sailspike;
        public override int DyingTime => 30;
        public override int FadeTime => 20;
        public override int OriginalAlpha => 100;
        public override float BaseDistance => 60;
        public override float DyingSpeed => 20;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Picabichos");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Парящий Шип");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "飞梭尖刺");
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 14;
        }


        public override void FirstTick()
        {
            base.FirstTick();
            Movement();

            for (int i = 0; i < 15; i++)
            {
                var dust = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    /*Type*/172, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                dust.velocity *= 0.2f;
                dust.noLight = false;
                dust.noGravity = true;
            }
        }


        public override void Movement()
        {
            // Stays in front of the player
            Projectile.spriteDirection = player.direction;
            SetPosition(new Vector2(player.direction * RelativeDistance, 0));
        }


        public override void PostAll()
        {
            Lighting.AddLight(Projectile.Center, 0.15f, 0.5f, 1.5f);
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
            return new Color(28, 77, 255, 200) * Projectile.Opacity;
        }
    }
}
