using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using ShinyShoe.Loading;
using Void.Init;
using Void.Enhancers;

namespace Void.Enhancers
{
    [HarmonyPatch(typeof(EnhancerPool), "GetFilteredChoices")]
    class MakeEnhancersClanExclusive
    {
        //Trainworks replaces the original code with their own version that does not take class restrictions of custom enhancers into account.
        //This will edit the result to remove Beyonder clan-specific enhancers from the pool if they're not present.
        static public void Postfix(ref List<EnhancerData> __result) 
        {
            if (!Beyonder.IsInit) { return; }

            SaveManager saveManager = ProviderManager.SaveManager;

            if (saveManager.HasMainClass())
            {
                if (saveManager.GetMainClass().name == Beyonder.BeyonderClanData.name || saveManager.GetSubClass().name == Beyonder.BeyonderClanData.name)
                {
                    return;
                }
                else
                {
                    __result.Remove(Riftstone.Enhancer);
                    __result.Remove(Veilstone.Enhancer);
                    __result.Remove(Voidstone.Enhancer);
                    __result.Remove(Sanitystone.Enhancer);
                }
            }
        }
    }
}