using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Virtuous.Orbitals
{
    public class HolyLight_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Circle of Protection");
            Tooltip.SetDefault("Holy lights surround you and increase life regeneration\nAligns with either magic or melee users");
            DisplayName.AddTranslation(GameCulture.Spanish, "Círculo Sagrado");
            Tooltip.AddTranslation(GameCulture.Spanish, "Luces santas te rodean y aumentan la regeneración de vida\nEl daño se alínea con magia o cuerpo a cuerpo");
            DisplayName.AddTranslation(GameCulture.Russian, "Круг Защиты");
            Tooltip.AddTranslation(GameCulture.Russian, "Святые огни окружают вас, увеличивая регенерацию здоровья\nПодходит воинам и магам");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.HolyLight;
            duration = 30 * 60;
            amount = 6;

            item.width = 30;
            item.height = 30;
            item.damage = 100;
            item.knockBack = 3f;
            item.mana = 60;
            item.rare = 8;
            item.value = Item.sellPrice(0, 40, 0, 0);
        }
    }


    public class HolyLight_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.HolyLight;
        public override int DyingTime => 10; //Time it spends bursting
        public override float BaseDistance => 70;
        public override float OrbitingSpeed => 1 / 30f * Tools.RevolutionPerSecond;
        public override float RotationSpeed => -OrbitingSpeed;
        public override float OscillationSpeedMax => 0.2f;
        public override float OscillationAcc => OscillationSpeedMax / 60;

        private const int OriginalSize = 30; //Size of the sprite
        private const int BurstSize = 120; //Size of the area where bursting causes damage


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
			DisplayName.AddTranslation(GameCulture.Russian, "Святое Сияние");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = OriginalSize;
            projectile.height = OriginalSize;
        }

        public override void PlayerEffects()
        {
            player.lifeRegen += 5;

            if (Main.rand.OneIn(6))
            {
                Dust newDust = Dust.NewDustDirect(player.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, default(Color), /*Scale*/0.5f);
                newDust.velocity = Main.rand.NextVector2(4, 5); // Random direction, random magnitude
                newDust.position -= newDust.velocity.OfLength(50f); //Sets the distance in a position where it will move towards the player
                newDust.velocity += player.velocity; //Follows the player somewhat
                newDust.noGravity = true;
                newDust.fadeIn = 1.3f;
            }
        }

        public override void FirstTick()
        {
            base.FirstTick();

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/55, 0f, 0f, /*Alpha*/50, default(Color), Main.rand.NextFloat(1.2f, 1.5f));
                newDust.velocity *= 0.8f;
                newDust.noLight = false;
                newDust.noGravity = true;
            }
        }

        public override bool PreMovement()
        {
            return true; //Never stops moving even while dying
        }

        public override void DyingFirstTick()
        {
            projectile.alpha = 255; //Transparent
            Main.PlaySound(SoundID.Item14, projectile.Center); //Explosion

            if (Main.myPlayer == projectile.owner) Tools.ResizeProjectile(projectile.whoAmI, BurstSize, BurstSize);

            for (int i = 0; i < 15; i++) //Dust
            {
                Dust newDust = Dust.NewDustDirect(projectile.Center, 0, 0, /*Type*/55, 0f, 0f, /*Alpha*/200, new Color(255, 230, 100), /*Scale*/1.0f);
                newDust.velocity *= 2;
            }
        }

        public override void Dying() //Nothing
        {
        }

        public override void PostAll()
        {
            if (IsDying) Lighting.AddLight(projectile.Center, 2.0f, 2.0f, 1.2f);
            else Lighting.AddLight(projectile.Center, 1.0f, 1.0f, 0.6f);
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 3;
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 3;
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(255, 255, 255, 50) * projectile.Opacity;
        }
    }
}
