using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    /// <summary>
    /// Base class for an orbital projectile, that defines behavior for all orbitals
    /// and allows the customization of an orbital's unique traits.
    /// </summary>
    public abstract class OrbitalProjectile : ModProjectile
    {
        // Constants
        public const bool Inwards = false;
        public const bool Outwards = true;

        // Owner alias
        public Player player => Main.player[projectile.owner];
        public OrbitalPlayer orbitalPlayer => player.GetModPlayer<OrbitalPlayer>();



        // Type traits

        /// <summary>The orbital ID associated with the projectile. A valid ID must be provided.</summary>
        public abstract int Type { get; }

        /// <summary>How many ticks, if any, the projectile fades away for.</summary>
        public virtual int FadeTime => 0;

        /// <summary>How many ticks, if any, the projectile spends in "dying mode" at the end of its lifespan, during which no orbital items can be used.</summary>
        public virtual int DyingTime => 0;

        /// <summary>Original alpha value of the projectile.</summary>
        public virtual int OriginalAlpha => 50;

        /// <summary>Distance from the player the orbital starts at.</summary>
        public virtual float BaseDistance => 50;

        /// <summary>Speed at which the projectile's sprite rotates.</summary>
        public virtual float RotationSpeed => 0;

        /// <summary>Speed at which the projectile orbits around the player.</summary>
        public virtual float OrbitingSpeed => 0;

        /// <summary>Speed at which the projectile will shoot out in DyingTime (default behavior).</summary>
        public virtual float DyingSpeed => 0;

        /// <summary>Oscillation speed limit. Will affect how far it can go before changing direction of movement.</summary>
        public virtual float OscillationSpeedMax => 0;

        /// <summary>Oscillation acceleration rate. Will affect how quickly it reaches the point of direction change.</summary>
        public virtual float OscillationAcc => OscillationSpeedMax / 60;




        // Current state

        /// <summary>Relative position to the player, stored as velocity.</summary>
        public Vector2 relativePosition
        {
            get { return projectile.velocity; }
            set { projectile.velocity = value; }
        }

        /// <summary>Distance away from the player. Affects <see cref="relativePosition"/> directly.</summary>
        public float relativeDistance
        {
            get { return relativePosition.Length(); }
            set { relativePosition = relativePosition.OfLength(value); }
        }

        /// <summary>Current speed of back-and-forth oscillation, Stored as the projectile's ai[0].</summary>
        public float oscillationSpeed
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        /// <summary>Direction of movement, inwards or outwards, used by default for oscillation. Stored as the projectile's ai[1].</summary>
        public bool direction
        {
            get { return projectile.ai[1] == 0; }
            set { projectile.ai[1] = value ? 0 : 1; }
        }

        /// <summary>Ticks upward as long as <see cref="IsDoingSpecial"/> is true. Stored as the projectile's localAI[0].</summary>
        public int specialFunctionTimer
        {
            get { return (int)projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        }



        // Checks

        /// <summary>Whether it's the first tick of the orbital's life.</summary>
        public virtual bool IsFirstTick => relativeDistance == 1.0f; // Orbitals are created with a velocity vector of length 1

        /// <summary>Whether the projectile is at the end of its life</summary>
        public virtual bool IsDying => DyingTime > 0 && Main.myPlayer == projectile.owner && orbitalPlayer.time <= DyingTime;

        /// <summary>Whether to execute <see cref="SpecialFunction"/>.</summary>
        public virtual bool IsDoingSpecial => Main.myPlayer == projectile.owner && orbitalPlayer.SpecialFunctionActive && !IsDying;




        // Utility methods

        /// <summary>Moves the orbital relative to the player.</summary>
        public void SetPosition(Vector2? newPos = null)
        {
            if (newPos != null) relativePosition = (Vector2)newPos;
            projectile.Center = player.MountedSpriteCenter() + relativePosition;
        }

        /// <summary>Rotates the orbital relative to the player by the given amount.</summary>
        public void RotatePosition(float radians)
        {
            SetPosition(relativePosition.RotatedBy(radians));
        }

        /// <summary>Moves the orbital relative to the player and at the given distance.</summary>
        public void SetDistance(float newDistance)
        {
            SetPosition(relativePosition.OfLength(newDistance));
        }

        /// <summary>Adjusts the distance relative to the player by the given amount.</summary>
        public void AddDistance(float distance)
        {
            SetDistance(relativeDistance + distance);
        }




        // Behavior methods

        /// <summary>Where this orbital projectile's projectile traits can be set.</summary>
        public virtual void SetOrbitalDefaults()
        {
        }

        public sealed override void SetDefaults() // Safe way to set standard defaults
        {
            projectile.netImportant = true; // So it syncs more frequently in multiplayer
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true; // Doesn't interfere with other piercing damage
            projectile.idStaticNPCHitCooldown = 10;
            projectile.alpha = OriginalAlpha;
            projectile.timeLeft = (DyingTime > 0) ? DyingTime : 2; // Time left resets every tick during the orbital's life

            SetOrbitalDefaults();
        }


        /// <summary>Effects the orbital type will apply on the player while it is active.
        /// Only runs once if there are multiple orbitals of the same type.</summary>
        public virtual void PlayerEffects()
        {
        }


        /// <summary>Runs once when the projectile is spawned, according to <see cref="IsFirstTick"/>.</summary>
        public virtual void FirstTick()
        {
            SetDistance(BaseDistance);
            oscillationSpeed = OscillationSpeedMax;
            projectile.rotation = relativePosition.ToRotation();
        }


        /// <summary>Whether to execute <see cref="Movement"/>.
        /// By default, doesn't execute movement if dying or in special mode.</summary>
        public virtual bool PreMovement()
        {
            return !IsDying && !IsDoingSpecial;
        }


        /// <summary>Main orbital behavior. By default, it will orbit around the player, rotating and oscillating,
        /// dictated by the values of the orbital's properties.</summary>
        public virtual void Movement()
        {
            if (OscillationSpeedMax != 0)
            {
                if      (oscillationSpeed >= +OscillationSpeedMax) direction = Inwards;
                else if (oscillationSpeed <= -OscillationSpeedMax) direction = Outwards;
                oscillationSpeed += OscillationAcc * (direction ? +1 : -1); // Accelerate in the corresponding direction
                AddDistance(oscillationSpeed);
            }

            RotatePosition(OrbitingSpeed);
            projectile.rotation += RotationSpeed;
        }


        /// <summary>Special effect of the orbital, none by default. Runs when <see cref="IsDoingSpecial"/> is true.</summary>
        public virtual void SpecialFunction()
        {
        }


        /// <summary>Runs once at the beginning of <see cref="DyingTime"/>.
        /// By default, it shoots outward according to <see cref="DyingSpeed"/>.</summary>
        public virtual void DyingFirstTick()
        {
            projectile.velocity = relativePosition.OfLength(DyingSpeed);
        }


        /// <summary>Runs every tick during <see cref="DyingTime"/>.
        /// By default, it shoots outward according to <see cref="DyingSpeed"/>.</summary>
        public virtual void Dying()
        {
            projectile.velocity -= projectile.velocity.OfLength(DyingSpeed / DyingTime); // Slows down to a halt
            projectile.position += projectile.velocity; // Re-applies velocity as it would normally be nullified for orbitals
        }


        /// <summary>Runs every tick after everything else. By default, it manages the projectile's transparency
        /// according to <see cref="FadeTime"/> and <see cref="OriginalAlpha"/>.</summary>
        public virtual void PostAll()
        {
            if (FadeTime > 0 && Main.myPlayer == projectile.owner)
            {
                if (orbitalPlayer.time <= FadeTime)
                {
                    projectile.alpha += Math.Max(1, (int)((255f - OriginalAlpha) / FadeTime)); // Fades away completely over fadeTime
                }
                else
                {
                    projectile.alpha = OriginalAlpha; // Resets the alpha in case the time resets during fading time
                }
            }
        }




        /// <summary>Skeleton for orbital behavior running every tick.</summary>
        public sealed override void AI()
        {
            projectile.netUpdate = true; // Sync to multiplayer, I'm lazy

            if (!orbitalPlayer.active[Type] && Main.myPlayer == projectile.owner) // Keep it alive only while the summon is active
            {
                projectile.Kill();
            }
            else
            {
                if (IsFirstTick)
                {
                    FirstTick();
                }

                if (PreMovement())
                {
                    Movement();
                }

                if (IsDoingSpecial)
                {
                    SpecialFunction();
                    specialFunctionTimer++;
                }
                else
                {
                    specialFunctionTimer = 0;
                }

                if (IsDying) // timeLeft ticks down during dying time
                {
                    if (orbitalPlayer.time == DyingTime)
                    {
                        DyingFirstTick();
                    }

                    Dying();
                }
                else // Keeps the orbital from dying naturally
                {
                    projectile.timeLeft = Math.Max(2, DyingTime);
                }


                PostAll();

                projectile.position -= projectile.velocity; // Reverses the effect of velocity so the orbital doesn't move by default
            }
        }



        public override bool? CanCutTiles() => false; // Orbitals would be glorified lawnmowers otherwise

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100) * projectile.Opacity; // Fully lit


        // Syncs local ai slots in multiplayer

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }
    }
}
