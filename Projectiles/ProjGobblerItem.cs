using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Utils;

namespace Virtuous.Projectiles
{
    public class ProjGobblerItem : ModProjectile
    {
        private const int Lifespan = 10 * 60;


        public GobblerStoredItem GobblerItem // Set when creating the projectile
        {
            get { return new GobblerStoredItem(ItemType, Prefix); }
            set
            {
                ItemType = value.type;
                Prefix = value.prefix;
            }
        }

        public bool Consumed // Whether the item was consumed and will drop from this projectile. Stored as ai[1]
        {
            get { return Projectile.ai[1] != 0; }
            set { Projectile.ai[1] = value ? 1 : 0; }
        }

        private int ItemType // Stored as ai[0]
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        private byte Prefix // Stored as localAI[0]
        {
            get { return (byte)Projectile.localAI[0]; }
            set { Projectile.localAI[0] = value; }
        }


        private Item _internalItem = null;
        private Item StoredItem => _internalItem ?? (_internalItem = GobblerItem.MakeItem());




        // If the item is a tool, it will bounce off in the direction it came from when specified
        private bool ToolBounce()
        {
            if (GobblerHelper.IsTool(StoredItem))
            {
                if (Projectile.penetrate > 1)
                {
                    if (StoredItem.UseSound != null)
                    {
                        SoundEngine.PlaySound(StoredItem.UseSound, Projectile.Center);
                    }
                    Projectile.penetrate--;
                    Projectile.velocity *= new Vector2(-0.5f, -0.5f);
                    Projectile.netUpdate = true; // Sync to multiplayer just in case
                    return true;
                }
                else Projectile.Kill();
            }

            return false;
        }


