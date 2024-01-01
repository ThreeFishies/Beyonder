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
using Void.Artifacts;

namespace Void.Status 
{ 
    public class StatusEffectMutated : StatusEffectState
    {
        public static StatusEffectData data;
        public const string statusId = "beyonder_mutated";

        //Associated Text keys:
        //StatusEffect_beyonder_mutated_CardText
        //StatusEffect_beyonder_mutated_CharacterTooltipText
        //StatusEffect_beyonder_mutated_CardTooltipText
        //StatusEffect_beyonder_mutated_NotificationText
        //StatusEffect_beyonder_mutated_Stack_CardText

        public static void Build() 
        {
            data = new StatusEffectDataBuilder()
            {
                StatusID = statusId,
                IsStackable = false,
                IconPath = "ClanAssets/StatusIconsBig/Mutated.png",
                TooltipIconPath = "ClanAssets/Mutated.png",
                TriggerStage = StatusEffectData.TriggerStage.None,
                DisplayCategory = StatusEffectData.DisplayCategory.Persistent,
                ShowStackCount = false,
                StatusEffectStateType = typeof(StatusEffectMutated)
            }.Build();
        }

        public override void OnStacksAdded(CharacterState character, int numStacksAdded)
        {
            if (RadioactiveWaste.HasIt() && numStacksAdded > 0)
            {
                character.AddStatusEffect(VanillaStatusEffectIDs.Multistrike, 1);
            }
        }
    }
}