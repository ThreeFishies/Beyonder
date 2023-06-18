using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Trainworks.Builders;
using Trainworks.Constants;
using Trainworks.Enums;
using Trainworks.Managers;
using CustomEffects;
using Equestrian.CustomEffects;
using Void.Init;
using Void.Triggers;
using Void.Status;

namespace Void.Spells
{
    class SpikeFromBeyond
    {
        public static readonly string ID = "Beyonder_SpikeFromBeyond_" + Beyonder.GUID;
        public static CardData Card;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_SpikeFromBeyond_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_SpikeFromBeyond_Description_Key",
                Cost = 0,
                Rarity = CollectableRarity.Rare,
                CardType = CardType.Spell,
                CostType = CardData.CostType.ConsumeRemainingEnergy,
                TargetsRoom = true,
                Targetless = false,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/SpikeFromBeyond.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_SpikeFromBeyond_Lore_Key"
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
                        TraitStateName = "CardTraitScalingUpgradeUnitAttack",
                        ParamTrackedValue = CardStatistics.TrackedValueType.PlayedCost,
                        ParamInt = 6,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamUseScalingParams = true,
                        ParamFloat = 1.0f
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitScalingUpgradeUnitHealth",
                        ParamTrackedValue = CardStatistics.TrackedValueType.PlayedCost,
                        ParamInt = 9,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamUseScalingParams = true,
                        ParamFloat = 1.0f
                    },
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
                        EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = true,

                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        { 
                            UpgradeTitleKey = "SpikeFromBeyondScalingEffect",
                            BonusDamage = 0,
                            BonusHP = 0,

                            StatusEffectUpgrades = new List<StatusEffectStackData>
                            { 
                                new StatusEffectStackData
                                { 
                                    statusId = StatusEffectMutated.statusId,
                                    count = 1
                                }
                            }
                        }.Build(),
                    },
                    new CardEffectDataBuilder
                    { 
                        EffectStateName = typeof(CustomCardEffectSecretStatusEffect).AssemblyQualifiedName,
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = false,
                        ParamStr = "mod.equestrian.clan.monstertrain_GenderReveal",

                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            { 
                                statusId = "male",
                                count = 1
                            }
                        }
                    }
                }
            }.BuildAndRegister();
        }
    }
}
