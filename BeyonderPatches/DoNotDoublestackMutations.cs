using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using Trainworks.Constants;
using Trainworks.Builders;
using ShinyShoe.Loading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Void.Init;
using Void.Status;

namespace Void.Patches
{
    public static class DoNotDoublestackMutations 
    {
        public static bool done = false;
        public static void JustDont() 
        {
            if (done) return;

            EnhancerData JuiceStone = ProviderManager.SaveManager.GetAllGameData().FindEnhancerData("72f61ae8-7e0f-4066-a3fb-a1273f3aa273");
            if (JuiceStone != null) 
            {
                CardUpgradeMaskData ExcludeNonstackableStatusEffectsfilter = JuiceStone.GetEffects()[0].GetParamCardUpgradeData().GetFilters()[3];
                List<StatusEffectStackData> excludedStatusEffects = AccessTools.Field(typeof(CardUpgradeMaskData), "excludedStatusEffects").GetValue(ExcludeNonstackableStatusEffectsfilter) as List<StatusEffectStackData>;
                excludedStatusEffects.Add(new StatusEffectStackData 
                { 
                    statusId = StatusEffectMutated.statusId,
                    count = 1
                });
            }
            else 
            {
                Beyonder.LogError("Unable to find doublestack magic upgrade. What happened to it?");
            }

            done = true;
        }
    }
}