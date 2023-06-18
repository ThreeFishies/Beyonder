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
using Void.CardPools;
using Trainworks.Managers;
using Malee;

namespace Void.Enhancers
{
    class Sanitystone
    {
        public static readonly string EnhancerID = Beyonder.GUID + "_Sanitystone";
        public static EnhancerData Enhancer = ScriptableObject.CreateInstance<EnhancerData>();

        public static EnhancerData BuildAndRegister()
        {
            Enhancer = new EnhancerDataBuilder
            {
                ID = EnhancerID,
                AssetPath = "ClanAssets/Sanitystone.png",
                ClanID = BeyonderClan.ID,
                LinkedClass = Beyonder.BeyonderClanData,
                NameKey = "Beyonder_Enhancer_Sanitystone_Name_Key",
                DescriptionKey = "Beyonder_Enhancer_Sanitystone_Description_Key",
                EnhancerPoolIDs = { VanillaEnhancerPoolIDs.SpellUpgradePool }, //Adds this to the pool irregardless of clan. Needs a fix.
                Rarity = CollectableRarity.Uncommon,
                CardType = CardType.Spell,

                Upgrade = new CardUpgradeDataBuilder
                {
                    UpgradeTitleKey = "Beyonder_Enhancer_Sanitystone_Name_Key",
                    UpgradeDescriptionKey = "Beyonder_Enhancer_Sanitystone_Description_Key",
                    UpgradeIconPath = "ClanAssets/Sanitystone.png",
                    HideUpgradeIconOnCard = false,
                    BonusDamage = 0,
                    BonusHeal = 0,
                    CostReduction = 0,
                    XCostReduction = 0,

                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                    {
                        new CardTraitDataBuilder
                        {
                            TraitStateName = typeof(BeyonderCardTraitTherapeutic).AssemblyQualifiedName,
                        }
                    },

                    RemoveTraitUpgrades = new List<string>
                    {
                        typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName
                    },

                    FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                    {
                        new CardUpgradeMaskDataBuilder
                        {
                            ExcludedCardTraitsOperator = CardUpgradeMaskDataBuilder.CompareOperator.Or,
                            ExcludedCardTraits = new List<string>
                            {
                                "CardTraitUnplayable"
                            },
                            DisallowedCardPools = new List<CardPool> 
                            {
                                BeyonderCardPools.UnTherapeutic 
                            }
                        }
                    }
                },

            }.BuildAndRegister();

            AccessTools.Field(typeof(GameData), "id").SetValue(Enhancer, EnhancerID);

            //If we can add a lore key, why not?
            List<string> LoreKeys = new List<string> { "Beyonder_Enhancer_Sanitystone_Lore_Key" };

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
                    style = TooltipDesigner.TooltipDesignType.Keyword,
                    isStatusTooltip = false,
                    statusId = "",
                    isTipTooltip = false,
                    isTriggerTooltip = false,
                    trigger = CharacterTriggerData.Trigger.OnDeath,
                    titleKey = "BeyonderCardTraitTherapeutic_CardText",
                    descriptionKey = "BeyonderCardTraitTherapeutic_TooltipText",
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

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipKeys").SetValue((RelicData)Enhancer, LoreKeys);
            AccessTools.Field(typeof(RelicEffectData), "additionalTooltips").SetValue(Enhancer.GetEffects()[0], additionalTooltips);

            return Enhancer;
        }
    }
}