using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Void.Init;
using Trainworks.Builders;
using Trainworks.Constants;
using Trainworks.Enums;
using CustomEffects;
using UnityEngine;
using HarmonyLib;
using Void.Clan;
using Void.Mania;
using Trainworks.Managers;
using Malee;

namespace Void.Enhancers
{
    class Voidstone
    {
        public static readonly string EnhancerID = Beyonder.GUID + "_Voidstone";
        public static EnhancerData Enhancer = ScriptableObject.CreateInstance<EnhancerData>();

        public static EnhancerData BuildAndRegister()
        {
            //List<CardUpgradeMaskData> filters = new List<CardUpgradeMaskData>();
            EnhancerData emberstone = ProviderManager.SaveManager.GetAllGameData().FindEnhancerData("7301ebb3-4d99-46b9-b4bd-aeabf530fd0a");
            //filters.Add(powerstone.GetEffects()[0].GetParamCardUpgradeData().GetFilters()[0]);
            //filters.Add(powerstone.GetEffects()[0].GetParamCardUpgradeData().GetFilters()[1]);
            /*
            filters.Add(new CardUpgradeMaskDataBuilder
            {
                ExcludedCardTraitsOperator = CardUpgradeMaskDataBuilder.CompareOperator.Or,
                ExcludedCardTraits = new List<string>
                {
                    typeof(BeyonderCardTraitAffictive).AssemblyQualifiedName,
                    typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                }
            }.Build());
            */

            Enhancer = new EnhancerDataBuilder
            {
                ID = EnhancerID,
                AssetPath = "ClanAssets/Voidstone.png",
                ClanID = BeyonderClan.ID,
                LinkedClass = Beyonder.BeyonderClanData,
                NameKey = "Beyonder_Enhancer_Voidstone_Name_Key",
                DescriptionKey = "Beyonder_Enhancer_Voidstone_Description_Key",
                EnhancerPoolIDs = { VanillaEnhancerPoolIDs.SpellUpgradePoolCostReduction }, //Adds this to the pool irregardless of clan. Needs a fix.
                Rarity = CollectableRarity.Common,
                CardType = CardType.Spell,

                Upgrade = new CardUpgradeDataBuilder
                {
                    UpgradeTitleKey = "Beyonder_Enhancer_Voidstone_Name_Key",
                    UpgradeDescriptionKey = "Beyonder_Enhancer_Voidstone_Description_Key",
                    UpgradeIconPath = "ClanAssets/Voidstone.png",
                    HideUpgradeIconOnCard = false,
                    BonusDamage = 0,
                    BonusHeal = 0,
                    CostReduction = 1,
                    XCostReduction = 1,

                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                    {
                        new CardTraitDataBuilder
                        {
                            TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                            ParamInt = 1
                        }
                    },

                    RemoveTraitUpgrades = new List<string>
                    {
                        typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                    },

                    //No need to exclude 0 cost cards.
                    //Filters = emberstone.GetEffects()[0].GetParamCardUpgradeData().GetFilters(),

                    FiltersBuilders = new List<CardUpgradeMaskDataBuilder> 
                    { 
                        new CardUpgradeMaskDataBuilder
                        {
                            UpgradeDisabledReason = CardState.UpgradeDisabledReason.NotEligible,
                            ExcludedCardTraitsOperator = CardUpgradeMaskDataBuilder.CompareOperator.Or,
                            ExcludedCardTraits = new List<string>
                            {
                                "CardTraitUnplayable",
                                typeof(BeyonderCardTraitTherapeutic).AssemblyQualifiedName
                            }
                        }
                    }
                },

            }.BuildAndRegister();

            AccessTools.Field(typeof(GameData), "id").SetValue(Enhancer, EnhancerID);

            //Modify the cost reduction pool to add an extra copy of the plain -1 upgrade to the pool.
            //This will make the custom enhancer appear at a 1/3 chance, to match the same chance of the spell power upgrade.
            MerchantData spellFrank = ProviderManager.SaveManager.GetAllGameData().FindMapNodeData("9a70610f-8900-4900-b96d-4f88faa0f105") as MerchantData; //Spell upgrade merchant
            EnhancerPoolRewardData spellCostReductionPool = spellFrank.GetReward(0).RewardData as EnhancerPoolRewardData;
            EnhancerPool relicPool = AccessTools.Field(typeof(EnhancerPoolRewardData), "relicPool").GetValue(spellCostReductionPool) as EnhancerPool;
            ReorderableArray<EnhancerData> data = AccessTools.Field(typeof(EnhancerPool), "relicDataList").GetValue(relicPool) as ReorderableArray<EnhancerData>;

            //if (data.Count == 1) 
            //{
                data.Add(data[0]);
                Beyonder.Log($"Added extra copy of -1 cost upgrade to {relicPool.name}.");
            //}

            //If we can add a lore key, why not?
            List<string> LoreKeys = new List<string> { "Beyonder_Enhancer_Voidstone_Lore_Key" };

            //Because the Trainworks EnhancerDataBuilder is bad.
            AdditionalTooltipData[] additionalTooltips = new AdditionalTooltipData[]
            {
                new AdditionalTooltipData
                {
                    style = TooltipDesigner.TooltipDesignType.Keyword,
                    isStatusTooltip = false,
                    statusId = "",
                    isTipTooltip = false,
                    isTriggerTooltip = false,
                    trigger = CharacterTriggerData.Trigger.OnDeath,
                    titleKey = "BeyonderCardTraitAfflictive_CardText",
                    descriptionKey = "BeyonderCardTraitAfflictive_TooltipText",
                },
                new AdditionalTooltipData
                {
                    style = TooltipDesigner.TooltipDesignType.Keyword,
                    isStatusTooltip = false,
                    statusId = "",
                    isTipTooltip = false,
                    isTriggerTooltip = false,
                    trigger = CharacterTriggerData.Trigger.OnDeath,
                    titleKey = "BeyonderCardTraitCompulsive_CardText",
                    descriptionKey = "BeyonderCardTraitCompulsive_TooltipText",
                },
                new AdditionalTooltipData
                {
                    style = TooltipDesigner.TooltipDesignType.Default,
                    isStatusTooltip = false,
                    statusId = "",
                    isTipTooltip = true,
                    isTriggerTooltip = false,
                    trigger = CharacterTriggerData.Trigger.OnDeath,
                    titleKey = "Beyonder_Enhancer_Tip_Title",
                    descriptionKey = "Beyonder_Enhancer_Tip_Text",
                }
            };
            additionalTooltips.AddRangeToArray<AdditionalTooltipData>(emberstone.GetEffects()[0].GetAdditionalTooltips());

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipKeys").SetValue((RelicData)Enhancer,LoreKeys);
            AccessTools.Field(typeof(RelicEffectData), "additionalTooltips").SetValue(Enhancer.GetEffects()[0], additionalTooltips);

            return Enhancer;
        }
    }
}