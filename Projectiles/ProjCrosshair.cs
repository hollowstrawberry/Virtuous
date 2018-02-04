using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Virtuous.Projectiles
{
    class ProjCrosshair : ModProjectile
    {
        private const float MaxSpeed = 30;
        private const int MaxVerticalOffset = 30;
        private const byte Tracking = 0, Holding = 1, GoingUp = 2, GoingDown = 3, Dying = 4;
        private static readonly int[] StateTime = { 120, 30, 5, 10, 0 };

        private int State
        {
            get { return (int)projectile.ai[1]; }
            set { projectile.ai[1] = (int)value; }
        }

        private int Target => (int)projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper");
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
            projectile.friendly = false; //Doesn't damage enemies
            projectile.hostile = false; //Doesn't damage players
            projectile.usesIDStaticNPCImmunity = true; //Doesn't interfere with other piercing damage
            projectile.idStaticNPCHitCooldown = StateTime[GoingUp] + 1;
        }

        public override void AI()
        {
            Vector2 targetCenter = Main.npc[Target].SpriteCenter();
            Vector2 relativePosition = targetCenter - projectile.Center;
            Vector2 vOffset;

            //Mode deciding

            if (projectile.timeLeft == StateTime[Tracking]) State = Tracking; //First tick

            if (projectile.timeLeft == 1 || State == Tracking && relativePosition.Length() < MaxSpeed)
            {
                State++; //Advance state
                projectile.timeLeft = StateTime[State];
            }

            //Mode behavior

            switch (State)
            {
                case Tracking:
                    projectile.alpha -= 15; //Fades in
                    float speed = MaxSpeed * 1/5f + MaxSpeed * 4/5f * (1f - ((float)projectile.timeLeft / StateTime[State]));
                    projectile.velocity = relativePosition.OfLength(Math.Min(relativePosition.Length(), speed)); //Homes on target
                    break;

                case Holding:
                    projectile.alpha = 0;
                    projectile.velocity = Vector2.Zero;
                    projectile.Center = targetCenter;
                    break;

                case GoingUp:
                    if (projectile.timeLeft == StateTime[State]) //First tick
                    {
                        projectile.friendly = true; //Damages enemies
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/SniperShot"), projectile.Center); //Sniper shot
                    }
                    vOffset = new Vector2(0, -(StateTime[State] - projectile.timeLeft) * MaxVerticalOffset / StateTime[State]);
                    projectile.Center = targetCenter + vOffset;
                    break;

                case GoingDown:
                    projectile.friendly = false; //Doesn't damage enemies anymore
                    projectile.alpha += 15; //Fades out
                    vOffset = new Vector2(0, -(projectile.timeLeft) * MaxVerticalOffset / StateTime[State]);
                    projectile.Center = targetCenter + vOffset;
                    break;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = true;
            damage /= 2;
            //Always has the crit visual effect
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity; //Fullbright
        }
    }
}
