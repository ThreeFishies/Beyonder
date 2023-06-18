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
    public static class MentalDisorder
    {
        public static readonly string ID = "Beyonder_MentalDisorder_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/MentalDisorder.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Uncommon,
                NameKey = "Beyonder_Spell_MentalDisorder_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_MentalDisorder_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.ConsumeRemainingEnergy,
                Cost = 0,
                TargetsRoom = false,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_MentalDisorder_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(CustomCardTraitScalingAddCardsAndDropThem).AssemblyQualifiedName,
                        ParamTrackedValue = CardStatistics.TrackedValueType.PlayedCost,
                        ParamInt = 1,
                        ParamFloat = 1.0f,
                        ParamTeamType = Team.Type.None,
                        ParamUseScalingParams = true,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                    },
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = "CardTraitScalingAddEnergy",
                        ParamTrackedValue = CardStatistics.TrackedValueType.PlayedCost,
                        ParamCardType = CardStatistics.CardTypeTarget.Any,
                        ParamInt = 1,
                        ParamFloat = 1.0f,
                        ParamTeamType = Team.Type.None,
                        ParamUseScalingParams = true,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisBattle
                    },
                    //new CardTraitDataBuilder
                    //{
                    //    TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                    //    ParamInt = 1,
                    //}
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectDraw",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.None,
                        ParamInt = 0,
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectGainEnergy",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.None,
                        ParamInt = 0,
                        ParamMultiplier = 1.0f
                    }
                }

            }.BuildAndRegister();

            return Card;
        }
    }
}