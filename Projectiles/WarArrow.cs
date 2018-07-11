using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous.Projectiles
{
    public class WarArrow : ModProjectile
    {
        // The player's vertical position when this arrow was shot, past which arrows will start coliding with tiles again
        public float CollidePositionY { get { return projectile.ai[1]; } set { projectile.ai[1] = value; } }

        private const int ArmorPenetration = 42;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("War Arrow");
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

        public override bool PreAI()
        {
            if (!projectile.tileCollide)
            {
                for (int i = 0; i < Main.maxProjectiles; i++) //Loops through all projectiles
                {
                    Projectile p = Main.projectile[i];
                    if (p.owner == projectile.owner && p.arrow && p.Center.Y >= projectile.Center.Y && p.Center.Y >= CollidePositionY)
                    {
                        p.tileCollide = true; //Causes all the arrows below this one to start colliding if they are also below the player's vertical position when this arrow was shot
                    }
                }
                projectile.netUpdate = true;
            }

            return true;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Main.player[projectile.owner].armorPenetration += ArmorPenetration; //We increase the penetration for the following hit
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].armorPenetration -= ArmorPenetration; //We return the penetration back to normal
        }


        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            Main.player[projectile.owner].armorPenetration += ArmorPenetration; //We increase the penetration for the following hit
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[projectile.owner].armorPenetration -= ArmorPenetration; //We return the penetration back to normal
        }
    }
}
