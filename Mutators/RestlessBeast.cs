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
using Spine;

namespace Void.Mutators
{
    public static class RestlessBeast
    {
        public static string ID = Beyonder.GUID + "_RestlessBeast";
        public static MutatorData mutatorData;

        public static void BuildAndRegister()
        {
            CardUpgradeData upgradeData = new CardUpgradeDataBuilder 
            { 
                UpgradeID = ID + "_Upgrade",
                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateType = typeof(BeyonderCardTraitStalkerState)
                    }
                },
                FiltersBuilders = new List<CardUpgradeMaskDataBuilder> 
                { 
                    new CardUpgradeMaskDataBuilder
                    { 
                        CardUpgradeMaskID = "RestlessBeastCardFIlter",
                        AllowedCardPools = new List<CardPool>
                        { 
                            BeyonderCardPools.CaveStoryCardPool
                        }
                    }
                }
            }.Build();

            mutatorData = new Void.Builders.MutatorDataBuilder
            {
                ID = ID,
                NameKey = "Beyonder_Mutator_RestlessBeast_Name_Key",
                DescriptionKey = "Beyonder_Mutator_RestlessBeast_Description_Key",
                RelicActivatedKey = "Beyonder_Mutator_RestlessBeast_Activated_Key",
                RelicLoreTooltipKeys = new List<string>()
                {
                    "Beyonder_Mutator_RestlessBeast_Lore_Key"
                },
                DisableInDailyChallenges = false,
                DivineVariant = false,
                BoonValue = 8,
                RequiredDLC = DLC.None,
                IconPath = "Mutators/Sprite/MTR_RestlessBeast.png",
                Tags = new List<string>
                {
                },
                Effects = new List<RelicEffectData>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(RelicEffectAddTempUpgrade),
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCardPool = BeyonderCardPools.CaveStoryCardPool,
                        ParamCardUpgradeData = upgradeData,
                        AdditionalTooltips = new List<AdditionalTooltipData>
                        { 
                            new AdditionalTooltipData
                            {
                                titleKey = "BeyonderCardTraitStalkerState_TooltipTitle",
                                descriptionKey = "BeyonderCardTraitStalkerState_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Keyword,
                            }
                        }
                    }.Build(),
                    new RelicEffectDataBuilder
                    { 
                        RelicEffectClassType = typeof(RelicEffectAddCardsStartOfRun),
                        ParamBool = true,
                        ParamCardPool = BeyonderCardPools.CaveStoryCardPool,
                    }.Build(),
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof (CustomRelicEffectGiveGoldAtStartOfRun),
                        ParamInt = 25,
                    }.Build(),
                }                
            }.BuildAndRegister();

            if (ProviderManager.SaveManager != null)
            {
                StoryEventData gambleEvent = ProviderManager.SaveManager.GetAllGameData().FindStoryEventData("eb406e98-2959-4fd7-b994-d75fedcb7247");
                AccessTools.Field(typeof(StoryEventData), "excludedMutator").SetValue(gambleEvent, mutatorData);
            }
            else
            {
                Beyonder.Log("Failed to modify Gamble Event to exclude mutator: Restless Beast.");
            }
        }
    }
}