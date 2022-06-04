using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class Facade_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Facade");
            Tooltip.SetDefault("Summons barriers to protect you for a short time\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Tapia");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Invoca barreras para protegerte por un corto tiempo\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Преграда");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Призывает временные защитные барьеры\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "禁制屏障");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "召唤障碍来保护你一段时间\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.Facade;
            Duration = 20 * 60;
            Amount = 4;

            Item.width = 30;
            Item.height = 30;
            Item.damage = 20;
            Item.knockBack = 2.0f;
            Item.mana = 30;
            Item.rare = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }
    }
}

