using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Items;
using static Virtuous.Tools;

namespace Virtuous.Projectiles
{
    public class ProjGobblerItem : ModProjectile
    {
        ///* Private projectile fields and properties *///

        private const float RotationSpeed = 1 * RevolutionPerSecond;
        private const int Lifespan = 10 * 60;

        private bool Consumed //Whether the item was consumed and will drop from this projectile. Passed as ai[0]
        {
            get { return projectile.ai[0] > 0; }
        } 

        private Item MasterStoredItem //Returns an item copy of the type being shot. Passed as ai[1]
        {
            get
            {
                Item item = new Item();
                item.SetDefaults((int)projectile.ai[1]);
                return item;
            }
        }


        ///* Static public methods for projectile utilities and characteristics *///

        public static float ItemDiagonalSize(Item item) //The size of the item's sprite from corner to corner
        {
            return (float)Math.Sqrt((double)item.width * item.width + (double)item.height * item.height);
        }

        public static void ApplyClassDamage(ref float damage, Item item, Player player) //Modifies the given ref damage to apply class damage bonuses based on the given item and player
        {
            if      (item.melee ) damage *= player.meleeDamage;
            else if (item.ranged) damage *= player.rangedDamage;
            else if (item.magic ) damage *= player.magicDamage;
            else if (item.summon) damage *= player.minionDamage;
            else if (item.thrown) damage *= player.thrownDamage;
        }

        public static int ShotDamage(Item item, Player player) //Returns the final damage of the specified item when being shot by the specified player
        {
            //Adds:
            //The item's base damage
            //3 damage per defense point
            //10 damage per gold coin of sell price
            //10 damage per rarity point
            //5 damage per pixel across the sprite size
            float damage = TheGobbler.BaseDamage + item.damage + item.defense * 3f + item.value / 5000f + ItemDiagonalSize(item) * 5f + (item.rare > 0 ? item.rare * 10 : 0);
            ApplyClassDamage(ref damage, item, player);
            return (int)damage;
        }

        public static float ShotKnockBack(Item item, Player player) //Returns the final knockback of the specified item when being shot by the specified player
        {
            //Adds:
            //The item's base knockback
            //1 knockback per every 6 defense of the item
            //1 knockback per 10 pixels across the sprite size
            //5 knockback for being a block or wall
            float knockBack = TheGobbler.BaseKnockBack + item.knockBack + +item.defense / 6f + ItemDiagonalSize(item) / 10f + (item.createTile > 0 || item.createWall > 0 ? 5 : 0);

            if (player.kbGlove) knockBack *= 1.4f;
            if (player.kbBuff) knockBack *= 1.2f;

            return knockBack;
        }

        public static bool IsReforgeableWeapon(Item item)
        {
            return (item.Prefix(-3) && !item.accessory);
        }

        public static bool IsTool(Item item)
        {
            return (item.pick > 0 || item.axe > 0 || item.hammer > 0);
        }

        public static bool IsExplosive(Item item) //Whether the specified item is probably a consumable explosive
        {
            return (item.consumable && item.shoot > 0 && (item.damage <= 0 || item.useStyle == 5)); //It's a consumable that is thrown, but either has no damage of its own or uses the grenade useStyle.
            
            //bool[] explosive = ItemID.Sets.Factory.CreateBoolSet(new int[] { ProjectileID.Dynamite, ProjectileID.BouncyDynamite, ProjectileID.StickyDynamite, ProjectileID.Bomb, ProjectileID.BouncyBomb, ProjectileID.StickyBomb, ProjectileID.Grenade, ProjectileID.BouncyGrenade, ProjectileID.StickyGrenade });
        }

