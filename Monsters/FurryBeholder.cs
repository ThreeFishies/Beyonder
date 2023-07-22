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
    class FurryBeholder
    {
        public static readonly string ID = Beyonder.GUID + "_FurryBeholder_Card";
        public static readonly string CharID = Beyonder.GUID + "_FurryBeholder_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;
        public const int UbaneIndex = 2;
        public const int EssenceUbaneIndex = 3;
        public static Dictionary<string, CardUpgradeData> DynamicEssenceData = new Dictionary<string, CardUpgradeData> { };
        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_FurryBeholder_Name_Key",
                Size = 2,
                Health = 25,
                AttackDamage = 14,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/FurryBeholder_Monster.png",
                SubtypeKeys = new List<string> { SubtypeUndretch.Key },
                CharacterChatterData = new CharacterChatterDataBuilder 
                {
                    name = "FurryBeholderChatterData",
                    gender = CharacterChatterData.Gender.Female,

                    characterAddedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Added_0",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Added_1",
                    },
                    characterIdleExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_0",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_1",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_2",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_3",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_4",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_5",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_6",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_7",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Idle_8",
                    },
                    characterSlayedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Slay_0",
                        "Beyonder_Unit_FurryBeholder_Chatter_Key_Slay_1",
                    }
                }.Build(),

                StartingStatusEffects = new StatusEffectStackData[] 
                { 
                    new StatusEffectStackData
                    { 
                        statusId = VanillaStatusEffectIDs.Sweep,
                        count = 1
                    }
                },

                //ProjectilePrefab = CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.SeraphtheChaste).GetBossRoomSpellCastVfx(),

            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_FurryBeholder_Name_Key",
                Cost = 1,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Uncommon,
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/FurryBeholder_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_FurryBeholder_Lore_Key"
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
                    ChaosManager.UBanesData[ChaosManager.Ubanes[UbaneIndex]],
                }
            }.BuildAndRegister();

            //Dynamic synthesis data will be constructed as needed.
            Synthesis = new CardUpgradeDataBuilder
            {
                UpgradeTitle = "FurryBeholderEssenceBase",
                SourceSynthesisUnit = Character,
                //UpgradeDescriptionKey = "Beyonder_Unit_FurryBeholder_Description_Key",
                BonusHP = 10,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = CharacterTriggerData.Trigger.PostCombat,
                        DescriptionKey = "Beyonder_Unit_FurryBeholder_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectPlayUnitTrigger",
                                TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                                TargetMode = TargetMode.Room,
                                ParamTrigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                                ParamInt = 1,
                                ShouldTest = true,
                            }
                        }
                    }
                }
            }.Build();

            //Cache synthesis for loading purposes
            GetSynthesis();
        }
        public static CardUpgradeData GetSynthesis()
        {
            string DynamicEssenceKey = $"FurryBeholderEssence_FurryBeholderEssenceBase_{ChaosManager.Ubanes[EssenceUbaneIndex]}";

            if (DynamicEssenceData.ContainsKey(DynamicEssenceKey))
            {
                return DynamicEssenceData[DynamicEssenceKey];
            }

            CardUpgradeData DynamicSynthesis = ScriptableObject.CreateInstance<CardUpgradeData>();

            DynamicSynthesis = ChaosManager.MergeUpgrades(Synthesis, ChaosManager.UBanesData[ChaosManager.Ubanes[EssenceUbaneIndex]], Character, DynamicEssenceKey);
            string moist = ChaosManager.GenerateDescription(DynamicEssenceKey, DynamicSynthesis, true);

            DynamicEssenceData.Add(DynamicEssenceKey, DynamicSynthesis);

            //Beyonder.Log(DynamicEssenceKey + ": " + moist);

            return DynamicSynthesis;
        }
        public static void UpdateStartingUpgrades()
        {
            List<CardUpgradeData> startingUpgrades = new List<CardUpgradeData>
            {
                ChaosManager.UBanesData[ChaosManager.Ubanes[UbaneIndex]],
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