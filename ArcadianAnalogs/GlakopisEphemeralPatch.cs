using System;
using Trainworks.Managers;
using HarmonyLib;

namespace Void.Arcadian
{
    /// <summary>
    /// For whatever reason, starter spells don't use the 'starter' rarity.
    /// So to make the Equestrian Analogs work with Glakopis' Ephemeral path, they need to actually be his starter card.
    /// </summary>
    [HarmonyPatch(typeof(ClassData), "GetChampionData")]
    public static class GlakopisEphemeralPatch
    {
        public static void Postfix(ref ChampionData __result) 
        { 
            if(!ArcadianCompatibility.ArcadianExists) { return; }

            if (__result != null && __result.championCardData != null && __result.championCardData.GetID() == CustomCardManager.GetCardDataByID("SecondDisciple").GetID())
            {
                ArcadianCompatibility.CheckRunStatus();

                if (ProviderManager.SaveManager.IsInBattle() == false)
                {
                    __result.starterCardData = ArcadianCompatibility.Analog;
                    return;
                }
                else if (ArcadianCompatibility.isArcadianExile && ArcadianCompatibility.isBeyonder && !ArcadianCompatibility.isBeyonderExile) 
                {
                    __result.starterCardData = CustomCardManager.GetCardDataByID(BeyonderAnalogBaseCompulsive.ID);
                    return;
                }
                else if (ArcadianCompatibility.isArcadianExile && ArcadianCompatibility.isBeyonder && ArcadianCompatibility.isBeyonderExile)
                {
                    __result.starterCardData = CustomCardManager.GetCardDataByID(BeyonderAnalogExileAfflictive.ID);
                    return;
                }
            }
        }
    }
}