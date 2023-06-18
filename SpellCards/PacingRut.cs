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
    class PacingRut
    {
        public static readonly string ID = "Beyonder_PacingRut_" + Beyonder.GUID;
        public static CardData Card;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_PacingRut_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_PacingRut_Description_Key",
                Cost = 0,
                Rarity = CollectableRarity.Uncommon,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                TargetsRoom = false,
                Targetless = true,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/PacingRut.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_PacingRut_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                UnlockLevel = 7,

                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitRetain",
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1
                    }
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
                        ParamStr = "ScreenDeck_Select_CustomCardEffectAddTraitToCard_PacingRut",

                        AdditionalTooltips = new AdditionalTooltipData[]
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "CardTraitRetain_CardText",
                                descriptionKey = "CardTraitRetain_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Keyword
                            },
                        },

                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "DummyUpgradeThatOnlyExistsToHarvestTraitData_Holdover",
                            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                            {
                                new CardTraitDataBuilder
                                {
                                    TraitStateName = "CardTraitRetain"
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
                                        "CardTraitRetain"
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
