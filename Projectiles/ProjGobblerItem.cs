using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
            get { return projectile.ai[1] != 0; }
            set { projectile.ai[1] = value ? 1 : 0; }
        }

        private int ItemType // Stored as ai[0]
        {
            get { return (int)projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        private byte Prefix // Stored as localAI[0]
        {
            get { return (byte)projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        }


        private Item _internalItem = null;
        private Item StoredItem => _internalItem ?? (_internalItem = GobblerItem.MakeItem());




        // If the item is a tool, it will bounce off in the direction it came from when specified
        private bool ToolBounce()
        {
            if (GobblerHelper.IsTool(StoredItem))
            {
                if (projectile.penetrate > 1)
                {
                    if (StoredItem.UseSound != null)
                    {
                        Main.PlaySound(StoredItem.UseSound, projectile.Center);
                    }
                    projectile.penetrate--;
                    projectile.velocity *= new Vector2(-0.5f, -0.5f);
                    projectile.netUpdate = true; // Sync to multiplayer just in case
                    return true;
                }
                else projectile.Kill();
            }

            return false;
        }


        // If the item is a magic weapon it will explode when the projectile dies
        private void MagicExplode()
        {
            if (!StoredItem.magic) return;

            if (Main.myPlayer == projectile.owner)
            {
                Tools.ResizeProjectile(projectile.whoAmI, projectile.width + 50, projectile.height + 50);
                projectile.Damage(); // Applies damage in the area
                projectile.netUpdate = true;
            }

            Main.PlaySound(SoundID.Item14, projectile.Center);
            for (int i = 0; i < Math.Max(4, (int)GobblerHelper.DiagonalSize(StoredItem) / 4); i++) // More dust the higher the size
            {
                Gore.NewGore(
                    projectile.position + new Vector2(Main.rand.NextFloat(projectile.width),
                    Main.rand.NextFloat(projectile.height)), Vector2.Zero, Main.rand.Next(61, 64), Main.rand.NextFloat(0.2f, 1.2f));

                Dust.NewDust(
                    projectile.position, projectile.width, projectile.height,
                    /*Type*/31, 0f, 0f, 0, default(Color), Main.rand.NextFloat(2));
            }
        }


        // When the projectile dies and the stored item was set to consumed
        private void ItemConsume()
        {
            if (!Consumed) return;

            Player player = Main.player[projectile.owner];

            if (GobblerHelper.IsDepletable(StoredItem)) // Item won't be returned
            {
                if (StoredItem.buffTime != 0) // Buff-giving consumables
                {
                    player.AddBuff(StoredItem.buffType, StoredItem.buffTime);
                }
                if (StoredItem.makeNPC != 0 && Main.netMode != NetmodeID.MultiplayerClient) // Critters
                {
                    NPC.NewNPC((int)(projectile.Center.X), (int)(projectile.Center.Y), StoredItem.makeNPC);
                }
            }
            else // Drops the stored item
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // Will be synced from the server in multiplayer
                {
                    Item.NewItem(projectile.Center, StoredItem.type, prefixGiven: StoredItem.prefix);
                }
            }
        }


        // When this projectile dies it can shoot out what the stored item would shoot
        private void ItemShoot()
        {
            if (projectile.owner != Main.myPlayer) return; // It'll get synced in multiplayer

            Player player = Main.player[projectile.owner];

            // Orbital behavior
            var orbitalItem = StoredItem.modItem as OrbitalItem;
            if (orbitalItem != null)
            {
                Vector2 position = player.Center;
                Vector2 velocity = Vector2.Zero;
                int type = orbitalItem.item.shoot;
                int damage = StoredItem.damage;
                orbitalItem.GetWeaponDamage(player, ref damage);

                orbitalItem.Shoot(player, ref position, ref velocity.X, ref velocity.Y, ref type, ref damage, ref StoredItem.knockBack);
                return;
            }

            // Shooting exceptions
            else if (StoredItem.type == ItemID.ExplosiveBunny) // I love these, and they don't have a value for shoot
            {
                var proj = Projectile.NewProjectileDirect(
                    projectile.Center, Vector2.Zero, ProjectileID.ExplosiveBunny,
                    StoredItem.damage, projectile.knockBack, player.whoAmI);
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
                        var summon = Main.projectile.First(x => x.active && x.type == StoredItem.shoot && x.owner == projectile.owner);
                        summon.Center = projectile.Center; // Teleports the pet or summon to this projectile
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
                    Vector2 position = projectile.Center + rotation;
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
            DisplayName.AddTranslation(GameCulture.Spanish, "Objeto");
        }


        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.damage = 100;
            projectile.alpha = 0;
            projectile.timeLeft = Lifespan;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.usesLocalNPCImmunity = true; // Hits go by individual projectile
            projectile.localNPCHitCooldown = 10;
        }


        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            // First tick
            if (projectile.timeLeft == Lifespan && StoredItem.type != ItemID.None)
            {
                // Inherit properties from the stored item
                Tools.ResizeProjectile(projectile.whoAmI, StoredItem.width, StoredItem.height);
                projectile.damage = GobblerHelper.ShotDamage(StoredItem, player);
                projectile.knockBack = GobblerHelper.ShotKnockBack(StoredItem, player);
                projectile.melee  = StoredItem.melee;
                projectile.magic  = StoredItem.magic;
                projectile.ranged = StoredItem.ranged;
                projectile.Name = StoredItem.Name;

                // Transparency; copies appear translucent
                if (!Consumed && !GobblerHelper.IsDepletable(StoredItem))
                {
                    projectile.alpha = 120;
                }

                // Items without gravity
                if (ItemID.Sets.ItemNoGravity[StoredItem.type]) 
                {
                    projectile.timeLeft = 2 * 60;
                    projectile.penetrate = -1;
                }

                // Penetration
                if (StoredItem.melee) projectile.penetrate = -1; // Melee weapons penetrate infinitely
                if (GobblerHelper.IsTool(StoredItem)) projectile.penetrate = 3; // But tools penetrate only a bit
                else if (StoredItem.accessory) projectile.penetrate = 2;

                projectile.netUpdate = true; // Sync to multiplayer
            }

            // Every tick: Projectile rotation and movement
            if (!ItemID.Sets.ItemNoGravity[StoredItem.type])
            {
                projectile.velocity.Y += GobblerHelper.DiagonalSize(StoredItem) / 200f; // How fast they fall down depends on their size

                // Swingable weapons and projectiles point to where they're going, adjusted for sprite orientation
                if (StoredItem.useStyle == 1 && !StoredItem.consumable && !GobblerHelper.IsTool(StoredItem))
                {
                    projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians();
                }
                else if (StoredItem.ammo > 0)
                {
                    projectile.rotation = projectile.velocity.ToRotation();
                    if (StoredItem.ammo == AmmoID.Arrow) projectile.rotation += -90.ToRadians();
                    else if (StoredItem.ammo == AmmoID.Bullet || StoredItem.ammo == AmmoID.Dart) projectile.rotation += 90.ToRadians();
                    else if (StoredItem.ammo == AmmoID.StyngerBolt || StoredItem.ammo == AmmoID.CandyCorn) projectile.rotation += 45.ToRadians();
                }
                else projectile.rotation += Tools.RevolutionPerSecond; // The rest of the items spin
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (ToolBounce()) return false;

            return base.OnTileCollide(oldVelocity); // Die
        }


        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];

            ItemShoot();
            MagicExplode();
            ItemConsume();

            if (StoredItem.UseSound != null) Main.PlaySound(StoredItem.UseSound, projectile.Center);
        }


        public override bool? CanHitNPC(NPC target)
        {
            bool canHit = projectile.CanHit(target);

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




        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) // Draws the stored item's texture as the projectile's
        {
            if (StoredItem.type != ItemID.None) 
            {
                Texture2D texture = Main.itemTexture[StoredItem.type];
                Rectangle? frame = null; // Which part of the texture to load, the sourceRect
                Vector2 drawOrigin = texture.Size() / 2; // Center of the texture
                Vector2 position = projectile.Center - Main.screenPosition; // Where to draw the texture

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
                    texture, position, frame, lightColor * projectile.Opacity,
                    projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);

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
