using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    public class OrbitalItem : GlobalItem
    {
        public override bool InstancePerEntity
        {
            get { return true; }
        }

        public override bool CloneNewInstances
        {
            get { return true; }
        }


        public int type = OrbitalID.None; //The orbital this item spawns
        public int duration = 5 * 60; //How long the summoned orbital will last
        public int amount = 1; //The amount of orbitals that will be spawned in a circle

        public const int SpecialNone = 0;
        public const int SpecialReuse = 1;
        public const int SpecialRightClick = 2;
        public int specialFunctionType = SpecialNone;


        public static int GetOrbitalType(Mod mod, int type) //Returns a corresponding projectile type to the given OrbitalItem.type value
        {
            switch (type)
            {
                case OrbitalID.Sailspike:    return mod.ProjectileType<Sailspike_Proj>();
                case OrbitalID.Facade:       return mod.ProjectileType<Facade_Proj>();
                case OrbitalID.Bubble:       return mod.ProjectileType<Bubble_Proj>();
                case OrbitalID.SpikedBubble: return mod.ProjectileType<SpikedBubble_Proj>();
                case OrbitalID.HolyLight:    return mod.ProjectileType<HolyLight_Proj>();
                case OrbitalID.SacDagger:    return mod.ProjectileType<SacDagger_Proj>();
                case OrbitalID.Bullseye:     return mod.ProjectileType<Bullseye_Proj>();
                case OrbitalID.Shuriken:     return mod.ProjectileType<Shuriken_Proj>();
                case OrbitalID.SpiralSword:  return mod.ProjectileType<SpiralSword_Proj>();
            }

            Main.NewText("Virtuous: OrbitalID has no corresponding projectile type, at OrbitalItem.GetOrbitalType()");
            return 0;
        }

        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (this.type != OrbitalID.None)
            {
                OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

                if (orbitalPlayer.active[this.type]) //The orbital is already active
                {
                    if (specialFunctionType != SpecialNone) //Alternate use mechanics
                    {
                        if (specialFunctionType == SpecialRightClick) //Right-click mode
                        {
                            if (player.altFunctionUse != 2) //Left click: Resets duration
                            {
                                orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(item);
                            }
                            else //Right click: Special function
                            {
                                orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOn] = true;
                            }
                            return false;
                        }
                        else if (specialFunctionType == SpecialReuse) //Reuse mode
                        {
                            orbitalPlayer.specialFunction[OrbitalPlayer.SpecialOn] = true; //Special function
                            orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(item); //Resets duration
                            return false;
                        }
                    }
                }
                else //The orbital is not active
                {
                    orbitalPlayer.ResetOrbitals();
                    orbitalPlayer.active[this.type] = true;

                    for (int i = 0; i < amount; i++)
                    {
                        Vector2 rotation; //We will pass this as the velocity of the projectile, which will be stored then set to 0
                        rotation = Vector2.UnitX.RotatedBy(Tools.FullCircle * i / amount); //Divides the circle into a set amount of points and picks the current one in the loop
                        Projectile.NewProjectile(position, rotation, GetOrbitalType(mod, this.type), damage, knockBack, player.whoAmI);
                    }
                }
                orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(item); //Reset duration

                return false; //Doesn't shoot normally
            }

            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void GetWeaponDamage(Item item, Player player, ref int damage)
        {
            if (type != OrbitalID.None)
            {
                OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

                float oDmgBuff = 0; //Doesn't count buffs from any orbitals themselves
                if (orbitalPlayer.active[OrbitalID.SpikedBubble]) oDmgBuff = SpikedBubble_Proj.DamageBoost;
                if (orbitalPlayer.active[OrbitalID.SpiralSword ]) oDmgBuff = SpiralSword_Proj.DamageBoost;

                //Gets boosted by the player's strongest damage between magic and melee
                damage = (int)(item.damage * ((player.magicDamage > player.meleeDamage) ? player.magicDamage - oDmgBuff : player.meleeDamage - oDmgBuff));
                if (orbitalPlayer.accessoryDmgBoost) damage = (int)(damage * 1.5);
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (type != OrbitalID.None)
            {
                OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

                if (specialFunctionType == SpecialRightClick && player.altFunctionUse == 2 && !orbitalPlayer.active[this.type])
                {
                    return false; //Can't use the right click special function with the orbital not active
                }

                //Checks if the player currently has an orbital in the process of dying, as you can't use an orbital item during that time
                if (  orbitalPlayer.active[OrbitalID.SpiralSword]  && orbitalPlayer.time <= SpiralSword_Proj.DyingTime
                   || orbitalPlayer.active[OrbitalID.Sailspike]    && orbitalPlayer.time <= Sailspike_Proj.DyingTime
                   || orbitalPlayer.active[OrbitalID.SpikedBubble] && orbitalPlayer.time <= SpikedBubble_Proj.DyingTime
                   || orbitalPlayer.active[OrbitalID.HolyLight]    && orbitalPlayer.time <= HolyLight_Proj.DyingTime
                   || orbitalPlayer.active[OrbitalID.SacDagger]    && orbitalPlayer.time <= SacDagger_Proj.DyingTime
                   || orbitalPlayer.active[OrbitalID.Shuriken]     && orbitalPlayer.time <= Shuriken_Proj.DyingTime
                )
                {
                    return false;
                }
            }

            return base.CanUseItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (type != OrbitalID.None)
            {
                bool hasDamage = false;

                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name == "Damage") //Cuts the text to say "orbital damage"
                    {
                        line.text = line.text.Split(' ')[0] + " orbital damage";
                        hasDamage = true;
                    }
                }

                OrbitalPlayer orbitalPlayer = Main.player[item.owner].GetModPlayer<OrbitalPlayer>();
                string durationText = ((int)(orbitalPlayer.ModifiedOrbitalTime(item) / 60)).ToString() + " seconds duration";

                foreach (TooltipLine line in tooltips) //Adds the duration, with the place depending on whether the item has damage or not
                {
                    if (!hasDamage && line.mod == "Terraria" && line.Name == "UseMana") //Puts it above mana use
                    {
                        line.text = durationText + "\n" + line.text;
                    }
                    else if (hasDamage &&  line.mod == "Terraria" && line.Name == "Speed") //Intentionally replaces use speed
                    {
                        line.text = durationText;
                    }
                }
            }
        }
    }
}
