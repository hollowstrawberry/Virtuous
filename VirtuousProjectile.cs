using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Projectiles;


namespace Virtuous
{
    class VirtuousProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;


        public bool spotter = false; //Was shot by the spotter gun

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (target.active && projectile.GetGlobalProjectile<VirtuousProjectile>().spotter) //If this projectile was shot by the spotter
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == mod.ProjectileType<ProjCrosshair>()) return;
                } //Doesn't spawn a crosshair if there's already one in the world

                Vector2 position = target.SpriteCenter() + Tools.RandomDirection().OfLength(Tools.RandomInt(300, 500));
                Projectile.NewProjectile(position, Vector2.Zero, mod.ProjectileType<ProjCrosshair>(), damage * 5, knockback * 2, projectile.owner, target.whoAmI);
            }
        }
    }
}
