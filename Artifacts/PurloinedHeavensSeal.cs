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
    public static class PurloinedHeavensSeal
    {
        public static CollectableRelicData Artifact;
        public static string ID = "PurloinedHeavensSeal_" + Beyonder.GUID;

        public static bool TryGetClassData(out ClassData clan)
        {
            clan = null;

            if (!Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("ca.chronometry.disciple"))
            {
                return false;
            }

            clan = CustomClassManager.GetClassDataByID("Chrono");

            if (clan == null)
            {
                Beyonder.LogError("Arcadian detected but failed to find class data.");
                return false;
            }

            return true;
        }

        public static CharacterTriggerData.Trigger GetOnRelocate() 
        {
            CharacterTriggerData.Trigger trigger = CharacterTriggerData.Trigger.OnDeath;

            if (TryGetClassData(out ClassData clan)) 
            {
                trigger = clan.GetChampionData(0).upgradeTree.GetUpgradeTrees()[2].GetCardUpgrades()[0].GetTriggerUpgrades()[0].GetTrigger();
            }

            return trigger;
        }

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
            if (!TryGetClassData(out ClassData clan))
            {
                return null;
            }

            Artifact = new CollectableRelicDataBuilder
            {
                CollectableRelicID = ID,
                ClanID = clan.GetID(),
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_PurloinedHeavensSeal_Name_Key",
                DescriptionKey = "Malicka_Artifact_PurloinedHeavensSeal_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_PurloinedHeavensSeal_Lore_Key"
                },
                IconPath = "ArtifactAssets/PurloinedHeavensSeal.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                LinkedClass = clan,
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "EmptyString-0000000000000000-00000000000000000000000000000000-v2",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = typeof(CustomRelicEffectHealFriendlyUnitsOnRelocate).AssemblyQualifiedName,
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamInt = 999,
                        ParamExcludeCharacterSubtypes = new string[] {}
                    },
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}