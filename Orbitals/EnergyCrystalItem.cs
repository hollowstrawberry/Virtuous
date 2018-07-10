using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class EnergyCrystalItem : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Crystal");
            Tooltip.SetDefault(
                "The crystals fire at nearby enemies\nGetting hurt causes a momentary overdrive\n" +
                "Aligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Cristal de Energía");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "Los cristales dispararán a enemigos cercanos\nSer herido los sobrecalentará por un momento\n" +
                "El daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Энергетический Кристалл");
            Tooltip.AddTranslation(GameCulture.Russian,
                "Кристаллы стреляют по ближайшим врагам\nПолучение урона вызывает короткую перегрузку\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "能量水晶");
            Tooltip.AddTranslation(GameCulture.Chinese,
                "能量水晶会向附近的敌人射击\n受到伤害会瞬间过载\n更适合战士或法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.EnergyCrystal;
            duration = 30 * 60;
            amount = 5;

            item.width = 18;
            item.height = 32;
            item.damage = 55;
            item.knockBack = 2f;
            item.mana = 40;
            item.rare = 6;
            item.value = Item.sellPrice(0, 8, 0, 0);
        }
    }
}
