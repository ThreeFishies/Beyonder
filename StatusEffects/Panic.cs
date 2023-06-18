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

namespace Void.Status
{
    public class StatusEffectPanic : StatusEffectState
    {
        public static StatusEffectData data;
        public static int Multiplier = 2;
        public const string statusId = "beyonder_panic";

        //Associated Text keys:
        //StatusEffect_beyonder_panic_CardText
        //StatusEffect_beyonder_panic_CharacterTooltipText
        //StatusEffect_beyonder_panic_CardTooltipText
        //StatusEffect_beyonder_panic_NotificationText
        //StatusEffect_beyonder_panic_Stack_CardText

        public static void Build()
        {
            data = new StatusEffectDataBuilder()
            {
                StatusId = statusId,
                IsStackable = true,
                IconPath = "ClanAssets/Panic.png",
                TriggerStage = StatusEffectData.TriggerStage.OnPreAttacked,
                DisplayCategory = StatusEffectData.DisplayCategory.Negative,
                ShowStackCount = true,
                StatusEffectStateType = typeof(StatusEffectPanic),
                RemoveStackAtEndOfTurn = true,
                RemoveWhenTriggered = true,
            }.Build();
            //List<StatusEffectData.TriggerStage> triggerStages = new List<StatusEffectData.TriggerStage>
            //{
            //    StatusEffectData.TriggerStage.OnPostRoomCombat,
            //};
            //AccessTools.Field(typeof(StatusEffectData), "additionalTriggerStages").SetValue(data, triggerStages);
        }

		public override bool TestTrigger(StatusEffectState.InputTriggerParams inputTriggerParams, StatusEffectState.OutputTriggerParams outputTriggerParams)
		{
            outputTriggerParams.count = 1;
            outputTriggerParams.damage = inputTriggerParams.damage * GetDamageMultiplier(1);

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

        // Token: 0x06002C0A RID: 11274 RVA: 0x000AC217 File Offset: 0x000AA417
        public override int GetEffectMagnitude(int stacks = 1)
		{
			return this.GetDamageMultiplier(stacks);
		}

		// Token: 0x06002C0B RID: 11275 RVA: 0x000AC220 File Offset: 0x000AA420
		private int GetDamageMultiplier(int stacks)
		{
			return Multiplier;
		}
	}
}