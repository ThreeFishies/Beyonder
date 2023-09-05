using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Trainworks.Builders;
using Trainworks.Constants;
using Trainworks.Enums;
using Trainworks.Managers;
using CustomEffects;
using Void.Init;
using Void.Triggers;
using Void.Status;

namespace Void.Spells
{
    class PostItNoteOfForbiddenKnowledge
    {
        public static readonly string ID = "Beyonder_PostItNoteOfForbiddenKnowledge_" + Beyonder.GUID;
        public static CardData Card;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_PostItNoteOfForbiddenKnowledge_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_PostItNoteOfForbiddenKnowledge_Description_Key",
                Cost = 0,
                Rarity = CollectableRarity.Uncommon,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                TargetsRoom = false,
                Targetless = true,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/PostItNoteOfForbiddenKnowledge.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_PostItNoteOfForbiddenKnowledge_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                UnlockLevel = 3,

                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitExhaustState",
                    },
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = "CardTraitPermafrost",
                    }
                    //new CardTraitDataBuilder
                    //{
                    //    TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                    //    ParamInt = 1
                    //}
                },

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectAddTraitToCard).AssemblyQualifiedName,
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.None,
                        ShouldTest = true,
                        ParamBool = false,
                        ParamInt = (int)CardPile.HandPile,
                        ParamStr = "ScreenDeck_Select_CustomCardEffectAddTraitToCard_PostItNoteOfForbiddenKnowledge",
                        //ParamSubtype = "BeyonderCardTraitEntropic",
                        AdditionalTooltips = new AdditionalTooltipData[] 
                        {
                            new AdditionalTooltipData
                            { 
                                titleKey = "BeyonderCardTraitEntropic_TooltipTitle",
                                descriptionKey = "BeyonderCardTraitEntropic_TooltipText_Lame",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Keyword
                            },
                            new AdditionalTooltipData
                            {
                                titleKey = string.Empty, // "BeyonderCardTraitEntropic_Tip_TooltipTitle",
                                descriptionKey = "BeyonderCardTraitEntropic_Tip_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = true,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Default
                            },
                        },

                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        { 
                            UpgradeTitleKey = "DummyUpgradeThatOnlyExistsToHarvestTraitData_Entrpoic",
                            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                            { 
                                new CardTraitDataBuilder
                                {
                                    TraitStateName = typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName
                                },
                                new CardTraitDataBuilder
                                { 
                                    TraitStateName = "CardTraitFreeze",
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardType = CardType.Spell,
                                    ExcludedCardTraitsOperator = CardUpgradeMaskDataBuilder.CompareOperator.And,
                                    ExcludedCardTraits = new List<string>
                                    {
                                        //typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName
                                    },
                                    RequiredCardTraitsOperator = CardUpgradeMaskDataBuilder.CompareOperator.And,

                                    ExcludeXCost = false,
                                    RequireXCost = false,
                                    ExcludeNonAttackingMonsters = false,
                                }
                            }
                        }.Build(),
                    },
                }
            }.BuildAndRegister();
        }
    }
}
