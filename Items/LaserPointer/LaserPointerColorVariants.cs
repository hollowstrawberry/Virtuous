using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Virtuous.Projectiles;

namespace Virtuous.Items.LaserPointer
{
    class LaserPointerBlue : LaserPointer
    {
        protected override LaserColor LaserColor => LaserColor.Blue;
        protected override short ColorMaterial => ItemID.SapphireGemsparkBlock;
    }

    class LaserPointerGreen : LaserPointer
    {
        protected override LaserColor LaserColor => LaserColor.Green;
        protected override short ColorMaterial => ItemID.EmeraldGemsparkBlock;
    }

    class LaserPointerYellow : LaserPointer
    {
        protected override LaserColor LaserColor => LaserColor.Yellow;
        protected override short ColorMaterial => ItemID.TopazGemsparkBlock;
    }

    class LaserPointerPurple : LaserPointer
    {
        protected override LaserColor LaserColor => LaserColor.Purple;
        protected override short ColorMaterial => ItemID.AmethystGemsparkBlock;
    }

    class LaserPointerWhite : LaserPointer
    {
        protected override LaserColor LaserColor => LaserColor.White;
        protected override short ColorMaterial => ItemID.DiamondGemsparkBlock;
    }

    class LaserPointerOrange : LaserPointer
    {
        protected override LaserColor LaserColor => LaserColor.Orange;
        protected override short ColorMaterial => ItemID.AmberGemsparkBlock;
    }
}
