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
    public static class EntropicStorm
    {
        public static readonly string ID = "EntropicStorm_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/EntropicStorm.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Rare,
                NameKey = "Beyonder_Spell_EntropicStorm_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_EntropicStorm_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 3,
                TargetsRoom = true,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_EntropicStorm_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        ParamInt = 1
                    },
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = "CardTraitIgnoreArmor"
                    },
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = "CardTraitScalingAddDamage",
                        ParamInt = 10,
                        ParamFloat = 1.0f,
                        ParamTrackedValue = Beyonder.TrackCardsByEntropic.GetEnum(),
                        ParamCardType = CardStatistics.CardTypeTarget.Spell,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamUseScalingParams = true,
                        ParamTeamType = Team.Type.Heroes,
                        ParamSubtype = "SubtypesData_None",
                    },
                    //In this case, the order of traits is important. Entropic needs to be after CardTraitScalingAddDamage or it won't have any damage to multiply.
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName,
                    }
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectDiscardHandByEntropic).AssemblyQualifiedName,
                        TargetCardType = CardType.Invalid,
                        ParamInt = (int)CardEffectDiscardHand.DiscardMode.Consume,
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectDamage",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ParamInt = 0,
                        //AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.Inferno).GetEffects()[0].GetAppliedVFX(),
                        AppliedVFX = CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.SeraphtheChaste).GetBossRoomSpellCastVfx(),
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }
}