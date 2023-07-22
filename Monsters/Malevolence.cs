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
    class Malevolence
    {
        public static readonly string ID = Beyonder.GUID + "_Malevolence_Card";
        public static readonly string CharID = Beyonder.GUID + "_Malevolence_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;
        public const int VbaneIndex = 2;
        public const int EssenceVbaneIndex = 3;
        public static Dictionary<string, CardUpgradeData> DynamicEssenceData = new Dictionary<string, CardUpgradeData> { };

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_Malevolence_Name_Key",
                Size = 2,
                Health = 14,
                AttackDamage = 25,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/Malevolence_Monster.png",
                SubtypeKeys = new List<string> { SubtypeVeilrich.Key },

                TriggerBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_Malevolence_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        { 
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectDamage",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Heroes,
                                ParamInt = 10
                            }
                        }
                    }
                },

                CharacterChatterData = new CharacterChatterDataBuilder 
                {
                    name = "MalevolenceChatterData",
                    gender = CharacterChatterData.Gender.Neutral,

                    characterAddedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Malevolence_Chatter_Key_Added_0",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Added_1",
                    },
                    characterIdleExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_0",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_1",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_2",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_3",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_4",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_5",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_6",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Idle_7"
                    },
                    characterSlayedExpressionKeys = new List<string> 
                    {
                        "Beyonder_Unit_Malevolence_Chatter_Key_Slay_0",
                        "Beyonder_Unit_Malevolence_Chatter_Key_Slay_1",
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    { 
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        { 
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            Key = "Beyonder_Unit_Malevolence_Chatter_Key_Hysteria_0",
                        }
                    }
                }.Build(),
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_Malevolence_Name_Key",
                Cost = 1,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Uncommon,
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/Malevolence_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_Malevolence_Lore_Key"
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
                    ChaosManager.VBanesData[ChaosManager.Vbanes[VbaneIndex]],
                }
            }.BuildAndRegister();

            //Dynamic synthesis data will be constructed as needed.
            Synthesis = new CardUpgradeDataBuilder
            {
                UpgradeTitle = "MalevolenceEssenceBase",
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_Malevolence_Description_Key",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Beyonder_Unit_Malevolence_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectDamage",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Heroes,
                                ParamInt = 10,
                                AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.BlazingBolts).GetEffects()[0].GetAppliedVFX()
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
            string DynamicEssenceKey = $"MalevolenceEssence_MalevolenceEssenceBase_{ChaosManager.Vbanes[EssenceVbaneIndex]}";

            if (DynamicEssenceData.ContainsKey(DynamicEssenceKey))
            { 
                return DynamicEssenceData[DynamicEssenceKey];
            }

            CardUpgradeData DynamicSynthesis = ScriptableObject.CreateInstance<CardUpgradeData>();

            DynamicSynthesis = ChaosManager.MergeUpgrades(Synthesis, ChaosManager.VBanesData[ChaosManager.Vbanes[EssenceVbaneIndex]], Character, DynamicEssenceKey);
            string moist = ChaosManager.GenerateDescription(DynamicEssenceKey, DynamicSynthesis, true);

            DynamicEssenceData.Add(DynamicEssenceKey, DynamicSynthesis);

            //Beyonder.Log(DynamicEssenceKey + ": " + moist);

            return DynamicSynthesis;
        }

        public static void UpdateStartingUpgrades() 
        {
            List<CardUpgradeData> startingUpgrades = new List<CardUpgradeData>
            {
                ChaosManager.VBanesData[ChaosManager.Vbanes[VbaneIndex]],
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