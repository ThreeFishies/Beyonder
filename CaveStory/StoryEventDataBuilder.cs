//This is not needed since we're just editing an existing event instead of making a new one.

/*
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ShinyShoe;
using Trainworks.Managers;
using Trainworks.Utilities;
using Trainworks.Builders;
using UnityEngine;
using Equestrian.Init;
using Malee;

namespace Trainworks.Builders
{
	/// <summary>
	///	A utility for building a StoryEventData object. This defines the meta data for cavern events.
	/// </summary>
	public class StoryEventDataBuilder
	{
		/// <summary>
		/// Set this make an ID consistent when exiting and restarting. Otherwise the event won't load properly and be inaccessable.
		/// </summary>
		public string overrideID;

		/// <summary>
		/// Make same as KnotName.
		/// </summary>
		public string name = "";

		/// <summary>
		/// Make same as KnotName.
		/// </summary>
		public string storyId = "";

		/// <summary>
		/// The primary ID for the event.
		/// </summary>
		public string knotName = "";

		/// <summary>
		/// For now, this defaults to GimmeGold.
		/// </summary>
		public StoryEventAssetFrame eventPrefab = ProviderManager.SaveManager.GetAllGameData().FindStoryEventDataByName("GimmeGold").EventPrefab;
		public List<FollowupEventData> followupEvents = new List<FollowupEventData>();
		public int numRunsCompletedToSee = 0;

		/// <summary>
		/// Ranges from 0 to 100 with higher numbers appearing less frequently.
		/// </summary>
		public int priorityTicketCount = 0;
		public int numClassesNeededToShow = 1;
		public int covenantLevelRequired = 0;
		public bool allClassesNeededToShow = false;
		public int minDistanceAllowed = 0;
		public int maxDistanceAllowed = 7;
		public MutatorData excludedMutator;
		public DLC associatedDLC = DLC.None;
		public bool requireDlcModeActive = false;

		/// <summary>
		/// Set to 0 to skip the check.
		/// </summary>
		public int numCrystalsRequired = 0;
		public List<StoryEventData> excludingEventData = new List<StoryEventData>();
		public List<RewardData> possibleRewards = new List<RewardData>();

		public StoryEventData Build() 
		{ 
			StoryEventData newStory = ScriptableObject.CreateInstance<StoryEventData>();

			newStory.name = name;

			AccessTools.Field(typeof(StoryEventData), "associatedDLC").SetValue(newStory, associatedDLC);
			AccessTools.Field(typeof(StoryEventData), "storyId").SetValue(newStory, storyId);
			AccessTools.Field(typeof(StoryEventData), "knotName").SetValue(newStory, knotName);
			AccessTools.Field(typeof(StoryEventData), "eventPrefab").SetValue(newStory, eventPrefab);
			AccessTools.Field(typeof(StoryEventData), "followupEvents").SetValue(newStory, followupEvents);
			AccessTools.Field(typeof(StoryEventData), "numRunsCompletedToSee").SetValue(newStory, numRunsCompletedToSee);
			AccessTools.Field(typeof(StoryEventData), "priorityTicketCount").SetValue(newStory, priorityTicketCount);
			AccessTools.Field(typeof(StoryEventData), "numClassesNeededToShow").SetValue(newStory, numClassesNeededToShow);
			AccessTools.Field(typeof(StoryEventData), "covenantLevelRequired").SetValue(newStory, covenantLevelRequired);
			AccessTools.Field(typeof(StoryEventData), "allClassesNeededToShow").SetValue(newStory, allClassesNeededToShow);
			AccessTools.Field(typeof(StoryEventData), "minDistanceAllowed").SetValue(newStory, minDistanceAllowed);
			AccessTools.Field(typeof(StoryEventData), "maxDistanceAllowed").SetValue(newStory, maxDistanceAllowed);
			AccessTools.Field(typeof(StoryEventData), "excludedMutator").SetValue(newStory, excludedMutator);
			AccessTools.Field(typeof(StoryEventData), "requireDlcModeActive").SetValue(newStory, requireDlcModeActive);
			AccessTools.Field(typeof(StoryEventData), "numCrystalsRequired").SetValue(newStory, numCrystalsRequired);
			AccessTools.Field(typeof(StoryEventData), "excludingEventData").SetValue(newStory, excludingEventData);
			AccessTools.Field(typeof(StoryEventData), "possibleRewards").SetValue(newStory, possibleRewards);

			if (!overrideID.IsNullOrEmpty()) 
			{
				AccessTools.Field(typeof(GameData), "id").SetValue(newStory, overrideID);
			}

			return newStory;
		}

		/// <summary>
		/// Adds a StoryEventData object to AllGameData.
		/// </summary>
		/// <param name="storyEventData">
		/// The data to add to AllGameData.
		/// </param>
		/// <returns>
		/// True if successful, false if null or duplicate.
		/// </returns>
		public bool Register(StoryEventData storyEventData) 
		{
			if (storyEventData == null) 
			{
				Ponies.Log("Attempting to register a null StoryEventData. Fail.");
				return false;
			}

			if (ProviderManager.SaveManager.GetAllGameData().FindStoryEventData(storyEventData.GetID()) != null) 
			{
				Ponies.Log("Attempting to register a StoryEventData that already exists.");
				return false;
			}

			List<StoryEventData> list = (List<StoryEventData>)AccessTools.Field(typeof(AllGameData), "storyEventDatas").GetValue(ProviderManager.SaveManager.GetAllGameData());
			list.Add(storyEventData);

			//This adds the story to the cavern event pool.
			StoryEventPoolData pool = (StoryEventPoolData)ProviderManager.SaveManager.GetAllGameData().FindMapNodeData("a183b211-8a77-4dcb-8790-125884ca9373");
			ReorderableArray<StoryEventData> stories = (ReorderableArray<StoryEventData>)AccessTools.Field(typeof(StoryEventPoolData), "storyEvents").GetValue(pool);
			stories.Add(storyEventData);

			return true;
		}

		public StoryEventData BuildAndRegister() 
		{
			StoryEventData newStory = this.Build();

			this.Register(newStory);

			return newStory;
		}
	}
}
*/