        // If the item is a magic weapon it will explode when the projectile dies
        private void MagicExplode()
        {
            if (!StoredItem.magic) return;

            if (Main.myPlayer == Projectile.owner)
            {
                Tools.ResizeProjectile(Projectile.whoAmI, Projectile.width + 50, Projectile.height + 50);
                Projectile.Damage(); // Applies damage in the area
                Projectile.netUpdate = true;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < Math.Max(4, (int)GobblerHelper.DiagonalSize(StoredItem) / 4); i++) // More dust the higher the size
            {
                Gore.NewGore(
                    Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width),
                    Main.rand.NextFloat(Projectile.height)), Vector2.Zero, Main.rand.Next(61, 64), Main.rand.NextFloat(0.2f, 1.2f));

                Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    /*Type*/31, 0f, 0f, 0, default(Color), Main.rand.NextFloat(2));
            }
        }


        // When the projectile dies and the stored item was set to consumed
        private void ItemConsume()
        {
            if (!Consumed) return;

            Player player = Main.player[Projectile.owner];

            if (GobblerHelper.IsDepletable(StoredItem)) // Item won't be returned
            {
                if (StoredItem.buffTime != 0) // Buff-giving consumables
                {
                    player.AddBuff(StoredItem.buffType, StoredItem.buffTime);
                }
                if (StoredItem.makeNPC != 0 && Main.netMode != NetmodeID.MultiplayerClient) // Critters
                {
                    NPC.NewNPC((int)(Projectile.Center.X), (int)(Projectile.Center.Y), StoredItem.makeNPC);
                }
            }
            else // Drops the stored item
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // Will be synced from the server in multiplayer
                {
                    Item.NewItem(Projectile.Center, StoredItem.type, prefixGiven: StoredItem.prefix);
                }
            }
        }


        // When this projectile dies it can shoot out what the stored item would shoot
        private void ItemShoot()
        {
            if (Projectile.owner != Main.myPlayer) return; // It'll get synced in multiplayer

            Player player = Main.player[Projectile.owner];

            // Orbital behavior
            var orbitalItem = StoredItem.ModItem as OrbitalItem;
            if (orbitalItem != null)
            {
                Vector2 position = player.Center;
                Vector2 velocity = Vector2.Zero;
                int type = orbitalItem.Item.shoot;
                int damage = StoredItem.damage;
                orbitalItem.GetWeaponDamage(player, ref damage);

                orbitalItem.Shoot(player, ref position, ref velocity.X, ref velocity.Y, ref type, ref damage, ref StoredItem.knockBack);
                return;
            }

            // Shooting exceptions
            else if (StoredItem.type == ItemID.ExplosiveBunny) // I love these, and they don't have a value for shoot
            {
                var proj = Projectile.NewProjectileDirect(
                    Projectile.Center, Vector2.Zero, ProjectileID.ExplosiveBunny,
                    StoredItem.damage, Projectile.knockBack, player.whoAmI);
                proj.timeLeft = 3;
            }

            // General shooting behavior
            else if (StoredItem.shoot > 0 && StoredItem.ammo == 0 && StoredItem.type != ItemID.StardustDragonStaff) // Stardust dragon was very buggy so it's excluded
            {
                // Summon and pet behavior
                if (StoredItem.buffType != 0)
                {
                    if (player.FindBuffIndex(StoredItem.buffType) < 0)
                    {
                        player.AddBuff(StoredItem.buffType, 60); // Player doesn't have the buff active, adds it
                    }
                    else // Player has the corresponding buff active
                    {
                        var summon = Main.projectile.First(x => x.active && x.type == StoredItem.shoot && x.owner == Projectile.owner);
                        summon.Center = Projectile.Center; // Teleports the pet or summon to this projectile
                        if (StoredItem.damage <= 0) return; // Does nothing else if it's a pet
                    }
                }


                // Get projectile amount
                int projAmount = 1;
                if (StoredItem.damage > 0 && !StoredItem.consumable && (!StoredItem.melee || StoredItem.useStyle == 1)
                    || StoredItem.type == ItemID.Clentaminator)
                {
                    projAmount = Main.rand.Next(4, 7); // Weapons excluding non-swingable melee weapons shoot a random amount of projectiles
                }
                if (StoredItem.ranged)
                {
                    projAmount += projAmount / 2 + (int)(60f / StoredItem.useAnimation); // Ranged weapons shoot a lot more based on their speed
                }

                // Get projectile type. Some guns and dart guns don't have the right values so we overwrite them
                int projType = StoredItem.shoot;
                if (projType == 10 && StoredItem.useAmmo == AmmoID.Bullet) projType = ProjectileID.Bullet;
                else if (StoredItem.useAmmo == AmmoID.Dart) projType = ProjectileID.PoisonDart;

                // Get projectile damage
                float damage = StoredItem.damage;
                GobblerHelper.ApplyClassDamage(ref damage, StoredItem, player);


                // Spawning the projectiles
                Vector2 startingRotation = Main.rand.NextVector2();
                for (int i = 0; i < projAmount; i++)
                {
                    Vector2 rotation = startingRotation.RotatedBy(Tools.FullCircle * i / projAmount); // In a circle
                    Vector2 position = Projectile.Center + rotation;
                    Vector2 velocity = rotation * StoredItem.shootSpeed;

                    var proj = Projectile.NewProjectileDirect(
                        position, velocity, projType, (int)damage, StoredItem.knockBack, player.whoAmI);

                    if (GobblerHelper.IsExplosive(StoredItem)) proj.timeLeft = 3; // Explodes immediately

                    if (StoredItem.sentry) // Turrets
                    {
                        proj.position.Y -= 20; // So it doesn't sink into the ground
                        proj.position.X += Main.rand.Next(-20, 21);
                        player.UpdateMaxTurrets();
                    }
                    proj.friendly = true;
                    proj.hostile = false;
                }
            }
        }




        ///* tModLoader methods and projectile behavior *///

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Item");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Objeto");
        }


        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.damage = 100;
            Projectile.alpha = 0;
            Projectile.timeLeft = Lifespan;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true; // Hits go by individual projectile
            Projectile.localNPCHitCooldown = 10;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // First tick
            if (Projectile.timeLeft == Lifespan && StoredItem.type != ItemID.None)
            {
                // Inherit properties from the stored item
                Tools.ResizeProjectile(Projectile.whoAmI, StoredItem.width, StoredItem.height);
                Projectile.damage = GobblerHelper.ShotDamage(StoredItem, player);
                Projectile.knockBack = GobblerHelper.ShotKnockBack(StoredItem, player);
                Projectile.melee  = StoredItem.melee;
                Projectile.magic  = StoredItem.magic;
                Projectile.ranged = StoredItem.ranged;
                Projectile.Name = StoredItem.Name;

                // Transparency; copies appear translucent
                if (!Consumed && !GobblerHelper.IsDepletable(StoredItem))
                {
                    Projectile.alpha = 120;
                }

                // Items without gravity
                if (ItemID.Sets.ItemNoGravity[StoredItem.type]) 
                {
                    Projectile.timeLeft = 2 * 60;
                    Projectile.penetrate = -1;
                }

                // Penetration
                if (StoredItem.melee) Projectile.penetrate = -1; // Melee weapons penetrate infinitely
                if (GobblerHelper.IsTool(StoredItem)) Projectile.penetrate = 3; // But tools penetrate only a bit
                else if (StoredItem.accessory) Projectile.penetrate = 2;

                Projectile.netUpdate = true; // Sync to multiplayer
            }

            // Every tick: Projectile rotation and movement
            if (!ItemID.Sets.ItemNoGravity[StoredItem.type])
            {
                Projectile.velocity.Y += GobblerHelper.DiagonalSize(StoredItem) / 200f; // How fast they fall down depends on their size

                // Swingable weapons and projectiles point to where they're going, adjusted for sprite orientation
                if (StoredItem.useStyle == 1 && !StoredItem.consumable && !GobblerHelper.IsTool(StoredItem))
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + 45.ToRadians();
                }
                else if (StoredItem.ammo > 0)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    if (StoredItem.ammo == AmmoID.Arrow) Projectile.rotation += -90.ToRadians();
                    else if (StoredItem.ammo == AmmoID.Bullet || StoredItem.ammo == AmmoID.Dart) Projectile.rotation += 90.ToRadians();
                    else if (StoredItem.ammo == AmmoID.StyngerBolt || StoredItem.ammo == AmmoID.CandyCorn) Projectile.rotation += 45.ToRadians();
                }
                else Projectile.rotation += Tools.RevolutionPerSecond; // The rest of the items spin
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            if (ToolBounce()) return false;

            return base.OnTileCollide(oldVelocity); // Die
        }


        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            ItemShoot();
            MagicExplode();
            ItemConsume();

            if (StoredItem.UseSound != null) SoundEngine.PlaySound(StoredItem.UseSound, Projectile.Center);
        }


        public override bool? CanHitNPC(NPC target)
        {
            bool canHit = Projectile.CanHit(target);

            if (canHit && target.type == StoredItem.makeNPC) return false; // Critters can't hit themselves

            return canHit;
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ToolBounce();

            if (StoredItem.InternalNameHas("banner")) return;
            
            // Generic effects based on keywords
            if (StoredItem.InternalNameHas("slime", "gel")) target.AddBuff(BuffID.Slimed, 120);
            if (StoredItem.InternalNameHas("water")) target.AddBuff(BuffID.Wet, 120);
            if (StoredItem.InternalNameHas("fire", "flame", "magma", "lava", "torch", "solar")) target.AddBuff(BuffID.OnFire, 120);
            if (StoredItem.InternalNameHas("ice")) target.AddBuff(BuffID.Chilled, 120);
            if (StoredItem.InternalNameHas("frost")) target.AddBuff(BuffID.Frostburn, 120);
            if (StoredItem.InternalNameHas("ichor")) target.AddBuff(BuffID.Ichor, 120);
            if (StoredItem.InternalNameHas("cursed")) target.AddBuff(BuffID.CursedInferno, 120);
            if (StoredItem.InternalNameHas("shadowflame")) target.AddBuff(BuffID.ShadowFlame, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            ToolBounce();

            if (StoredItem.InternalNameHas("banner")) return;

            if (StoredItem.InternalNameHas("slime", "gel")) target.AddBuff(BuffID.Slimed, 120);
            if (StoredItem.InternalNameHas("water")) target.AddBuff(BuffID.Wet, 120);
            if (StoredItem.InternalNameHas("fire", "flame", "magma", "lava", "torch", "solar")) target.AddBuff(BuffID.OnFire, 120);
            if (StoredItem.InternalNameHas("ice")) target.AddBuff(BuffID.Chilled, 120);
            if (StoredItem.InternalNameHas("frost")) target.AddBuff(BuffID.Frostburn, 120);
            if (StoredItem.InternalNameHas("ichor")) target.AddBuff(BuffID.Ichor, 120);
            if (StoredItem.InternalNameHas("cursed")) target.AddBuff(BuffID.CursedInferno, 120);
            if (StoredItem.InternalNameHas("shadowflame")) target.AddBuff(BuffID.ShadowFlame, 120);

            if (StoredItem.vanity) target.AddBuff(BuffID.Slow, 120);
            if (StoredItem.buffType != 0) target.AddBuff(StoredItem.buffType, 120); // Why not
        }




        public override bool PreDraw(ref Color lightColor) // Draws the stored item's texture as the projectile's
        {
            if (StoredItem.type != ItemID.None) 
            {
                Texture2D texture = TextureAssets.Item[StoredItem.type].Value;
                Rectangle? frame = null; // Which part of the texture to load, the sourceRect
                Vector2 drawOrigin = texture.Size() / 2; // Center of the texture
                Vector2 position = Projectile.Center - Main.screenPosition; // Where to draw the texture

                if (Main.itemAnimations[StoredItem.type] != null) // For animated items, gets the current frame
                {
                    int frameCount = Main.itemAnimations[StoredItem.type].FrameCount;
                    int frameDelay = Main.itemAnimations[StoredItem.type].TicksPerFrame;
                    int currentFrame = (int)(Main.GameUpdateCount / frameDelay) % frameCount; // Cycles through the frames at a constant pace

                    frame = texture.Frame(1, frameCount, 0, currentFrame);
                    drawOrigin.Y /= frameCount;
                }

                if (StoredItem.color != default(Color))
                {
                    lightColor = Color.Lerp(lightColor, StoredItem.color, 0.75f); // Gel gains its blue, etc.
                }
               
                spriteBatch.Draw(
                    texture, position, frame, lightColor * Projectile.Opacity,
                    Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

                return false;
            }

            return true; // Without an item, it has the default texture
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return StoredItem.GetAlpha(lightColor);
        }
    }
}
