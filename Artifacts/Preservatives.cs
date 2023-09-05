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
    public static class Preservatives
    {
        public static CollectableRelicData Artifact;
        public static string ID = "Preservatives_" + Beyonder.GUID;

        public static bool TryGetClassData(out ClassData clan)
        {
            clan = null;

            if (!Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("Exas4000"))
            {
                return false;
            }

            clan = CustomClassManager.GetClassDataByID("Sweetkin_Clan");

            if (clan == null)
            {
                Beyonder.LogError("Sweetkin detected but failed to find class data.");
                return false;
            }

            return true;
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
                NameKey = "Malicka_Artifact_Preservatives_Name_Key",
                DescriptionKey = "Malicka_Artifact_Preservatives_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_Preservatives_Lore_Key"
                },
                IconPath = "ArtifactAssets/Preservatives.png",
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
                        RelicEffectClassName = "RelicEffectAddStatusEffectOnStatusApplied",
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamInt = 100,
                        ParamBool = true,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamExcludeCharacterSubtypes = new string[] {},
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData()
                            {
                                statusId = "eatmany",
                                count = 1
                            }
                        },
                        ParamTargetMode = TargetMode.FrontInRoom,
                    },
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }

    [HarmonyPatch(typeof(CombatFeederRules), "GetFrontFeederUnit")]
    public static class MalickasPasteFeederRulesPatch1
    {
        public static void Postfix(List<CharacterState> allTeamCharactersInRoom, ref CharacterState __result)
        {
            if (Preservatives.HasIt())
            {
                for (int i = 0; i < allTeamCharactersInRoom.Count; i++)
                {
                    CharacterState characterState = allTeamCharactersInRoom[i];
                    if (!characterState.HasStatusEffect("untouchable") && !(CombatFeederRules.GetIsFoodUnit(characterState) && characterState.GetStatusEffectStacks("eatmany") < 2))
                    {
                        __result = characterState;
                        return;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(CombatFeederRules), "GetAnyFeederUnits")]
    public static class MalickasPasteFeederRulesPatch2
    {
        public static void Postfix(List<CharacterState> inList, ref bool __result)
        {
            if (Preservatives.HasIt() && !__result) 
            {
                foreach (CharacterState characterState in inList)
                {
                    if (!characterState.HasStatusEffect("untouchable") && characterState.IsAlive && !(CombatFeederRules.GetIsFoodUnit(characterState) && characterState.GetStatusEffectStacks("eatmany") < 2))
                    {
                        __result = true;
                        return;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(CombatFeederRules), "GetIsFoodUnit")]
    public static class DoNotEatSilencedMorsels 
    {
        public static void Postfix(CharacterState charState, ref bool __result)
        {
            if (charState != null && charState.IsAlive && charState.HasStatusEffect(VanillaStatusEffectIDs.Silenced)) 
            {
                __result = false;
            }
            return;
        }
    }
}