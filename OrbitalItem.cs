using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Orbitals;

namespace Virtuous
{
    public abstract class OrbitalItem : ModItem
    {
        public override bool CloneNewInstances => true; // The defaults are copied to new items

        // Traits set in defaults
        public int type = OrbitalID.None; // The orbital this item spawns. Failing to provide a valid one will cause an exception
        public int duration = 5 * 60; // How long the summoned orbital will last, in ticks
        public int amount = 1; // The amount of orbitals that will be spawned in a circle
        public SpecialType specialType = SpecialType.None; // If and how an orbital special effect triggers


        public enum SpecialType
        {
            None,
            Reuse,
            RightClick,
        }




        public virtual void SetOrbitalDefaults()
        {
        }

        public sealed override void SetDefaults() // Safe way of setting standard default values
        {
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

            if (type < 0 || type >= OrbitalID.Orbital.Length)
            {
                throw new Exception($"Virtuous: The type of the orbital item {item.Name} was set to the orbital ID {type}, " +
                                    $"which is either invalid or doesn't have any matching orbital.");
            }

            item.shoot = mod.OrbitalProjectileType(type); //Sets the orbital projectile to shoot
            if (specialType != SpecialType.None) item.autoReuse = true;

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
            return specialType == SpecialType.RightClick;
        }


        public override void GetWeaponDamage(Player player, ref int damage)
        {
            var orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            float highestDamage = Math.Max(player.meleeDamage, player.magicDamage);
            damage = (int)(item.damage * orbitalPlayer.damageMultiplier * (highestDamage - orbitalPlayer.damageBuffFromOrbitals));

            if (specialType == SpecialType.RightClick)
            {
                Tools.HandleAltUseAnimation(player);
            }
        }


        public override bool CanUseItem(Player player)
        {
            var orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            // Can't use the right click special function with the orbital not active
            if (specialType == SpecialType.RightClick && player.altFunctionUse == 2 && !orbitalPlayer.active[this.type])
            {
                return false;
            }

            // Can't use an orbital if any other orbital is in the process of dying
            if (Enumerable.Range(0, OrbitalID.Orbital.Length)
                    .Any(id => orbitalPlayer.active[id] && orbitalPlayer.time <= OrbitalID.Orbital[id].DyingTime))
            {
                return false;
            }

            return base.CanUseItem(player);
        }


        public sealed override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (orbitalPlayer.active[this.type]) // The orbital is already active
            {
                if (specialType != SpecialType.None) // Alternate use mechanics
                {
                    if (specialType == SpecialType.RightClick)
                    {
                        // Left-click resets duration, right-click activates special
                        if (player.altFunctionUse != 2) orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this);
                        else orbitalPlayer.SpecialFunctionActive = true;
                    }
                    else if (specialType == SpecialType.Reuse)
                    {
                        // Activates special and resets duration
                        orbitalPlayer.SpecialFunctionActive = true;
                        orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this);
                    }
                }
            }
            else // The orbital is not active
            {
                orbitalPlayer.ResetOrbitals();
                orbitalPlayer.active[this.type] = true;
                orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this);

                for (int i = 0; i < this.amount; i++)
                {
                    // The desired rotation will be passed as velocity
                    Vector2 rotation = Vector2.UnitX.RotatedBy(Tools.FullCircle * i / this.amount);
                    Projectile.NewProjectile(position, rotation, type, damage, knockBack, player.whoAmI);
                }
            }

            return false; // Doesn't shoot normally
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var orbitalPlayer = Main.player[item.owner].GetModPlayer<OrbitalPlayer>();

            int duration = ((orbitalPlayer.ModifiedOrbitalTime(this) - OrbitalID.Orbital[type].DyingTime) / 60);

            string damageText;
            string durationText;

            if (Language.ActiveCulture == GameCulture.Spanish)
            {
                damageText = "daño orbital";
                durationText = $"{duration} segundos de duracion";
            }
            else
            {
                damageText = "orbital damage";
                durationText = $"{duration} seconds duration";
            }


            TooltipLine critLine = tooltips.FirstOrDefault(line => line.mod == "Terraria" && line.Name == "CritChance");
            if (critLine != null) tooltips.Remove(critLine);

            TooltipLine damageLine = tooltips.FirstOrDefault(line => line.mod == "Terraria" && line.Name == "Damage");
            if (damageLine != null) damageLine.text = $"{damageLine.text.Split(' ')[0]} {damageText}";

            int durationIndex;
            if (damageLine == null) // Above mana
            {
                durationIndex = tooltips.IndexOf(tooltips.FirstOrDefault(line => line.mod == "Teraria" && line.Name == "UseMana"));
            }
            else // Replaces speed
            {
                durationIndex = tooltips.IndexOf(tooltips.FirstOrDefault(line => line.mod == "Terraria" && line.Name == "Speed"));
                tooltips.RemoveAt(durationIndex);
            }

            tooltips.Insert(Math.Max(durationIndex, 0), new TooltipLine(mod, "OrbitalDuration", durationText));
        }
    }
}
