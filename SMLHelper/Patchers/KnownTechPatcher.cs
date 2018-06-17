﻿using System.Collections.Generic;
using KnownTechPatcher2 = SMLHelper.V2.Patchers.KnownTechPatcher;

namespace SMLHelper.Patchers
{
    [System.Obsolete("SMLHelper.KnownTechPatcher is obsolete. Please use SMLHelper.V2 instead.")]
    public class KnownTechPatcher
    {
        [System.Obsolete("SMLHelper.KnownTechPatcher.unlockedAtStart is obsolete. Please use SMLHelper.V2 instead.")]
        public static List<TechType> unlockedAtStart = new List<TechType>();

        internal static void Patch()
        {
            unlockedAtStart.ForEach(x => KnownTechPatcher2.unlockedAtStart.Add(x));

            V2.Logger.Log("Old KnownTechPatcher is done.");
        }
    }
}
