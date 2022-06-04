using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Orbitals;
using Terraria.DataStructures;

namespace Virtuous
{
    /// <summary>
    /// Base class for an item that summons an orbital. Its default traits can be set in <see cref="SetOrbitalDefaults"/>.
    /// </summary>
    public abstract class OrbitalItem : ModItem
    {
        protected override bool CloneNewInstances => true; // The defaults are copied to new items


        /// <summary>The orbital this item spawns.</summary>
        public abstract int OrbitalType { get; }

        /// <summary>How long the summoned orbital will last, in ticks.</summary>
        public virtual int Duration => 5 * 60;

        /// <summary>The amount of orbitals that will be summoned in a circle.</summary>
        public virtual int Amount => 1;

        /// <summary>If and how the summoned orbital's special effect triggers.</summary>
        public virtual SpecialType Special => SpecialType.None;



        /// <summary>If and how an orbital's special effect is triggered by its item.</summary>
        public enum SpecialType
        {
            None,
            Reuse,
            RightClick,
        }




        /// <summary>Where this orbital item's traits can be set, including the summoned orbital's traits.</summary>
        public virtual void SetOrbitalDefaults()
        {
        }

        public sealed override void SetDefaults() // Safe way of setting standard default values
        {
            Item.width = 30;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 40;
            Item.useAnimation = Item.useTime;
            Item.UseSound = SoundID.Item8;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.useTurn = true;

            SetOrbitalDefaults();

            if (OrbitalType < 0 || OrbitalType >= OrbitalID.Orbital.Length)
            {
                throw new Exception($"Virtuous: The type of the orbital item {Item.Name} was set to the orbital ID {OrbitalType}, " +
                                    $"which is either invalid or doesn't have any matching orbital.");
            }

            Item.shoot = Mod.OrbitalProjectileType(OrbitalType); //Sets the orbital projectile to shoot
            if (Special != SpecialType.None) Item.autoReuse = true;

            //Overwrites
            Item.crit = 0;
            Item.DamageType = DamageClass.Generic;
        }


        public override bool AltFunctionUse(Player player)
        {
            return Special == SpecialType.RightClick;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            var orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            float highestDamage = Math.Max(player.GetDamage<MeleeDamageClass>().Multiplicative, player.GetDamage<MagicDamageClass>().Multiplicative);
            damage.Flat = (Item.damage * orbitalPlayer.damageMultiplier * (highestDamage - orbitalPlayer.damageBuffFromOrbitals));

            if (Special == SpecialType.RightClick)
            {
                Tools.HandleAltUseAnimation(player);
            }
        }


        public override bool CanUseItem(Player player)
        {
            var orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            // Can't use the right click special function with the orbital not active
            if (Special == SpecialType.RightClick && player.altFunctionUse == 2 && !orbitalPlayer.active[this.OrbitalType])
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


        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            var orbitalPlayer = player.GetModPlayer<OrbitalPlayer>();

            if (orbitalPlayer.active[this.OrbitalType]) // The orbital is already active
            {
                if (Special != SpecialType.None) // Alternate use mechanics
                {
                    if (Special == SpecialType.RightClick)
                    {
                        // Left-click resets duration, right-click activates special
                        if (player.altFunctionUse != 2) orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this);
                        else orbitalPlayer.SpecialFunctionActive = true;
                    }
                    else if (Special == SpecialType.Reuse)
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
                orbitalPlayer.active[this.OrbitalType] = true;
                orbitalPlayer.time = orbitalPlayer.ModifiedOrbitalTime(this);

                for (int i = 0; i < this.Amount; i++)
                {
                    // The desired rotation will be passed as velocity
                    Vector2 rotation = Vector2.UnitX.RotatedBy(Tools.FullCircle * i / this.Amount);
                    Projectile.NewProjectile(source, position, rotation, type, damage, knockBack, player.whoAmI);
                }
            }

            return false; // Doesn't shoot normally
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var orbitalPlayer = Main.player[Item.playerIndexTheItemIsReservedFor].GetModPlayer<OrbitalPlayer>();

            int duration = ((orbitalPlayer.ModifiedOrbitalTime(this) - OrbitalID.Orbital[OrbitalType].DyingTime) / 60);

            string damageText;
            string durationText;

            if (Language.ActiveCulture == GameCulture.FromCultureName(GameCulture.CultureName.Spanish))
            {
                damageText = "dano orbital";
                durationText = $"{duration} segundos de duracion";
            }
            else
            {
                damageText = "orbital damage";
                durationText = $"{duration} seconds duration";
            }


            TooltipLine critLine = tooltips.FirstOrDefault(line => line.Mod == "Terraria" && line.Name == "CritChance");
            if (critLine != null) tooltips.Remove(critLine);

            TooltipLine damageLine = tooltips.FirstOrDefault(line => line.Mod == "Terraria" && line.Name == "Damage");
            if (damageLine != null) damageLine.Text = $"{damageLine.Text.Split(' ')[0]} {damageText}";

            int durationIndex;
            if (damageLine == null) // Above mana
            {
                durationIndex = tooltips.IndexOf(tooltips.FirstOrDefault(line => line.Mod == "Teraria" && line.Name == "UseMana"));
            }
            else // Replaces speed
            {
                durationIndex = tooltips.IndexOf(tooltips.FirstOrDefault(line => line.Mod == "Terraria" && line.Name == "Speed"));
                tooltips.RemoveAt(durationIndex);
            }

            tooltips.Insert(Math.Max(durationIndex, 1), new TooltipLine(Mod, "OrbitalDuration", durationText));
        }
    }
}
