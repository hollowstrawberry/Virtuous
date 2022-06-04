using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class EnergyCrystal_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Crystal");
            Tooltip.SetDefault(
                "The crystals fire at nearby enemies\nGetting hurt causes a momentary overdrive\n" +
                "Aligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Cristal de Energía");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "Los cristales dispararán a enemigos cercanos\nSer herido los sobrecalentará por un momento\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Энергетический Кристалл");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "Кристаллы стреляют по ближайшим врагам\nПолучение урона вызывает короткую перегрузку\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "能量水晶");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "能量水晶会向附近的敌人射击\n受到伤害会瞬间过载\n更适合战士或法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.EnergyCrystal;
            Duration = 30 * 60;
            Amount = 5;

            Item.width = 18;
            Item.height = 32;
            Item.damage = 55;
            Item.knockBack = 2f;
            Item.mana = 40;
            Item.rare = 6;
            Item.value = Item.sellPrice(0, 8, 0, 0);
        }
    }
}
