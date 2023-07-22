using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Builders;
using Trainworks.Managers;
using Trainworks.Constants;
using System.Linq;
using UnityEngine;
using Trainworks.Utilities;
using Void.Init;
using CustomEffects;
using Void.Clan;
using Void.Unit;
using Void.Triggers;
using Void.Status;
using Void.Builders;

namespace Void.Monsters
{
    class Microaggression
    {
        public static readonly string ID = Beyonder.GUID + "_Microaggression_Card";
        public static readonly string CharID = Beyonder.GUID + "_Microaggression_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_Microaggression_Name_Key",
                Size = 1,
                Health = 3,
                AttackDamage = 9,
                PriorityDraw = false,
                AssetPath = "Monsters/Assets/Microaggression_Monster.png",
                SubtypeKeys = new List<string> { SubtypeVeilrich.Key },
                CharacterChatterData = new CharacterChatterDataBuilder 
                { 
                    name = "MicroaggressionChatterData",
                    gender = CharacterChatterData.Gender.Neutral,

                    characterAddedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Microaggression_Chatter_Key_Added_0"
                    },
                    characterIdleExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_0",
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_1",
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_2",
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_3",
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_4",
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_5",
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_6",
                        "Beyonder_Unit_Microaggression_Chatter_Key_Idle_7"
                    },
                    characterSlayedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Microaggression_Chatter_Key_Slay_0"
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    { 
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        { 
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_Microaggression_Chatter_Key_Hysteria_0"
                        }
                    }
                }.Build(),

                StartingStatusEffects = new StatusEffectStackData[]
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectFormless.statusId,
                        count = 1
                    }
                },
                TriggerBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_Microaggression_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            { 
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                {
                                    UpgradeTitleKey = "Microaggression_OnHysteria_Attack_Boost",
                                    BonusDamage = 4,
                                }.Build(),
                            },
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectDamage",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamInt = 1,
                                //AppliedToSelfVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.Torch).GetEffects()[0].GetAppliedVFX(),
                                AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.Torch).GetEffects()[0].GetAppliedVFX(),
                            }
                        }
                    },
                }
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_Microaggression_Name_Key",
                Cost = 1,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Common, 
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/Microaggression_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_Microaggression_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                IgnoreWhenCountingMastery = false,

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateType = typeof(CardEffectSpawnMonster),
                        TargetMode = TargetMode.DropTargetCharacter,
                        ParamCharacterData = Character,
                        EffectStateName = "CardEffectSpawnMonster"
                    }
                }
            }.BuildAndRegister();

            Synthesis = new CardUpgradeDataBuilder
            {
                UpgradeTitle = "MicroaggressionEssence",
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_Microaggression_Essence_Key",
                BonusDamage = 5,
                StatusEffectUpgrades = new List<StatusEffectStackData> 
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectFormless.statusId,
                        count = 1
                    },
                },
                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName
                    }
                },
                /*
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_Microaggression_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                {
                                    UpgradeTitleKey = "Microaggression_OnHysteria_Attack_Boost_From_Essence",
                                    BonusDamage = 3,
                                    SourceSynthesisUnit = Character
                                }.Build(),
                            },
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectDamage",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamInt = 1,
                                //AppliedToSelfVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.Torch).GetEffects()[0].GetAppliedVFX(),
                                AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.Torch).GetEffects()[0].GetAppliedVFX(),
                            }
                        }
                    },
                }
                */
            }.Build();
        }
    }
}