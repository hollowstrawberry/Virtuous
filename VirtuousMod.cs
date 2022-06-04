using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Virtuous
{
    class VirtuousMod : Mod
    {
        public VirtuousMod()
        {
        }


        //public override void Load()
        //{
        //    var translatable = Assembly.GetAssembly(typeof(VirtuousMod)).GetTypes()
        //        .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(ITranslatable)))
        //        .Select(x => (ITranslatable)Activator.CreateInstance(x));

        //    foreach (var obj in translatable)
        //    {
        //        obj.AddTranslations(this);
        //    }
        //}


        public override void AddRecipeGroups()
        {
            RecipeGroup.RegisterGroup("Virtuous:Wings", new RecipeGroup(() => "Any Wings", new int[] {
                ItemID.DemonWings,
                ItemID.AngelWings,
                ItemID.RedsWings,
                ItemID.ButterflyWings,
                ItemID.FairyWings,
                ItemID.HarpyWings,
                ItemID.BoneWings,
                ItemID.FlameWings,
                ItemID.FrozenWings,
                ItemID.GhostWings,
                ItemID.SteampunkWings,
                ItemID.LeafWings,
                ItemID.BatWings,
                ItemID.BeeWings,
                ItemID.DTownsWings,
                ItemID.WillsWings,
                ItemID.CrownosWings,
                ItemID.CenxsWings,
                ItemID.TatteredFairyWings,
                ItemID.SpookyWings,
                ItemID.FestiveWings,
                ItemID.BeetleWings,
                ItemID.FinWings,
                ItemID.FishronWings,
                ItemID.MothronWings,
                ItemID.WingsSolar,
                ItemID.WingsVortex,
                ItemID.WingsNebula,
                ItemID.WingsStardust,
                ItemID.Yoraiz0rWings,
                ItemID.JimsWings,
                ItemID.SkiphsWings,
                ItemID.LokisWings,
                ItemID.BetsyWings,
                ItemID.ArkhalisWings,
                ItemID.LeinforsWings,
            }));

            RecipeGroup.RegisterGroup("Virtuous:CelestialWings", new RecipeGroup(() => "Any Celestial Wings", new int[] {
                ItemID.WingsSolar,
                ItemID.WingsVortex,
                ItemID.WingsNebula,
                ItemID.WingsStardust,
            }));
        }
    }
}

