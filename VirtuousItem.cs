using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Virtuous.Orbitals;

namespace Virtuous
{
    public class VirtuousItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;


        // Gobbler

        public bool beingGobbled = false;

        public override bool CanPickup(Item item, Player player)
        {
            return beingGobbled ? false : base.CanPickup(item, player);
        }
    }
}
