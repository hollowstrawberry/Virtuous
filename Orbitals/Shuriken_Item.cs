using Terraria;
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

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Crepúsculo");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Los Shuriken te defenderán y aumentarán la velocidad cuerpo a cuerpo y regeneración de maná\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Сумерки");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Сюрикены защищают вас, увеличивая скорость ближнего боя и регенерации маны\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "暮色旋镖");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "暮色旋镖将保护你,提高近战攻速及法力再生\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.Shuriken;
            Duration = 50 * 60;
            Amount = 3;

            Item.width = 30;
            Item.height = 30;
            Item.damage = 190;
            Item.knockBack = 6.2f;
            Item.mana = 70;
            Item.rare = 9;
            Item.value = Item.sellPrice(0, 60, 0, 0);
        }
    }
}
