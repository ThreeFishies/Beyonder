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
    class Phleghmbuyoancy
    {
        public static readonly string ID = "Beyonder_Phleghmbuyoancy_" + Beyonder.GUID;
        public static CardData Card;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_Phleghmbuyoancy_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_Phleghmbuyoancy_Description_Key",
                Cost = 2,
                Rarity = CollectableRarity.Uncommon,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                TargetsRoom = true,
                Targetless = false,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/Phleghmbuyoancy.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_Phleghmbuyoancy_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                UnlockLevel = 2,

                TraitBuilders = new List<CardTraitDataBuilder> 
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = "CardTraitExhaustState",
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
                            },
                            new AdditionalTooltipData
                            {
                                titleKey = "Trigger_Beyonder_OnAnxiety_CardText",
                                descriptionKey = "Trigger_Beyonder_OnAnxiety_TooltipText",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = true,
                                trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                                style = TooltipDesigner.TooltipDesignType.Trigger,
                            }
                        },

                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "PhleghmbuyoancyMutationUpgrade",
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
                                    DescriptionKey = "Beyonder_Spell_Phleghmbuyoancy_Hysteria_Key",
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        {
                                            EffectStateName = typeof(CustomCardEffectBumpPreviewConditional).AssemblyQualifiedName,
                                            TargetMode = TargetMode.Self,
                                            ParamInt = 1,
                                        }
                                    }
                                }.Build(),
                                new CharacterTriggerDataBuilder
                                {
                                    Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                                    DescriptionKey = "Beyonder_Spell_Phleghmbuyoancy_Anxiety_Key",
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        {
                                            EffectStateName = typeof(CustomCardEffectBumpPreviewConditional).AssemblyQualifiedName,
                                            TargetMode = TargetMode.Self,
                                            ParamInt = -1
                                        }
                                    }
                                }.Build(),
                            }
                        }.Build()
                    },
                }
            }.BuildAndRegister();
        }
    }
}
