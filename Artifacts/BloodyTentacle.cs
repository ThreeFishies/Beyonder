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
    public static class BloodyTentacles
    {
        public static CollectableRelicData Artifact;
        public static string ID = "BloodyTentacles_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_BloodyTentacles_Name_Key",
                DescriptionKey = "Beyonder_Artifact_BloodyTentacles_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_BloodyTentacles_Lore_Key"
                },
                IconPath = "ArtifactAssets/BloodyTentacle.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 6,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        //RelicEffectClassType = typeof(RelicEffectModifyStatusMagnitude),
                        RelicEffectClassType = typeof(RelicEffectNull),
                        ParamInt = 2,
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamTargetMode = TargetMode.FrontInRoom,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectChronic.statusId,
                                count = 0
                            }
                        },
                        AdditionalTooltips = new AdditionalTooltipData[] 
                        {
                            new AdditionalTooltipData
                            { 
                                titleKey = "StatusEffect_beyonder_chronic_CardText",
                                descriptionKey = "StatusEffect_beyonder_chronic_CardTooltipText",
                                isStatusTooltip = true,
                                statusId = StatusEffectChronic.statusId,
                                isTipTooltip = false,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Positive
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            return Artifact;
        }
    }
}