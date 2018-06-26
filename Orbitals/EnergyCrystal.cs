using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class EnergyCrystal : OrbitalProjectile
    {
        public override int Type => OrbitalID.EnergyCrystal;
        public override int FadeTime => 60;
        public override int OriginalAlpha => 100;
        public override float BaseDistance => 70;
        public override float OrbitingSpeed => Tools.FullCircle / 5 / CycleMoveTime;

        public override bool IsDoingSpecial => true; // Always keeps increasing specialFunctionTimer

        private const int CycleTime = 60; // Time between cycles
        private const int CycleMoveTime = 15; // Part of CycleTime where the crystals move
        private const int TrailLength = 10;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Crystal");
            DisplayName.AddTranslation(GameCulture.Spanish, "Cristal de Energía");
            DisplayName.AddTranslation(GameCulture.Russian, "Энергетический Кристалл");

            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = TrailLength; // Length of projectile.oldPos array
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 22;
            projectile.height = 36;
        }



        private bool InOverdrive() // When it will shoot faster
        {
            return (player.immune || orbitalPlayer.time < FadeTime);
        }


        private NPC FindTarget()
        {
            return Main.npc.FirstOrDefault(
                npc => npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.immortal && !npc.dontTakeDamage
                && (npc.Center - projectile.Center).Length() < (InOverdrive() ? 600 : 400));
        }



        public override void PlayerEffects()
        {
            if (specialFunctionTimer == CycleTime - CycleMoveTime)
            {
                Main.PlaySound(SoundID.Item15.WithVolume(0.5f), player.Center); //Woosh
            }
        }


        public override void FirstTick()
        {
            base.FirstTick();
            projectile.rotation = 0;
            RotatePosition(-Tools.FullCircle / 4); // The first crystal will be above the player instead of to the right
        }


        public override bool PreMovement()
        {
            return true; // Always move
        }

        public override void Movement()
        {
            if (specialFunctionTimer == CycleTime) specialFunctionTimer = 0; // I'll use specialFunctionTimer as our clock for the cycles

            // Animation
            if (specialFunctionTimer % 2 == 0)
            {
                if (projectile.frame < Main.projFrames[projectile.type] - 1) projectile.frame++;
                else projectile.frame = 0;
            }

            // Orbiting
            if (CycleTime - specialFunctionTimer <= CycleMoveTime) // Cycles positions
            {
                RotatePosition(OrbitingSpeed);
            }
            else
            {
                SetPosition();
            }

            // Firing
            int fireChances = InOverdrive() ? 15 : 5; // How many times in a cycle it gets the chance to shoot
            if (specialFunctionTimer % (CycleTime / fireChances) == 0 && Main.rand.OneIn(3))
            {
                NPC target = FindTarget();
                if (target != null)
                {
                    Main.PlaySound(SoundID.Item12.WithVolume(0.5f).WithPitchVariance(Main.rand.NextFloat(-0.2f, +0.2f)), projectile.Center);

                    Projectile.NewProjectile(
                        projectile.Center, (target.Center - projectile.Center).OfLength(10), ProjectileID.LaserMachinegunLaser,
                        projectile.damage / 2, projectile.knockBack, projectile.owner);
                }
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 2;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 2;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) // Trail
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            var drawOrigin = new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2);

            int increment = player.velocity == Vector2.Zero ? 2 : 1; // Draws them further apart when stationary
            int length = player.velocity == Vector2.Zero ? TrailLength : TrailLength / 2 - 1;

            for (int i = 0; i < length; i += increment)
            {
                if (i == 0 || projectile.oldPos[i] != projectile.position) // Doesn't stack them if they're all the same
                {
                    Rectangle? frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
                    Vector2 position = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                    Color color = projectile.GetAlpha(lightColor) * (0.8f * (projectile.oldPos.Length - i) / projectile.oldPos.Length);

                    spriteBatch.Draw(
                        texture, position, frame, color, projectile.rotation,
                        drawOrigin, projectile.scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 230, 255, 50) * projectile.Opacity;
        }
    }
}