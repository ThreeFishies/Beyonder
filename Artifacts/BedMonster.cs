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
    public static class BedMonster
    {
        public static CollectableRelicData Artifact;
        public static string ID = "BedMonster_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_BedMonster_Name_Key",
                DescriptionKey = "Beyonder_Artifact_BedMonster_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_BedMonster_Lore_Key"
                },
                IconPath = "ArtifactAssets/BedMonster.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    /*
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(RelicEffectModifyStatusMagnitude),
                        ParamInt = 1,
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamTargetMode = TargetMode.FrontInRoom,
                        ParamStatusEffects = new StatusEffectStackData[]
                        { 
                            new StatusEffectStackData
                            { 
                                statusId = StatusEffectJitters.statusId,
                                count = 0
                            }
                        }
                    },
                    */
                    /*
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(RelicEffectModifyStatusMagnitude),
                        ParamInt = 1,
                        ParamSourceTeam = Team.Type.Heroes,
                        ParamTargetMode = TargetMode.FrontInRoom,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectJitters.statusId,
                                count = 0
                            }
                        }
                    },
                    */
                    new RelicEffectDataBuilder
                    { 
                        RelicEffectClassName = "RelicEffectAddStatusEffectOnOtherStatusRemoved",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamString = StatusEffectJitters.statusId,
                        ParamStatusEffects = new StatusEffectStackData[]
                        { 
                            new StatusEffectStackData
                            { 
                                statusId = VanillaStatusEffectIDs.Rage,
                                count = 3
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            return Artifact;
        }
    }
}