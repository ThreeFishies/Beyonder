using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.BuildersV2; //Swapping to the new Trainworks builder.
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
        public bool shouldSilence = false;
        public StatusEffectData.TriggerStage triggerStage { get; set; }

        //Associated Text keys:
        //StatusEffect_beyonder_shock_CardText
        //StatusEffect_beyonder_shock_CharacterTooltipText
        //StatusEffect_beyonder_shock_CardTooltipText
        //StatusEffect_beyonder_shock_NotificationText
        //StatusEffect_beyonder_shock_Stack_CardText

        public static void Build()
        {
            VfxAtLoc persistent = null;
            VfxAtLoc added = null;

            if (ProviderManager.TryGetProvider<StatusEffectManager>(out StatusEffectManager statusEffectManager)) 
            {
                StatusEffectData dazed = statusEffectManager.GetStatusEffectDataById(VanillaStatusEffectIDs.Dazed);
                persistent = dazed.GetPersistentVFX();
                added = dazed.GetOnAddedVFX();
            };


            data = new StatusEffectDataBuilder()
            {
                StatusID = statusId,
                IsStackable = true,
                IconPath = "ClanAssets/StatusIconsBig/Shock.png",
                TooltipIconPath = "ClanAssets/Shock.png",
                TriggerStage = StatusEffectData.TriggerStage.OnAttacked,
                DisplayCategory = StatusEffectData.DisplayCategory.Negative,
                ShowStackCount = true,
                StatusEffectStateType = typeof(StatusEffectShock),
                RemoveStackAtEndOfTurn = false,
                RemoveWhenTriggered = true,
                PersistentVFX = persistent,
                AddedVFX = added
            }.Build();
            List<StatusEffectData.TriggerStage> triggerStages = new List<StatusEffectData.TriggerStage>
            {
                StatusEffectData.TriggerStage.OnCombatTurnDazed,
                StatusEffectData.TriggerStage.OnPreCharacterTrigger
            };
            AccessTools.Field(typeof(StatusEffectData), "additionalTriggerStages").SetValue(data, triggerStages);
        }

        public override bool TestTrigger(StatusEffectState.InputTriggerParams inputTriggerParams, StatusEffectState.OutputTriggerParams outputTriggerParams)
        {
            outputTriggerParams.count = 1;

            if (triggerStage == StatusEffectData.TriggerStage.OnAttacked) 
            {
                if (this.GetAssociatedCharacter() != null && this.GetAssociatedCharacter().HasEffectTrigger(CharacterTriggerData.Trigger.OnHit))
                {
                    return false;
                }
            }

            if (shouldSilence)
            {
                outputTriggerParams.canAttackOrHeal = false;
                outputTriggerParams.canFireTriggers = false;
                shouldSilence = false;
            }
            else 
            {
                return false;
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
                inputTriggerParams.associatedCharacter.ShowNotification("StatusEffect_Beyonder_shock_NotificationText".Localize(), PopupNotificationUI.Source.General, null);
            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(StatusEffectState), "IsValidTriggerStage")]
    public static class SetShouldFireTriggersShockState 
    {
        public static void Postfix(ref StatusEffectData.TriggerStage testTriggerStage, ref StatusEffectState __instance)
        {
            if (!Beyonder.IsInit) { return; }

            if (__instance is StatusEffectShock) 
            {
                StatusEffectShock @this = __instance as StatusEffectShock;

                if (@this == null) { return; }

                @this.triggerStage = testTriggerStage;

                if (testTriggerStage == StatusEffectData.TriggerStage.OnAttacked)
                {
                    @this.shouldSilence = true;
                }
                if (testTriggerStage == StatusEffectData.TriggerStage.OnCombatTurnDazed)
                {
                    @this.shouldSilence = true;
                }

                __instance = @this;
            }
        }
    }
}