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
using UnityEngine.Rendering;

namespace Void.Monsters
{
    class HairyPotty
    {
        public static readonly string ID = Beyonder.GUID + "_HairyPotty_Card";
        public static readonly string CharID = Beyonder.GUID + "_HairyPotty_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;
        public const int UboonIndex = 2;
        public const int EssenceUboonIndex = 3;
        public static Dictionary<string, CardUpgradeData> DynamicEssenceData = new Dictionary<string, CardUpgradeData> { };
        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_HairyPotty_Name_Key",
                Size = 3,
                Health = 44,
                AttackDamage = 14,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/HairyPotty_Monster.png",
                SubtypeKeys = new List<string> { SubtypeUndretch.Key },
                CharacterChatterData = new CharacterChatterDataBuilder 
                { 
                    name = "HairyPottyChatterData",
                    gender = CharacterChatterData.Gender.Neutral,

                    characterAddedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Added_0"
                    },
                    characterIdleExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_0",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_1",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_2",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_3",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_4",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_5",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_6",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_7",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_8",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Idle_9"
                    },
                    characterSlayedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Slay_0",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Slay_1",
                        "Beyonder_Unit_HairyPotty_Chatter_Key_Slay_2"
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    {
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_HairyPotty_Chatter_Key_Anxiety_0"
                        }
                    }
                }.Build(),

                TriggerBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_HairyPotty_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddStatusEffect",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                                ParamInt = 0,

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
                    }
                }
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_HairyPotty_Name_Key",
                Cost = 1,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Uncommon,
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/HairyPotty_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_HairyPotty_Lore_Key"
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
                },

                StartingUpgrades = new List<CardUpgradeData>
                {
                    ChaosManager.UBoonsData[ChaosManager.Uboons[UboonIndex]],
                }
            }.BuildAndRegister();

            //Dynamic synthesis data will be constructed as needed.
            Synthesis = new CardUpgradeDataBuilder
            {
                UpgradeTitle = "HairyPottyEssenceBase",
                SourceSynthesisUnit = Character,
                //UpgradeDescriptionKey = "Beyonder_Unit_HairyPotty_Description_Key",
                BonusHP = 30,
                BonusSize = 1
            }.Build();

            //Cache synthesis for loading purposes
            GetSynthesis();
        }
        public static CardUpgradeData GetSynthesis()
        {
            string DynamicEssenceKey = $"HairyPottyEssence_HairyPottyEssenceBase_{ChaosManager.Uboons[EssenceUboonIndex]}";

            if (DynamicEssenceData.ContainsKey(DynamicEssenceKey))
            {
                return DynamicEssenceData[DynamicEssenceKey];
            }

            CardUpgradeData DynamicSynthesis = ScriptableObject.CreateInstance<CardUpgradeData>();

            DynamicSynthesis = ChaosManager.MergeUpgrades(Synthesis, ChaosManager.UBoonsData[ChaosManager.Uboons[EssenceUboonIndex]], Character, DynamicEssenceKey);
            string moist = ChaosManager.GenerateDescription(DynamicEssenceKey, DynamicSynthesis, true);

            DynamicEssenceData.Add(DynamicEssenceKey, DynamicSynthesis);

            //Beyonder.Log(DynamicEssenceKey + ": " + moist);

            return DynamicSynthesis;
        }
        public static void UpdateStartingUpgrades()
        {
            List<CardUpgradeData> startingUpgrades = new List<CardUpgradeData>
            {
                ChaosManager.UBoonsData[ChaosManager.Uboons[UboonIndex]],
            };

            AccessTools.Field(typeof(CardData), "startingUpgrades").SetValue(Card, startingUpgrades);

            //cache this now so it won't have to be generated later
            if (ProviderManager.SaveManager.IsDlcAvailableWhenStartingRun(ShinyShoe.DLC.Hellforged))
            {
                GetSynthesis();
            }
        }
    }
}