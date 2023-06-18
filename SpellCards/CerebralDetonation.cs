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
using Void.Mania;
using Void.Status;
using Void.Triggers;
using HarmonyLib;

namespace Void.Spells
{
    public static class CerebralDetonation
    {
        public static readonly string ID = "CerebralDetonation_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/CerebralDetonation.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Rare,
                NameKey = "Beyonder_Spell_CerebralDetonation_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_CerebralDetonation_Description_Key",
                UnlockLevel = 1, 
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 4,
                TargetsRoom = true,
                Targetless = false,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_CerebralDetonation_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    //new CardTraitDataBuilder
                    //{
                    //    TraitStateName = "CardTraitExhaustState",
                    //},
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitScalingAddStatusEffect",
                        ParamTrackedValue = Beyonder.trackSacrificedHP.GetEnum(),
                        ParamInt = 1,
                        ParamFloat = 1.0f,
                        ParamTeamType = Team.Type.None,
                        ParamUseScalingParams = true,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectJitters.statusId,
                                count = 1
                            }
                        },
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitScalingAddStatusEffect",
                        ParamTrackedValue = Beyonder.trackSacrificedSize.GetEnum(),
                        ParamInt = 1,
                        ParamFloat = 1.0f,
                        ParamTeamType = Team.Type.None,
                        ParamUseScalingParams = true,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectChronic.statusId,
                                count = 1
                            }
                        },
                    },
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectSacrificeAssertTarget).AssemblyQualifiedName,
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Monsters,
                        ParamSubtype = "SubtypesData_None",
                        ParamBool = false,
                        ShouldTest = true
                    },
                    //new CardEffectDataBuilder
                    //{
                    //    EffectStateName = typeof(CustomCardEffectMultiplyAllNegativeStatusEffects).AssemblyQualifiedName,
                    //    TargetMode = TargetMode.Room,
                    //    TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                    //    ParamInt = 2,
                    //    ShouldTest = false
                    //}
                    new CardEffectDataBuilder
                    { 
                        EffectStateName = "CardEffectAddStatusEffect",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Monsters,
                        ParamInt = 0,
                        ParamStatusEffects = new StatusEffectStackData[]
                        { 
                            new StatusEffectStackData
                            { 
                                statusId = StatusEffectChronic.statusId,
                                count = 0
                            }
                        }
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectAddStatusEffect",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                        ParamInt = 0,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectJitters.statusId,
                                count = 0
                            }
                        }
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }
}