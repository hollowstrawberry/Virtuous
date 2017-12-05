using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Orbitals
{
    public class Sailspike_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sailspike");
            Tooltip.SetDefault("Summons a spike for a short time\nAligns with either magic or melee users");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Sailspike;
            duration = 5 * 60;
            amount = 1;

            item.width = 30;
            item.height = 30;
            item.damage = 15;
            item.knockBack = 1f;
            item.mana = 15;
            item.rare = 3;
            item.value = Item.sellPrice(0, 2, 0, 0);
        }
    }


    public class Sailspike_Proj : OrbitalProjectile
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

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/172, 0f, 0f, /*Alpha*/50, default(Color), 1.5f);
                newDust.velocity *= 0.2f;
                newDust.noLight = false;
                newDust.noGravity = true;
            }
        }

        public override void Movement()
        {
            //Stays in front of the player
            projectile.spriteDirection = player.direction;
            MoveRelativePosition(new Vector2(player.direction * relativeDistance, 0));
        }

        public override void PostAll()
        {
            Lighting.AddLight(projectile.Center, 0.15f, 0.5f, 1.5f);
            base.PostAll(); //Fades
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (isDying) damage *= 2;
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (isDying) damage *= 2;
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(28, 77, 255, 200) * projectile.Opacity;
        }
    }
}
