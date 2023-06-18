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
    public static class BlackLight
    {
        public static CollectableRelicData Artifact;
        public static string ID = "BlackLight_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_BlackLight_Name_Key",
                DescriptionKey = "Malicka_Artifact_BlackLight_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_BlackLight_Lore_Key"
                },
                IconPath = "ArtifactAssets/BlackLight.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,
                //RelicActivatedKey = "CollectableRelicData_relicActivatedKey-c7744f879617ab87-c45d0829b04acdb429ab1f6ca8ddca0d-v2",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectNull",
                        AdditionalTooltips = new AdditionalTooltipData[] 
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "Beyonder_Mechanic_Manic_TooltipKey",
                                descriptionKey = "Beyonder_Mechanic_Manic_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Keyword,
                            },
                            new AdditionalTooltipData
                            {
                                titleKey = "Trigger_Beyonder_OnHysteria",
                                descriptionKey = "Trigger_Beyonder_OnHysteria_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = true,
                                trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Trigger,
                            },
                            new AdditionalTooltipData
                            {
                                titleKey = "Trigger_Beyonder_OnAnxiety",
                                descriptionKey = "Trigger_Beyonder_OnAnxiety_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = true,
                                trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Trigger,
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}