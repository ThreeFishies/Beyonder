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
    public static class MakeChildFormless 
    {
        public static bool Didit = false;

        public static void DoIt() 
        {
            if (Didit) { return; }

            CharacterData FormlessChild = CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.FormlessChild);
            StatusEffectStackData[] statusEffectStackDatas = AccessTools.Field(typeof(CharacterData), "startingStatusEffects").GetValue(FormlessChild) as StatusEffectStackData[];
            statusEffectStackDatas = statusEffectStackDatas.AddRangeToArray(new StatusEffectStackData[] 
            { 
                new StatusEffectStackData 
                {
                    statusId = StatusEffectFormless.statusId,
                    count = 1
                }
            });
            AccessTools.Field(typeof(CharacterData), "startingStatusEffects").SetValue(FormlessChild, statusEffectStackDatas);

            Didit = true;
        }
    }
}