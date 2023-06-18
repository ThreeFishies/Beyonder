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
    public static class ScourgeMagnet
    {
        public static CollectableRelicData Artifact;
        public static string ID = "ScourgeMagnet_" + Beyonder.GUID;

        public static bool TryGetClassData(out ClassData clan)
        {
            clan = null;

            if (!Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("com.name.package.succclan-mod"))
            {
                return false;
            }

            clan = CustomClassManager.GetClassDataByID("Succubus");

            if (clan == null)
            {
                Beyonder.LogError("Succubus detected but failed to find class data.");
                return false;
            }

            return true;
        }

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
            if (!TryGetClassData(out ClassData clan))
            {
                return null;
            }

            Artifact = new CollectableRelicDataBuilder
            {
                CollectableRelicID = ID,
                ClanID = clan.GetID(),
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_ScourgeMagnet_Name_Key",
                DescriptionKey = "Malicka_Artifact_ScourgeMagnet_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_ScourgeMagnet_Lore_Key"
                },
                IconPath = "ArtifactAssets/ScourgeMagnet.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                LinkedClass = clan,
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "EmptyString-0000000000000000-00000000000000000000000000000000-v2",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectAddTempUpgrade",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamExcludeCharacterSubtypes = new string[] {},
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        { 
                            UpgradeTitleKey = "MagneticForBLightCards",
                            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                            { 
                                new CardTraitDataBuilder
                                { 
                                    TraitStateName = "CardTraitMagneticState",
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                { 
                                    CardType = CardType.Blight
                                }
                            }
                        }.Build(),
                        ParamTargetMode = TargetMode.FrontInRoom,
                        AdditionalTooltips = new AdditionalTooltipData[]
                        { 
                            new AdditionalTooltipData
                            { 
                                titleKey = "CardTraitMagneticState_CardText",
                                descriptionKey = "CardTraitMagneticState_TooltipText_Verbose",
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = false,
                                style = TooltipDesigner.TooltipDesignType.Keyword
                            }
                        }
                    },
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectAddTempUpgrade",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamExcludeCharacterSubtypes = new string[] {},
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "MagneticForScourgeCards",
                            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                            {
                                new CardTraitDataBuilder
                                {
                                    TraitStateName = "CardTraitMagneticState",
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            {
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardType = CardType.Junk
                                }
                            }
                        }.Build(),
                        ParamTargetMode = TargetMode.FrontInRoom,
                    },
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}