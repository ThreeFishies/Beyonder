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
using UnityEngine.Rendering;

namespace Void.Monsters
{
    class SoundlessSwarm
    {
        public static readonly string ID = Beyonder.GUID + "_SoundlessSwarm_Card";
        public static readonly string CharID = Beyonder.GUID + "_SoundlessSwarm_Character";
        public static CharacterData Character;
        public static CardData Card;
        public static CardUpgradeData Synthesis;
        public const int UboonIndex = 0;
        public const int UbaneIndex = 0;
        public const int EssenceUboonIndex = 1;
        public const int EssenceUbaneIndex = 1;
        public static Dictionary<string, CardUpgradeData> DynamicEssenceData = new Dictionary<string, CardUpgradeData> { };

        public static void BuildAndRegister()
        {
            Character = new CharacterDataBuilder
            {
                CharacterID = CharID,
                NameKey = "Beyonder_Unit_SoundlessSwarm_Name_Key",
                Size = 2,
                Health = 25,
                AttackDamage = 9,
                PriorityDraw = true,
                AssetPath = "Monsters/Assets/SoundlessSwarm_Monster.png",
                SubtypeKeys = new List<string> { SubtypeUndretch.Key },

                StartingStatusEffects = new StatusEffectStackData[]
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectSoundless.statusId,
                        count = 1
                    },
                },
            }.BuildAndRegister();

            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Unit_SoundlessSwarm_Name_Key",
                Cost = 1,
                CardType = CardType.Monster,
                Rarity = CollectableRarity.Uncommon, 
                TargetsRoom = true,
                Targetless = false,
                AssetPath = "Monsters/Assets/SoundlessSwarm_Card.png",
                ClanID = BeyonderClan.ID,
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.UnitsAllBanner },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Unit_SoundlessSwarm_Lore_Key"
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
                    ChaosManager.UBanesData[ChaosManager.Ubanes[UbaneIndex]],
                }
            }.BuildAndRegister();

            //Dynamic synthesis data will be constructed as needed.
            Synthesis = new CardUpgradeDataBuilder
            {
                UpgradeTitle = "SoundlessSwarmEssence",
                SourceSynthesisUnit = Character,
                UpgradeDescriptionKey = "Beyonder_Unit_SoundlessSwarm_Essence_Key",
            }.Build();

            //Cache synthesis for loading purposes
            GetSynthesis();
        }

        public static CardUpgradeData GetSynthesis() 
        {
            string DynamicEssenceKey = $"SoundlessSwarmEssence_{ChaosManager.Uboons[EssenceUboonIndex]}_{ChaosManager.Ubanes[EssenceUbaneIndex]}";

            if (DynamicEssenceData.ContainsKey(DynamicEssenceKey))
            { 
                return DynamicEssenceData[DynamicEssenceKey];
            }

            CardUpgradeData DynamicSynthesis = ScriptableObject.CreateInstance<CardUpgradeData>();

            DynamicSynthesis = ChaosManager.MergeUpgrades(ChaosManager.UBoonsData[ChaosManager.Uboons[EssenceUboonIndex]], ChaosManager.UBanesData[ChaosManager.Ubanes[EssenceUbaneIndex]], Character, DynamicEssenceKey);
            string moist = ChaosManager.GenerateDescription(DynamicEssenceKey, DynamicSynthesis, true);

            DynamicEssenceData.Add(DynamicEssenceKey, DynamicSynthesis);

            //Beyonder.Log(DynamicEssenceKey + ": " + moist);
            //Beyonder.Log("Soundless Swarm Synthesis Title Key: " + DynamicSynthesis.GetUpgradeTitleKey());

            return DynamicSynthesis;
        }

        public static void UpdateStartingUpgrades() 
        {
            List<CardUpgradeData> startingUpgrades = new List<CardUpgradeData>
            {
                ChaosManager.UBoonsData[ChaosManager.Uboons[UboonIndex]],
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