        public static bool IsDepletable(Item item) //Whether the specified item will be lost upon being shot
        {
            if (item.type == ItemID.Gel || item.type == ItemID.FallenStar || (item.type >= ItemID.CopperCoin && item.type <= ItemID.PlatinumCoin)
                || item.createTile > 0 || item.createWall > 0 || item.potion || item.healLife > 0 || item.healMana > 0 || item.buffType > 0
                || item.Name.ToUpper().Contains("TREASURE BAG")
            )
            {
                return false; //Exceptions
            }

            else return (item.consumable || item.ammo != 0 || item.type == ItemID.ExplosiveBunny);
        }



        ///* Private methods run by the projectile for easy access and utility *///

        private bool ToolBounce() //If the item is a tool, it will bounce off in the direction it came from when specified
        {
            Item storedItem = MasterStoredItem;
            if (IsTool(storedItem))
            {
                if (projectile.penetrate > 1)
                {
                    if (storedItem.UseSound != null) Main.PlaySound(storedItem.UseSound, projectile.Center);
                    projectile.penetrate--;
                    projectile.velocity *= new Vector2(-0.5f, -0.5f);
                    projectile.netUpdate = true; //Syncs to multiplayer
                    return true;
                }
                else projectile.Kill();
            }

            return false;
        }

        private void MagicExplode() //If the item is a magic weapon it will explode when the projectile dies
        {
            Item storedItem = MasterStoredItem;
            if (storedItem.magic)
            {
                ResizeProjectile(projectile.whoAmI, projectile.width + 50, projectile.height + 50);
                projectile.Damage(); //Applies damage in the area

                Main.PlaySound(SoundID.Item14, projectile.Center);
                for (int i = 0; i < Math.Max(4, (int)ItemDiagonalSize(storedItem) / 4); i++) //More dust the higher the size
                {
                    Gore.NewGore(projectile.position + new Vector2(RandomFloat(projectile.width), RandomFloat(projectile.height)), Vector2.Zero, RandomInt(61, 63), RandomFloat(0.2f, 1.2f));
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, /*Type*/31, 0f, 0f, /*Alpha*/0, default(Color), RandomFloat(2));
                }
            }
        }

