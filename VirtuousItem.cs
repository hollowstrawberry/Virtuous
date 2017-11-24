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

        //Gobbler

        public bool beingGobbled = false;

        public override bool CanPickup(Item item, Player player)
        {
            if (beingGobbled) return false;

            else return base.CanPickup(item, player);
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            base.Update(item, ref gravity, ref maxFallSpeed);
        }
    }
}
