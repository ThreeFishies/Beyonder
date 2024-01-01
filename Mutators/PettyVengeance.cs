using Trainworks.Managers;
using Trainworks.BuildersV2;
using Trainworks.Constants;
using Void.Init;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using HarmonyLib;
using ShinyShoe;
using UnityEngine;
using CustomEffects;
using Void.Builders;

namespace Void.Mutators
{
    public static class PettyVengeance
    {
        public static string ID = Beyonder.GUID + "_PettyVengeance";
        public static MutatorData mutatorData;

        public static void BuildAndRegister()
        {
            RewardData VengefulShard = ProviderManager.SaveManager.GetAllGameData().FindRewardData("e55de0c4-6194-4beb-aa36-5c9afbd1b1ab");
            AccessTools.Field(typeof(RewardData), "_rewardTitleKey").SetValue(VengefulShard, "Beyonder_Mutator_PettyVengeance_Reward_Title_Key");

            mutatorData = new Void.Builders.MutatorDataBuilder
            {
                ID = ID,
                NameKey = "Beyonder_Mutator_PettyVengeance_Name_Key",
                DescriptionKey = "Beyonder_Mutator_PettyVengeance_Description_Key",
                RelicActivatedKey = "Beyonder_Mutator_PettyVengeance_Activated_Key",
                RelicLoreTooltipKeys = new List<string>()
                {
                    "Beyonder_Mutator_PettyVengeance_Lore_Key"
                },
                DisableInDailyChallenges = false,
                DivineVariant = false,
                BoonValue = -4,
                RequiredDLC = DLC.None,
                IconPath = "Mutators/Sprite/MTR_PettyVengeance.png",
                Tags = new List<string>
                {
                },
                Effects = new List<RelicEffectData>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(RelicEffectAddPostBattleReward),
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamTargetMode = TargetMode.Room,
                        ParamBool = false,
                        ParamInt = 1,
                        ParamReward = VengefulShard
                    }.Build(),
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof (CustomRelicEffectNULL),
                    }.Build(),
                }
            }.BuildAndRegister();
        }
    }

    [HarmonyPatch(typeof(CardRewardData), "get_CanBeSkipped")]
    public static class MakeVengefulShardUnskippable 
    {
        public static void Postfix(ref CardRewardData __instance, ref bool __result)
        {
            if (__instance.name == "BlightPyreDamageReward") 
            {
                __result = false;
            }
        }
    }
}