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
    /// <summary>
    /// Custom data given to all projectiles by this mod.
    /// Includes <see cref="Items.SpotterGun"/>'s crosshair effect and <see cref="Items.ArtOfWar"/>'s collision mechanics.
    /// </summary>
    class VirtuousProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        /// <summary>Whether this projectile was shot by <see cref="Items.SpotterGun"/> and can spawn a <see cref="ProjCrosshair"/>.</summary>
        public bool spotter = false;

        /// <summary>Whether this projectile was shot by <see cref="Items.ArtOfWar"/>.</summary>
        public bool artOfWar = false;

        /// <summary>Position past which the arrow will collide again if <see cref="artOfWar"/> is true.</summary>
        public float collidePositionY = 0;



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
                proj.netUpdate = true;
            }
        }
    }
}
