using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    /// <summary>
    /// Custom data given to all items by this mod.
    /// Includes the "being sucked" state induced by <see cref="Items.TheGobbler"/>.
    /// </summary>
    public class VirtuousItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;


        /// <summary>Whether an item is being sucked by <see cref="Items.TheGobbler"/> and thus can't be picked up.</summary>
        public bool beingGobbled = false;


        public override bool CanPickup(Item item, Player player)
        {
            return beingGobbled ? false : base.CanPickup(item, player);
        }
    }
}
