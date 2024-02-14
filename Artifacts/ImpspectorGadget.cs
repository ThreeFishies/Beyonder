using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.BuildersV2;
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
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "Malicka_Artifact_ImpspectorGadget_Activated_Key",
                RelicLoreTooltipStyle = RelicData.RelicLoreTooltipStyle.Malicka,
                RequiredDLC = ShinyShoe.DLC.Hellforged,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(CustomRelicEffectAddRoomModifierByStatusTypeExclusive), //roomstate modifiers should be added to spawned unit, not the spawner card.
                        ParamStatusEffects = new List<StatusEffectStackData> 
                        {
                            new StatusEffectStackData
                            {
                                statusId = VanillaStatusEffectIDs.Multistrike,
                                count = 1,
                            },
                            new StatusEffectStackData
                            { 
                                statusId = VanillaStatusEffectIDs.Cardless, 
                                count = 1,
                            }
                        },
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCharacterSubtype = "SubtypesData_Champion_83f21cbe-9d9b-4566-a2c3-ca559ab8ff34",
                        ParamString = "SubtypesData_Chosen",
                        ParamBool = true,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeID = "ImpGangRiseUpUpgrade",
                            RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>
                            {
                                new RoomModifierDataBuilder
                                {
                                    RoomModifierID = "Malicka_Artifact_ImpspectorGadget_Room_Modifier_ID",
                                    DescriptionKey = "Malicka_Artifact_ImpspectorGadget_Card_Description_Key",
                                    RoomModifierClassType = typeof(CustomRoomStateImpGangRiseUp),
                                    ParamStatusEffects = new List<StatusEffectStackData>
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = VanillaStatusEffectIDs.Multistrike,
                                            count = 1,
                                        },
                                        new StatusEffectStackData
                                        {
                                            statusId = VanillaStatusEffectIDs.Cardless,
                                            count = 1,
                                        }
                                    },
                                    ParamSubtype = "SubtypesData_Champion_83f21cbe-9d9b-4566-a2c3-ca559ab8ff34",
                                    ExtraTooltipTitleKey = "",
                                    ExtraTooltipBodyKey = "",
                                    DescriptionKeyInPlay = "Malicka_Artifact_ImpspectorGadget_In_Play_Key",
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardUpgradeMaskID = "Imspector_Gadget_Filter_ID",
                                    CardType = CardType.Monster,
                                    ExcludedSubtypes = new List<string>
                                    {
                                        "SubtypesData_Champion_83f21cbe-9d9b-4566-a2c3-ca559ab8ff34",
                                        "SubtypesData_Chosen"
                                    },
                                    ExcludedSubtypesOperator = CardUpgradeMaskDataBuilder.CompareOperator.Or,
                                    ExcludedStatusEffects = new List<StatusEffectStackData>
                                    { 
                                        new StatusEffectStackData
                                        { 
                                            statusId = VanillaStatusEffectIDs.Cardless,
                                            count = 1,
                                        }
                                    },
                                    ExcludedStatusEffectsOperator = CardUpgradeMaskDataBuilder.CompareOperator.Or
                                }
                            }
                        }.Build(),
                        AdditionalTooltips = new List<AdditionalTooltipData>
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "Malicka_Artifact_ImpspectorGadget_Token_Title_Key",
                                descriptionKey = "Malicka_Artifact_ImpspectorGadget_Token_Description_Key",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Keyword
                            },
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

            return Artifact;
        }
    }
}