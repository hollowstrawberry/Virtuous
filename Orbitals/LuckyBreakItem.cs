using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class LuckyBreakItem : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucky Break");
            Tooltip.SetDefault(
                $"The cards shuffle every few seconds, each giving individual effects\n" +
                $"Hearts increase movement speed and life regeneration\nDiamonds make enemies drop more coins\n" +
                $"Spades increase all critical strike chance by {LuckyBreak.CritBuff}%\n" +
                $"Clubs REDUCE all damage by {LuckyBreak.DamageDebuff}%\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Golpe de Suerte");
            Tooltip.AddTranslation(GameCulture.Spanish,
                $"Nueva baraja cada poco tiempo, cada carta dando efectos distintos\n" +
                $"Corazones aumentan la regeneración de vida y velocidad\nDiamantes entregan más monedas\n" +
                $"Picas aumentan el golpe crítico en {LuckyBreak.CritBuff}%\n" +
                $"Tréboles REDUCEN el daño en {LuckyBreak.DamageDebuff}%\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Lucky Break");
            Tooltip.AddTranslation(GameCulture.Russian,
                $"Карты перемешиваются каждые несколько секунд, давая разные эффекты\n" +
                $"Червы увеличивают скорость передвижения и регенерации здоровья\nБубны увеличивают количество монет, выпадаемых с врагов\n" +
                $"Пики увеличивают шанс критического удара на {LuckyBreak.CritBuff}%\n" +
                $"Трефы уменьшают получаемый урон на {LuckyBreak.DamageDebuff}%\nПодходит воинам и магам");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.LuckyBreak;
            duration = 42 * 60;
            amount = 5;

            item.width = 40;
            item.height = 34;
            item.damage = 35;
            item.knockBack = 2f;
            item.mana = 50;
            item.rare = 7;
            item.value = Item.sellPrice(0, 20, 0, 0);
            item.useStyle = 2;
            item.useTime = 15;
            item.useAnimation = item.useTime;
        }
    }
}