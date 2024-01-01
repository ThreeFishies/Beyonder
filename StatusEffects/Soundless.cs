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

namespace Void.Status
{
    public class StatusEffectSoundless : StatusEffectState
    {
        public static StatusEffectData data;
        public const string statusId = "beyonder_soundless";

        //Associated Text keys:
        //StatusEffect_beyonder_soundless_CardText
        //StatusEffect_beyonder_soundless_CharacterTooltipText
        //StatusEffect_beyonder_soundless_CardTooltipText
        //StatusEffect_beyonder_soundless_NotificationText
        //StatusEffect_beyonder_soundless_Stack_CardText

        public static void Build()
        {
            data = new StatusEffectDataBuilder()
            {
                StatusID = statusId,
                IsStackable = false,
                IconPath = "ClanAssets/StatusIconsBig/soundless.png",
                TooltipIconPath = "ClanAssets/soundless.png",
                TriggerStage = StatusEffectData.TriggerStage.None,
                DisplayCategory = StatusEffectData.DisplayCategory.Persistent,
                ShowStackCount = false,
                StatusEffectStateType = typeof(StatusEffectSoundless)
            }.Build();
        }

        public override void OnStacksAdded(CharacterState character, int numStacksAdded)
        {
            if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.PreviewMode) 
            {
                base.OnStacksAdded(character, numStacksAdded);
                return;
            }

            if (numStacksAdded > 0)
            {
                character.AddImmunity("beyonder_shock");
            }
            base.OnStacksAdded(character, numStacksAdded);
        }

        public override void OnStacksRemoved(CharacterState character, int numStacksRemoved)
        {
            if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.PreviewMode)
            {
                base.OnStacksRemoved(character, numStacksRemoved);
                return;
            }

            if (numStacksRemoved > 0)
            {
                character.AssertNotDestroyed();
                List<string> immunities = (List<string>)AccessTools.Field(typeof(CharacterState), "statusEffectImmunities").GetValue(character);
                immunities.Remove("beyonder_shock");
            }
            base.OnStacksRemoved(character, numStacksRemoved);
        }
    }
}