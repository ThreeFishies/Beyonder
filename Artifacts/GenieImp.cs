using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Void.Unit;
using Void.Clan;
using Void.Status;
using Void.Init;
using Void.Triggers;
using CustomEffects;
using RunHistory;
using Void.Spells;

namespace Void.Unused
{
    public static class GenieImp
    {
        public static CollectableRelicData Artifact;
        public static string ID = "GenieImp_" + Beyonder.GUID;

        public static bool HasIt()
        {
            if (Artifact != null)
            {
                return ProviderManager.SaveManager.GetHasRelic(Artifact);
            }
            return false;
        }

        public static CollectableRelicData BuildAndRegister()
        {
            Artifact = new CollectableRelicDataBuilder
            {
                CollectableRelicID = ID,
                ClanID = VanillaClanIDs.Hellhorned,
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_GenieImp_Name_Key",
                DescriptionKey = "Malicka_Artifact_GenieImp_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_GenieImp_Lore_Key"
                },
                IconPath = "ArtifactAssets/GenIMP.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = CustomClassManager.GetClassDataByID(VanillaClanIDs.Hellhorned),
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "Malicka_Artifact_GenieImp_Activated_Key",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = typeof(CustomRelicEffectSpawnUnitOnAllFloorsOnUnitSpawned).AssemblyQualifiedName,
                        ParamCharacters = new List<CharacterData>
                        { 
                            CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.Transcendimp),
                        },
                        //ParamCardPool = new CardPoolBuilder
                        //{ 
                        //    CardPoolID = "Transcendimp Only Pool",
                        //    CardIDs = new List<string> 
                        //    { 
                        //        VanillaCardIDs.Transcendimp 
                        //    }
                        //}.Build(),
                        ParamSourceTeam = Team.Type.Monsters,

                        EffectConditionBuilders = new List<RelicEffectConditionBuilder>
                        { 
                            new RelicEffectConditionBuilder
                            { 
                                paramTrackedValue = CardStatistics.TrackedValueType.MonsterSubtypePlayed,
                                paramCardType = CardStatistics.CardTypeTarget.Monster,
                                allowMultipleTriggersPerDuration = false,
                                paramEntryDuration = CardStatistics.EntryDuration.ThisBattle,
                                paramInt = 20,
                                paramComparator = RelicEffectCondition.Comparator.Equal,
                                paramSubtype = "SubtypesData_Imp_0f9b989f-15b5-4b16-8378-5d8ed8691e7c",
                                paramTrackTriggerCount = false
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}