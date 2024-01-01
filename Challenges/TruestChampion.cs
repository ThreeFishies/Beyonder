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
using Trainworks.ConstantsV2;
using Trainworks.ManagersV2;
using Equestrian.Metagame;

namespace Void.Mutators
{
    public static class TruestChampion
    {
        public static string ID = Beyonder.GUID + "_TruestChampion";
        public static SpChallengeData challengeData;

        public static void BuildAndRegister() 
        { 
            challengeData = new SpChallengeDataBuilder() 
            {
                ChallengeID = ID,
                NameKey = "Beyonder_spChallenge_TruestChampion_Name_Key",
                DescriptionKey = "Beyonder_spChallenge_TruestChampion_Description_Key",
                RequiredDLC = DLC.None,
                Mutators = new List<MutatorData> 
                { 
                    RestlessBeast.mutatorData,
                    CustomMutatorManager.GetMutatorDataByID(VanillaMutatorIDs.FallenChampion)
                }
            }.BuildAndRegister();

            //     [Info   :Beyonder Clan] Tracking spChallenge ID: 353b6308-edd6-44e9-b72f-c211bc955e0b
            //[Info   :Beyonder Clan] Registered Challenge with ID: 353b6308-edd6-44e9-b72f-c211bc955e0b
            PonyMetagame.RegisterChallenge(challengeData.GetID());
            //Beyonder.Log($"Registered Challenge with ID: {challengeData.GetID()}");
        }
    }
}