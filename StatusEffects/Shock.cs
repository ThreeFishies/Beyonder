using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Void.Init;
using System.Collections;

namespace Void.Status
{
    public class StatusEffectShock : StatusEffectState
    {
        public static StatusEffectData data;
        public const string statusId = "beyonder_shock";

        //Associated Text keys:
        //StatusEffect_beyonder_shock_CardText
        //StatusEffect_beyonder_shock_CharacterTooltipText
        //StatusEffect_beyonder_shock_CardTooltipText
        //StatusEffect_beyonder_shock_NotificationText
        //StatusEffect_beyonder_shock_Stack_CardText

        public static void Build()
        {
            data = new StatusEffectDataBuilder()
            {
                StatusId = statusId,
                IsStackable = true,
                IconPath = "ClanAssets/Shock.png",
                TriggerStage = StatusEffectData.TriggerStage.OnAttacked,
                DisplayCategory = StatusEffectData.DisplayCategory.Negative,
                ShowStackCount = true,
                StatusEffectStateType = typeof(StatusEffectShock),
                RemoveStackAtEndOfTurn = false,
                RemoveWhenTriggered = true,
                
            }.Build();
            List<StatusEffectData.TriggerStage> triggerStages = new List<StatusEffectData.TriggerStage>
            {
                StatusEffectData.TriggerStage.OnCombatTurnDazed,
            };
            AccessTools.Field(typeof(StatusEffectData), "additionalTriggerStages").SetValue(data, triggerStages);
        }

        public override bool TestTrigger(StatusEffectState.InputTriggerParams inputTriggerParams, StatusEffectState.OutputTriggerParams outputTriggerParams)
        {
            outputTriggerParams.count = 1;

            //if (inputTriggerParams.damage > 0)
            //{
            //    outputTriggerParams.count = 1;
            //}

            if (inputTriggerParams.canAttackOrHeal || inputTriggerParams.canFireTriggers)
            {
                outputTriggerParams.canAttackOrHeal = false;
                outputTriggerParams.canFireTriggers = false;
            }

            if (inputTriggerParams.attacked != null)
            {
                if (inputTriggerParams.attacked.HasStatusEffect("untouchable"))
                {
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        protected override IEnumerator OnTriggered(StatusEffectState.InputTriggerParams inputTriggerParams, StatusEffectState.OutputTriggerParams outputTriggerParams) 
        {
            if (inputTriggerParams != null && inputTriggerParams.associatedCharacter != null)
            {
                inputTriggerParams.associatedCharacter.ShowNotification("StatusEffect_beyonder_shock_NotificationText".Localize(), PopupNotificationUI.Source.General, null);
            }
            yield break;
        }
    }
}