        private void ItemConsume() //When the projectile dies and the stored item was set to consumed
        {
            Item storedItem = MasterStoredItem;
            Player player = Main.player[projectile.owner];

            if (IsDepletable(storedItem)) //Item won't be returned
            {
                if (storedItem.buffTime != 0) //Buff-giving consumables
                {
                    player.AddBuff(storedItem.buffType, storedItem.buffTime);
                }
                if (storedItem.makeNPC != 0 && Main.netMode != NetmodeID.MultiplayerClient) //Critter consumables. In multiplayer it only spawns the mob server-side
                {
                    NPC.NewNPC((int)(projectile.Center.X), (int)(projectile.Center.Y), storedItem.makeNPC);
                }
            }
            else //Drops the stored item
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) //In multiplayer it only spawns the item server-side
                {
                    Item.NewItem(projectile.Center, storedItem.type, 1, false, IsReforgeableWeapon(storedItem) ? PrefixID.Broken : 0);
                }
            }
        }

        private void ItemShoot() //When this projectile dies it can shoot out what the stored item would shoot
        {
            Item storedItem = MasterStoredItem;
            Player player = Main.player[projectile.owner];

            //Orbital behavior
            OrbitalItem orbitalItem = storedItem.modItem as OrbitalItem;
            if (orbitalItem != null)
            {
                Vector2 position = player.Center;
                Vector2 velocity = Vector2.Zero;
                int type = orbitalItem.item.shoot;
                int damage = storedItem.damage;
                orbitalItem.GetWeaponDamage(player, ref damage);

                orbitalItem.Shoot(player, ref position, ref velocity.X, ref velocity.Y, ref type, ref damage, ref storedItem.knockBack);
                return;
            }

            //Shooting exceptions
            else if (storedItem.type == ItemID.ExplosiveBunny) //I love these, alright? And they don't normally have a value for shoot
            {
                Projectile newProj = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ProjectileID.ExplosiveBunny, storedItem.damage, projectile.knockBack, player.whoAmI);
                newProj.timeLeft = 3;
            }

            //General shooting behavior
            else if (storedItem.shoot > 0 && storedItem.ammo == 0 && storedItem.type != ItemID.StardustDragonStaff) //Stardust dragon is very buggy
            {
                //Summon and pet behavior
                if (storedItem.buffType != 0)
                {
                    if (player.FindBuffIndex(storedItem.buffType) < 0)
                    {
                        player.AddBuff(storedItem.buffType, 60); //Player doesn't have the buff active, adds it
                    }
                    else //Player has the corresponding  buff active
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].type == storedItem.shoot && Main.projectile[i].owner == projectile.owner) //Finds the pet or summon among all projectiles
                            {
                                Main.projectile[i].Center = projectile.Center; //Teleports the pet or summon to this projectile
                                if (storedItem.damage <= 0) return; //If it's a pet, stops the method and doesn't shoot anything
                            }
                        }
                    }
                }

                //Shot projectile amount
                int projAmount = 1;
                if (storedItem.damage > 0 && !storedItem.consumable && (!storedItem.melee || storedItem.useStyle == 1) || storedItem.type == ItemID.Clentaminator)
                {
                    projAmount = RandomInt(4, 6); //Weapons excluding non-swingable melee weapons shoot a random amount of projectiles
                }
                if (storedItem.ranged)
                {
                    projAmount += projAmount / 2 + (int)(60f / storedItem.useAnimation); //Ranged weapons shoot a lot more based on their speed
                }

                //Shot projectile type
                int projType = storedItem.shoot;
                if      (projType == ProjectileID.WoodenArrowFriendly) projType = ProjectileID.WoodenArrowHostile; //So it doesn't drop the item. We will reverse the friendly and hostile later
                else if (projType == 10 && storedItem.useAmmo == AmmoID.Bullet) projType = ProjectileID.Bullet; //For some reasons some guns have 10
                else if (storedItem.useAmmo == AmmoID.Dart) projType = ProjectileID.PoisonDart; //Manually makes dart guns shoot darts

                //Shot projectile damage
                float damage = storedItem.damage; //The normal item's damage
                ApplyClassDamage(ref damage, storedItem, player);

                //Spawning the projectiles
                for (int i = 0; i < projAmount; i++)
                {
                    Vector2 rotation = projAmount == 1 ? Vector2.UnitY.RotatedByRandom(FullCircle) : Vector2.UnitY.RotatedBy(FullCircle * i / projAmount); //Weapons shoot in a circle, others shot in a random direction
                    Vector2 position = projectile.Center + rotation;
                    Vector2 velocity = rotation * storedItem.shootSpeed;

                    Projectile newProj = Projectile.NewProjectileDirect(position, velocity, projType, (int)damage, storedItem.knockBack, player.whoAmI);
                    if (IsExplosive(storedItem)) newProj.timeLeft = 3; //Explodes instantly
                    if (storedItem.sentry)
                    {
                        newProj.position.Y -= 20; //So it doesn't sink into the ground
                        newProj.position.X += RandomInt(-20, +20);
                        player.UpdateMaxTurrets();
                    }
                    newProj.friendly = true;
                    newProj.hostile = false;
                }
            }
        }



        ///* tModLoader methods and projectile behavior *///

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Item");
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
            projectile.usesLocalNPCImmunity = true; //Hits per individual projectile
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Item storedItem = MasterStoredItem; //It's important that we make a single copy to utilize so that a new item instance isn't created every time we access the property

            //First tick
            if (projectile.timeLeft == Lifespan && storedItem.type != ItemID.None)
            {
                //Inherit properties from the stored item
                ResizeProjectile(projectile.whoAmI, storedItem.width, storedItem.height);
                projectile.damage = ShotDamage(storedItem, player);
                projectile.knockBack = ShotKnockBack(storedItem, player);
                projectile.melee  = storedItem.melee;
                projectile.magic  = storedItem.magic;
                projectile.ranged = storedItem.ranged;

                //Transparency
                if (!Consumed && !IsDepletable(storedItem))
                {
                    projectile.alpha = 120; //Copies appear translucent, unless they're depleted after used
                }

                //Items without gravity
                if (ItemID.Sets.ItemNoGravity[storedItem.type]) 
                {
                    projectile.timeLeft = 2 * 60;
                    projectile.penetrate = -1;
                }

                //Penetration
                if (storedItem.melee) projectile.penetrate = -1; //Melee weapons penetrate infinitely
                if (IsTool(storedItem)) projectile.penetrate = 3; //But tools penetrate only a bit
                else if (storedItem.accessory) projectile.penetrate = 2;

                projectile.netUpdate = true; //Sync to multiplayer
            }

            //Every tick: Projectile rotation and movement
            if (!ItemID.Sets.ItemNoGravity[storedItem.type])
            {
                projectile.velocity.Y += ItemDiagonalSize(storedItem) / 200f; //How fast they fall down depends on their size

                if (storedItem.useStyle == 1 && !storedItem.consumable && !IsTool(storedItem))
                {
                    projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians(); //Swingable weapons point to where they're going (+45 degrees)
                }
                else if (storedItem.ammo > 0) //Projectiles point to where they're going
                {
                    projectile.rotation = projectile.velocity.ToRotation();
                    if (storedItem.ammo == AmmoID.Arrow) projectile.rotation += -90.ToRadians(); //Sprite pointing down
                    else if (storedItem.ammo == AmmoID.Bullet || storedItem.ammo == AmmoID.Dart) projectile.rotation += 90.ToRadians(); //Sprite pointing up
                    else if (storedItem.ammo == AmmoID.StyngerBolt || storedItem.ammo == AmmoID.CandyCorn) projectile.rotation += 45.ToRadians(); //Sprite pointing diagonally
                }
                else projectile.rotation += RotationSpeed; //The rest of the items spin
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (ToolBounce()) //Tools bounce
            {
                return false;
            }
            else //Other items die
            {
                return base.OnTileCollide(oldVelocity); 
            }
        }

        public override void Kill(int timeLeft)
        {
            Item storedItem = MasterStoredItem; //A single copy
            Player player = Main.player[projectile.owner];

            if (projectile.owner == Main.myPlayer) //So it doesn't repeat the code for everyone in a server
            {
                ItemShoot();
                MagicExplode();
            }

            if (Consumed) ItemConsume();

            if (storedItem.UseSound != null) Main.PlaySound(storedItem.UseSound, projectile.Center);
        }

        public override bool? CanHitNPC(NPC target)
        {
            //Checking a new stored item instance once per tick per NPC in the world caused a lot of lag
            //Instead I narrow down the check, only checking the stored item after checking that the target was close after checking that it was available for hit

            bool canHit = projectile.CanHit(target);

            if (canHit && (target.Center - projectile.Center).Length() < 50 && target.type == MasterStoredItem.makeNPC)
            {
                return false; //Critters can't hit themselves
            }

            return canHit;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Item storedItem = MasterStoredItem; //A single copy

            ToolBounce();

            if (!storedItem.Name.ToUpper().Contains("BANNER")) //Generic effects based on keywords
            {
                if (storedItem.Name.ToUpper().Contains("SLIME") || storedItem.Name.ToUpper().Contains("GEL")) target.AddBuff(BuffID.Slimed, 120);
                if (storedItem.Name.ToUpper().Contains("WATER")) target.AddBuff(BuffID.Wet, 120);
                if (storedItem.Name.ToUpper().Contains("FIRE") || storedItem.Name.ToUpper().Contains("FLAME") || storedItem.Name.ToUpper().Contains("LAVA") || storedItem.Name.ToUpper().Contains("MAGMA") || storedItem.Name.ToUpper().Contains("TORCH")) target.AddBuff(BuffID.OnFire, 120);
                if (storedItem.Name.ToUpper().Contains("ICE")) target.AddBuff(BuffID.Chilled, 120);
                if (storedItem.Name.ToUpper().Contains("FROST")) target.AddBuff(BuffID.Frostburn, 120);
                if (storedItem.Name.ToUpper().Contains("ICHOR")) target.AddBuff(BuffID.Ichor, 120);
                if (storedItem.Name.ToUpper().Contains("CURSED")) target.AddBuff(BuffID.CursedInferno, 120);
                if (storedItem.Name.ToUpper().Contains("SHADOWFLAME")) target.AddBuff(BuffID.ShadowFlame, 120);
            }
            //More?
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Item storedItem = MasterStoredItem; //A single copy

            ToolBounce();

            if (!storedItem.Name.ToUpper().Contains("BANNER")) //Generic effects based on keywords
            {
                if (storedItem.Name.ToUpper().Contains("SLIME") || storedItem.Name.ToUpper().Contains("GEL")) target.AddBuff(BuffID.Slimed, 120);
                if (storedItem.Name.ToUpper().Contains("WATER")) target.AddBuff(BuffID.Wet, 120);
                if (storedItem.Name.ToUpper().Contains("FIRE") || storedItem.Name.ToUpper().Contains("FLAME") || storedItem.Name.ToUpper().Contains("LAVA") || storedItem.Name.ToUpper().Contains("MAGMA") || storedItem.Name.ToUpper().Contains("TORCH")) target.AddBuff(BuffID.OnFire, 120);
                if (storedItem.Name.ToUpper().Contains("ICE")) target.AddBuff(BuffID.Chilled, 120);
                if (storedItem.Name.ToUpper().Contains("FROST")) target.AddBuff(BuffID.Frostburn, 120);
                if (storedItem.Name.ToUpper().Contains("ICHOR")) target.AddBuff(BuffID.Ichor, 120);
                if (storedItem.Name.ToUpper().Contains("CURSED")) target.AddBuff(BuffID.CursedInferno, 120);
                if (storedItem.Name.ToUpper().Contains("SHADOWFLAME")) target.AddBuff(BuffID.ShadowFlame, 120);
            }
            if (storedItem.vanity) target.AddBuff(BuffID.Slow, 120); //Vanity slows down
            if (storedItem.buffType != 0) target.AddBuff(storedItem.buffType, 120); //Why not
            //More?
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) //Draws the stored item's texture as the projectile's
        {
            Item storedItem = MasterStoredItem; //A single copy

            if (storedItem.type != ItemID.None) 
            {
                Texture2D texture = Main.itemTexture[storedItem.type];
                Rectangle? frame = null; //Which part of the texture to load, the sourceRect
                int frameHeight = texture.Height; //The height of an individual frame in the texture

                if (Main.itemAnimations[storedItem.type] != null) //For animated items, gets the current frame
                {
                    int frameCount = Main.itemAnimations[storedItem.type].FrameCount;
                    int frameDelay = Main.itemAnimations[storedItem.type].TicksPerFrame;
                    int currentFrame = (int)(Main.GameUpdateCount / frameDelay) % frameCount; //This code cycles through the frames at a constant pace
                    frame = texture.Frame(1, frameCount, 0, currentFrame);
                    frameHeight /= frameCount;
                }

                if (storedItem.color != default(Color))
                {
                    lightColor = Color.Lerp(lightColor, storedItem.color, 0.75f); //Gel gains its blue, etc.
                }

                Vector2 position = projectile.Center - Main.screenPosition + new Vector2(0, texture.Height / 2 - frameHeight / 2); //Adds the difference between the spritesheet's center and the frame's center
                spriteBatch.Draw(texture, position, frame, lightColor * projectile.Opacity, projectile.rotation, texture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            else return true; //Without an item, it has the default texture
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return MasterStoredItem.GetAlpha(lightColor);
        }
    }
}
