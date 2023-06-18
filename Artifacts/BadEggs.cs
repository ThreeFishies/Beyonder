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

namespace Void.Artifacts
{
    public static class BadEggs
    {
        public static CollectableRelicData Artifact;
        public static string ID = "BadEggs_" + Beyonder.GUID;

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
                ClanID = "46ae87db-d92e-4fcb-a3bc-67c723d7bebd",
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_BadEggs_Name_Key",
                DescriptionKey = "Malicka_Artifact_BadEggs_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_BadEggs_Lore_Key"
                },
                IconPath = "ArtifactAssets/BadEggs.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                LinkedClass = CustomClassManager.GetClassDataByID("46ae87db-d92e-4fcb-a3bc-67c723d7bebd"),
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "Malicka_Artifact_BadEggs_Activated_Key",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = typeof(CustomRelicEffectDamageEnemyOnFriendlyTriggerScaledByStatus).AssemblyQualifiedName,
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamInt = 6,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamTrigger = CharacterTriggerData.Trigger.OnDeath,
                        //AppliedVfx = CustomCollectableRelicManager.GetRelicDataByID(VanillaCollectableRelicIDs.ExplodingCandle).GetFirstRelicEffectData<RelicEffectDamageEnemyOnFriendlyTrigger>().GetAppliedVfx(),
                        AppliedVfx = CustomCardManager.GetCardDataByID(VanillaCardIDs.BrambleLash).GetEffects()[0].GetAppliedVFX(),
                        ParamStatusEffects = new StatusEffectStackData[]
                        { 
                            new StatusEffectStackData
                            { 
                                statusId = "hatch",
                                count = 1
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