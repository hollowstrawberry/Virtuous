using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Virtuous.Projectiles;


namespace Virtuous.Items
{
    class SpotterGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spotter");
            Tooltip.SetDefault("\"They won't see it coming\"");
            DisplayName.AddTranslation(GameCulture.Spanish, "Pistola Señuelo");
            Tooltip.AddTranslation(GameCulture.Spanish, "\"No lo verán venir\"");
            //DisplayName.AddTranslation(GameCulture.Russian, "");
            //Tooltip.AddTranslation(GameCulture.Russian, "");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useStyle = 5;
            item.useTime = 18;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item40;
            item.damage = 150;
            item.crit = 15;
            item.knockBack = 2.5f;
            item.shoot = 1;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 12f;
            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.rare = 8;
            item.value = Item.sellPrice(0, 15, 0, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile newProj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            newProj.GetGlobalProjectile<VirtuousProjectile>().spotter = true; //Projectile carries the spotter flag
            return false;
        }

        //Crosshair spawning code in VirtuousProjectile
    }
}
