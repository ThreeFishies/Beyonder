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
    class ThreeEyedFish
    {
        public static readonly string ID = Beyonder.GUID + "_ThreeEyedFish_Card";
        public static readonly string CharID = Beyonder.GUID + "_ThreeEyedFish_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_ThreeEyedFish_Name_Key",
                Size = 1,
                Health = 3,
                AttackDamage = 0,
                PriorityDraw = false,
                AssetPath = "Monsters/Assets/ThreeEyedFish_Monster.png",
                SubtypeKeys = new List<string> { SubtypeUndretch.Key },
                CharacterChatterData = new CharacterChatterDataBuilder 
                {
                    name = "ThreeEyedFishChatterData",
                    gender = CharacterChatterData.Gender.Male,

                    characterAddedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_ThreeEyedFish_Chatter_Key_Added_0",
                    },
                    characterIdleExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_ThreeEyedFish_Chatter_Key_Idle_0",
                        "Beyonder_Unit_ThreeEyedFish_Chatter_Key_Idle_1",
                        "Beyonder_Unit_ThreeEyedFish_Chatter_Key_Idle_2",
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    {
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_ThreeEyedFish_Chatter_Key_Anxiety_0",
                        }
                    }
                }.Build(),

                StartingStatusEffects = new StatusEffectStackData[]
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectSoundless.statusId,
                        count = 1
                    },
                    new StatusEffectStackData
                    {
                        statusId = VanillaStatusEffectIDs.Stealth,
                        count = 3
                    }
                },
                TriggerBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_ThreeEyedFish_Description_0_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            { 
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Self,
                                //TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                {
                                    UpgradeTitleKey = "ThreeEyedFish_OnHysteria_Decay",
                                    BonusHP = -1,
                                }.Build(),
                            }
                        }
                    },
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_ThreeEyedFish_Description_1_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddStatusEffect",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamStatusEffects = new StatusEffectStackData[]
                                {
                                    new StatusEffectStackData
                                    { 
                                        statusId = VanillaStatusEffectIDs.Stealth,
                                        count = 1,
                                    }
                                }
                            }
                        }
                    }
                }
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_ThreeEyedFish_Name_Key",
                Cost = 1,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Common, //Revert to common after testing
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/ThreeEyedFish_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_ThreeEyedFish_Lore_Key"
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
                UpgradeTitle = "ThreeEyedFishEssence",
                BonusHP = 5,
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_ThreeEyedFish_Essence_Key",
                StatusEffectUpgrades = new List<StatusEffectStackData> 
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectSoundless.statusId,
                        count = 1
                    },
                    new StatusEffectStackData() 
                    { 
                        statusId = StatusEffectChronic.statusId,
                        count = 5
                    }
                },
                /*
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_ThreeEyedFish_Essence_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                { 
                                    UpgradeTitleKey = "Beyonder_Unit_ThreeEyedFish_Essence_Health_Effect",
                                    SourceSynthesisUnit = Character,
                                    BonusHP = 3,
                                }.Build(),
                            }
                        }
                    }
                }
                */
            }.Build();
            Synthesis.InternalSetLinkedPactDuplicateRarity(CollectableRarity.Rare);
        }
    }
}