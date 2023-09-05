using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using System.Collections;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using PubNubAPI;
using Void.Init;
using Void.Artifacts;

namespace Void.Status
{
    public class StatusEffectChronic : StatusEffectState
    {
        public static StatusEffectData data;
        public const string statusId = "beyonder_chronic";
        //public static int healthPerStack = 3;
        //public static List<CardState> cardsTriggeredOn = new List<CardState>() { };

        //Associated Text keys:
        //StatusEffect_beyonder_chronic_CardText
        //StatusEffect_beyonder_chronic_CharacterTooltipText
        //StatusEffect_beyonder_chronic_CardTooltipText
        //StatusEffect_beyonder_chronic_NotificationText
        //StatusEffect_beyonder_chronic_Stack_CardText

        public static void Build()
        {
            data = new StatusEffectDataBuilder()
            {
                StatusId = statusId,
                IsStackable = true,
                IconPath = "ClanAssets/Chronic.png",
                TriggerStage = StatusEffectData.TriggerStage.OnDeath,
                DisplayCategory = StatusEffectData.DisplayCategory.Positive,
                ShowStackCount = true,
                ParamInt = 3,
                StatusEffectStateType = typeof(StatusEffectChronic),
                RemoveWhenTriggered = false,
            }.Build();
            List<StatusEffectData.TriggerStage> triggerStages = new List<StatusEffectData.TriggerStage>
            {
                StatusEffectData.TriggerStage.OnPostEaten
            };
            AccessTools.Field(typeof(StatusEffectData), "additionalTriggerStages").SetValue(data, triggerStages);
        }

        //Chronic upgrade was not triggered by CardEffectsacrifice. Unclear why. Unit was not consumed, though.

        protected override IEnumerator OnTriggered(StatusEffectState.InputTriggerParams inputTriggerParams, StatusEffectState.OutputTriggerParams outputTriggerParams)
        {
            CharacterState characterState = inputTriggerParams.fromEaten ? inputTriggerParams.associatedCharacter : inputTriggerParams.attacked;

            if (characterState == null)
            {
                yield break;
            }

            CardState spawnerCard = characterState.GetSpawnerCard();
            int numStacks = characterState.GetStatusEffectStacks(base.GetStatusId());

            //Sometimes Chronic isn't adding the correct value. I suspect it's being triggered early in preview mode which 'locks in' a lower value than expected.
            //Try disabling in preview mode.
            if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.PreviewMode) 
            {
                //A sweep attacked KOed three chronic units and only one respawned. Trying adding back limited functionality in preview mode.
                if (spawnerCard != null && numStacks == 0 && !characterState.HasStatusEffect(VanillaStatusEffectIDs.Endless)) 
                {
                    spawnerCard.ClearRemoveFromStandByPileOverride();
                }
                if (spawnerCard != null && numStacks > 0 && !characterState.HasStatusEffect(VanillaStatusEffectIDs.Endless))
                {
                    spawnerCard.SetRemoveFromStandByPileOverride(CardPile.DiscardPile);
                }

                yield break;
            }

            //outputTriggerParams.count = 0;

            if (spawnerCard != null) // && !characterState.HasStatusEffect(VanillaStatusEffectIDs.Cardless)) Check unnecessary. Cardless units are immune to Chronic.
            {
                if (numStacks > 0)
                {
                    if (!characterState.HasStatusEffect(VanillaStatusEffectIDs.Endless))
                    {
                        spawnerCard.SetRemoveFromStandByPileOverride(CardPile.DiscardPile);
                    }

                    string notice = "StatusEffect_beyonder_chronic_NotificationText";

                    if (BloodyTentacles.HasIt()) 
                    {
                        notice = "StatusEffect_beyonder_chronic_AndDamage";
                    }

                    characterState.ShowNotification(string.Format(notice.Localize(null), GetEffectMagnitude(numStacks)), PopupNotificationUI.Source.General, null);

                    //Prevent the buff from being applied multiple times
                    //outputTriggerParams.count = numStacks;
                    //spawnerCard.GetTemporaryCardStateModifiers().AddUpgrade(GetCardUpgradeState(numStacks));

                    //if (!cardsTriggeredOn.Contains(spawnerCard))
                    //{
                        spawnerCard.GetTemporaryCardStateModifiers().AddUpgrade(GetCardUpgradeState(numStacks));

                        //Made things worse by allowing upgrade to get duplicated
                        //Try adding a preview check to see if this helps protect against sacrifice fail.
                        //if (!ProviderManager.SaveManager.PreviewMode)
                        //{ 
                    //        cardsTriggeredOn.Add(spawnerCard);
                        //}
                    //}
                }
            }
            yield break;
        }

        public override void OnStacksRemoved(CharacterState character, int numStacksRemoved) 
        {
            if (character == null || character.GetSpawnerCard() == null)
            { 
                return; 
            }

            if (!character.HasStatusEffect(statusId) && !character.HasStatusEffect(VanillaStatusEffectIDs.Endless))
            {
                character.GetSpawnerCard().ClearRemoveFromStandByPileOverride();
            }
        }

        public CardUpgradeState GetCardUpgradeState(int numStacks = 1) 
        {
            CardUpgradeData upgrade = new CardUpgradeDataBuilder()
            {
                UpgradeTitle = "Beyonder_Chronic_Status_Upgrade",
                BonusHP = GetEffectMagnitude(numStacks),
                BonusDamage = GetEffectMagnitude(numStacks) * (BloodyTentacles.HasIt() ? 1 : 0),
            }.Build();

            CardUpgradeState upgradeState = new CardUpgradeState();
            upgradeState.Setup(upgrade);

            //Beyonder.Log($"Chronic upgrade triggered. Stacks: {numStacks}. BonusHP: {upgradeState.GetAdditionalHP()}. ExpectedBonusHP: {GetEffectMagnitude(numStacks)}.");

            return upgradeState;
        }

        public override int GetMagnitudePerStack()
        {
            return base.GetParamInt() + this.relicManager.GetModifiedStatusMagnitudePerStack(statusId, base.GetAssociatedCharacter().GetTeamType());
        }

        public override int GetEffectMagnitude(int stacks = 1)
        {
            return this.GetMagnitudePerStack() * stacks;
        }
    }

    [HarmonyPatch(typeof(StatusEffectCardlessState), "OnStacksAdded")]
    public static class AddChronicImmunityToCardlessState 
    { 
        public static void Postfix(CharacterState character) 
        {
            character.AddImmunity(StatusEffectChronic.statusId);
        }
    }

    //[HarmonyPatch(typeof(CardState), "ClearRemoveFromStandByPileOverride")]
    //public static class ResetChronicState 
    //{
    //    public static void Postfix(ref CardState __instance) 
    //    { 
    //        StatusEffectChronic.cardsTriggeredOn.Remove(__instance);
    //    }
    //}
}