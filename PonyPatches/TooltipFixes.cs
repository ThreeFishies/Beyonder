using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using HarmonyLib;
using Void.Init;
using Void.Monsters;
using Trainworks.Constants;
using Trainworks.Builders;
using ShinyShoe.Loading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Void.Champions;

namespace Equestrian.HarmonyPatches
{
    //Finally Glaukopis will tell you what 'Rebate' actually does.
    [HarmonyPatch(typeof(RoomModifierData), "GetParamStatusEffects")]
    public static class FixRoomModifierTooltipsI
    {
        public static void Postfix(ref StatusEffectStackData[] __result)
        {
            if (__result == null)
            {
                __result = new StatusEffectStackData[0] { };
            }
        }
    }

    [HarmonyPatch(typeof(RoomModifierData), "GetExtraTooltipTitleKey")]
    public static class FixRoomModifierTooltipsII
    {
        public static void Postfix(ref string __result)
        {
            if (__result == null)
            {
                __result = string.Empty;
            }
        }
    }

    [HarmonyPatch(typeof(CardState), "GenerateCardBodyText")]
    public static class EpidemialAndGenericColonectomy
    {
        public static void Postfix(ref string generatedText, ref CardState __instance)
        {
            if (!Beyonder.IsInit) { return; }

            if (__instance.GetCardDataID() == Epidemial.card.GetID())
            {
                generatedText = generatedText.Replace(": When played, add a copy to the top of your draw pile.", "When played, add a copy to the top of your draw pile.");
            
                //Beyonder.Log(generatedText);
            }

            if (generatedText.Contains("<upgradeHighlight>:</upgradeHighlight> "))
            {
                generatedText = generatedText.Replace("<upgradeHighlight>:</upgradeHighlight> ", "");
            }

            if (generatedText.StartsWith(": "))
            {
                generatedText = generatedText.Substring(2);
            }
        }
    }

    /*
    [HarmonyPatch(typeof(CardTooltipContainer), "GetTooltipContentForGeneratedCard")]
    public static class YoLoNeedsExtraColonectomy
    {
        public static void Postfix(ref string __result, ref CardData cardData)
        {
            if (!Ponies.EquestrianClanIsInit) { return; }

            if (cardData.GetID() == CustomCardManager.GetCardDataByID(YoLo.ID).GetID())
            {
                __result = __result.Replace(": ", "");
            }
        }
    }
    */

    [HarmonyPatch(typeof(TooltipContainer), "InstantiateTooltip")]
    public static class OnUnscaledSummonEmptyTriggerTooltipRemoval 
    { 
        public static bool Prefix(ref string tooltipId, ref TooltipUI __result)
        {
            if (tooltipId == CharacterTriggerData.Trigger.OnUnscaledSpawn.ToString()) 
            {
                __result = null;
                return false;
            }

            return true;
        }
    }
}