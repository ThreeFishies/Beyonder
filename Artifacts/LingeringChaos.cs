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
using Steamworks;

namespace Void.Artifacts
{
    public static class LingeringChaos
    {
        public static CollectableRelicData Artifact;
        public static string ID = "LingeringChaos_" + Beyonder.GUID;

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
                ClanID = Beyonder.BeyonderClanData.GetID(),
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Beyonder_Artifact_LingeringChaos_Name_Key",
                DescriptionKey = "Beyonder_Artifact_LingeringChaos_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_LingeringChaos_Lore_Key"
                },
                IconPath = "ArtifactAssets/LingeringChaos.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectAddStatusEffectOnSpawn",
                        ParamInt = 0,
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamTargetMode = TargetMode.FrontInRoom,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamExcludeCharacterSubtypes = new string[]
                        {
                            "SubtypesData_Snack"
                        },
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectChronic.statusId,
                                count = 2
                            }
                        }
                    },
                },
            }.BuildAndRegister();

            return Artifact;
        }
    }
}