using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Projectiles
{
    class ProjCrosshair : ModProjectile
    {
        private const byte Tracking = 0, Holding = 1, GoingUp = 2, GoingDown = 3, Dying = 4; // States
        private const float BaseSpeed = 6, MaxSpeed = 60; // Speed range when tracking its target
        private const int MaxVerticalOffset = 80; // How high it can go

        private static readonly int[] StateTime = { 240, 30, 5, 20, 0 };



        public int Target // Stored as ai[0]
        {
            get { return (int)projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        private int State // Stored as ai[1]
        {
            get { return (int)projectile.ai[1]; }
            set { projectile.ai[1] = value; }
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper");
            DisplayName.AddTranslation(GameCulture.Spanish, "Francotirador");
        }


        public override void SetDefaults()
        {
            projectile.width = 78;
            projectile.height = 78;
            projectile.timeLeft = StateTime[Tracking];
            projectile.alpha = 250;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.friendly = false; // Doesn't damage enemies
            projectile.hostile = false; // Doesn't damage players
            projectile.usesIDStaticNPCImmunity = true; // Doesn't interfere with other piercing damage
            projectile.idStaticNPCHitCooldown = StateTime[GoingUp] + 1;
        }


        public override void AI()
        {
            Vector2 targetCenter = Main.npc[Target].SpriteCenter();
            Vector2 relativePosition = targetCenter - projectile.Center;
            Vector2 offset;

            // Mode deciding

            if (projectile.timeLeft == StateTime[Tracking]) State = Tracking; // First tick

            if (projectile.timeLeft == 1 || State == Tracking && relativePosition.Length() < MaxSpeed / 3)
            {
                State++; // Advance state
                projectile.timeLeft = StateTime[State];
            }

            // Mode behavior

            switch (State)
            {
                case Tracking:
                    projectile.alpha -= 6; // Fades in
                    float speed = BaseSpeed + (MaxSpeed - BaseSpeed) * (1 - ((float)projectile.timeLeft / StateTime[State])); // Speeds up
                    projectile.velocity = relativePosition.OfLength(Math.Min(relativePosition.Length(), speed)); // Homes in on target
                    break;

                case Holding:
                    projectile.alpha = 0;
                    projectile.velocity = Vector2.Zero;
                    projectile.Center = targetCenter;
                    break;

                case GoingUp:
                    if (projectile.timeLeft == StateTime[State]) // First tick
                    {
                        projectile.friendly = true; // Damages enemies
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/SniperShot"), projectile.Center);
                    }
                    offset = new Vector2(0, -(StateTime[State] - projectile.timeLeft) * MaxVerticalOffset / StateTime[State]);
                    projectile.Center = targetCenter + offset;
                    break;

                case GoingDown:
                    projectile.friendly = false; // Doesn't damage enemies anymore
                    projectile.alpha += 12; // Fades out
                    offset = new Vector2(0, -projectile.timeLeft * MaxVerticalOffset / StateTime[State]);
                    projectile.Center = targetCenter + offset;
                    break;
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Always has the crit visual effect
            crit = true;
            damage /= 2;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity; // Unaffected by lighting
        }
    }
}
