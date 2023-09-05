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

namespace Void.Enhancers
{
    class Veilstone
    {
        public static readonly string EnhancerID = Beyonder.GUID + "_Veilstone";
        public static EnhancerData Enhancer = ScriptableObject.CreateInstance<EnhancerData>();

        public static EnhancerData BuildAndRegister()
        {
            //List<CardUpgradeMaskData> filters = new List<CardUpgradeMaskData>();
            EnhancerData powerstone = ProviderManager.SaveManager.GetAllGameData().FindEnhancerData("015f4d9d-3a87-4053-8e30-45a80fdf78ee");
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
                AssetPath = "ClanAssets/Veilstone.png",
                ClanID = BeyonderClan.ID,
                LinkedClass = Beyonder.BeyonderClanData,
                NameKey = "Beyonder_Enhancer_Veilstone_Name_Key",
                DescriptionKey = "Beyonder_Enhancer_Veilstone_Description_Key",
                EnhancerPoolIDs = { VanillaEnhancerPoolIDs.SpellUpgradePoolCommon }, //Adds this to the pool irregardless of clan. Needs a fix.
                Rarity = CollectableRarity.Common,
                CardType = CardType.Spell,

                Upgrade = new CardUpgradeDataBuilder
                {
                    UpgradeTitleKey = "Beyonder_Enhancer_Veilstone_Name_Key",
                    UpgradeDescriptionKey = "Beyonder_Enhancer_Veilstone_Description_Key",
                    UpgradeIconPath = "ClanAssets/Veilstone.png",
                    HideUpgradeIconOnCard = false,
                    BonusDamage = 10,
                    BonusHeal = 10,

                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                    {
                        new CardTraitDataBuilder
                        {
                            TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                            ParamInt = 1
                        }
                    },

                    RemoveTraitUpgrades = new List<string> 
                    {
                        typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                    },

                    //use existing magic power filters
                    //Filters = powerstone.GetEffects()[0].GetParamCardUpgradeData().GetFilters(),

                    //Testing playable-only filter
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

            //If we can add a lore key, why not?
            List<string> LoreKeys = new List<string> { "Beyonder_Enhancer_Veilstone_Lore_Key" };

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
                    titleKey = string.Empty, //"Beyonder_Enhancer_Tip_Title",
                    descriptionKey = "Beyonder_Enhancer_Tip_Text",
                }
            };
            additionalTooltips.AddRangeToArray<AdditionalTooltipData>(powerstone.GetEffects()[0].GetAdditionalTooltips());

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipKeys").SetValue((RelicData)Enhancer, LoreKeys);
            AccessTools.Field(typeof(RelicEffectData), "additionalTooltips").SetValue(Enhancer.GetEffects()[0], additionalTooltips);

            return Enhancer;
        }
    }
}