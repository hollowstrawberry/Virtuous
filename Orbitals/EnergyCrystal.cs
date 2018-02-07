using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class EnergyCrystal_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Crystal");
            Tooltip.SetDefault("The crystals fire at nearby enemies\nGetting hurt causes a momentary overdrive\nAligns with either magic or melee users");
            DisplayName.AddTranslation(GameCulture.Spanish, "Cristal de Energía");
            Tooltip.AddTranslation(GameCulture.Spanish, "Los cristales dispararán a enemigos cercanos\nSer herido los sobrecalentará por un momento\nEl daño se alínea con magia o cuerpo a cuerpo");
            DisplayName.AddTranslation(GameCulture.Russian, "Энергетический Кристалл");
			Tooltip.AddTranslation(GameCulture.Russian, "Кристаллы стреляют по ближайшим врагам\nПолучение урона вызывает короткую перегрузку\nПодходит воинам и магам");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.EnergyCrystal;
            duration = 30 * 60;
            amount = 5;

            item.width = 18;
            item.height = 32;
            item.damage = 55;
            item.knockBack = 2f;
            item.mana = 40;
            item.rare = 6;
            item.value = Item.sellPrice(0, 8, 0, 0);
        }
    }



    public class EnergyCrystal_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.EnergyCrystal;
        public override int FadeTime => 60;
        public override int OriginalAlpha => 100;
        public override float BaseDistance => 70;
        public override float OrbitingSpeed => Tools.FullCircle / 5 / CycleMoveTime; //Moves a fifth of a circle over CycleMoveTime ticks

        public override bool isDoingSpecial => true; //Always keeps increasing specialFunctionTimer

        public const int CycleTime = 60; //Time between cycles
        public const int CycleMoveTime = 15; //Part of CycleTime where the crystals move


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Crystal");
			DisplayName.AddTranslation(GameCulture.Russian, "Энергетический Кристалл");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 22;
            projectile.height = 36;
        }


        private bool InOverdrive() //When it will shoot faster
        {
            return (player.immune || orbitalPlayer.time < FadeTime);
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
            RotatePosition(-Tools.FullCircle / 4); //The first crystal will be above the player instead of right to it
        }

        public override bool PreMovement()
        {
            return true; //Always move
        }

        public override void Movement()
        {
            if (specialFunctionTimer == CycleTime) specialFunctionTimer = 0; //We will use specialFunctionTimer as our clock for the cycles

            //Animation
            if (specialFunctionTimer % 2 == 0)
            {
                if (projectile.frame < Main.projFrames[projectile.type] - 1) projectile.frame++;
                else projectile.frame = 0;
            }

            //Orbiting
            if (CycleTime - specialFunctionTimer <= CycleMoveTime) //Cycles positions
            {
                RotatePosition(OrbitingSpeed);
            }
            else
            {
                SetPosition();
            }

            //Firing
            int fireChances = InOverdrive() ? 15 : 5; //How many times in a cycle it gets the chance to shoot
            if (specialFunctionTimer % (CycleTime / fireChances) == 0 && Tools.OneIn(3))
            {
                NPC target = FindTarget();
                if (target != null)
                {
                    Main.PlaySound(SoundID.Item12.WithVolume(0.5f).WithPitchVariance(Tools.RandomFloat(-0.2f, +0.2f)), projectile.Center);
                    Projectile.NewProjectileDirect(projectile.Center, (target.Center - projectile.Center).OfLength(10), ProjectileID.LaserMachinegunLaser, projectile.damage / 2, projectile.knockBack, projectile.owner);
                }
            }
        }

        private NPC FindTarget()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].lifeMax > 5 && !Main.npc[i].immortal && !Main.npc[i].dontTakeDamage)
                {
                    if ((Main.npc[i].Center - projectile.Center).Length() < (InOverdrive() ? 600 : 400)) return Main.npc[i];
                }
            }

            return null;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (isDying) damage *= 2;
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (isDying) damage *= 2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) //Trail
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2);

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                if (i == 0 || projectile.oldPos[i] != projectile.position) //Doesn't stack them if they're all the same
                {
                    Rectangle? frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
                    Vector2 position = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                    Color color = projectile.GetAlpha(lightColor) * 0.8f * ((projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);

                    spriteBatch.Draw(texture, position, frame, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
                }

                if (player.velocity == Vector2.Zero) i++; //If the player is stationary, skip every second oldPos
                else if (i >= 4) break; //If the player is moving, only draw the first five oldPos
            }

            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 230, 255, 50) * projectile.Opacity;
        }
    }
}