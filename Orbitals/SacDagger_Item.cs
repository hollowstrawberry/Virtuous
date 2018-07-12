using System;
using Terraria;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class SacDagger_Item : OrbitalItem
    {
        private const int ManaCost = 50;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrificial Daggers");
            Tooltip.SetDefault(
                "\"Feed them\"\nThe daggers drain your life, but heal you when harming an enemy\n" +
                "Use again after summoning to spin and reset duration\nAligns with either magic or melee users");

            DisplayName.AddTranslation(GameCulture.Spanish, "Dagas de Sacrificio");
            Tooltip.AddTranslation(GameCulture.Spanish,
                "\"Dales de beber\"\nLas dagas succionan tu fuerza vital, pero te sanan al tocar enemigos\n" +
                "Vuelve a usar el objeto para hacer girar las dagas y reiniciar su duración\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.Russian, "Жертвенные Кинжалы");
            Tooltip.AddTranslation(GameCulture.Russian,
                "\"Накорми их\"\nКинжалы высасывают ваше здоровье, но крадут его у врагов\n" +
                "Используйте повторно, чтобы раскрутить и сбросить время действия\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.Chinese, "牺牲匕首");
            Tooltip.AddTranslation(GameCulture.Chinese,
                "\"喂养它鲜血\"\n匕首会吞噬你的生命,但攻击敌人时会为你吸血\n" +
                "再次召唤会旋转匕首并重置持续时间\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            type = OrbitalID.SacDagger;
            duration = 20 * 60;
            amount = 2;
            specialType = SpecialType.Reuse;

            item.width = 30;
            item.height = 30;
            item.damage = 180;
            item.knockBack = 5f;
            item.mana = ManaCost; // Overwritten by CanUseItem
            item.rare = 8;
            item.value = Item.sellPrice(0, 40, 0, 0);
            item.autoReuse = true;
            item.useStyle = 4;
            item.useTime = 16;
            item.useAnimation = item.useTime;
            item.useTurn = false;
        }


        public override bool CanUseItem(Player player)
        {
            item.mana = player.GetModPlayer<OrbitalPlayer>().active[type]
                ? (int)Math.Ceiling(ManaCost / 5f) // There's already a dagger
                : ManaCost; // No dagger active

            return base.CanUseItem(player);
        }
    }
}
