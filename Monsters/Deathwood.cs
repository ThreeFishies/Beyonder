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
    class Deathwood
    {
        public static readonly string ID = Beyonder.GUID + "_Deathwood_Card";
        public static readonly string CharID = Beyonder.GUID + "_Deathwood_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_Deathwood_Name_Key",
                Size = 2,
                Health = 1,
                AttackDamage = 1,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/Deathwood_Monster.png",
                SubtypeKeys = new List<string> { SubtypeUndretch.Key },
                CharacterChatterData = new CharacterChatterDataBuilder 
                {
                    name = "DeathwoodChatterData",
                    gender = CharacterChatterData.Gender.Neutral,

                    characterAddedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Deathwood_Chatter_Key_Added_0",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Added_1"
                    },
                    characterIdleExpressionKeys = new List<string>
                    {
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_0",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_1",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_2",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_3",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_4",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_5",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_6",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Idle_7"
                    },
                    characterSlayedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Deathwood_Chatter_Key_Slay_0",
                        "Beyonder_Unit_Deathwood_Chatter_Key_Slay_1"
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    {
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = CharacterTriggerData.Trigger.OnDeath,
                            Key = "Beyonder_Unit_Deathwood_Chatter_Key_Extinguish_0"
                        },
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = CharacterTriggerData.Trigger.OnTurnBegin,
                            Key = "Beyonder_Unit_Deathwood_Chatter_Key_Action_0"
                        }
                    }

                }.Build(),

                StartingStatusEffects = new StatusEffectStackData[]
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectChronic.statusId,
                        count = 5
                    }
                },
                TriggerBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = CharacterTriggerData.Trigger.OnTurnBegin,
                        DescriptionKey = "Beyonder_Unit_Deathwood_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = typeof(CustomCardEffectAddStatusEffectByHP).AssemblyQualifiedName,
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Heroes,
                                ShouldTest = true,
                                ParamInt = 100,
                                UseStatusEffectStackMultiplier = true,
                                StatusEffectStackMultiplier = "HP", //already does this

                                ParamStatusEffects = new StatusEffectStackData[]
                                { 
                                    new StatusEffectStackData
                                    { 
                                        statusId = StatusEffectJitters.statusId,
                                        count = 1
                                    }
                                }
                            }
                        }
                    },
                }
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_Deathwood_Name_Key",
                Cost = 1,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Rare,
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/Deathwood_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_Deathwood_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                IgnoreWhenCountingMastery = false,
                UnlockLevel = 6,

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
                UpgradeTitle = "DeathwoodEssence",
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_Deathwood_Essence_Key",
                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData()
                    {
                        statusId = StatusEffectChronic.statusId,
                        count = 5
                    }
                },
                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName
                    }
                }
            }.Build();
        }
    }
}