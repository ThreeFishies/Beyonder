
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
    //Traitor's Quill is not working on cards that have more than one card effect outside of preview mode. Reason unknown.
    //Testing: targetsRoom => true on effeted cards
    //Fixed. targetsRoom and targetless should never both be false or index will be set to -1 on play. Preview pulls the index from the target of the individual card effects instead.

    /*
    public static class UpdateRewardCountTest 
    { 
        public static void AwokenHerzalTestReward() 
        {
            //This adds an additional option to the Awoken Banner and Herzal's Hoard. Testing was successful.

            DraftRewardData AwokenBannerDraft = ProviderManager.SaveManager.GetAllGameData().FindRewardData("ddc71f3d-e2b2-4abd-8de4-accb646db775") as DraftRewardData;
            uint newcount = 3U;
            AccessTools.Field(typeof(DraftRewardData), "draftOptionsCount").SetValue(AwokenBannerDraft, newcount);

            RelicDraftRewardData HerzalsHoardReward = ProviderManager.SaveManager.GetAllGameData().FindRewardData("67f2e2e9-7c55-4c4e-9022-003aa66055de") as RelicDraftRewardData;
            AccessTools.Field(typeof(RelicDraftRewardData), "draftOptionsCount").SetValue(HerzalsHoardReward, newcount);
        }
    }
    */
    /*
    [HarmonyPatch(typeof(RelicEffectDamageOnExhaust), "TestCardPlayed")]
    public static class LogTestEffect 
    {
        public static void Postfix(CardPlayedRelicEffectParams relicEffectParams, ref bool __result, ref TargetMode ___targetMode, ref Team.Type ___targetTeam, ref List<CharacterState> ___targets) 
        {
            if (relicEffectParams == null) { return; }
            if (ProviderManager.SaveManager == null) { return; }
            Beyonder.Log($"Testing RelicEffectDamageOnExhaust on card: {relicEffectParams.cardState.GetTitle()}. Result: {__result}. IsPreview: {ProviderManager.SaveManager.PreviewMode}.");

            //Room index changes to -1. Reason: Unknown.
            Beyonder.Log($"Target mode: {___targetMode}. Team Type: {___targetTeam}. Room index: {relicEffectParams.roomIndex}. NumTatgets: {___targets.Count}.");
        }
    }
    */

    //This works. Must be issue with targeting then.
    /*
    [HarmonyPatch(typeof(CardTraitExhaustState), "WillExhaustOnNextPlay")]
    public static class LogTestEffect2
    {
        public static void Postfix(CardState cardState, ref bool __result)
        {
            if (cardState == null) { return; }
            if (ProviderManager.SaveManager == null) { return; }
            Beyonder.Log($"Testing WillExhaustOnNextPlay on card: {cardState.GetTitle()}. Result: {__result}. IsPreview: {ProviderManager.SaveManager.PreviewMode}.");
        }
    }
    */
    /*
    [HarmonyPatch(typeof(CardManager), "PlayAnyCard")]
    public static class LogTestEffect3 
    {
        public static void Prefix(CardState cardState, ref RoomManager ___roomManager) 
        {
            Beyonder.Log($"Testing PlayAnyCard on card: {cardState.GetTitle()}. .");
        }
    }
    */
}
