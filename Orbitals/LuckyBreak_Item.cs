using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class LuckyBreak_Item : OrbitalItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucky Break");
            Tooltip.SetDefault(
                $"The cards shuffle every few seconds, each giving individual effects\n" +
                $"Hearts increase movement speed and life regeneration\nDiamonds make enemies drop more coins\n" +
                $"Spades increase all critical strike chance by {LuckyBreak.CritBuff}%\n" +
                $"Clubs REDUCE all damage by {LuckyBreak.DamageDebuff}%\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Golpe de Suerte");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                $"Nueva baraja cada poco tiempo, cada carta dando efectos distintos\n" +
                $"Corazones aumentan la regeneración de vida y velocidad\nDiamantes entregan más monedas\n" +
                $"Picas aumentan el golpe crítico en {LuckyBreak.CritBuff}%\n" +
                $"Tréboles REDUCEN el daño en {LuckyBreak.DamageDebuff}%\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Lucky Break");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                $"Карты перемешиваются каждые несколько секунд, давая разные эффекты\n" +
                $"Червы увеличивают скорость передвижения и регенерации здоровья\nБубны увеличивают количество монет, выпадаемых с врагов\n" +
                $"Пики увеличивают шанс критического удара на {LuckyBreak.CritBuff}%\n" +
                $"Трефы уменьшают получаемый урон на {LuckyBreak.DamageDebuff}%\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "时来运转");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                $"卡牌会每隔几秒会自动洗牌,每一副牌都有不同的效果\n" +
                $"[红心]提高移动速度及生命再生\n[方块]使敌人掉落会更多钱币\n" +
                $"[黑桃]增加{LuckyBreak.CritBuff}%所有暴击率\n" +
                $"[梅花]减少{LuckyBreak.DamageDebuff}%所有伤害\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.LuckyBreak;
            Duration = 42 * 60;
            Amount = 5;

            Item.width = 40;
            Item.height = 34;
            Item.damage = 35;
            Item.knockBack = 2f;
            Item.mana = 50;
            Item.rare = 7;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.useStyle = 2;
            Item.useTime = 15;
            Item.useAnimation = Item.useTime;
        }
    }
}