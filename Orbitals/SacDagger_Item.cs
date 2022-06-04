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

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Dagas de Sacrificio");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish),
                "\"Dales de beber\"\nLas dagas succionan tu fuerza vital, pero te sanan al tocar enemigos\n" +
                "Vuelve a usar el objeto para hacer girar las dagas y reiniciar su duración\nEl daño se alínea con magia o cuerpo a cuerpo");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Жертвенные Кинжалы");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian),
                "\"Накорми их\"\nКинжалы высасывают ваше здоровье, но крадут его у врагов\n" +
                "Используйте повторно, чтобы раскрутить и сбросить время действия\nПодходит воинам и магам");

            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "牺牲匕首");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese),
                "\"喂养它鲜血\"\n匕首会吞噬你的生命,但攻击敌人时会为你吸血\n" +
                "再次召唤会旋转匕首并重置持续时间\n更适合战士与法师使用");
        }


        public override void SetOrbitalDefaults()
        {
            OrbitalType = OrbitalID.SacDagger;
            Duration = 15 * 60;
            Amount = 2;
            Special = SpecialType.Reuse;

            Item.width = 30;
            Item.height = 30;
            Item.damage = 180;
            Item.knockBack = 5f;
            Item.mana = ManaCost; // Overwritten by CanUseItem
            Item.rare = 8;
            Item.value = Item.sellPrice(0, 40, 0, 0);
            Item.autoReuse = true;
            Item.useStyle = 4;
            Item.useTime = 16;
            Item.useAnimation = Item.useTime;
            Item.useTurn = false;
        }


        public override bool CanUseItem(Player player)
        {
            Item.mana = player.GetModPlayer<OrbitalPlayer>().active[OrbitalType]
                ? (int)Math.Ceiling(ManaCost / 5f) // There's already a dagger
                : ManaCost; // No dagger active

            return base.CanUseItem(player);
        }
    }
}
