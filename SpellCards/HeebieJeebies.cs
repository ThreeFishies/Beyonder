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
    public static class HeebieJeebies
    {
        public static readonly string ID = "HeebieJeebies_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/HeebieJeebies.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Uncommon,
                NameKey = "Beyonder_Spell_HeebieJeebies_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_HeebieJeebies_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 0,
                TargetsRoom = true,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_HeebieJeebies_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        ParamInt = 1,
                    },
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
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Heroes,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectJitters.statusId,
                                count = 6
                            }
                        },
                        AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.MementoMori).GetEffects()[0].GetAppliedVFX(),
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }
}