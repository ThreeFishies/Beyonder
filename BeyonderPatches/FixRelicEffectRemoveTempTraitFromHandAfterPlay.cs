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
    [HarmonyPatch(typeof(CardState), "HasTemporaryTrait")]
    public static class BeCarefulOrABadThingCouldHappen 
    {
        public static bool ItHAPPENED = false;

        public static bool Prefix(ref bool __result, ref Type cardTraitType)
        {
            if (cardTraitType == null) 
            {
                //Beyonder.Log("A BAD THING HAPPENED!");
                ItHAPPENED = true;
                __result = false;
                return false;
            }

            ItHAPPENED = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(CardState), "HasTrait")]
    public static class BeVeryVeryCarefulOrABadThingCouldHappen
    {
        public static bool ItHAPPENED = false;

        public static bool Prefix(ref bool __result, ref Type cardTraitType)
        {
            if (cardTraitType == null)
            {
                //Beyonder.Log("A BAD THING HAPPENED AGAIN!");
                ItHAPPENED = true;
                __result = false;
                return false;
            }

            ItHAPPENED = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(RelicEffectRemoveTempTraitFromHandAfterPlay), "TraitDataShouldBeRemoved")]
    public static class FixRelicEffectRemoveTempTraitFromHandAfterPlay
    {
        public static bool Prefix(ref bool __result, ref CardTraitData traitData, ref CardState cardState) 
        {
            if (traitData == null || cardState == null) 
            {
                //Beyonder.Log("Error: Card trait or state were null.");
                __result = false;
                return false;
            }

            return true;
        }
        public static void Postfix(ref bool __result, ref CardTraitData traitData, ref CardState cardState, ref bool ____removeIfCardHasNonTempTrait, ref CardType ____cardType)
        {
            if (!BeCarefulOrABadThingCouldHappen.ItHAPPENED || traitData == null || cardState == null) 
            {
                return;
            }

            //Beyonder.Log("Oopsie detected. Attempting to fix.");

            string assemblyQualifier = typeof(CardTraitExhaustState).AssemblyQualifiedName;
            assemblyQualifier = assemblyQualifier.Substring("CardTraitExhaustState".Length);

            Type traitType = Type.GetType(traitData.GetTraitStateName());

            if (traitType == null)
            {
                traitType = Type.GetType(traitData.GetTraitStateName() + assemblyQualifier);
            }

            if (traitType == null)
            {
                Beyonder.Log("Failed to correct for oopsie. Failing badly. Stuff will be broken.", BepInEx.Logging.LogLevel.Warning);
                return;
            }

            bool flag = cardState.HasTemporaryTrait(traitType);
            if (____removeIfCardHasNonTempTrait)
            {
                flag |= cardState.HasTrait(traitType);
            }
            __result = cardState != null && traitData != null && !cardState.IsChampionCard() && cardState.GetCardType() == ____cardType && flag;

            BeCarefulOrABadThingCouldHappen.ItHAPPENED = false;
            return;
        }
    }
}