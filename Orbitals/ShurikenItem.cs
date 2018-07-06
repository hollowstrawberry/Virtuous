using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Shuriken_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
            Tooltip.SetDefault(
                "Shuriken defend you, raising melee speed and mana regeneration\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Crepúsculo");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "Los Shuriken te defenderán y aumentarán la velocidad cuerpo a cuerpo y regeneración de maná\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Сумерки");
            Tooltip.AddTranslation(GameCulture.Russian,
                "Сюрикены защищают вас, увеличивая скорость ближнего боя и регенерации маны\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "暮色旋镖");
            Tooltip.AddTranslation(GameCulture.Chinese,
                "暮色旋镖将保护你,提高近战攻速及法力再生\n" +
                "更适合战士与法师使用");
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
}
