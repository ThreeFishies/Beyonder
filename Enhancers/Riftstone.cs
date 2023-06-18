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

namespace Void.Enhancers
{
    class Riftstone
    {
        public static readonly string EnhancerID = Beyonder.GUID + "_Riftstone";
        public static EnhancerData Enhancer = ScriptableObject.CreateInstance<EnhancerData>();

        public static EnhancerData BuildAndRegister()
        {
            Enhancer = new EnhancerDataBuilder
            {
                ID = EnhancerID,
                AssetPath = "ClanAssets/Riftstone.png",
                ClanID = BeyonderClan.ID,
                LinkedClass = Beyonder.BeyonderClanData,
                NameKey = "Beyonder_Enhancer_Riftstone_Name_Key",
                DescriptionKey = "Beyonder_Enhancer_Riftstone_Description_Key",
                EnhancerPoolIDs = { VanillaEnhancerPoolIDs.UnitUpgradePoolCommon }, //Adds this to the pool irregardless of clan. Needs a fix.
                Rarity = CollectableRarity.Common,
                CardType = CardType.Monster,

                Upgrade = new CardUpgradeDataBuilder
                {
                    UpgradeTitleKey = "Beyonder_Enhancer_Riftstone_Name_Key",
                    UpgradeDescriptionKey = "Beyonder_Enhancer_Riftstone_Description_Key",
                    UpgradeIconPath = "ClanAssets/Riftstone.png",
                    BonusHP = 0,
                    BonusDamage = 0,
                    HideUpgradeIconOnCard = false,

                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                    {
                        new CardTraitDataBuilder
                        { 
                            TraitStateName = typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName,
                        }
                    },
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(GameData), "id").SetValue(Enhancer, EnhancerID);

            //If we can add a lore key, why not?
            List<string> LoreKeys = new List<string> { "Beyonder_Enhancer_Riftstone_Lore_Key" };

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipKeys").SetValue((RelicData)Enhancer, LoreKeys);

            //AdditionalTooltipData[] additionalTooltips = new AdditionalTooltipData[]
            //{
            //    new AdditionalTooltipData
            //        titleKey = "BeyonderCardTraitStalkerState_TooltipTitle",
            //        descriptionKey = "BeyonderCardTraitStalkerState_TooltipText",
            //        isStatusTooltip = false,
            //        statusId = "",
            //        isTipTooltip = false,
            //        isTriggerTooltip = false,
            //        trigger = CharacterTriggerData.Trigger.OnDeath,
            //        style = TooltipDesigner.TooltipDesignType.Keyword
            //    }
            //};

            //AccessTools.Field(typeof(RelicEffectData), "additionalTooltips").SetValue(Enhancer.GetEffects()[0], additionalTooltips);

            return Enhancer;
        }
    }
}