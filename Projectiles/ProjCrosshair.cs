using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        private int State // Stored as ai[1]
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Francotirador");
        }


        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.timeLeft = StateTime[Tracking];
            Projectile.alpha = 250;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false; // Doesn't damage enemies
            Projectile.hostile = false; // Doesn't damage players
            Projectile.usesIDStaticNPCImmunity = true; // Doesn't interfere with other piercing damage
            Projectile.idStaticNPCHitCooldown = StateTime[GoingUp] + 1;
        }


        public override void AI()
        {
            Vector2 targetCenter = Main.npc[Target].SpriteCenter();
            Vector2 relativePosition = targetCenter - Projectile.Center;
            Vector2 offset;

            // Mode deciding

            if (Projectile.timeLeft == StateTime[Tracking]) State = Tracking; // First tick

            if (Projectile.timeLeft == 1 || State == Tracking && relativePosition.Length() < MaxSpeed / 3)
            {
                State++; // Advance state
                Projectile.timeLeft = StateTime[State];
            }

            // Mode behavior

            switch (State)
            {
                case Tracking:
                    Projectile.alpha -= 6; // Fades in
                    float speed = BaseSpeed + (MaxSpeed - BaseSpeed) * (1 - ((float)Projectile.timeLeft / StateTime[State])); // Speeds up
                    Projectile.velocity = relativePosition.OfLength(Math.Min(relativePosition.Length(), speed)); // Homes in on target
                    break;

                case Holding:
                    Projectile.alpha = 0;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = targetCenter;
                    break;

                case GoingUp:
                    if (Projectile.timeLeft == StateTime[State]) // First tick
                    {
                        Projectile.friendly = true; // Damages enemies
                        SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/SniperShot"), Projectile.Center);
                    }
                    offset = new Vector2(0, -(StateTime[State] - Projectile.timeLeft) * MaxVerticalOffset / StateTime[State]);
                    Projectile.Center = targetCenter + offset;
                    break;

                case GoingDown:
                    Projectile.friendly = false; // Doesn't damage enemies anymore
                    Projectile.alpha += 12; // Fades out
                    offset = new Vector2(0, -Projectile.timeLeft * MaxVerticalOffset / StateTime[State]);
                    Projectile.Center = targetCenter + offset;
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
            return Color.White * Projectile.Opacity; // Unaffected by lighting
        }
    }
}
