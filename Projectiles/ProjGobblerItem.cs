using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Items;

namespace Virtuous.Projectiles
{
    public class ProjGobblerItem : ModProjectile
    {
        ///* Private projectile fields and properties *///
        private const float RotationSpeed = 1 * Tools.RevolutionPerSecond;
        private const int Lifespan = 10 * 60;

        private bool Consumed //Whether the item was consumed and will drop from this projectile. Passed as ai[0]
        {
            get { return projectile.ai[0] > 0; }
        } 

        private Item StoredItem //An item of the type being shot. Passed as ai[1]
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

        public static int ShotDamage(Item item, Player player) //Returns the final damage of the specified item when being shot by the specified player
        {
            //Adds:
            //The item's base damage
            //3 damage per defense point
            //10 damage per gold coin of sell price
            //10 damage per rarity point
            //5 damage per pixel across the sprite size
            float damage = TheGobbler.BaseDamage + item.damage + item.defense * 3f + item.value / 5000f + ItemDiagonalSize(item) * 5f + (item.rare > 0 ? item.rare * 10 : 0);

            if      (item.melee ) damage *= player.meleeDamage;
            else if (item.ranged) damage *= player.rangedDamage;
            else if (item.magic ) damage *= player.magicDamage;
            else if (item.summon) damage *= player.minionDamage;
            else if (item.thrown) damage *= player.thrownDamage;

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

        public static bool IsWeapon(Item item) //Whether the specified item can probably be reforged
        {
            if (item.damage > 0 && item.useStyle > 0 && !item.consumable && item.GetGlobalItem<OrbitalItem>().type == OrbitalID.None)
            {
                return true;
            }
            else return false;
        }

        public static bool IsExplosive(Item item) //Whether the specified item is probably a consumable explosive
        {
            //bool[] explosive = ItemID.Sets.Factory.CreateBoolSet(new int[] { ProjectileID.Dynamite, ProjectileID.BouncyDynamite, ProjectileID.StickyDynamite, ProjectileID.Bomb, ProjectileID.BouncyBomb, ProjectileID.StickyBomb, ProjectileID.Grenade, ProjectileID.BouncyGrenade, ProjectileID.StickyGrenade });
            if (item.consumable && item.shoot > 0 && (item.damage <= 0 || item.useStyle == 5))
            {
                return true; //It's a consumable that is thrown, but either has no damage of its own or uses the grenade useStyle
            }
            else return false;
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
            else if (item.consumable || item.ammo != 0 || item.type == ItemID.ExplosiveBunny)
            {
                return true;
            }
            else return false;
        }


        ///* Private methods run by the projectile for easy access and utility *///

        private void ChangeSize(int newWidth, int newHeight) //Change the projectile's size but keep its center
        {
            projectile.position += new Vector2(projectile.width / 2, projectile.height / 2);
            projectile.width = newWidth;
            projectile.height = newHeight;
            projectile.position -= new Vector2(projectile.width / 2, projectile.height / 2);
        }

        private bool ToolBounce() //If the item is a tool, it will bounce off in the direction it came from when specified
        {
            Item storedItem = StoredItem;
            if (storedItem.pick > 0 || storedItem.axe > 0 || storedItem.hammer > 0)
            {
                if (projectile.penetrate > 1)
                {
                    if (storedItem.UseSound != null) Main.PlaySound(storedItem.UseSound, projectile.Center);
                    projectile.penetrate--;
                    projectile.velocity *= new Vector2(-0.5f, -0.5f);
                    return true;
                }
                else projectile.Kill();
            }
            return false;
        }


        ///* tModLoader methods and projectile behavior *///

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shot Item");
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
            projectile.usesLocalNPCImmunity = true; //Hits once per individual projectile
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Item storedItem = StoredItem; //We make a single copy to utilize

            if (projectile.timeLeft == Lifespan && storedItem.type != ItemID.None) //First tick
            {
                //Inherit properties from the stored item
                ChangeSize(storedItem.width, storedItem.height);
                projectile.damage = ShotDamage(storedItem, player);
                projectile.knockBack = ShotKnockBack(storedItem, player);
                projectile.melee = storedItem.melee;
                projectile.magic = storedItem.magic;
                projectile.ranged = storedItem.ranged;

                if (!Consumed && (!IsDepletable(storedItem) || storedItem.makeNPC > 0))
                {
                    projectile.alpha = 135; //Copies appear translucent, unless they're a non-critter consumable
                }

                if (ItemID.Sets.ItemNoGravity[storedItem.type]) //Items without gravity
                {
                    projectile.timeLeft = 2 * 60;
                    projectile.penetrate = -1;
                }

                if (storedItem.melee) projectile.penetrate = -1; //Melee weapons penetrate infinitely
                if (storedItem.pick > 0 || storedItem.axe > 0 || storedItem.hammer > 0) projectile.penetrate = 3; //Tools penetrate only a bit
                else if (storedItem.accessory) projectile.penetrate = 2;

                projectile.netUpdate = true; //Sync to multiplayer;
            }

            if (!ItemID.Sets.ItemNoGravity[storedItem.type]) //Movement and rotation is ignored by items that don't have gravity
            {
                projectile.velocity.Y += ItemDiagonalSize(storedItem) / 200f; //How fast they fall down depends on their size

                if (storedItem.useStyle == 1 && !storedItem.consumable && storedItem.pick == 0 && storedItem.axe == 0 && storedItem.hammer == 0)
                {
                    projectile.rotation = projectile.velocity.ToRotation() + 45.ToRadians(); //Swingable weapons point to where they're going (+45 degrees)
                }
                else if (storedItem.ammo > 0) //Projectiles point to where they're going
                {
                    projectile.rotation = projectile.velocity.ToRotation();
                    if (storedItem.ammo == AmmoID.Arrow) projectile.rotation -= -90.ToRadians(); //Sprite pointing down
                    else if (storedItem.ammo == AmmoID.Bullet || storedItem.ammo == AmmoID.Dart) projectile.rotation += 90.ToRadians(); //Sprite pointing up
                    else if (storedItem.ammo == AmmoID.StyngerBolt || storedItem.ammo == AmmoID.CandyCorn) projectile.rotation += 45.ToRadians(); //Sprite pointing diagonally
                }
                else projectile.rotation += RotationSpeed; //The rest of the items spin
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            projectile.penetrate--;
            if (ToolBounce()) return false;
            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreKill(int timeLeft)
        {
            Item storedItem = StoredItem; //We make a single copy to utilize
            Player player = Main.player[projectile.owner];

            if (Consumed && !IsDepletable(storedItem)) Item.NewItem(projectile.Center, storedItem.type, 1, false, IsWeapon(storedItem) ? PrefixID.Broken : 0); //Drops the item, applies the Broken modifier if it's a weapon.
            if (storedItem.UseSound != null) Main.PlaySound(storedItem.UseSound, projectile.Center);


            if (storedItem.shoot > 0 && storedItem.ammo == 0 && storedItem.GetGlobalItem<OrbitalItem>().type == OrbitalID.None) //Shoots whatever projectile this item shoots, unless it's an ammo
            {
                int projAmount = 1;
                if (storedItem.damage > 0 && !storedItem.consumable && !storedItem.summon && (!storedItem.melee || storedItem.useStyle == 1)
                    || storedItem.type == ItemID.Clentaminator
                )
                {
                    projAmount = Main.rand.Next(4, 6 + 1); //Weapons excluding non-swingable melee weapons and summons shoot a random amount of projectiles
                }
                if (storedItem.ranged) projAmount *= 2; //Ranged weapons shoot twice as many projectiles

                int projType = storedItem.shoot;
                if (projType == ProjectileID.WoodenArrowFriendly) projType = ProjectileID.WoodenArrowHostile; //So it doesn't drop the item
                else if (projType == 10) projType = ProjectileID.Bullet; //For some reasons some guns have 10

                int damage = storedItem.damage; //Deals the normal item's damage
                if      (storedItem.melee ) damage = (int)(damage * player.meleeDamage);
                else if (storedItem.ranged) damage = (int)(damage * player.rangedDamage);
                else if (storedItem.magic ) damage = (int)(damage * player.magicDamage);
                else if (storedItem.summon) damage = (int)(damage * player.minionDamage);
                else if (storedItem.thrown) damage = (int)(damage * player.thrownDamage);

                for (int i = 0; i < projAmount; i++)
                {
                    Vector2 rotation = projAmount == 1 ? Vector2.UnitY.RotatedByRandom(Tools.FullCircle) : Vector2.UnitY.RotatedBy(Tools.FullCircle * i / projAmount); //Weapons shoot in a circle, others shot in a random direction
                    Vector2 position = projectile.Center + rotation;
                    Vector2 velocity = rotation * storedItem.shootSpeed;

                    Projectile newProj = Projectile.NewProjectileDirect(position, velocity, projType, damage, storedItem.knockBack, player.whoAmI);
                    newProj.friendly = true;
                    newProj.hostile = false;
                    if (IsExplosive(storedItem)) newProj.timeLeft = 3; //Explodes instantly
                }
            }
            else if (storedItem.type == ItemID.ExplosiveBunny) //I love these, alright? And they don't normally have a value for shoot
            {
                Projectile newProj = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ProjectileID.ExplosiveBunny, storedItem.damage, projectile.knockBack, player.whoAmI);
                newProj.timeLeft = 3;
            }
            else if (storedItem.GetGlobalItem<OrbitalItem>().type != OrbitalID.None) //Orbitals activate themselves
            {
                OrbitalItem orbitalItem = storedItem.GetGlobalItem<OrbitalItem>();
                Vector2 position = player.Center;
                Vector2 velocity = Vector2.Zero;
                int type = 0;
                int damage = storedItem.damage;
                orbitalItem.GetWeaponDamage(storedItem, player, ref damage);

                orbitalItem.Shoot(storedItem, player, ref position, ref velocity.X, ref velocity.Y, ref type, ref damage, ref storedItem.knockBack);
            }


            if (storedItem.magic) //Magic weapons make an explosion
            {
                ChangeSize(projectile.width + 50, projectile.height + 50);
                if (projectile.owner == Main.myPlayer) projectile.Damage(); //Applies damage in the area

                Main.PlaySound(SoundID.Item14, projectile.Center);
                for (int i = 0; i < Math.Max(4, (int)ItemDiagonalSize(storedItem) / 4); i++) //More dust the higher the size
                {
                    Gore.NewGore(projectile.position + new Vector2(Main.rand.Next(projectile.width), Main.rand.Next(projectile.height)), Vector2.Zero, Main.rand.Next(61, 63 + 1), 0.2f + Main.rand.NextFloat());
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, /*Type*/31, 0f, 0f, /*Alpha*/0, default(Color), Main.rand.NextFloat() * 2f);
                }
            }

            if (storedItem.makeNPC != 0 && Consumed) //If the item spawns an NPC, it spawns it when the item is consumed
            {
                NPC.NewNPC((int)(projectile.Center.X), (int)(projectile.Center.Y), storedItem.makeNPC);
            }

            return base.PreKill(timeLeft);
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Item storedItem = StoredItem; //A single copy

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
            if (storedItem.vanity) target.AddBuff(BuffID.Slow, 120);
            //More?
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Item storedItem = StoredItem; //A single copy

            ToolBounce();

            if (!storedItem.Name.ToUpper().Contains("BANNER"))
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
            Item storedItem = StoredItem;
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

                Vector2 position = projectile.Center - Main.screenPosition + new Vector2(0, texture.Height / 2 - frameHeight / 2); //Adds the difference between the spritesheet's center and the frame's center
                spriteBatch.Draw(texture, position, frame, lightColor * projectile.Opacity, projectile.rotation, texture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            else return true; //Without an item, it has the default texture
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return StoredItem.GetAlpha(lightColor);
        }
    }
}
