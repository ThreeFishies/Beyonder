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
    public static class Seizure
    {
        public static readonly string ID = "Beyonder_Seizure_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/Seizure.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Common,
                NameKey = "Beyonder_Spell_Seizure_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_Seizure_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 1,
                TargetsRoom = true,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_Seizure_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitScalingAddDamage",
                        ParamTrackedValue = Beyonder.ScalingByHysteria.GetEnum(),
                        ParamInt = 30,
                        ParamFloat = 1.0f,
                        ParamTeamType = Team.Type.None,
                        ParamUseScalingParams = true,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamStatusEffects = new StatusEffectStackData[] { },
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
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
                        EffectStateName = "CardEffectDamage",
                        TargetMode = TargetMode.FrontInRoom,
                        TargetTeamType = Team.Type.Heroes,
                        ParamInt = 0,

                        AdditionalTooltips = new AdditionalTooltipData[]
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "",
                                descriptionKey = "TipTooltip_RoomTargetSpell",
                                style = TooltipDesigner.TooltipDesignType.Default,
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = true,
                            }
                        },
                        AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.CryptBuilder).GetEffects()[0].GetAppliedVFX(),
                    }
                }

            }.BuildAndRegister();

            return Card;
        }
    }
}