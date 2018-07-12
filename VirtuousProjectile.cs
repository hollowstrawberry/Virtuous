using System;
using System.IO;
using System.Linq;
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

        public bool spotter = false; // Was shot by the spotter gun
        public bool artOfWar = false; // Was shot by the war bow
        public float collidePositionY = 0; // Position past which the arrow will collide again



        public override void AI(Projectile projectile)
        {
            if (artOfWar && projectile.position.Y > collidePositionY)
            {
                projectile.tileCollide = true;
            }
        }


        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (target.active && spotter) // If this projectile was shot by the spotter
            {
                if (Main.projectile.Any(x => x.active && x.owner == projectile.owner && x.type == mod.ProjectileType<ProjCrosshair>()))
                {
                    return; // Doesn't spawn a crosshair if there's already one in the world
                }

                Vector2 position = target.SpriteCenter() + Main.rand.NextVector2(400, 600);
                var proj = Projectile.NewProjectileDirect(
                    position, Vector2.Zero, mod.ProjectileType<ProjCrosshair>(),
                    damage*5, knockback*2, projectile.owner);
                var crosshair = proj.modProjectile as ProjCrosshair;
                crosshair.Target = target.whoAmI;
            }
        }
    }
}
