using System;
using Terraria;
using Terraria.ID;
using Virtuous.Items;

namespace Virtuous.Utils
{
    public static class GobblerHelper
    {
        /// <summary>The size of the item's sprite from corner to corner.</summary> 
        public static float DiagonalSize(Item item)
        {
            return (float)Math.Sqrt(item.width*item.width + item.height*item.height);
        }


        /// <summary>Modifies the given ref damage to apply class damage bonuses based on the given item and player.</summary>
        public static void ApplyClassDamage(ref float damage, Item item, Player player)
        {
            if (item.melee)       damage *= player.meleeDamage;
            else if (item.ranged) damage *= player.rangedDamage;
            else if (item.magic)  damage *= player.magicDamage;
            else if (item.summon) damage *= player.minionDamage;
            else if (item.thrown) damage *= player.thrownDamage;
        }


        /// <summary>Final damage of the given item when being shot by the given player, affected by various factors.</summary>
        public static int ShotDamage(Item item, Player player)
        {
            float damage = TheGobbler.BaseDamage
                + item.damage
                + item.defense * 3f
                + item.value / 5000f
                + DiagonalSize(item) * 5f
                + (item.rare > 0 ? item.rare * 10 : 0);

            ApplyClassDamage(ref damage, item, player);
            return (int)damage;
        }


        /// <summary>Final knockback of the given item when being shot by the given player, affected by various factors.</summary>
        public static float ShotKnockBack(Item item, Player player)
        {
            float knockBack = TheGobbler.BaseKnockBack
                + item.knockBack
                + item.defense / 6f
                + DiagonalSize(item) / 10f
                + (item.createTile > 0 || item.createWall > 0 ? 5 : 0);

            if (player.kbGlove) knockBack *= 1.4f;
            if (player.kbBuff) knockBack *= 1.2f;

            return knockBack;
        }


        /// <summary>Whether the item is a tool.</summary>
        public static bool IsTool(Item item)
        {
            return item.pick > 0 || item.axe > 0 || item.hammer > 0;
        }


        /// <summary>Whether the specified item is (probably) a consumable explosive.</summary>
        public static bool IsExplosive(Item item)
        {
            return item.consumable && item.shoot > 0 && (item.damage <= 0 || item.useStyle == 5);
            //bool[] explosive = ItemID.Sets.Factory.CreateBoolSet(new int[] { ProjectileID.Dynamite, ProjectileID.BouncyDynamite, ProjectileID.StickyDynamite, ProjectileID.Bomb, ProjectileID.BouncyBomb, ProjectileID.StickyBomb, ProjectileID.Grenade, ProjectileID.BouncyGrenade, ProjectileID.StickyGrenade });
        }


        /// <summary>Whether the specified item will be lost upon being shot.</summary>
        public static bool IsDepletable(Item item)
        {
            if (item.type == ItemID.Gel || item.type == ItemID.FallenStar || (item.type >= ItemID.CopperCoin && item.type <= ItemID.PlatinumCoin)
                || item.createTile > 0 || item.createWall > 0 || item.potion || item.healLife > 0 || item.healMana > 0 || item.buffType > 0
                || item.InternalNameHas("BossBag", "TreasureBag")
            )
            {
                return false; // Exceptions
            }

            return item.consumable || item.ammo != 0 || item.type == ItemID.ExplosiveBunny;
        }
    }
}
