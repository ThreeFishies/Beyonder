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
using Void.Artifacts;

namespace Void.Mania
{
    //Scale via Anxiety
    public class ScalingByAnxiety : ExtendedEnum<ScalingByAnxiety, CardStatistics.TrackedValueType>
    {
        // Token: 0x06000106 RID: 262 RVA: 0x00005CDC File Offset: 0x00003EDC
        public ScalingByAnxiety(string localizationKey, int? ID = null) : base(localizationKey, ID ?? ScalingByAnxiety.GetNewTooltipTypeGUID())
        {
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00005D20 File Offset: 0x00003F20
        public static int GetNewTooltipTypeGUID()
        {
            ScalingByAnxiety.NumTypes++;
            return ScalingByAnxiety.NumTypes;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005D44 File Offset: 0x00003F44
        public static implicit operator ScalingByAnxiety(CardStatistics.TrackedValueType trackedValue)
        {
            return ExtendedEnum<ScalingByAnxiety, CardStatistics.TrackedValueType>.Convert(trackedValue);
        }

        // Token: 0x0400016E RID: 366
        // 33 is next in the list.
        private static int NumTypes = 32;

        public static void Initialize()
        {
    
        }
    }

    //Scale via Hysteria
    public class ScalingByHysteria : ExtendedEnum<ScalingByHysteria, CardStatistics.TrackedValueType>
    {
        // Token: 0x06000106 RID: 262 RVA: 0x00005CDC File Offset: 0x00003EDC
        public ScalingByHysteria(string localizationKey, int? ID = null) : base(localizationKey, ID ?? ScalingByHysteria.GetNewTooltipTypeGUID())
        {
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00005D20 File Offset: 0x00003F20
        public static int GetNewTooltipTypeGUID()
        {
            ScalingByHysteria.NumTypes++;
            return ScalingByHysteria.NumTypes;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005D44 File Offset: 0x00003F44
        public static implicit operator ScalingByHysteria(CardStatistics.TrackedValueType trackedValue)
        {
            return ExtendedEnum<ScalingByHysteria, CardStatistics.TrackedValueType>.Convert(trackedValue);
        }

        // Token: 0x0400016E RID: 366
        // 34 is next in the list.
        private static int NumTypes = 33;

        public static void Initialize()
        {

        }
    }

    [HarmonyPatch(typeof(CardStatistics), "GetStatValue")]
    public static class AddScalingByAnxietyAndHysteria 
    { 
        public static bool Prefix(ref CardStatistics.StatValueData statValueData, ref int __result) 
        { 
            if (!Beyonder.IsInit) { return true; }

            if (statValueData.trackedValue == Beyonder.ScalingByAnxiety.GetEnum()) 
            {
                __result = ManiaManager.GetCurrentMania(statValueData.cardState);

                /*

                //if (statValueData.forPreviewText && statValueData.cardState != null && statValueData.cardState.GetNumTraits() > 0)
                if (statValueData.cardState != null && statValueData.cardState.GetNumTraits() > 0)
                {
                    foreach (CardTraitData cardTrait in statValueData.cardState.GetTraits())
                    {
                        if (cardTrait.GetTraitStateName() == typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName)
                        {
                            __result += cardTrait.GetParamInt();
                        }

                        if (cardTrait.GetTraitStateName() == typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName)
                        {
                            __result -= cardTrait.GetParamInt();
                        }

                        //if (cardTrait.GetTraitStateName() == typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName)
                        //{
                        //    __result *= ManiaManager.GetEntopicScalingValue(false, __result);
                        //}
                    }
                }
                */
                
                __result = (__result < 0) ? -__result : (BlackLight.HasIt() ? __result : 0);

                return false;
            }
            if (statValueData.trackedValue == Beyonder.ScalingByHysteria.GetEnum())
            {
                __result = ManiaManager.GetCurrentMania(statValueData.cardState);

                /*
                //if (statValueData.forPreviewText && statValueData.cardState != null && statValueData.cardState.GetNumTraits() > 0)
                if (statValueData.cardState != null && statValueData.cardState.GetNumTraits() > 0)
                {
                    foreach (CardTraitData cardTrait in statValueData.cardState.GetTraits())
                    {
                        if (cardTrait.GetTraitStateName() == typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName)
                        {
                            __result += cardTrait.GetParamInt();
                        }

                        if (cardTrait.GetTraitStateName() == typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName)
                        {
                            __result -= cardTrait.GetParamInt();
                        }

                        //if (cardTrait.GetTraitStateName() == typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName)
                        //{
                        //    __result *= ManiaManager.GetEntopicScalingValue(false, __result);
                        //}
                    }
                }
                */

                __result = (__result > 0) ? __result : (BlackLight.HasIt() ? -__result : 0);

                return false;
            }

            return true;
        }
    }
}