using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ShinyShoe;
using Trainworks.Managers;
using Trainworks.Utilities;
using Trainworks.Builders;
using UnityEngine;
using Void.Init;

namespace Void.Builders
{
    /// <summary>
    ///	A utility for building a CardRewardData object. This defines a possible outcome from a cavern event.
    /// </summary>
    public class CardRewardDataBuilder
    {
        /// <summary>
        /// Set this make an ID consistent when exiting and restarting. Leave empty to use an auto-generated value.
        /// </summary>
        public string overrideID;

        public string name = "";

        //Base RewardData fields
        public Sprite _rewardSprite = ProviderManager.SaveManager.GetAllGameData().FindRewardDataByName("Class1TomeReward").RewardSprite;
        public string _rewardTitleKey = "";
        public string _rewardDescriptionKey = "";
        public string _collectSFXCueName = "Collect_CardRare";
        public int[] costs =
        {
            20,
            30,
            40,
            50,
            60,
            70,
            80,
            90,
            100
        };
        public int crystals = 0;
        public bool _showRewardFlowInEvent = false;
        public bool ShowRewardAnimationInEvent = false;
        public bool _showCancelOverride = false;
        //public int RewardValue = 0;
        //public string RewardDetail = null;

        //Base GrantableRewardData fields
        public bool CanBeSkippedOverride = false;
        public bool ForceContentUnlocked = false;
        public bool _isServiceMerchantReward = false;
        public int _merchantServiceIndex = 0;
        public SaveManager saveManager = ProviderManager.SaveManager;

        //Specific CardRweardData fields
        /// <summary>
        /// The CardData to be rewarded.
        /// </summary>
        public CardData _cardData;
        /// <summary>
        /// This field appears to be unused.
        /// </summary>
        public string _cardDataId = "";

        public CardRewardData Build()
        {
            CardRewardData cardRewardData = ScriptableObject.CreateInstance<CardRewardData>();

            cardRewardData.name = name;
            AccessTools.Field(typeof(CardRewardData), "_rewardSprite").SetValue(cardRewardData, _rewardSprite);
            AccessTools.Field(typeof(CardRewardData), "_rewardTitleKey").SetValue(cardRewardData, _rewardTitleKey);
            AccessTools.Field(typeof(CardRewardData), "_rewardDescriptionKey").SetValue(cardRewardData, _rewardDescriptionKey);
            AccessTools.Field(typeof(CardRewardData), "_collectSFXCueName").SetValue(cardRewardData, _collectSFXCueName);
            AccessTools.Field(typeof(CardRewardData), "costs").SetValue(cardRewardData, costs);
            AccessTools.Field(typeof(CardRewardData), "crystals").SetValue(cardRewardData, crystals);
            AccessTools.Field(typeof(CardRewardData), "_showRewardFlowInEvent").SetValue(cardRewardData, _showRewardFlowInEvent);
            AccessTools.Field(typeof(CardRewardData), "ShowRewardAnimationInEvent").SetValue(cardRewardData, ShowRewardAnimationInEvent);
            AccessTools.Field(typeof(CardRewardData), "_showCancelOverride").SetValue(cardRewardData, _showCancelOverride);
            //AccessTools.Field(typeof(CardRewardData), "RewardValue").SetValue(cardRewardData, RewardValue);
            //AccessTools.Field(typeof(CardRewardData), "RewardDetail").SetValue(cardRewardData, RewardDetail);
            AccessTools.Field(typeof(CardRewardData), "CanBeSkippedOverride").SetValue(cardRewardData, CanBeSkippedOverride);
            AccessTools.Field(typeof(CardRewardData), "ForceContentUnlocked").SetValue(cardRewardData, ForceContentUnlocked);
            AccessTools.Field(typeof(CardRewardData), "_isServiceMerchantReward").SetValue(cardRewardData, _isServiceMerchantReward);
            AccessTools.Field(typeof(CardRewardData), "_merchantServiceIndex").SetValue(cardRewardData, _merchantServiceIndex);
            AccessTools.Field(typeof(CardRewardData), "saveManager").SetValue(cardRewardData, saveManager);
            AccessTools.Field(typeof(CardRewardData), "_cardData").SetValue(cardRewardData, _cardData);
            AccessTools.Field(typeof(CardRewardData), "_cardDataId").SetValue(cardRewardData, _cardDataId);

            if (!overrideID.IsNullOrEmpty())
            {
                AccessTools.Field(typeof(GameData), "id").SetValue(cardRewardData, overrideID);
            }

            return cardRewardData;
        }

        /// <summary>
        /// Adds the CardRewardData to AllGameData.
        /// </summary>
        /// <param name="cardRewardData">The data to be added to AllGameData.</param>
        /// <returns>True if successful, False on null or duplicate.</returns>
        public bool Register(CardRewardData cardRewardData)
        {
            if (cardRewardData == null) 
            {
                Beyonder.Log("Attempting to register a null CardRewardData object.");
                return false; 
            }
            if (ProviderManager.SaveManager.GetAllGameData().FindRewardDataByName(cardRewardData.name) != null) 
            {
                Beyonder.Log($"CardRewardData with name {cardRewardData.name} already exists.");
                return false;
            }

            List<GrantableRewardData> rewards = (List<GrantableRewardData>)AccessTools.Field(typeof(AllGameData), "rewardDatas").GetValue(ProviderManager.SaveManager.GetAllGameData());
            rewards.Add(cardRewardData);

            RewardDataList dataList = (RewardDataList)AccessTools.Field(typeof(AllGameData), "storyRewardList").GetValue(ProviderManager.SaveManager.GetAllGameData());
            dataList.Rewards.Add(cardRewardData);

            Beyonder.Log($"Registered CardRewardData {cardRewardData.name} with AllGameData.");
            return true;
        }

        /// <summary>
        /// Builds the CardRewardData object and adds it AllGameData.
        /// </summary>
        /// <returns>The newly created CardRewardData object.</returns>
        public CardRewardData BuildAndRegister() 
        {
            CardRewardData cardRewardData = this.Build();
            this.Register(cardRewardData);
            return cardRewardData;
        }
   	}
}