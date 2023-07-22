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
    public static class UnstableEnergy
    {
        public static CollectableRelicData Artifact;
        public static string ID = "UnstableEnergy_" + Beyonder.GUID;

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
                NameKey = "Beyonder_Artifact_UnstableEnergy_Name_Key",
                DescriptionKey = "Beyonder_Artifact_UnstableEnergy_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Artifact_UnstableEnergy_Lore_Key"
                },
                IconPath = "ArtifactAssets/UnstableEnergy.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = Beyonder.BeyonderClanData,
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectDamageEnemyOnFriendlyTrigger",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamInt = 30,
                        ParamBool = false,
                        ParamTargetMode = TargetMode.FrontInRoom,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectChronic.statusId,
                                count = 0
                            }
                        },
                        ParamTrigger = CharacterTriggerData.Trigger.OnDeath,
                        AppliedVfx = CustomCardManager.GetCardDataByID(VanillaCardIDs.EntombedExplosive).GetSpawnCharacterData().GetTriggers()[0].GetEffects()[0].GetAppliedVFX(),
                        TriggerTooltipsSuppressed = true,
                    },
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectDamageEnemyOnFriendlyTrigger",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamInt = 30,
                        ParamBool = false,
                        ParamTargetMode = TargetMode.FrontInRoom,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectChronic.statusId,
                                count = 0
                            }
                        },
                        ParamTrigger = CharacterTriggerData.Trigger.OnEaten,
                        AppliedVfx = CustomCardManager.GetCardDataByID(VanillaCardIDs.EntombedExplosive).GetSpawnCharacterData().GetTriggers()[0].GetEffects()[0].GetAppliedVFX(),
                        TriggerTooltipsSuppressed = true,
                    },
                    /*
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectDamageEnemyOnFriendlyTrigger",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamInt = 30,
                        ParamBool = false,
                        ParamTargetMode = TargetMode.FrontInRoom,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectChronic.statusId,
                                count = 0
                            }
                        },
                        ParamTrigger = CharacterTriggerData.Trigger.OnBurnout,
                        AppliedVfx = CustomCardManager.GetCardDataByID(VanillaCardIDs.EntombedExplosive).GetSpawnCharacterData().GetTriggers()[0].GetEffects()[0].GetAppliedVFX(),
                        TriggerTooltipsSuppressed = true,
                    }*/
                },
            }.BuildAndRegister();

            return Artifact;
        }
    }
}