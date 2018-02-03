using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Virtuous.Orbitals
{
    public class Facade_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Facade");
			Tooltip.SetDefault("Summons barriers to protect you for a short time\nAligns with either magic or melee users");
            DisplayName.AddTranslation(GameCulture.Spanish, "Tapia");
            Tooltip.AddTranslation(GameCulture.Spanish, "Invoca barreras para protegerte por un corto tiempo\nEl daño se alínea con magia o cuerpo a cuerpo");
            DisplayName.AddTranslation(GameCulture.Russian, "Преграда");
            Tooltip.AddTranslation(GameCulture.Russian, "Призывает временные защитные барьеры\nПодходит воинам и магам");
        }

        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.Facade;
            duration = 15 * 60;
            amount = 4;

            item.width = 30;
            item.height = 30;
            item.damage = 20;
            item.knockBack = 2.0f;
            item.mana = 30;
            item.rare = 4;
            item.value = Item.sellPrice(0, 5, 0, 0);
        }
    }


    public class Facade_Proj : OrbitalProjectile
    {
        public override int Type => OrbitalID.Facade;
        public override int OriginalAlpha => 80;
        public override int FadeTime => 30;
        public override float BaseDistance => 50;
        public override float OscillationSpeedMax => 1 / 15f * Tools.RevolutionPerSecond;
        public override float OscillationAcc => OscillationSpeedMax / 60;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoned Barrier");
			DisplayName.AddTranslation(GameCulture.Russian, "Призванный Барьер");
        }

        public override void SetOrbitalDefaults()
        {
            projectile.width = 18;
            projectile.height = 44;
        }

        public override void PostMovement()
        {
            relativeDistance -= oscillationSpeed; //Undoes the distance oscillation
            RotatePosition(oscillationSpeed); //Applies it as angular oscillation
            projectile.rotation += oscillationSpeed; //Rotates the sprite
        }

        public override Color? GetAlpha(Color newColor)
        {
            return new Color(250, 233, 0, 100) * projectile.Opacity;
        }
    }
}

