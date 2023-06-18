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
    public static class OcularInfection 
    {
        public static readonly string ID = "Beyonder_OcularInfection_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister() 
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/OcularInfection.png",
                CardPoolIDs = new List<string> { },
                Rarity = CollectableRarity.Common, //Note 'Starter' rarity appears to be unused.
                NameKey = "Beyonder_Spell_OcularInfection_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_OcularInfection_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 1,
                TargetsRoom = true,
                Targetless = false,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_OcularInfection_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitScalingAddStatusEffect",
                        ParamTrackedValue = Beyonder.ScalingByAnxiety.GetEnum(),
                        ParamInt = 4,
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
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ParamInt = 0,
                        ParamStatusEffects = new StatusEffectStackData[]
                        { 
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectJitters.statusId,
                                count = 0
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