using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Builders;
using Trainworks.Managers;
using Trainworks.Enums;
using Trainworks.Constants;
using Void.Init;

namespace Void.Artifacts
{
    class MalickasGift
    {
        public static CollectableRelicData Artifact;
        public static string ID = "MalickasGift_" + Beyonder.GUID;

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
                NameKey = "Malicka_Artifact_MalickasGift_Name_Key",
                DescriptionKey = "Malicka_Artifact_MalickasGift_Description_Key",
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                IconPath = "ArtifactAssets/MalickasGift.png",
                ClanID = VanillaClanIDs.Stygian,
                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectAddTempUpgrade",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "MalickasGiftUpgrade",
                            BonusDamage = 0,
                            BonusHP = 0,
                            TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                            {
                                new CharacterTriggerDataBuilder
                                {
                                    Trigger = CharacterTriggerData.Trigger.CardSpellPlayed,
                                    DescriptionKey = "Malicka_Artifact_MalickasGift_Trigger_Key",
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        { 
                                            EffectStateName = "CardEffectAddStatusEffect",
                                            TargetMode = TargetMode.Self,
                                            TargetTeamType = Team.Type.Monsters,
                                            ParamInt = 0,

                                            ParamStatusEffects = new StatusEffectStackData[]
                                            { 
                                                new StatusEffectStackData
                                                { 
                                                    statusId = VanillaStatusEffectIDs.Armor,
                                                    count = 2
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Filters = new List<CardUpgradeMaskData>
                            {
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardType = CardType.Monster,
                                    RequiredSubtypes = new List<string>
                                    {
                                        "SubtypesData_Champion_83f21cbe-9d9b-4566-a2c3-ca559ab8ff34"
                                    }
                                }.Build(),
                            }
                        }.Build(),
                    }
                },
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                LinkedClass = CustomClassManager.GetClassDataByID(VanillaClanIDs.Stygian),
                Rarity = CollectableRarity.Common,
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_MalickasGift_Lore_Key"
                },
                UnlockLevel = 0,
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}