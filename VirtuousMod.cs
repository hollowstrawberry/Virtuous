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
    }
}

