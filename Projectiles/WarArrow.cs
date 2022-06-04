using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    public class WarArrow : ModProjectile
    {
        private const int ArmorPenetration = 42;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("War Arrow");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Flecha de Guerra");
        }


        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 1;
            Projectile.arrow = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 600;
            Projectile.ranged = true;
            Projectile.arrow = true;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Main.player[Projectile.owner].armorPenetration += ArmorPenetration; // We increase the penetration for the following hit
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].armorPenetration -= ArmorPenetration; // We return the penetration back to normal
        }


        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            Main.player[Projectile.owner].armorPenetration += ArmorPenetration;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].armorPenetration -= ArmorPenetration;
        }
    }
}
