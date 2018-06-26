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
    public abstract class OrbitalProjectile : ModProjectile
    {
        /*
         * This class contains all the properties and methods used by orbitals.
         * The orbitals themselves will override these virtual members to obtain the exact behavior of each unique orbital.
         */


        // Constants
        public const bool Outwards = true;
        public const bool Inwards = false;

        // Owner alias
        public Player player => Main.player[projectile.owner];
        public OrbitalPlayer orbitalPlayer => player.GetModPlayer<OrbitalPlayer>();


        // Constant traits

        public virtual int Type => OrbitalID.None; // The orbital ID associated with the projectile. An invalid ID will throw an exception
        public virtual int FadeTime => 0; // How many ticks, if any, the projectile fades away for
        public virtual int DyingTime => 0; // How many ticks, if any, the projectile spends in "dying mode" at the end of its lifespan, during which no orbital items can be used
        public virtual int OriginalAlpha => 50; // Original alpha value of the projectile
        public virtual float BaseDistance => 50; // Distance from the player it starts at
        public virtual float RotationSpeed => 0; // Speed at which the projectile's sprite rotates
        public virtual float OrbitingSpeed => 0; // Speed at which the projectile orbits around the player
        public virtual float DyingSpeed => 0; // Speed at which the projectile will shoot out in DyingTime (default behavior)
        public virtual float OscillationSpeedMax => 0; // Oscillation peed limit. Means how far it can go before changing direction of movement
        public virtual float OscillationAcc => OscillationSpeedMax / 60; // Oscillation acceleration rate. Means how fast it reaches the point of direction change


        // Current state

        public Vector2 relativePosition // Relative position to the player, stored as velocity
        {
            get { return projectile.velocity; }
            set { projectile.velocity = value; }
        } 
        public float relativeDistance // Distance away from the player. Affects RelativePosition directly.
        {
            get { return relativePosition.Length(); }
            set { relativePosition = relativePosition.OfLength(value); }
        }
        public float oscillationSpeed // Current speed of back-and-forth oscillation, Stored as ai[0]
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }
        public bool direction // Direction of movement, inwards or outwards, used by default for oscillation. Stored as ai[1]
        {
            get { return projectile.ai[1] == 0; }
            set { projectile.ai[1] = value ? 0 : 1; }
        }
        public int specialFunctionTimer //Time passed since the special effect was used. Stored as localAI[0]
        {
            get { return (int)projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        }


        // Checks

        public virtual bool IsFirstTick // Whether it's the first tick of the orbital's life.
            => (relativeDistance == 1.0f); // Orbitals are always created with a velocity vector of size 1, but it's changed in the first tick

        public virtual bool IsDying // Whether the projectile is at the end of its life
            => (DyingTime > 0 && Main.myPlayer == projectile.owner && orbitalPlayer.time <= DyingTime);

        public virtual bool IsDoingSpecial // Whether to run the special effect method or not
            => (Main.myPlayer == projectile.owner && orbitalPlayer.SpecialFunctionActive && !IsDying);




        // Utility methods

        public void SetPosition(Vector2? newPos = null) // Moves the orbital relative to the player
        {
            if (newPos != null) relativePosition = (Vector2)newPos;
            projectile.Center = player.MountedSpriteCenter() + relativePosition;
        }

        public void RotatePosition(float radians) // Rotates the orbital relative to the player
        {
            SetPosition(relativePosition.RotatedBy(radians));
        }

        public void SetDistance(float newDistance) // Applies a new distance relative to the player and moves the orbital accordingly
        {
            SetPosition(relativePosition.OfLength(newDistance));
        }

        public void AddDistance(float distance)
        {
            SetDistance(relativeDistance + distance);
        }

        public static OrbitalProjectile FindFirst(Mod mod, Player player, int id = OrbitalID.None)
        {
            foreach (var proj in Main.projectile.Where(x => x.active && x.owner == player.whoAmI))
            {
                var orbital = proj.modProjectile as OrbitalProjectile;
                if (orbital != null && (id == OrbitalID.None || orbital.Type == id))
                {
                    return orbital;
                }
            }

            return null;
        }




        //Defaults

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


        // Effects the orbital type will apply on the player while it is active.
        // Only runs for the first orbital of a type. Called by OrbitalPlayer
        public virtual void PlayerEffects()
        {
        }


        // Only runs once at the beginning of the orbital's life
        public virtual void FirstTick()
        {
            SetDistance(BaseDistance);
            oscillationSpeed = OscillationSpeedMax;
            projectile.rotation = relativePosition.ToRotation();
        }


        // Returns whether to execute movement
        public virtual bool PreMovement()
        {
            return (!IsDying && !IsDoingSpecial); // By default doesn't do normal movement if it's dying or in special mode
        }


        // Main orbital behavior
        public virtual void Movement()
        {
            if (OscillationSpeedMax != 0) // Oscillation
            {
                if      (oscillationSpeed >= +OscillationSpeedMax) direction = Inwards;  //If it has reached the outwards speed limit, begin to switch direction
                else if (oscillationSpeed <= -OscillationSpeedMax) direction = Outwards; //If it has reached the inwards speed limit, begin to switch direction
                oscillationSpeed += OscillationAcc * (direction ? +1 : -1); //Accelerate in the corresponding direction
                AddDistance(oscillationSpeed);
            }

            RotatePosition(OrbitingSpeed); //Rotates the projectile around the player
            projectile.rotation += RotationSpeed; //Rotates the projectile itself
        }


        // Executes special effect if isDoingSpecial is true
        public virtual void SpecialFunction()
        {
        }


        // Only runs once at the beginning of DyingTime
        public virtual void DyingFirstTick()
        {
            projectile.velocity = relativePosition.OfLength(DyingSpeed); //Shoots out
        }


        // Runs every tick during DyingTime
        public virtual void Dying()
        {
            projectile.velocity -= projectile.velocity.OfLength(DyingSpeed / DyingTime); //Slows down to a halt
            projectile.position += projectile.velocity; //Re-applies velocity as it would normally be nullified for orbitals
        }


        // Runs every tick after everything else. Used for fading away, light, etc.
        public virtual void PostAll()
        {
            if (FadeTime > 0 && Main.myPlayer == projectile.owner)
            {
                if (orbitalPlayer.time <= FadeTime)
                {
                    projectile.alpha += Math.Max(1, (int)((255f - OriginalAlpha) / FadeTime)); //Fades away completely over fadeTime
                }
                else
                {
                    projectile.alpha = OriginalAlpha; //Resets the alpha in case the time resets during fading time
                }
            }
        }



        //Head of the operation
        public sealed override void AI()
        {
            if (!orbitalPlayer.active[Type] && Main.myPlayer == projectile.owner) // Keep it alive only while the summon is active
            {
                projectile.netUpdate = true; // Sync to multiplayer
                projectile.Kill();
            }
            else
            {
                if (IsFirstTick)
                {
                    projectile.netUpdate = true;
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
                        projectile.netUpdate = true;
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


        public override bool? CanCutTiles()
        {
            return false; // So they don't become a lawnmower
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100) * projectile.Opacity; // Fullbright
        }


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
