using System;
using System.Collections.Generic;
using Trainworks.Builders;
using Trainworks.Constants;
using Trainworks.Managers;
using UnityEngine;
using System.Text;
using HarmonyLib;
using Trainworks.Enums;
using Void.Init;
using Void.Monsters;
using Void.Artifacts;

namespace Void.Clan
{
	internal class BeyonderClan
	{
		public static readonly string ID = "Beyonder";

		public static ClassData Buildclan()
		{
			return new ClassDataBuilder
			{
				ClassID = ID,
				DraftIconPath = "ClanAssets/Icon_CardBack_Beyonder.png",
				Name = "Beyonder",
				TitleLoc = "Beyonder_Clan_Name",
				Description = "With the world caught in a loop, the repeated resets are causing the fabric of reality to crumble. Through the cracks seep the terrifying and maddening Beyonders who join the conflict for the sheer pleasure of pure chaos.",
				DescriptionLoc = "Beyonder_Clan_desc",
				SubclassDescription = "Abandon your sanity and ally yourself with the madness of the Beyonders.",
				SubclassDescriptionLoc = "Beyonder_Clan_subdesc",
				IconAssetPaths = new List<string>
				{
					"ClanAssets/BeyonderLogo_32.png",
					"ClanAssets/BeyonderLogo_89.png",
					"ClanAssets/BeyonderLogo_89.png",
					"ClanAssets/BeyonderLogo_Silhouette.png"
				},
				CardFrameUnitPath = "ClanAssets/unit-cardframe-Beyonder.png",
				CardFrameSpellPath = "ClanAssets/spell-cardframe-Beyonder.png",
				//StarterRelics = new List<RelicData> 
				//{
				//	UnSeeingEye.Artifact
				//},
				
				//UiColor = new Color(0.49f, 0.325f, 0.776f, 1.0f),
				//UiColorDark = new Color(0.218f, 0.145f, 0.346f, 1.0f)

                UiColor = new Color(0.875f, 0.376f, 1.0f, 1.0f),
                UiColorDark = new Color(0.481f, 0.206f, 0.550f, 1.0f)

            }.BuildAndRegister();
		}
	}

	public class BeyonderBanner
	{
		public static readonly string BannerID = BeyonderClan.ID + "_Banner";
		public static readonly string RewardID = BeyonderClan.ID + "_BannerReward";
		public static CardPool draftPool = UnityEngine.ScriptableObject.CreateInstance<CardPool>();

		public static void buildbanner()
		{
			Malee.ReorderableArray<CardData> cardDataList = (Malee.ReorderableArray<CardData>)AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(draftPool);
			cardDataList.Add(FormlessHorror.Card);
			cardDataList.Add(SoundlessSwarm.Card);
			cardDataList.Add(Malevolence.Card);
			cardDataList.Add(HairyPotty.Card);
			cardDataList.Add(Vexation.Card);
			cardDataList.Add(FurryBeholder.Card);
			cardDataList.Add(Deathwood.Card);
			cardDataList.Add(Chutzpah.Card);
			cardDataList.Add(ApostleoftheVoid.Card);

			Beyonder.Log("Banner Card Pool");

			new RewardNodeDataBuilder
			{
				RewardNodeID = BannerID,
				MapNodePoolIDs = new List<string>
				{
					VanillaMapNodePoolIDs.RandomChosenMainClassUnit,
					VanillaMapNodePoolIDs.RandomChosenSubClassUnit
				},
				TooltipTitleKey = "name_beyonder_banner",
				TooltipBodyKey = "desc_beyonder_banner",
				RequiredClass = Beyonder.BeyonderClanData,
				FrozenSpritePath = "ClanAssets/Beyonder_Frozen.png",
				EnabledSpritePath = "ClanAssets/Beyonder_Enabled.png",
				DisabledSpritePath = "ClanAssets/Beyonder_Disabled.png",
				DisabledVisitedSpritePath = "ClanAssets/Beyonder_VisitedDisabled.png",
				GlowSpritePath = "ClanAssets/MSK_Map_Clan_CBeyonder_01.png",
				MapIconPath = "ClanAssets/Beyonder_Enabled.png",
				MinimapIconPath = "ClanAssets/BeyonderLogo_Silhouette.png",
				ControllerSelectedOutline = "ClanAssets/selection_outlines.png",
				SkipCheckInBattleMode = true,
				OverrideTooltipTitleBody = false,
				NodeSelectedSfxCue = "Node_Banner",
				RewardBuilders = new List<IRewardDataBuilder>
				{
					new DraftRewardDataBuilder()
					{
						DraftRewardID = RewardID,
						_RewardSpritePath = "ClanAssets/Icon_CardBack_Beyonder.png",
						_RewardTitleKey = "Beyonder_Banner",
						_RewardDescriptionKey = "Choose a card!",
						Costs = new int[]
						{
							100
						},
						_IsServiceMerchantReward = false,
						DraftPool = draftPool,
						ClassType = RunState.ClassType.MainClass | RunState.ClassType.SubClass | RunState.ClassType.None,
						DraftOptionsCount = 2,
						RarityFloorOverride = CollectableRarity.Uncommon,
					}
				}
			}.BuildAndRegister();
		}
	}
}
