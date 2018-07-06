using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class FacadeItem : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Facade");
			Tooltip.SetDefault("Summons barriers to protect you for a short time\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Tapia");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "Invoca barreras para protegerte por un corto tiempo\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Преграда");
            Tooltip.AddTranslation(GameCulture.Russian,
                "Призывает временные защитные барьеры\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "禁制屏障");
			Tooltip.AddTranslation(GameCulture.Chinese,
                "召唤障碍来保护你一段时间\n更适合战士与法师使用");
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
}

