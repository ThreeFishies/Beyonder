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
using Void.Chaos;
using Void.Builders;

namespace Void.Monsters
{
    class ApostleoftheVoid
    {
        public static readonly string ID = Beyonder.GUID + "_ApostleoftheVoid_Card";
        public static readonly string CharID = Beyonder.GUID + "_ApostleoftheVoid_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;
        public const int EssenceUbaneIndex = 4;
        public const int EssenceUboonIndex = 4;
        public const int EssenceVbaneIndex = 4;
        public const int EssenceVboonIndex = 4;
        public static Dictionary<string, CardUpgradeData> DynamicEssenceData = new Dictionary<string, CardUpgradeData> { };

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_ApostleoftheVoid_Name_Key",
                Size = 2,
                Health = 20,
                AttackDamage = 20,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/ApostleoftheVoid_Monster.png",
                SubtypeKeys = new List<string> { SubtypeUndretch.Key, SubtypeVeilrich.Key },
                CharacterChatterData = new CharacterChatterDataBuilder 
                {
                    name = "ApostleoftheVoidChatterData",
                    gender = CharacterChatterData.Gender.Male,

                    characterAddedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Added_0",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Added_1"
                    },
                    characterIdleExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_0",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_1",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_2",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_3",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_4",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_5",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_6",
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Idle_7"
                    },
                    characterSlayedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Slay_0",
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    {
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Anxiety_0"
                        },
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_ApostleoftheVoid_Chatter_Key_Hysteria_0"
                        }
                    }
                }.Build(),

                TriggerBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_ApostleoftheVoid_Hysteria_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            //Veilrich get +15 attack
                            new CardEffectDataBuilder
                            { 
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                TargetCharacterSubtype = SubtypeVeilrich.Key,
                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                { 
                                    UpgradeTitleKey = "ApostleoftheVoidVeilrichDamageBoost",
                                    BonusDamage = 10,
                                    //SourceSynthesisUnit = new CharacterDataBuilder{ CharacterID = "DummyNULL" }.Build(),
                                }.Build(),
                            },

                            //Undretch die at Mania of 2 or more
                            new CardEffectDataBuilder
                            { 
                                EffectStateName = typeof(CustomCardEffectKillConditional).AssemblyQualifiedName,
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                TargetCharacterSubtype = SubtypeUndretch.Key,
                                ParamInt = 2,
                                ParamBool = true, //or higher
                            }
                        }
                    },
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_ApostleoftheVoid_Anxiety_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            //Undretch get +15 health
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                TargetCharacterSubtype = SubtypeUndretch.Key,
                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                {
                                    UpgradeTitleKey = "ApostleoftheVoidUndretchHealthBoost",
                                    BonusHP = 10,
                                    //SourceSynthesisUnit = new CharacterDataBuilder{ CharacterID = "DummyNULL" }.Build(),
                                }.Build(),
                            },

                            //Veilritch die at Mania of -2 or less
                            new CardEffectDataBuilder
                            {
                                EffectStateName = typeof(CustomCardEffectKillConditional).AssemblyQualifiedName,
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                TargetCharacterSubtype = SubtypeVeilrich.Key,
                                ParamInt = -2,
                                ParamBool = false, // or lower
                            }
                        }
                    }
                }
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_ApostleoftheVoid_Name_Key",
                Cost = 2,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Rare,
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/ApostleoftheVoid_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_ApostleoftheVoid_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                IgnoreWhenCountingMastery = false,
                UnlockLevel = 9,

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
                UpgradeTitle = "ApostleoftheVoidEssence",
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_ApostleoftheVoid_Essence_Key",
            }.Build();

            //Cache synthesis for loading purposes
            GetSynthesis();
        }

        public static CardUpgradeData GetSynthesis()
        {
            string DynamicEssenceKey = $"ApostleoftheVoidEssence_{ChaosManager.Vboons[EssenceVboonIndex]}_{ChaosManager.Vbanes[EssenceVbaneIndex]}_{ChaosManager.Uboons[EssenceUboonIndex]}_{ChaosManager.Ubanes[EssenceUbaneIndex]}";

            if (DynamicEssenceData.ContainsKey(DynamicEssenceKey))
            {
                return DynamicEssenceData[DynamicEssenceKey];
            }

            CardUpgradeData DynamicSynthesis = ScriptableObject.CreateInstance<CardUpgradeData>();

            CardUpgradeData DynamicSynthesis1 = ChaosManager.MergeUpgrades(ChaosManager.VBoonsData[ChaosManager.Vboons[EssenceVboonIndex]], ChaosManager.VBanesData[ChaosManager.Vbanes[EssenceVbaneIndex]], Character, DynamicEssenceKey);
            CardUpgradeData DynamicSynthesis2 = ChaosManager.MergeUpgrades(ChaosManager.UBoonsData[ChaosManager.Uboons[EssenceUboonIndex]], ChaosManager.UBanesData[ChaosManager.Ubanes[EssenceUbaneIndex]], Character, DynamicEssenceKey);

            DynamicSynthesis = ChaosManager.MergeUpgrades(DynamicSynthesis1, DynamicSynthesis2, Character, DynamicEssenceKey);
            string moist = ChaosManager.GenerateDescription(DynamicEssenceKey, DynamicSynthesis, true);

            DynamicEssenceData.Add(DynamicEssenceKey, DynamicSynthesis);

            //Beyonder.Log(DynamicEssenceKey + ": " + moist);
            //Beyonder.Log("Apostile of the Void Synthesis Title Key: " + DynamicSynthesis.GetUpgradeTitleKey());

            return DynamicSynthesis;
        }
    }
}