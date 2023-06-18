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
using ShinyShoe;
using Void.Artifacts;

namespace Void.Status
{
    public class StatusEffectJitters : StatusEffectState, IDamageStatusEffect
    {
        public static StatusEffectData data;
        public const string statusId = "beyonder_jitters";

        //Associated Text keys:
        //StatusEffect_beyonder_jitters_CardText
        //StatusEffect_beyonder_jitters_CharacterTooltipText
        //StatusEffect_beyonder_jitters_CardTooltipText
        //StatusEffect_beyonder_jitters_NotificationText
        //StatusEffect_beyonder_jitters_Stack_CardText

        public static void Build()
        {
            data = new StatusEffectDataBuilder()
            {
                StatusId = statusId,
                IsStackable = true,
                IconPath = "ClanAssets/Jitters.png",
                TriggerStage = StatusEffectData.TriggerStage.OnPreAttackedSpellShield,
                DisplayCategory = StatusEffectData.DisplayCategory.Negative,
                ShowStackCount = true,
                StatusEffectStateType = typeof(StatusEffectJitters),
                RemoveStackAtEndOfTurn = true,
                RemoveWhenTriggered = true,
                ParamInt = 1
            }.Build();
        }

        public override int GetEffectMagnitude(int stacks = 1)
        {
            return this.GetDamageAmount(stacks);
        }

        // Token: 0x06002C32 RID: 11314 RVA: 0x000AC746 File Offset: 0x000AA946
        private int GetDamageAmount(int stacks)
        {
            //Beyonder.Log("JittersID : " + statusId + " On Team: " + base.GetAssociatedCharacter().GetTeamType().ToString());
            //Beyonder.Log("Relic Modifier: " + this.relicManager.GetModifiedStatusMagnitudePerStack(statusId, base.GetAssociatedCharacter().GetTeamType()));
            //Beyonder.Log("Bed Monster?: " + BedMonster.HasIt());
            //Beyonder.Log("HasFlagTest?: " + Team.Type.Heroes.HasFlag(Team.Type.Monsters | Team.Type.Heroes)); //FALSE
            return (base.GetParamInt() + this.relicManager.GetModifiedStatusMagnitudePerStack(statusId, base.GetAssociatedCharacter().GetTeamType())) * stacks;
        }

        public override bool TestTrigger(StatusEffectState.InputTriggerParams inputTriggerParams, StatusEffectState.OutputTriggerParams outputTriggerParams)
        {
            outputTriggerParams.count = 1;

            if (inputTriggerParams.attacked == null)
            {
                Beyonder.Log("Attempting to invoke Jitters on a non-existant unit.", BepInEx.Logging.LogLevel.Warning);
                return false;
            }
            else
            {
                if (inputTriggerParams.attacked.HasStatusEffect(VanillaStatusEffectIDs.Spellshield) || inputTriggerParams.attacked.HasStatusEffect(VanillaStatusEffectIDs.DamageShield)) 
                {
                    bool flag = false;
                    if (inputTriggerParams.damageSourceCard != null)
                    {
                        flag = inputTriggerParams.damageSourceCard.HasTrait(typeof(CardTraitIgnoreArmor));
                    }
                    if (flag && inputTriggerParams.damageSourceCard != null && inputTriggerParams.damageSourceCard.GetCardType() == CardType.Spell)
                    {
                    }
                    else if(inputTriggerParams.damageSourceCard != null && inputTriggerParams.damageSourceCard.GetCardType() == CardType.Spell || inputTriggerParams.attacked.HasStatusEffect(VanillaStatusEffectIDs.DamageShield))
                    {
                        outputTriggerParams.damage = 0;
                        return false;
                    }
                }

                if (inputTriggerParams.damage == 0)
                { 
                    outputTriggerParams.damage = 0;
                    return false;
                }

                outputTriggerParams.damage = inputTriggerParams.damage + GetDamageAmount(inputTriggerParams.attacked.GetStatusEffectStacks(base.GetStatusId()));
                return true;
            }
        }
    }
}