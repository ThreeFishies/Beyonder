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
    public static class EyeballsForDays
    {
        public static readonly string ID = "Beyonder_EyeballsForDays_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/EyeballsForDays.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Common,
                NameKey = "Beyonder_Spell_EyeballsForDays_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_EyeballsForDays_Description_Key",
                UnlockLevel = 5,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 0,
                TargetsRoom = true,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_EyeballsForDays_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitScalingAddStatusEffect",
                        ParamTrackedValue = Beyonder.ScalingByAnxiety.GetEnum(),
                        ParamInt = 2,
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
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1,
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName
                    }
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
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
                        },

                        AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.ExcavationEruption).GetEffects()[0].GetAppliedVFX(),
                    }
                }

            }.BuildAndRegister();

            return Card;
        }
    }
}