using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class HolyLight_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Circle of Protection");
            Tooltip.SetDefault("Holy lights surround you and increase life regeneration\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Círculo Sagrado");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Luces santas te rodean y aumentan la regeneración de vida\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Круг Защиты");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Святые огни окружают вас, увеличивая регенерацию здоровья\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "圣光庇护");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "圣光将围绕着你,提高生命再生\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.HolyLight;
            Duration = 45 * 60;
            Amount = 6;

            Item.width = 30;
            Item.height = 30;
            Item.damage = 100;
            Item.knockBack = 3f;
            Item.mana = 60;
            Item.rare = 8;
            Item.value = Item.sellPrice(0, 40, 0, 0);
        }
    }
}
