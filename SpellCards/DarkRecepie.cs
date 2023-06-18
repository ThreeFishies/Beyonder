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
    public static class DarkRecipe
    {
        public static readonly string ID = "DarkRecipe_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/DarkRecipe.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Uncommon,
                NameKey = "Beyonder_Spell_DarkRecipe_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_DarkRecipe_Description_Key",
                UnlockLevel = 8,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 1,
                TargetsRoom = true,
                Targetless = false,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_DarkRecipe_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitExhaustState",
                    },
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectSacrifice",
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Monsters,
                        ParamSubtype = "SubtypesData_None",
                        ParamBool = false,
                        ShouldTest = true
                    },
                    new CardEffectDataBuilder
                    { 
                        EffectStateName = typeof(CustomCardEffectAdjustHandSize).AssemblyQualifiedName,
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Monsters,
                        ParamInt = 1
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }
}