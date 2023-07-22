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
using Void.Triggers;
using CustomEffects;
using RunHistory;
using Void.Spells;
using Void.CardPools;
using Void.Init;

namespace Void.Artifacts 
{ 
    public static class BrainBleach 
    {
        public static CollectableRelicData Artifact;
        public static string ID = "BrainBleach_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_BrainBleach_Name_Key",
                DescriptionKey = "Beyonder_Artifact_BrainBleach_Description_Key",
                RelicLoreTooltipKeys = new List<string> 
                {
                    "Beyonder_Artifact_BrainBleach_Lore_Key"
                },
                IconPath = "ArtifactAssets/BrainBleach.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,
                UnlockLevel = 2,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectAddTempUpgrade",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        { 
                            UpgradeTitleKey = "BrainBleachArtifactCostReduction",
                            CostReduction = 2,
                            XCostReduction = 2,
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                { 
                                    AllowedCardPools = new List<CardPool> { BeyonderCardPools.MutationCards } 
                                }
                            }
                        }.Build(),

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
                            }
                        }
                    }
                },
                

            }.BuildAndRegister();

            return Artifact;
        }
    }
}