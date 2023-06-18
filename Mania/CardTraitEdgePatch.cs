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
    [HarmonyPatch(typeof(CardUI), "UpdateCardLayout")]
    public static class CardTraitEdgePatch
    {
        public static void Postfix(CardState cardState, ref Transform ____positiveReserveEdgeFXTransform, ref Transform ____reserveEdgeFXTransform, ref Transform ___freezeFxTransform, ref CardUI.CardUIState ___currentUIState)
        {
            if (___currentUIState != CardUI.CardUIState.Hand) 
            {
                return;
            }

            //Transform afflictive = GameObject.Instantiate<Transform>(____positiveReserveEdgeFXTransform);
            bool Afflictive = false;
            bool Compulsive = false;

            if (cardState.HasTrait(typeof(BeyonderCardTraitAfflictive))) 
            {
                Afflictive = true;
            }
            if (cardState.HasTrait(typeof(BeyonderCardTraitCompulsive)))
            {
                Compulsive = true;
            }

            int Mania = ManiaManager.Mania;

            bool IsManic = Afflictive || Compulsive;

            if (!IsManic) 
            { 
                return;
            }

            bool WillFireTrigger = (Afflictive && Mania >= 0) || (Compulsive && Mania <= 0);
            bool WillTriggerInsanity = (Afflictive && (Mania >= (ManiaManager.GetInsanityThreshold() - 1))) || (Compulsive && (Mania <= (-ManiaManager.GetInsanityThreshold() + 1)));

            //Beyonder.Log($"Current Mania: {Mania}. Afflictive: {Afflictive}. Compulsive: {Compulsive}. IsManic: {IsManic}. WillFireTrigger: {WillFireTrigger}. WillTriggerInsanity: {WillTriggerInsanity}");

            ____reserveEdgeFXTransform.gameObject.SetActive(WillTriggerInsanity);
            ____positiveReserveEdgeFXTransform.gameObject.SetActive(WillFireTrigger && !WillTriggerInsanity);

            //bool IsFrozen = cardState.HasTrait(typeof(CardTraitPermafrost)) || cardState.HasTrait(typeof(CardTraitFreeze));

            //___freezeFxTransform.gameObject.SetActive(IsFrozen || (IsManic && !WillFireTrigger));
        }
    }
}