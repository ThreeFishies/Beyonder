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
    public static class ShadowPuppeteer
    {
        public static CollectableRelicData Artifact;
        public static string ID = "ShadowPuppeteer_" + Beyonder.GUID;

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
                ClanID = VanillaClanIDs.Umbra,
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_ShadowPuppeteer_Name_Key",
                DescriptionKey = "Malicka_Artifact_ShadowPuppeteer_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_ShadowPuppeteer_Lore_Key"
                },
                IconPath = "ArtifactAssets/ShadowPuppeteer.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                LinkedClass = CustomClassManager.GetClassDataByID(VanillaClanIDs.Umbra),
                Rarity = CollectableRarity.Common,
                //RelicActivatedKey = "CollectableRelicData_relicActivatedKey-c7744f879617ab87-c45d0829b04acdb429ab1f6ca8ddca0d-v2",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectAddTempUpgrade",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "ShadowPuppeteerMultistrike",
                            StatusEffectUpgrades = new List<StatusEffectStackData>
                            {
                                new StatusEffectStackData
                                {
                                    statusId = VanillaStatusEffectIDs.Multistrike,
                                    count = 1
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                { 
                                    CardType = CardType.Monster,
                                    RequiredSubtypesOperator = CardUpgradeMaskDataBuilder.CompareOperator.And,
                                    RequiredSubtypes = new List<string>
                                    {
                                        "SubtypesData_Forager"
                                    }
                                }
                            }
                        }.Build(),
                    }
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}