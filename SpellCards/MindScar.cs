using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Trainworks.BuildersV2;
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
    public static class MindScar 
    {
        public static readonly string ID = "Beyonder_MindScar_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister() 
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                //LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/MentalScar.png",
                CardPoolIDs = new List<string> { },
                Rarity = CollectableRarity.Common, //Note 'Starter' rarity appears to be unused.
                NameKey = "Beyonder_Spell_MindScar_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_MindScar_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 1,
                TargetsRoom = true,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_MindScar_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        //TraitStateName = "CardTraitScalingAddDamage",
                        TraitStateType = typeof(CardTraitScalingAddDamage),
                        ParamTrackedValue = Beyonder.ScalingByHysteria.GetEnum(),
                        ParamInt = 8,
                        ParamFloat = 1.0f,
                        ParamTeamType = Team.Type.None,
                        ParamUseScalingParams = true,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamStatusEffects = new List<StatusEffectStackData> { },
                    },
                    new CardTraitDataBuilder 
                    { 
                        //TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        TraitStateType = typeof(BeyonderCardTraitAfflictive),
                        ParamInt = 1,
                    },
                    new CardTraitDataBuilder
                    {
                        //TraitStateName = typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName
                        TraitStateType = typeof(BeyonderCardTraitEntropic)
                    }
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        //EffectStateName = "CardEffectDamage",
                        EffectStateType = typeof(CardEffectDamage),
                        TargetMode = TargetMode.FrontInRoom,
                        TargetTeamType = Team.Type.Heroes,
                        ParamInt = 0,
                        
                        AdditionalTooltips = new List<AdditionalTooltipData>
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
                        AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.ForeverConsumed).GetEffects()[0].GetAppliedVFX(),
                    }
                }

            }.BuildAndRegister();

            return Card;
        }
    }
}