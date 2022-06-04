using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Items;

namespace Virtuous.Projectiles
{
    public class ProjTitanAOE : ModProjectile
    {
        public bool Crit // Whether the original hit was a crit, stored as ai[0]
        {
            get { return Projectile.ai[0] != 0; }
            set { Projectile.ai[0] = value ? 1 : 0; }
        }

        private bool FirstTick // Stored as ai[1], which is 0 by default (true in this case)
        {
            get { return Projectile.ai[1] == 0; }
            set { Projectile.ai[1] = value ? 0 : 1; }
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Shield");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Escudo Titán");
        }


        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255; // Transparent
            Projectile.timeLeft = 10;

            Projectile.usesIDStaticNPCImmunity = true; // Invincibility per projectile type
            Projectile.idStaticNPCHitCooldown = TitanShield.AoEInvincibility;

            //projectile.usesLocalNPCImmunity = true; // Invincibility per individual projectile
            //projectile.localNPCHitCooldown = TitanShield.AoEInvincibility;
        }


        public override void AI()
        {
            if (FirstTick)
            {
                FirstTick = false;
                for (int i = 0; i < 25; i++)
                {
                    Vector2 gorePosition = Projectile.position;
                    gorePosition += new Vector2(Main.rand.NextFloat(Projectile.width / 2), Main.rand.NextFloat(Projectile.height / 2));

                    Gore.NewGore(gorePosition, new Vector2(1, 1), Main.rand.Next(61, 64), Main.rand.NextFloat(0.2f, 1.5f));
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type: 31, Scale: Main.rand.NextFloat(2f));
                }
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = Crit;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            crit = Crit;
        }
    }
}