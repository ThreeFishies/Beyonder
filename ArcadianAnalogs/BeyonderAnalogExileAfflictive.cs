using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Void.Init;
using Trainworks.Builders;
using Trainworks.Constants;
using Trainworks.Enums;
using CustomEffects;
using HarmonyLib;
using Void.Status;

namespace Void.Arcadian
{
    //Beyonder base card is Mind Sear, so the Arcadian Analog needs to be Compulsive.
    class BeyonderAnalogExileAfflictive
    {
        public static readonly string ID = Beyonder.GUID + "_BeyonderAnalogExileAfflictive";

        public static void BuildAndRegister()
        {
            CardData cardData = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Arcadian_Spell_BeyonderAnalogExileAfflictive_Name_Key",
                OverrideDescriptionKey = "Arcadian_Spell_BeyonderAnalogExileAfflictive_Description_Key",
                Cost = 1,
                Rarity = CollectableRarity.Starter,
                CardType = CardType.Spell,
                TargetsRoom = true,
                Targetless = false,
                ClanID = ArcadianCompatibility.ArcadianClan.GetID(),
                AssetPath = "ArcadianAnalogs/Assets/BeyonderAnalogExileAfflictive.png",
                CardPoolIDs = new List<string> { },
                CardLoreTooltipKeys = new List<string>
                {
                    "Arcadian_Spell_BeyonderAnalogExileAfflictive_Lore_Key"
                },
                LinkedClass = ArcadianCompatibility.ArcadianClan,
                IgnoreWhenCountingMastery = true,
                LinkedMasteryCard = ArcadianCompatibility.Analog,

                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        ParamInt = 1
                    }
                },

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectAddStatusEffect",
                        //EffectStateType = VanillaCardEffectTypes.CardEffectAddStatusEffect,
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectPanic.statusId,
                                count = 1
                            },
                        }
                    }
                }
            }.BuildAndRegister();

            //AccessTools.Field(typeof(CardData), "ignoreWhenCountingMastery").SetValue(cardData, Cost);
        }
    }
}