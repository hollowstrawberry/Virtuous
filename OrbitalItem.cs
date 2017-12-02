using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Orbitals;
using static Virtuous.Tools;

namespace Virtuous
{
    public abstract class OrbitalItem : ModItem
    {
        public override bool CloneNewInstances => true; //So the defaults are copied to new items

        //Characteristics set in defaults
        public int type = OrbitalID.None; //The orbital this item spawns. Failing to provide a valid one will cause an out of bounds exception.
        public int duration = 5 * 60; //How long the summoned orbital will last, in ticks
        public int amount = 1; //The amount of orbitals that will be spawned in a circle

        public int specialFunctionType = SpecialNone; //Whether this orbital has a special effect, and whether it triggers by reusing it or by right-clicking
        public const int SpecialNone = 0;
        public const int SpecialReuse = 1;
        public const int SpecialRightClick = 2;


        public virtual void SetOrbitalDefaults()
        {
        }

        public sealed override void SetDefaults() //Safe way of setting item defaults
        {
            //Default values
            item.width = 30;
            item.height = 30;
            item.useStyle = 4;
            item.useTime = 40;
            item.useAnimation = item.useTime;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.autoReuse = false;
            item.useTurn = true;

            SetOrbitalDefaults();
            duration += OrbitalID.Orbital[this.type].DyingTime; //Adds the orbital's dying time to the total duration
            item.shoot = OrbitalID.GetOrbitalType(mod, this.type); //Sets the orbital projectile to shoot

            //Overwrites
            item.crit = 0;
            item.melee = false;
            item.ranged = false;
            item.magic = false;
            item.thrown = false;
            item.summon = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return specialFunctionType == SpecialRightClick;
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            //We don't want to count any of the damage buffs from orbitals themselves
            float meleeBuff = 0f, magicBuff = 0f;
            for (int type = 0; type < OrbitalID.Orbital.Length; type++)
            {
                if (orbitalPlayer.active[type]) //If an orbital is active
                {
                    Player tempPlayer = new Player(); //We create a temporal player instance to test on
                    tempPlayer.meleeDamage = 0f;
                    tempPlayer.magicDamage = 0f;

                    OrbitalID.Orbital[type].PlayerEffects(tempPlayer); //We run the effects of the active orbital on it
                    meleeBuff += tempPlayer.meleeDamage;
                    magicBuff += tempPlayer.magicDamage;
                }
            }

            //Gets boosted by the player's strongest damage between magic and melee
            damage = (int)(item.damage * (Math.Max(player.meleeDamage - meleeBuff, player.magicDamage - magicBuff)));
            if (orbitalPlayer.accessoryDmgBoost) damage = (int)(damage * 1.5);


            if (specialFunctionType == SpecialRightClick) //A trick to stop the bugged 1-tick delay between consecutive right-click uses of a weapon
            {
                HandleAltUseAnimation(player, item);
            }
        }

        public override bool CanUseItem(Player player)
        {
            OrbitalPlayer orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            //Can't use the right click special function with the orbital not active
            if (specialFunctionType == SpecialRightClick && player.altFunctionUse == 2 && !orbitalPlayer.active[this.type])
            {
                return false;
            }

            //Can't use an orbital if any other orbital is in the process of dying
            for (int id = 0; id < OrbitalID.Orbital.Length; id++)
            {
                if (orbitalPlayer.active[id] && orbitalPlayer.time <= OrbitalID.Orbital[id].DyingTime)
                {
                    return false;
                }
            }

            return base.CanUseItem(player);
        }

        public sealed override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
                            orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this);
                        }
                        else //Right click: Special function
                        {
                            orbitalPlayer.specialFunctionActive = true;
                        }
                        return false;
                    }
                    else if (specialFunctionType == SpecialReuse) //Reuse mode
                    {
                        orbitalPlayer.specialFunctionActive = true; //Special function
                        orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this); //Resets duration
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
                    rotation = Vector2.UnitX.RotatedBy(FullCircle * i / amount); //Divides the circle into a set amount of points and picks the current one in the loop
                    Projectile.NewProjectile(position, rotation, type, damage, knockBack, player.whoAmI);
                }
            }
            orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this); //Reset duration

            return false; //Doesn't shoot normally
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            OrbitalPlayer orbitalPlayer = Main.player[item.owner].GetModPlayer<OrbitalPlayer>();

            bool hasDamage = false;

            foreach (TooltipLine line in tooltips)
            {
                if (line.mod == "Terraria" && line.Name == "Damage") //Cuts the text to say "orbital damage"
                {
                    line.text = line.text.Split(' ')[0] + " orbital damage";
                    hasDamage = true;
                }
            }

            string durationText = ((int)(orbitalPlayer.ModifiedOrbitalTime(this) / 60)).ToString() + " seconds duration";

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

            tooltips.RemoveAll(line => line.mod == "Terraria" && line.Name.StartsWith("CritChance")); //Removes the critical chance line
        }
    }
}
