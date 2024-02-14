using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.BuildersV2;
using Trainworks.ManagersV2;
using Trainworks.ConstantsV2;
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
    class CaveofaThousandEyes
    {
        public static readonly string ID = Beyonder.GUID + "_CaveofaThousandEyes_Card";
        public static readonly string CharID = Beyonder.GUID + "_CaveofaThousandEyes_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_CaveofaThousandEyes_Name_Key",
                Size = 5,
                Health = 50,
                AttackDamage = 0,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/CaveofaThousandEyes_Monster.png",
                SubtypeKeys = new List<string> { SubtypeUndretch.Key },
                CharacterChatterData = new Void.Builders.CharacterChatterDataBuilder
                {
                    name = "CaveofaThousandEyesChatterData",
                    gender = CharacterChatterData.Gender.Neutral,

                    characterAddedExpressionKeys = new List<string>
                    {
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Added_0",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Added_1"
                    },
                    characterAttackingExpressionKeys = new List<string>
                    {
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Attacking_0",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Attacking_1",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Attacking_2",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Attacking_3"
                    },
                    characterIdleExpressionKeys = new List<string>
                    {
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_0",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_1",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_2",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_3",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_4",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_5",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_6",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Idle_7"
                    },
                    characterSlayedExpressionKeys = new List<string>
                    {
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Slay_0",
                        "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Slay_1"
                    },
                    characterTriggerExpressionKeys = new List<Void.Builders.CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys>
                    {
                        new Void.Builders.CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = CharacterTriggerData.Trigger.PostCombat,
                            Key = "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Resolve_0",
                        },
                        new Void.Builders.CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = CharacterTriggerData.Trigger.PostCombat,
                            Key = "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Resolve_1",
                        },
                        new Void.Builders.CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = CharacterTriggerData.Trigger.PostCombat,
                            Key = "Beyonder_Unit_CaveofaThousandEyes_Chatter_Key_Resolve_2",
                        }
                    }

                }.Build(),

                RoomModifierBuilders = new List<RoomModifierDataBuilder> 
                {
                    new RoomModifierDataBuilder
                    {
                        RoomModifierID = "CaveofaThousandEyesCustomRoomStateSelfDamagePerGoldModifierDefault",
                        RoomModifierClassType = typeof(CustomRoomStateSelfDamagePerGoldModifier),
                        ParamInt = 1, //X coins per 1 damage (rounded down)
                        IconPath = "ClanAssets/coin.png",
                        ParamStatusEffects = new List<StatusEffectStackData>{ },
                        //DescriptionKey = "",
                        //DescriptionKeyInPlay = "",
                    }
                },

                TriggerBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        TriggerID = "Beyonder_Unit_CaveofaThousandEyes_Resolve_Trigger_ID",
                        Trigger = CharacterTriggerData.Trigger.PostCombat,
                        DescriptionKey = "Beyonder_Unit_CaveofaThousandEyes_Resolve_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectRewardGold),
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamInt = -5,
                            }
                        }
                    },
                }
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_CaveofaThousandEyes_Name_Key",
                Cost = 2,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Rare,
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/CaveofaThousandEyes_Card.png",
                ClanID = null,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_CaveofaThousandEyes_Lore_Key"
                },
                IgnoreWhenCountingMastery = false,
                UnlockLevel = 1,

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateType = typeof(CardEffectSpawnMonster),
                        TargetMode = TargetMode.DropTargetCharacter,
                        ParamCharacterData = Character
                    }
                },

                TriggerBuilders = new List<CardTriggerEffectDataBuilder> 
                { 
                    new CardTriggerEffectDataBuilder
                    { 
                        TriggerID = "CaveofaThousandEyesPyreTributeEffect",
                        Trigger = CardTriggerType.OnUnplayed,
                        DescriptionKey = "CardData_descriptionKey-1e5013e062d40e66-19e3971abc1f11249ac8fded65a3fa32-v2",
                        CardEffectBuilders = new List<CardEffectDataBuilder>
                        { 
                            new CardEffectDataBuilder
                            { 
                                EffectStateType = typeof(CardEffectDamage),
                                ParamInt = 5,
                                TargetMode = TargetMode.Pyre,
                                TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                            }
                        }
                    }
                }
            }.BuildAndRegister();

            Synthesis = new CardUpgradeDataBuilder
            {
                UpgradeID = "CaveofaThousandEyesEssence",
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_CaveofaThousandEyes_Essence_Key",
                BonusHP = 10,

                RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>
                {
                    new RoomModifierDataBuilder
                    {
                        RoomModifierID = "CaveofaThousandEyesCustomRoomStateSelfDamagePerGoldModifierEssence",
                        RoomModifierClassType = typeof(CustomRoomStateSelfDamagePerGoldModifier),
                        ParamInt = 5, //X coins per 1 damage (rounded down)
                        IconPath = "ClanAssets/coin.png",
                        ParamStatusEffects = new List<StatusEffectStackData> { },
                        //DescriptionKey = "",
                        //DescriptionKeyInPlay = "",
                    }
                }
            }.Build();
            Synthesis.InternalSetLinkedPactDuplicateRarity(CollectableRarity.Rare);
        }
    }
}