using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using System.Collections;
using System;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Trainworks.Enums.MTTriggers;
using Void.Init;
using Void.Triggers;
using Trainworks.Enums;

namespace Void.Mania
{
    //Track Manic Cards Played
    public class TrackCardsByEntropic : ExtendedEnum<TrackCardsByEntropic, CardStatistics.TrackedValueType>
    {
        // Token: 0x06000106 RID: 262 RVA: 0x00005CDC File Offset: 0x00003EDC
        public TrackCardsByEntropic(string localizationKey, int? ID = null) : base(localizationKey, ID ?? TrackCardsByEntropic.GetNewTooltipTypeGUID())
        {
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00005D20 File Offset: 0x00003F20
        public static int GetNewTooltipTypeGUID()
        {
            TrackCardsByEntropic.NumTypes++;
            return TrackCardsByEntropic.NumTypes;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005D44 File Offset: 0x00003F44
        public static implicit operator TrackCardsByEntropic(CardStatistics.TrackedValueType trackedValue)
        {
            return ExtendedEnum<TrackCardsByEntropic, CardStatistics.TrackedValueType>.Convert(trackedValue);
        }

        // Token: 0x0400016E RID: 366
        // 36 is next in the list.
        private static int NumTypes = 35;

        public static void Initialize()
        {

        }
    }

    [HarmonyPatch(typeof(CardStatistics), "GetStatValue")]
    public static class AddTrackCardsByEntropic
    {
        public static bool Prefix(ref CardStatistics.StatValueData statValueData, ref int __result)
        {
            if (!Beyonder.IsInit) { return true; }

            if (statValueData.trackedValue == Beyonder.TrackCardsByEntropic.GetEnum())
            {
                __result = 0;

                if (ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager)) 
                { 
                    List<CardState> allCards = cardManager.GetAllCards();
                    foreach (CardState card in allCards) 
                    {
                        if (card.HasTrait(typeof(BeyonderCardTraitEntropic)) && ((statValueData.cardTypeTarget == CardStatistics.CardTypeTarget.Any) || (((int)statValueData.cardTypeTarget) -1 == (int)card.GetCardType()) || ((int)statValueData.cardTypeTarget + 2 == (int)card.GetCardType()))) 
                        {
                            __result++;
                        }
                    }
                }

                return false;
            }

            return true;
        }
    }
}