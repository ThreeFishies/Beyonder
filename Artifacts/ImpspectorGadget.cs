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
    public static class ImpspectorGadget
    {
        public static CollectableRelicData Artifact;
        public static string ID = "ImpspectorGadget_" + Beyonder.GUID;

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
                ClanID = VanillaClanIDs.Hellhorned,
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_ImpspectorGadget_Name_Key",
                DescriptionKey = "Malicka_Artifact_ImpspectorGadget_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_ImpspectorGadget_Lore_Key"
                },
                IconPath = "ArtifactAssets/GenIMP.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = CustomClassManager.GetClassDataByID(VanillaClanIDs.Hellhorned),
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "Malicka_Artifact_ImpspectorGadget_Activated_Key",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = typeof(CustomRelicEffectAddRoomModifierByTypeExclusive).AssemblyQualifiedName, //roomstate modifiers should be added to spawned unit, not the spawner card.
                        ParamStatusEffects = new StatusEffectStackData[] 
                        {
                            new StatusEffectStackData
                            {
                                statusId = VanillaStatusEffectIDs.Multistrike,
                                count = 1,
                            }
                        },
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCharacterSubtype = "SubtypesData_Imp_0f9b989f-15b5-4b16-8378-5d8ed8691e7c",
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "ImpGangRiseUpUpgrade",
                            RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>
                            {
                                new RoomModifierDataBuilder
                                {
                                    DescriptionKey = "Malicka_Artifact_ImpspectorGadget_Card_Description_Key",
                                    RoomStateModifierClassName = typeof(CustomRoomStateImpGangRiseUp).AssemblyQualifiedName,
                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = VanillaStatusEffectIDs.Multistrike,
                                            count = 1,
                                        }
                                    },
                                    ParamSubtype = "SubtypesData_Imp_0f9b989f-15b5-4b16-8378-5d8ed8691e7c",
                                    ExtraTooltipTitleKey = "",
                                    ExtraTooltipBodyKey = ""
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardType = CardType.Monster,
                                    RequiredSubtypes = new List<string>
                                    {
                                        "SubtypesData_Imp_0f9b989f-15b5-4b16-8378-5d8ed8691e7c"
                                    },
                                    RequiredSubtypesOperator = CardUpgradeMaskDataBuilder.CompareOperator.And,
                                }
                            }
                        }.Build(),
                        AdditionalTooltips = new AdditionalTooltipData[]
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = string.Empty, //"Malicka_Artifact_ImpspectorGadget_Tip_Title_Key",
                                descriptionKey = "Malicka_Artifact_ImpspectorGadget_Tip_Description_Key",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = true,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Default
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);
            AccessTools.Field(typeof(RoomModifierData), "descriptionKeyInPlay").SetValue(Artifact.GetEffects()[0].GetParamCardUpgradeData().GetRoomModifierUpgrades()[0], "Malicka_Artifact_ImpspectorGadget_In_Play_Key");

            return Artifact;
        }
    }
}