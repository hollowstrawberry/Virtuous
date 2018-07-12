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
            DisplayName.AddTranslation(GameCulture.Spanish, "Flecha de Guerra");
        }


        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.aiStyle = 1;
            projectile.arrow = true;
            projectile.alpha = 0;
            projectile.timeLeft = 600;
            projectile.ranged = true;
            projectile.arrow = true;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Main.player[projectile.owner].armorPenetration += ArmorPenetration; // We increase the penetration for the following hit
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].armorPenetration -= ArmorPenetration; // We return the penetration back to normal
        }


        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            Main.player[projectile.owner].armorPenetration += ArmorPenetration;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[projectile.owner].armorPenetration -= ArmorPenetration;
        }
    }
}
