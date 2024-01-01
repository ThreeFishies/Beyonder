using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using HarmonyLib;
using Void.Init;
using Trainworks.Managers;
using Void.Monsters;
using Trainworks.ConstantsV2;
using Trainworks.BuildersV2;
using Void.CardPools;
using UnityEngine;
using ShinyShoe;
using Void.Builders;

namespace Void.BeyonderStory
{
    public static class CaveStory
    {
        public static bool Registered = false;
        public const string CaveStoryPath = "CaveStory/BeyonderEvent.txt";
        public const string SourceEventPath = "CaveStory/GambleEvent.txt";
        public static StoryEventData CaveStoryEventData;

        public static void EditMasterStoryFile() 
        { 
            if (Registered) { return; }

            TextAsset masterStoryFile = ProviderManager.SaveManager.GetBalanceData().MasterStoryFile;

            if (masterStoryFile == null) 
            {
                Beyonder.Log("Unable to retrieve master story file.");
                return;
            }

            string directoryName = Path.GetDirectoryName(Beyonder.Instance.Info.Location);
            string[] caveStoryRaw = File.ReadAllLines(Path.Combine(directoryName, CaveStoryPath));

            if (caveStoryRaw.Length == 0) 
            {
                Beyonder.Log("Failed to read file: " + Path.Combine(directoryName, CaveStoryPath));
                return;
            }

            string[] sourceEventRaw = File.ReadAllLines(Path.Combine(directoryName, SourceEventPath));

            if (sourceEventRaw.Length == 0)
            {
                Beyonder.Log("Failed to read file: " + Path.Combine(directoryName, SourceEventPath));
                return;
            }

            string fullCaveStory = "";
            string fullSourceEvent = "";

            foreach (string caveStoryLine in caveStoryRaw) 
            {
                fullCaveStory += caveStoryLine.Trim();
            }

            foreach (string sourceEventLine in sourceEventRaw)
            {
                fullSourceEvent += sourceEventLine.Trim();
            }

            string masterStoryString = masterStoryFile.text;

            masterStoryString = masterStoryString.Replace(fullSourceEvent, fullCaveStory);

            AccessTools.Field(typeof(BalanceData), "masterStoryFile").SetValue(ProviderManager.SaveManager.GetBalanceData(), new TextAsset(masterStoryString));

            //Beyonder.Log("__________________________");
            //Beyonder.Log(ProviderManager.SaveManager.GetBalanceData().MasterStoryFile.text);
            //Beyonder.Log("__________________________");

            Registered = true;
        }

        public static void BuildEventData()
        {
            CardRewardData caveReward = new CardRewardDataBuilder()
            {
                overrideID = Beyonder.GUID + "_BeyonderCavernReward",
                name = "BeyonderCavernReward",
                _rewardTitleKey = "BeyonderCavernReward_RewardTitleKey",
                _rewardDescriptionKey = "",
                _cardData = CaveofaThousandEyes.Card,
                _cardDataId = CaveofaThousandEyes.Card.GetID()
            }.BuildAndRegister();

            //StoryEventAssetFrame flowerPoniesFrame = new StoryEventAssetFrame();
            //flowerPoniesFrame = ProviderManager.SaveManager.GetAllGameData().FindStoryEventDataByName("GimmeGold").EventPrefab;

            //Ponies.Log("Is same object as GimmeGold: " + UnityEngine.Object.ReferenceEquals(flowerPoniesFrame, ProviderManager.SaveManager.GetAllGameData().FindStoryEventDataByName("GimmeGold").EventPrefab));
            //Result: true
            //This is too complex to bother duplicating, so I'll just swap sprites as needed.

            StoryEventData eventData = ProviderManager.SaveManager.GetAllGameData().FindStoryEventData("eb406e98-2959-4fd7-b994-d75fedcb7247");

            if (eventData != null) 
            {
                List<RewardData> rewards = (List<RewardData>)AccessTools.Field(typeof(StoryEventData), "possibleRewards").GetValue(eventData);
                if (!rewards.Contains(caveReward))
                {
                    rewards.Add(caveReward);
                }
                AccessTools.Field(typeof(StoryEventData),"possibleRewards").SetValue(eventData, rewards);
            }

            /*
            StoryEventData flowerPoniesEventData = new StoryEventDataBuilder()
            {
                overrideID = Ponies.GUID + "_FlowerPonies",
                name = "FlowerPonies",
                storyId = "FlowerPonies",
                knotName = "FlowerPonies",
                priorityTicketCount = 20,
                eventPrefab = flowerPoniesFrame,
                possibleRewards = new List<RewardData>
                {
                    ProviderManager.SaveManager.GetAllGameData().FindRewardDataByName("GoldReward100"),
                    tomeReward,
                    spikeReward,
                    secretReward
                }
            }.BuildAndRegister();
            */

            //Already seems to be in the list.
            //if (!ProviderManager.SaveManager.GetAllGameData().GetStoryRewardList().Rewards.Contains(ProviderManager.SaveManager.GetAllGameData().FindRewardDataByName("GoldReward100")))
            //{
            //    Ponies.Log("Adding 100 gold to story reward list.");
            //    RewardDataList dataList = (RewardDataList)AccessTools.Field(typeof(AllGameData), "storyRewardList").GetValue(ProviderManager.SaveManager.GetAllGameData());
            //    dataList.Rewards.Add(ProviderManager.SaveManager.GetAllGameData().FindRewardDataByName("GoldReward100"));
            //}

            //List<StoryEventData> datas = (List<StoryEventData>)AccessTools.Field(typeof(AllGameData), "storyEventDatas").GetValue(ProviderManager.SaveManager.GetAllGameData());
            //
            //Ponies.Log("___________Staritng_Stories_Dump______________");
            //foreach (StoryEventData data in datas)
            //{
            //    Ponies.Log(data.KnotName);
            //}
            //Ponies.Log("______________________________________________");

            //List<StoryEventData> datas = (List<StoryEventData>)AccessTools.Field(typeof(StoryEventPoolData).GetNestedType("StoryEventDataList", BindingFlags.NonPublic | BindingFlags.Instance), "storyEvents").GetValue(ProviderManager.SaveManager.GetRunData().GetStartingEventPoolData());
            //datas.Add(flowerPoniesEventData);

            return; // flowerPoniesEventData;
        }
    }
}