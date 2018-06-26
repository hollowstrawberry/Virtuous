using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Virtuous.Orbitals
{
    class Fireball_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Tooltip.SetDefault("Burn nearby enemies\nRight-Click after summoning for a fire burst\nAligns with either magic or melee users");
            DisplayName.AddTranslation(GameCulture.Spanish, "Bola de Fuego");
            Tooltip.AddTranslation(GameCulture.Spanish, "Quema a los enemigos cercanos\nHaz Click Derecho tras invocarla para una pequeña explosión\nEl daño se alínea con magia o cuerpo a cuerpo");
            DisplayName.AddTranslation(GameCulture.Russian, "Огненный Шар");
            Tooltip.AddTranslation(GameCulture.Russian, "Поджигает ближайших врагов\nПКМ после вызова для всплеска огня\nПодходит воинам и магам");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Fireball;
            duration = 20 * 60;
            amount = 1;
            specialType = SpecialType.RightClick;

            item.width = 30;
            item.height = 30;
            item.damage = 50;
            item.knockBack = 3f;
            item.mana = 20;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = 5;
            item.useTime = 50;
            item.useAnimation = item.useTime;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.TatteredCloth, 1);
            recipe.AddTile(TileID.Books);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }


    class Fireball_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Fireball;
        public override int DyingTime => 60;
        public override int FadeTime => DyingTime;
        public override float BaseDistance => 90;
        public override float OrbitingSpeed => 0.5f * Tools.RevolutionPerSecond;
        public override float RotationSpeed => OrbitingSpeed;

        private const int OriginalSize = 30;
        private const int BurstSize = OriginalSize * 4;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = OriginalSize;
            projectile.height = OriginalSize;
        }


        private void MakeDust()
        {
            for (int i = 0; i < 7; i++) 
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[] { DustID.Fire, DustID.SolarFlare, 158 });
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, projectile.alpha, default(Color), 1f);
                if (newDust.type == DustID.SolarFlare) newDust.scale = 1.5f;
                else newDust.velocity *= 2;
                newDust.noGravity = true;
            }
        }


        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 15; i++)
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare);
                newDust.velocity *= 2;
            }
        }


        public override bool PreMovement()
        {
            return true; //Always move even during special effect or death
        }


        public override void Movement()
        {
            base.Movement();
            MakeDust();
        }


        public override void SpecialFunction()
        {
            if (specialFunctionTimer == 0) // First tick
            {
                Tools.ResizeProjectile(projectile.whoAmI, BurstSize, BurstSize, true);
                Main.PlaySound(SoundID.Item14, projectile.Center);
                for (int i = 0; i < 6; i++)
                {
                    MakeDust();
                    Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, projectile.alpha, default(Color), 2.5f);
                    newDust.noGravity = true;
                }

                projectile.Damage(); // Damages enemies instantly
            }
            else if (specialFunctionTimer >= 5) // Last tick
            {
                Tools.ResizeProjectile(projectile.whoAmI, OriginalSize, OriginalSize, true);
                orbitalPlayer.SpecialFunctionActive = false;
            }
        }


        public override void DyingFirstTick()
        {
        }

        public override void Dying()
        {
            // Gains more orbiting speed
            float extraRotationFactor = 7f * (DyingTime - projectile.timeLeft) / DyingTime;
            RotatePosition(OrbitingSpeed * extraRotationFactor);
            projectile.rotation += RotationSpeed * extraRotationFactor;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying || IsDoingSpecial) damage *= 2;
            target.AddBuff(BuffID.OnFire, 5 * 60);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying || IsDoingSpecial) damage *= 2;
            target.AddBuff(BuffID.OnFire, 5 * 60);
        }
    }
}
