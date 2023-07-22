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
    public static class VialOfBlackEyedBlood
    {
        public static CollectableRelicData Artifact;
        public static string ID = "VialOfBlackEyedBlood_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_VialOfBlackEyedBlood_Name_Key",
                DescriptionKey = "Beyonder_Artifact_VialOfBlackEyedBlood_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_VialOfBlackEyedBlood_Lore_Key"
                },
                IconPath = "ArtifactAssets/VialOfBlackEyedBlood.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 5,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectAddTempUpgrade",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        { 
                            StatusEffectUpgrades = new List<StatusEffectStackData>
                            { 
                                new StatusEffectStackData
                                { 
                                    statusId = StatusEffectChronic.statusId,
                                    count = 5
                                }
                            },
                            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                            {
                                new CardTraitDataBuilder
                                {
                                    TraitStateName = typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder> {
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardType = CardType.Monster,
                                    RequiredSubtypes = new List<string>{ "SubtypesData_Champion_83f21cbe-9d9b-4566-a2c3-ca559ab8ff34" }
                                },
                            }
                        }.Build(),
                        AdditionalTooltips = new AdditionalTooltipData[] 
                        {
                            new AdditionalTooltipData
                            { 
                                titleKey = "BeyonderCardTraitStalkerState_TooltipTitle",
                                descriptionKey = "BeyonderCardTraitStalkerState_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Keyword
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            CardUpgradeData upgrade = Artifact.GetEffects()[0].GetParamCardUpgradeData();
            AccessTools.Field(typeof(CardUpgradeData), "isUnique").SetValue(upgrade, true);
            AccessTools.Field(typeof(RelicEffectData), "paramCardUpgradeData").SetValue(Artifact.GetEffects()[0], upgrade);

            return Artifact;
        }
    }
}