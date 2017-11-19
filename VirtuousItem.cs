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
        public override bool InstancePerEntity
        {
            get { return true; }
        }

        public override bool CloneNewInstances
        {
            get { return true; }
        }

        //Gobbler

        public bool beingGobbled = false;

        public override bool CanPickup(Item item, Player player)
        {
            if (beingGobbled) return false;

            else return base.CanPickup(item, player);
        }
    }
}
