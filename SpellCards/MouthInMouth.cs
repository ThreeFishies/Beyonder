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
    class MouthInMouth
    {
        public static readonly string ID = "Beyonder_MouthInMouth_" + Beyonder.GUID;
        public static CardData Card;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_MouthInMouth_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_MouthInMouth_Description_Key",
                Cost = 2,
                Rarity = CollectableRarity.Common,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                TargetsRoom = true,
                Targetless = false,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/MouthInMouth.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_MouthInMouth_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                UnlockLevel = 0,

                TraitBuilders = new List<CardTraitDataBuilder> 
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = "CardTraitExhaustState",
                    },
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        ParamInt = 1,
                    }
                },

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectAddStatusEffect",
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = true,

                        ParamStatusEffects = new StatusEffectStackData[]
                        { 
                            new StatusEffectStackData
                            { 
                                statusId = StatusEffectMutated.statusId,
                                count = 1
                            }
                        }
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectAddTempCardUpgradeToUnits).AssemblyQualifiedName,
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = true,

                        AdditionalTooltips = new AdditionalTooltipData[]
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "Trigger_Beyonder_OnHysteria_CardText",
                                descriptionKey = "Trigger_Beyonder_OnHysteria_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = true,
                                trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                                style = TooltipDesigner.TooltipDesignType.Trigger,
                            }
                        },

                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "MouthInMouthMutationUpgrade",
                            StatusEffectUpgrades = new List<StatusEffectStackData>
                            {
                                new StatusEffectStackData
                                {
                                    statusId = StatusEffectMutated.statusId,
                                    count = 1
                                }
                            },

                            TriggerUpgrades = new List<CharacterTriggerData>
                            {
                                new CharacterTriggerDataBuilder
                                {
                                    Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                                    DescriptionKey = "Beyonder_Spell_MouthInMouth_Hysteria_Key",
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        {
                                            EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                            TargetMode = TargetMode.Self,
                                            ParamCardUpgradeData = new CardUpgradeDataBuilder
                                            { 
                                                UpgradeTitleKey = "MouthInMouthHysteriaEffect",
                                                BonusDamage = 6,
                                                BonusHP = -1,
                                                SourceSynthesisUnit = new CharacterDataBuilder { CharacterID = "DummyNULL" }.Build(),
                                            }.Build()
                                        }
                                    }
                                }.Build()
                            }
                        }.Build()
                    },
                }
            }.BuildAndRegister();
        }
    }
}
