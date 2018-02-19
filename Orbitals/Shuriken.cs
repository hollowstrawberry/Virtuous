using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Virtuous.Orbitals
{
    public class Shuriken_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
			Tooltip.SetDefault("Shuriken defend you, raising melee speed and mana regeneration\nAligns with either magic or melee users");
            DisplayName.AddTranslation(GameCulture.Spanish, "Crepúsculo");
            Tooltip.AddTranslation(GameCulture.Spanish, "Los Shuriken te defenderán y aumentarán la velocidad cuerpo a cuerpo y regeneración de maná\nEl daño se alínea con magia o cuerpo a cuerpo");
            DisplayName.AddTranslation(GameCulture.Russian, "Сумерки");
            Tooltip.AddTranslation(GameCulture.Russian, "Сюрикены защищают вас, увеличивая скорость ближнего боя и регенерации маны\nПодходит воинам и магам");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Shuriken;
            duration = 35 * 60;
            amount = 3;

            item.width = 30;
            item.height = 30;
            item.damage = 190;
            item.knockBack = 6.2f;
            item.mana = 70;
            item.rare = 9;
            item.value = Item.sellPrice(0, 60, 0, 0);
        }
    }


    public class Shuriken_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Shuriken;
        public override int DyingTime => 30;
        public override float BaseDistance => 120;
        public override float OrbitingSpeed => 1 * Tools.RevolutionPerSecond;
        public override float RotationSpeed => -2 * OrbitingSpeed;
        public override float OscillationSpeedMax => 6.0f;
        public override float OscillationAcc => OscillationSpeedMax / 20;

        private const float DyingOrbitingSpeed = 4 * Tools.RevolutionPerSecond;
        private const float DyingRotationSpeed = -2 * DyingOrbitingSpeed;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Shuriken");
			DisplayName.AddTranslation(GameCulture.Russian, "Призванный Сюрикен");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
        }

        public override void PlayerEffects()
        {
            player.manaRegenDelayBonus++;
            player.manaRegenBonus += 25;
            player.meleeSpeed += 0.12f;
        }

        public override void DyingFirstTick()
        {
            projectile.damage *= 3;
            relativeDistance = BaseDistance / 2;
        }

        public override void Dying()
        {
            AddDistance(OscillationSpeedMax); //Expands outwards
            RotatePosition(DyingOrbitingSpeed); //Spins
            projectile.rotation += DyingRotationSpeed; //Rotates the sprite as well

            if (projectile.timeLeft == 1) //Last tick
            {
                Main.PlaySound(SoundID.Dig, projectile.Center); //Thump

                for (int i = 0; i < 10; i++)
                {
                    Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, /*Type*/74, 0f, 0f, /*Alpha*/150, new Color(50, 255, 100, 150), /*Scale*/1.5f);
                    newDust.velocity += relativePosition.Perpendicular(10, clockwise: false); //Tangent linear velocity to the angular velocity
                }
            }
        }

        public override void PostAll()
        {
            Lighting.AddLight(projectile.Center, 0.0f, 1.0f, 0.2f);
        }


        public override Color? GetAlpha(Color newColor)
        {
            return new Color(10, 255, 50, 150) * projectile.Opacity;
        }
    }
}
