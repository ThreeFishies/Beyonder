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
    public static class SurvivalInstinct
    {
        public static readonly string ID = "SurvivalInstinct_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/SurvivalInstinct.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Common,
                NameKey = "Beyonder_Spell_SurvivalInstinct_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_SurvivalInstinct_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 1,
                TargetsRoom = true,
                Targetless = false,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_SurvivalInstinct_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1,
                    },
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "SurvivalInstinctHealthBuff",
                            BonusHP = 10
                        }.Build(),
                    },
                    new CardEffectDataBuilder
                    { 
                        EffectStateName = "CardEffectFloorRearrange",
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ParamInt = (int)SpawnMode.BackSlot, 
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }
}