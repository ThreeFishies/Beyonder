/*
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using HarmonyLib;
using Trainworks.Managers;
using Trainworks.Constants;
using Trainworks.Builders;
using ShinyShoe.Loading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Void.Init;

namespace Equestrian.HarmonyPatches
{
    [HarmonyPatch(typeof(EnhancerPoolRewardData), "GatherGrantableRewards")]
    public static class FindThemCustomEnhancers 
    { 
        public static void RegisterEnhancersFromList(ICollection<GrantableRewardData> enhancers) 
        {
            //Ponies.Log("Count: " + rewardsList.Count);

            AllGameData allGameData = ProviderManager.SaveManager.GetAllGameData();
            List<EnhancerData> enhancerList = allGameData.GetAllEnhancerData();

            foreach (GrantableRewardData rewardData in enhancers)
            {
                if (rewardData != null)
                {
                    if (rewardData is EnhancerRewardData enhancerRewardData)
                    {
                        //Ponies.Log(enhancerRewardData.GetRelicData().GetAssetKey());
                        string enhancerName = enhancerRewardData.GetRelicData().GetAssetKey();

                        if (allGameData.FindEnhancerDataByName(enhancerName) == null)
                        {
                            //Ponies.Log("Custom Enhancer detected.");
                            //Ponies.Log("Enhancer name: " + enhancerName);

                            AccessTools.Field(typeof(EnhancerRewardData), "id").SetValue(enhancerRewardData, enhancerName);
                            ProviderManager.SaveManager.SaveGeneratedGrantableReward<EnhancerRewardData>(enhancerRewardData);

                            enhancerList.Add(enhancerRewardData.GetRelicData() as EnhancerData);
                        }

                        if (allGameData.FindEnhancerDataByName(enhancerName) != null)
                        {
                            //Ponies.Log("Sucessfully added EnhancerData: " + enhancerName);
                        }
                    }
                }
            }
        }

        public static void Postfix(ref ICollection<GrantableRewardData> rewardsList) 
        { 
            if (!Beyonder.IsInit) { return; }
            if (rewardsList == null) { return; }
            if (rewardsList.Count == 0) { return; }

            RegisterEnhancersFromList(rewardsList);
        }
    }

    [HarmonyPatch(typeof(MerchantScreen), "Initialize")]
    public static class JotFranksMemory 
    { 
        public static void RepopulateEnhancerData() 
        {
            SaveManager saveManager = ProviderManager.SaveManager;

            int distance = saveManager.GetRunState().GetCurrentDistance();
            int branch = saveManager.GetCurrentBranch();
            int index = -1;

            List<MerchantData> list2 = saveManager.GetRunState().MerchantsAtDistance(distance, branch, index);

            foreach (MerchantData merchant in list2) 
            { 
                for (int jj = 0; jj < merchant.GetNumRewards(); jj++) 
                { 
                    if (merchant.GetReward(jj).RewardData is EnhancerPoolRewardData enhancerPoolRewardData)
                    {
                        ICollection<GrantableRewardData> grantables = new List<GrantableRewardData>();
                        //enhancerPoolRewardData.GatherGrantableRewards(grantables, saveManager, null, null, false);

                        enhancerPoolRewardData.GatherAllPossibleRewards(grantables);

                        FindThemCustomEnhancers.RegisterEnhancersFromList(grantables);
                    }
                }
            }
        }

        public static void Prefix(ref RewardState.Location ___merchantLocation) 
        {
            List<MerchantGoodState> list = new List<MerchantGoodState>();
            list.AddRange(ProviderManager.SaveManager.GetMerchantGoodsAtDistance(___merchantLocation, true));

            foreach (MerchantGoodState item in list) 
            {
                string rewardId1 = "Not a RewardState";

                if (item is RewardState)
                {
                    RewardState reward = item as RewardState;
                    rewardId1 = AccessTools.Field(typeof(RewardState), "rewardId").GetValue(reward) as string;
                    //Ponies.Log("Reward ID: " + rewardId1);
                }

                if (item == null)
                {
                    Beyonder.Log("Invalid Item detected.");
                }
                else if (item.RewardData == null)
                {
                    Beyonder.Log("Invalid Reward Detected.");
                }
                else 
                { 
                    if (item.RewardData == null) 
                    {
                        Beyonder.Log("Failed to load reward ID: " + rewardId1);
                    }
                    else 
                    {
                        if (item.RewardData is RelicRewardData)
                        {
                            RelicRewardData relic = item.RewardData as RelicRewardData;

                            if (relic.GetRelicData() == null)
                            {
                                Beyonder.Log("Missing Relic Data.");
                                Beyonder.Log("ID: " + relic.GetID());

                                EnhancerData enhancer = ProviderManager.SaveManager.GetAllGameData().FindEnhancerDataByName(relic.GetID());
                                if (enhancer == null) 
                                {
                                    Beyonder.Log("Attempt to locate missing data failed. Attempting to repopulate.");

                                    RepopulateEnhancerData();

                                    enhancer = ProviderManager.SaveManager.GetAllGameData().FindEnhancerDataByName(relic.GetID());

                                    if (enhancer == null) 
                                    {
                                        Beyonder.Log("Repopulation failed.");
                                    }
                                    else 
                                    {
                                        if (relic is EnhancerRewardData enhancer1)
                                        {
                                            Beyonder.Log("Repopulation successful. Replacing lost data.");
                                            enhancer1.SetEnhancerData(enhancer);
                                        }
                                    }
                                }
                                else 
                                { 
                                    if (relic is EnhancerRewardData enhancer1) 
                                    {
                                        Beyonder.Log("Replacing lost data.");
                                        enhancer1.SetEnhancerData(enhancer);
                                    }
                                }
                            }
                            else
                            {
                                //Ponies.Log("Line 69");
                                //Ponies.Log("name: " + relic.GetRelicData().name);
                                //Ponies.Log("name key: " + relic.GetRelicData().GetNameKey());
                                //Ponies.Log("get name:" + relic.GetRelicData().GetName());
                            }
                        }
                        else 
                        {
                            //Ponies.Log("RewardData is not RelicRewardData.");
                        }
                    }
                }

                if (item.LoadRewardData(ProviderManager.SaveManager)) 
                {
                    //Ponies.Log("Sucessfully loaded RewardData.");
                }
                else 
                {
                    Beyonder.Log("Failed to load RewardData.");
                }
            }
        }
    }
}*/