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
using CustomEffects;

namespace Void.Mania
{
    //Track Manic Cards Played
    public class TrackSacrificedSize : ExtendedEnum<TrackSacrificedSize, CardStatistics.TrackedValueType>
    {
        // Token: 0x06000106 RID: 262 RVA: 0x00005CDC File Offset: 0x00003EDC
        public TrackSacrificedSize(string localizationKey, int? ID = null) : base(localizationKey, ID ?? TrackSacrificedSize.GetNewTooltipTypeGUID())
        {
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00005D20 File Offset: 0x00003F20
        public static int GetNewTooltipTypeGUID()
        {
            TrackSacrificedSize.NumTypes++;
            return TrackSacrificedSize.NumTypes;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005D44 File Offset: 0x00003F44
        public static implicit operator TrackSacrificedSize(CardStatistics.TrackedValueType trackedValue)
        {
            return ExtendedEnum<TrackSacrificedSize, CardStatistics.TrackedValueType>.Convert(trackedValue);
        }

        // Token: 0x0400016E RID: 366
        // 39 is next in the list.
        private static int NumTypes = 38;

        public static void Initialize()
        {

        }
    }

    [HarmonyPatch(typeof(CardStatistics), "GetStatValue")]
    public static class AddTrackSacrificedSize
    {
        public static bool Prefix(ref CardStatistics.StatValueData statValueData, ref int __result)
        {
            if (!Beyonder.IsInit) { return true; }

            if (statValueData.trackedValue == Beyonder.trackSacrificedSize.GetEnum())
            {
                __result = CustomCardEffectSacrificeAssertTarget.SizeofSacrifice;

                //Beyonder.Log($"Tracking Sacrificed Allies Result: {__result}.");

                return false;
            }

            return true;
        }
    }
}