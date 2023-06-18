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
using Void.Artifacts;
using Void.Status;

namespace Void.Mania
{
    [HarmonyPatch(typeof(TooltipContainer), "TraitSupportedInTooltips")]
    public static class CardTraitTooltipsPatch 
    { 
        public static void Postfix(ref bool __result, string traitName) 
        {
            if (traitName == "BeyonderCardTraitAfflictive" || traitName == "BeyonderCardTraitAfflictive_CardText" || traitName == typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName || traitName == typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName + "_CardText") 
            {
                __result = true;
                return;
            }
            if (traitName == "BeyonderCardTraitCompulsive" || traitName == "BeyonderCardTraitCompulsive_CardText" || traitName == typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName || traitName == typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName + "_CardText")
            {
                __result = true;
                return;
            }
            if (traitName == "BeyonderCardTraitEntropic" || traitName == "BeyonderCardTraitEntropic_CardText" || traitName == typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName || traitName == typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName + "_CardText")
            {
                __result = true;
                return;
            }
            if (traitName == "BeyonderCardTraitStalkerState" || traitName == "BeyonderCardTraitStalkerState_CardText" || traitName == typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName || traitName == typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName + "_CardText")
            {
                __result = true;
                return;
            }
            if (traitName == "BeyonderCardTraitTherapeutic" || traitName == "BeyonderCardTraitTherapeutic_CardText" || traitName == typeof(BeyonderCardTraitTherapeutic).AssemblyQualifiedName || traitName == typeof(BeyonderCardTraitTherapeutic).AssemblyQualifiedName + "_CardText")
            {
                __result = true;
                return;
            }

            //if (__result == false) 
            //{
            //    Beyonder.Log("Trait unsupported by tooltips: " + traitName);
            //}
        }
    }
}