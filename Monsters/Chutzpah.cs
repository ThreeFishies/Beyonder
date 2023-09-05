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
    class Chutzpah
    {
        public static readonly string ID = Beyonder.GUID + "_Chutzpah_Card";
        public static readonly string CharID = Beyonder.GUID + "_Chutzpah_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_Chutzpah_Name_Key",
                Size = 3,
                Health = 40,
                AttackDamage = 20,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/Chutzpah_Monster.png",
                SubtypeKeys = new List<string> { SubtypeVeilrich.Key },
                CharacterChatterData = new CharacterChatterDataBuilder 
                {
                    name = "ChutzpahChatterData",
                    gender = CharacterChatterData.Gender.Neutral,

                    characterAddedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Added_0",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Added_1"
                    },
                    characterAttackingExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Attacking_0",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Attacking_1",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Attacking_2",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Attacking_3",
                    },
                    characterIdleExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_0",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_1",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_2",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_3",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_4",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_5",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_6",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Idle_7"
                    },
                    characterSlayedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Slay_0",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Slay_1",
                        "Beyonder_Unit_Chutzpah_Chatter_Key_Slay_2"
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    {
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        { 
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_Chutzpah_Chatter_Key_Anxiety_0"
                        },
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys 
                        {
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_Chutzpah_Chatter_Key_Hysteria_0"
                        },
                    }
                }.Build(),

                TriggerBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_Chutzpah_Anxiety_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        { 
                            new CardEffectDataBuilder
                            { 
                                EffectStateName = "CardEffectKill",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.MortalEntrapment).GetEffects()[0].GetAppliedVFX(),
                            }
                        }
                    },
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = CharacterTriggerData.Trigger.OnKill,
                        DescriptionKey = "Beyonder_Unit_Chutzpah_Slay_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectHeal",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamInt = 9999,
                                AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.UnleashtheWildwood).GetEffects()[0].GetAppliedVFX(),
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_Chutzpah_Name_Key",
                Cost = 3,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Rare,
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/Chutzpah_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_Chutzpah_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                IgnoreWhenCountingMastery = false,
                UnlockLevel = 4,

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateType = typeof(CardEffectSpawnMonster),
                        TargetMode = TargetMode.DropTargetCharacter,
                        ParamCharacterData = Character,
                        EffectStateName = "CardEffectSpawnMonster"
                    }
                },

                TriggerBuilders = new List<CardTriggerEffectDataBuilder>
                {
                    new CardTriggerEffectDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCardTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_Chutzpah_Description_Key",
                        CardTriggerEffects = new List<CardTriggerData>
                        {
                            new CardTriggerEffectDataBuilder{}.AddCardTrigger
                            (
                                PersistenceMode.SingleRun,
                                "CardTriggerEffectBuffCharacterDamage",
                                "None",
                                2
                            )
                        },
                    }
                }
            }.BuildAndRegister();

            Synthesis = new CardUpgradeDataBuilder
            {
                UpgradeTitle = "ChutzpahEssence",
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_Chutzpah_Essence_Key",
                BonusDamage = 100,
                BonusHP = 0,
                BonusSize = 1,
                CardTriggerUpgradeBuilders = new List<CardTriggerEffectDataBuilder>
                {
                    new CardTriggerEffectDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCardTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_Chutzpah_Essence_Anxiety_Key",
                        CardTriggerEffects = new List<CardTriggerData>
                        {
                            new CardTriggerEffectDataBuilder{}.AddCardTrigger
                            (
                                PersistenceMode.SingleRun,
                                typeof(CustomCardTriggerEffectDebuffCharacterDamage).AssemblyQualifiedName,
                                "None",
                                2
                            )
                        },
                    }
                }
            }.Build();
        }
    }
}