﻿using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Virtuous.Orbitals
{
    public class LuckyBreak : OrbitalProjectile
    {
        public override int OrbitalType => OrbitalID.LuckyBreak;
        public override int DyingTime => 30;
        public override int FadeTime => 15;
        public override int OriginalAlpha => 0;
        public override float BaseDistance => _BaseDistance; // Set to a constant so it can be used in other constants
        public override float OrbitingSpeed => 0.0f * Tools.RevolutionPerSecond;
        public override float OscillationSpeedMax => 15f / 30;
        public override float OscillationAcc => OscillationSpeedMax / 20;
        public override float DyingSpeed => 20;

        public override bool IsDoingSpecial => true; // Always keeps increasing specialFunctionTimer

        public const int CritBuff = 7;
        public const int DamageDebuff = 7; // In percentage

        private const int _BaseDistance = 65;
        private const int CycleTime = 7 * 60; // Time between shuffles
        private const int ShuffleTime = 30; // Part of CycleTime in which the cards do the shuffle motion
        private const int ShuffleSpeed = (_BaseDistance - 2) * 2 / ShuffleTime;

        private const int Hearts = 0, Diamonds = 1, Spades = 2, Clubs = 3; // Frames


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Card");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Spanish), "Carta");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Карта");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "卡牌");

            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetOrbitalDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 28;
        }



        private void ShuffleCard()
        {
            Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
        }



        public override void PlayerEffects()
        {
            // Shuffle sound
            if (!IsFirstTick && SpecialFunctionTimer == CycleTime - ShuffleTime / 2)
            {
                SoundEngine.PlaySound(18, player.Center);
            }
            
            // Individual card buffs
            foreach (var proj in Main.projectile.Where(x => x.active && x.owner == Projectile.owner && x.type == Projectile.type))
            {
                switch (proj.frame)
                {
                    case Hearts:
                        player.runAcceleration *= 1.15f;
                        player.maxRunSpeed *= 1.15f;
                        player.lifeRegen += 1;
                        break;

                    case Spades:
                        player.meleeCrit  += CritBuff;
                        player.magicCrit  += CritBuff;
                        player.rangedCrit += CritBuff;
                        player.thrownCrit += CritBuff;
                        break;

                    case Clubs:
                        player.meleeDamage  -= DamageDebuff / 100f;
                        player.magicDamage  -= DamageDebuff / 100f;
                        player.rangedDamage -= DamageDebuff / 100f;
                        player.thrownDamage -= DamageDebuff / 100f;
                        orbitalPlayer.damageBuffFromOrbitals -= DamageDebuff / 100f;
                        break;
                }
            }
        }


        public override void FirstTick()
        {
            RotatePosition(-Tools.FullCircle / 4); // Make the first card be above the player instead of to the right

            SpecialFunctionTimer = CycleTime - ShuffleTime / 2; // Puts the card in the middle of the shuffling motion
            SetDistance(2);
            OscillationSpeed = ShuffleSpeed;
        }


        public override bool PreMovement()
        {
            return !IsDying;
        }

        public override void Movement()
        {
            if (SpecialFunctionTimer >= CycleTime - ShuffleTime) // Shuffling motion
            {
                if (SpecialFunctionTimer == CycleTime - ShuffleTime) // First tick
                {
                    SetDistance(BaseDistance);
                    OscillationSpeed = ShuffleSpeed;
                    Direction = Inwards;
                }
                else if (SpecialFunctionTimer == CycleTime - ShuffleTime/2) // Middle of the motion
                {
                    ShuffleCard();
                    Direction = Outwards;
                }
                else if (SpecialFunctionTimer == CycleTime) // Last tick
                {
                    SetDistance(BaseDistance);
                    OscillationSpeed = OscillationSpeedMax;
                    SpecialFunctionTimer = 0;
                    base.Movement(); // Starts moving inward. TODO: Find out why I put this here
                    return;
                }

                AddDistance(OscillationSpeed * (Direction ? +1 : -1));
                RotatePosition(OrbitingSpeed);
            }
            else // Normal movement
            {
                base.Movement();
            }
        }


        public override void Dying()
        {
            Projectile.rotation += 5 * Tools.RevolutionPerSecond;
            Projectile.velocity.Y += 2f; // Gravity
            Projectile.position += Projectile.velocity;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsDying) damage *= 7;

            if (Projectile.frame == Diamonds && target.lifeMax > 5 && !target.immortal)
            {
                target.AddBuff(BuffID.Midas, 7 * 60);

                int newItem = Item.NewItem(
                    target.position, target.width, target.height,
                    (Main.rand.NextBool(10) ? (Main.rand.NextBool(10) ? ItemID.GoldCoin : ItemID.SilverCoin) : ItemID.CopperCoin),
                    Main.rand.Next(3, 16));

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem);
                }
            }
        }


        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (IsDying) damage *= 7;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255) * Projectile.Opacity;
        }
    }
}