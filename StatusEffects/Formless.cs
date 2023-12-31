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

namespace Void.Status
{
    public class StatusEffectFormless : StatusEffectState
    {
        public static StatusEffectData data;
        public const string statusId = "beyonder_formless";

        //Associated Text keys:
        //StatusEffect_beyonder_formless_CardText
        //StatusEffect_beyonder_formless_CharacterTooltipText
        //StatusEffect_beyonder_formless_CardTooltipText
        //StatusEffect_beyonder_formless_NotificationText
        //StatusEffect_beyonder_formless_Stack_CardText

        public static void Build()
        {
            data = new StatusEffectDataBuilder()
            {
                StatusId = statusId,
                IsStackable = false,
                IconPath = "ClanAssets/Formless.png",
                TriggerStage = StatusEffectData.TriggerStage.None,
                DisplayCategory = StatusEffectData.DisplayCategory.Persistent,
                ShowStackCount = false,
                StatusEffectStateType = typeof(StatusEffectFormless)
            }.Build();
        }

        public override void OnStacksAdded(CharacterState character, int numStacksAdded)
        {
            if (numStacksAdded > 0)
            {
                character.AddImmunity("beyonder_panic");
            }
            base.OnStacksAdded(character, numStacksAdded);
        }

        public override void OnStacksRemoved(CharacterState character, int numStacksRemoved)
        {
            if (numStacksRemoved > 0)
            {
                character.AssertNotDestroyed();
                List<string> immunities = (List<string>)AccessTools.Field(typeof(CharacterState), "statusEffectImmunities").GetValue(character);
                immunities.Remove("beyonder_panic");
            }
            base.OnStacksRemoved(character, numStacksRemoved);
        }
    }
}