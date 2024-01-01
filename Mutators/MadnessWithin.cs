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
using Void.CardPools;

namespace Void.Mutators
{
    public static class MadnessWithin
    {
        public static string ID = Beyonder.GUID + "_MadnessWithin";
        public static MutatorData mutatorData;

        public static bool HasIt() 
        {
            if (mutatorData != null)
            {
                List<MutatorState> mutatorStates = ProviderManager.SaveManager.GetMutators();

                if (mutatorStates.Count > 0) 
                { 
                    foreach (MutatorState state in mutatorStates) 
                    {
                        if (state.GetRelicDataID() == mutatorData.GetID()) 
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void BuildAndRegister()
        {
            mutatorData = new Void.Builders.MutatorDataBuilder
            {
                ID = ID,
                NameKey = "Beyonder_Mutator_MadnessWithin_Name_Key",
                DescriptionKey = "Beyonder_Mutator_MadnessWithin_Description_Key",
                RelicActivatedKey = "Beyonder_Mutator_MadnessWithin_Activated_Key",
                RelicLoreTooltipKeys = new List<string>()
                {
                    "Beyonder_Mutator_MadnessWithin_Lore_Key"
                },
                DisableInDailyChallenges = false,
                DivineVariant = false,
                BoonValue = 0,
                RequiredDLC = DLC.None,
                IconPath = "Mutators/Sprite/MTR_MadnessWithin.png",
                Tags = new List<string>
                {
                },
                Effects = new List<RelicEffectData>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(RelicEffectAddCardsStartOfRun),
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamTargetMode = TargetMode.Room,
                        ParamBool = true,
                        ParamCardPool = BeyonderCardPools.MadnessWithinCardPool,
                    }.Build(),
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof (CustomRelicEffectEmbraceTheMadness),
                        ParamInt = 0,
                        ParamUseIntRange = true,
                        ParamMaxInt = 2,
                        ParamMinInt = -2,
                    }.Build(),
                }
            }.BuildAndRegister();
        }
    }
}