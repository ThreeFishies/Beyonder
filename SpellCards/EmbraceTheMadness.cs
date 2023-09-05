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
    public static class EmbraceTheMadness
    {
        public static readonly string ID = "EmbraceTheMadness_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/EmbraceTheMadness.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Rare,
                NameKey = "Beyonder_Spell_EmbraceTheMadness_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_EmbraceTheMadness_Description_Key",
                UnlockLevel = 9,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 0,
                TargetsRoom = false,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_EmbraceTheMadness_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitExhaustState",
                    },
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = typeof(CustomCardTraitShowManicTargets).AssemblyQualifiedName,
                    }
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectApplyManicToDeck).AssemblyQualifiedName,
                        TargetMode = TargetMode.FrontInRoom,
                        TargetTeamType = Team.Type.None,
                        TargetCardType = CardType.Invalid,
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectDrawAdditionalNextTurn",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                        ShouldTest = false,
                        ParamInt = 1
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }
}