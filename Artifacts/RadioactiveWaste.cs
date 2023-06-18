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
using Void.Unit;
using Void.Clan;
using Void.Status;
using Void.Init;
using Void.Triggers;
using CustomEffects;
using RunHistory;
using Void.Spells;

namespace Void.Artifacts
{
    public static class RadioactiveWaste
    {
        public static CollectableRelicData Artifact;
        public static string ID = "RadioactiveWaste_" + Beyonder.GUID;

        public static bool HasIt()
        {
            if (Artifact != null)
            {
                return ProviderManager.SaveManager.GetHasRelic(Artifact);
            }
            return false;
        }

        public static CollectableRelicData BuildAndRegister()
        {
            Artifact = new CollectableRelicDataBuilder
            {
                CollectableRelicID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Beyonder_Artifact_RadioactiveWaste_Name_Key",
                DescriptionKey = "Beyonder_Artifact_RadioactiveWaste_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_RadioactiveWaste_Lore_Key"
                },
                IconPath = "ArtifactAssets/RadioactiveWaste.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 8,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        //Effect handled by the Mutated status effect data
                        RelicEffectClassName = "RelicEffectNull",

                        AdditionalTooltips = new AdditionalTooltipData[]
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "StatusEffect_beyonder_mutated_CardText",
                                descriptionKey = "StatusEffect_beyonder_mutated_CardTooltipText",
                                isStatusTooltip = true,
                                statusId = StatusEffectMutated.statusId,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Persistent,
                            },
                            new AdditionalTooltipData
                            {
                                titleKey = "StatusEffect_Multistrike_CardText",
                                descriptionKey = "StatusEffect_Multistrike_CardTooltipText",
                                isStatusTooltip = true,
                                statusId = VanillaStatusEffectIDs.Multistrike,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Persistent,
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            return Artifact;
        }
    }
}