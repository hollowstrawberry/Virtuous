using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class EnergyCrystal : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.EnergyCrystal;
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
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Cristal de Energía");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Энергетический Кристалл");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "能量水晶");

            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailLength; // Length of projectile.oldPos array
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 36;
        }



        private bool InOverdrive() // When it will shoot faster
        {
            return (player.immune || orbitalPlayer.time < FadeTime);
        }


        private NPC FindTarget()
        {
            return Main.npc.FirstOrDefault(
                npc => npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.immortal && !npc.dontTakeDamage
                && (npc.Center - Projectile.Center).Length() < (InOverdrive() ? 600 : 400));
        }



        public override void PlayerEffects()
        {
            if (SpecialFunctionTimer == CycleTime - CycleMoveTime)
            {
                SoundEngine.PlaySound(SoundID.Item15.WithVolume(0.5f), player.Center); //Woosh
            }
        }


        public override void FirstTick()
        {
            base.FirstTick();
            Projectile.rotation = 0;
            RotatePosition(-Tools.FullCircle / 4); // The first crystal will be above the player instead of to the right
        }


        public override bool PreMovement()
        {
            return true; // Always move
        }

        public override void Movement()
        {
            if (SpecialFunctionTimer == CycleTime) SpecialFunctionTimer = 0; // I'll use specialFunctionTimer as our clock for the cycles

            // Animation
            if (SpecialFunctionTimer % 2 == 0)
            {
                if (Projectile.frame < Main.projFrames[Projectile.type] - 1) Projectile.frame++;
                else Projectile.frame = 0;
            }

            // Orbiting
            if (CycleTime - SpecialFunctionTimer <= CycleMoveTime) // Cycles positions
            {
                RotatePosition(OrbitingSpeed);
            }
            else
            {
                SetPosition();
            }

            // Firing
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            int fireChances = InOverdrive() ? 15 : 5; // How many times in a cycle it gets the chance to shoot
            if (SpecialFunctionTimer % (CycleTime / fireChances) == 0 && Main.rand.NextBool(3))
            {
                NPC target = FindTarget();
                if (target != null)
                {
                    SoundEngine.PlaySound(SoundID.Item12.WithVolume(0.5f).WithPitchVariance(Main.rand.NextFloat(-0.2f, +0.2f)), Projectile.Center);

                    Projectile.NewProjectile(
                        Projectile.Center, (target.Center - Projectile.Center).OfLength(10), ProjectileID.LaserMachinegunLaser,
                        Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
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


        public override bool PreDraw(ref Color lightColor) // Trail
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var drawOrigin = new Vector2(texture.Width / 2, texture.Height / Main.projFrames[Projectile.type] / 2);

            int increment = player.velocity == Vector2.Zero ? 2 : 1; // Draws them further apart when stationary
            int length = player.velocity == Vector2.Zero ? TrailLength : TrailLength / 2 - 1;

            for (int i = 0; i < length; i += increment)
            {
                if (i == 0 || Projectile.oldPos[i] != Projectile.position) // Doesn't stack them if they're all the same
                {
                    Rectangle? frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                    Vector2 position = Projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * (0.8f * (Projectile.oldPos.Length - i) / Projectile.oldPos.Length);

                    spriteBatch.Draw(
                        texture, position, frame, color, Projectile.rotation,
                        drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 230, 255, 50) * Projectile.Opacity;
        }
    }
}