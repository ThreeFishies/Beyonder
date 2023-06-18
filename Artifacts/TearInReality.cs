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
    public static class TearInReality 
    {
        public static CollectableRelicData Artifact;
        public static string ID = "TearInReality_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_TearInReality_Name_Key",
                DescriptionKey = "Beyonder_Artifact_TearInReality_Description_Key",
                RelicLoreTooltipKeys = new List<string> 
                { 
                    "Beyonder_Artifact_TearInReality_Lore_Key" 
                },
                IconPath = "ArtifactAssets/TearInReality.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,
                UnlockLevel = 3,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectNull",

                        AdditionalTooltips = new AdditionalTooltipData[] 
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "Beyonder_Artifact_TearInReality_Entropic_Keyword_Title",
                                descriptionKey = "Beyonder_Artifact_TearInReality_Entropic_Keyword_Description",
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Keyword,
                            }
                        }
                    }
                },
                

            }.BuildAndRegister();

            return Artifact;
        }
    }
}