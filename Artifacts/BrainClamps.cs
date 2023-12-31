﻿using BepInEx;
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
    public static class BrainClamps
    {
        public static CollectableRelicData Artifact;
        public static string ID = "BrainClamps_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_BrainClamps_Name_Key",
                DescriptionKey = "Beyonder_Artifact_BrainClamps_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_BrainClamps_Lore_Key"
                },
                IconPath = "ArtifactAssets/BrainClamps.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        //RelicEffectClassType = typeof(BeyonderRelicEffectAddManicTraitToAlliedClanStarterCards),
                        RelicEffectClassName = "RelicEffectNull",

                        AdditionalTooltips = new AdditionalTooltipData[]
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "Beyonder_Mechanic_Insanity_TooltipKey",
                                descriptionKey = "Beyonder_Mechanic_Insanity_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Keyword,
                            },
                        }
                    }
                },
            }.BuildAndRegister();

            return Artifact;
        }
    }
}