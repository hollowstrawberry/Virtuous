using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Dusts;

namespace Virtuous.Projectiles
{
    public class RainbowArrow : ModProjectile
    {
        private const int Lifespan = 6 * 60; // Total duration of the projetile
        private const int RainDelay = 50; // Time it takes for a special arrow effect to activate

        private static readonly Color[] ArrowColors = new[]
        {
            new Color(000, 050, 255, 100), // Blue
            new Color(000, 125, 255, 100), // Sky
            new Color(000, 255, 255, 100), // Cyan
            new Color(000, 255, 110, 100), // Aqua
            new Color(000, 255, 000, 100), // Green
            new Color(110, 255, 000, 100), // Lime
            new Color(240, 240, 000, 100), // Yellow
            new Color(255, 100, 000, 100), // Orange
            new Color(255, 000, 000, 100), // Red
            new Color(255, 000, 150, 100), // Fuchsia
            new Color(150, 000, 255, 100), // Purple
            new Color(090, 020, 255, 100), // Violet
        };


        public enum ArrowMode
        {
            Normal, // Behave like regular arrows. The bow will shoot them faster
            White, // Are shot upward, don't affect enemies, and cause rain shots to spawn after a short time
            Rain,  // Spawned by white shots, fall from above and have no knockback
        }




        public ArrowMode Mode // Stored as ai[0]
        {
            get { return (ArrowMode)(int)Projectile.ai[0]; }
            set { Projectile.ai[0] = (int)value; }
        }

        private Color Color // Stored as ai[1]
        {
            get
            {
                if (Mode == ArrowMode.White) return new Color(255, 255, 255, 0);
                if (ColorId >= 0 && ColorId < ArrowColors.Length) return ArrowColors[ColorId];
                return Color.Black;
            }
        }

        public int ColorId
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }




        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Arrow");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Flecha Iris");
        }


        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.arrow = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = Lifespan;
            Projectile.ranged = true;
        }


        private void SpawnRainbowDust(bool Burst = false)
        {
            int type = 0;
            float scale = 0;
            Vector2 velocity = Vector2.Zero;

            switch(Mode)
            {
                case ArrowMode.Normal:
                    type = 16; // Cloud
                    if(Burst) scale = 1.2f;
                    else scale = 0.8f;
                    break;

                case ArrowMode.White:
                    type = 16; // Cloud
                    scale = 1.3f;
                    break;

                case ArrowMode.Rain:
                    type = Mod.Find<ModDust>(nameof(RainbowDust)).Type;
                    scale = Burst ? 1.5f : 1.2f; // Bigger size if it's a burst of dust
                    if (!Burst) // X is random, Y follows the arrow
                    {
                        velocity = new Vector2(Main.rand.NextFloat(-0.25f, +0.25f), Projectile.velocity.Length() / 2);
                    }
                    break;
            }

            int dustAmount = Burst ? 10 : 1;
            for (int i = 0; i < dustAmount; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, type, 0f, 0f, newColor: Color, Scale: scale);
                if (velocity != Vector2.Zero) dust.velocity = velocity; // If a special velocity was set, apply it
                if (Mode == ArrowMode.Rain) dust.position.Y -= Projectile.height/2; // Trails behind
            }
        }


        public override void AI()
        {
            // On projectile spawn
            if (Projectile.timeLeft == Lifespan) 
            {
                switch (Mode)
                {
                    case ArrowMode.White:
                        Projectile.scale *= 1.4f;
                        Projectile.friendly = false; // Doesn't affect enemies
                        break;

                    case ArrowMode.Rain:
                        SpawnRainbowDust(Burst: true);
                        break;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + 90.ToRadians(); // Adds 90 degrees because of the sprite
            }

            // On every tick
            SpawnRainbowDust();
            Color color = Color;
            Lighting.AddLight(Projectile.Center, 0.7f*color.R/255, 0.7f*color.G/255, 0.7f*color.B/255);

            // White arrow effect
            if (Mode == ArrowMode.White && Projectile.timeLeft == Lifespan - RainDelay && Projectile.owner == Main.myPlayer) 
            {
                const int ArrowAmount = 30;
                float arrowSpacing = Projectile.width * 6f;
                int nextColor = Main.rand.Next(12); // Starts the rainbow at a random color

                for(int i = 1; i <= ArrowAmount; i++)
                {
                    Vector2 position = Projectile.Center;
                    position.X += -ArrowAmount*arrowSpacing/2 - arrowSpacing/2 + i*arrowSpacing; // Evenly spaced
                    position.Y += 120; // Distance below the original arrow

                    if (!Main.tile[(int)position.X/16, (int)position.Y/16].HasTile
                        || !Main.tile[(int)position.X/16, (int)position.Y/16].HasUnactuatedTile) // Only spawns if it's open space
                    {
                        var proj = Projectile.NewProjectileDirect(
                            position, new Vector2(0, Projectile.velocity.Length()), Mod.Find<ModProjectile>(nameof(RainbowArrow)).Type,
                            Projectile.damage, 0, Projectile.owner);
                        var arrow = proj.ModProjectile as RainbowArrow;
                        arrow.Mode = ArrowMode.Rain;
                        arrow.ColorId = nextColor;
                    }

                    if (nextColor < ArrowColors.Length - 1) nextColor++; // Cycles through colors
                    else nextColor = 0;
                }

                Projectile.Kill();
            }
        }


        public override void Kill(int timeLeft) => SpawnRainbowDust(Burst: true);


        public override Color? GetAlpha(Color newColor) => Color;
    }
}