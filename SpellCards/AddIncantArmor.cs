/*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Trainworks.Builders;
using Trainworks.Constants;
using Trainworks.Enums;
using Trainworks.Managers;
using CustomEffects;

namespace TestSpellCards
{
    class GiveIncantArmor
    {
        public static readonly string ID = "TESTSPELLCARD_GiveIncantArmor";

        public static void BuildAndRegister()
        {
            new CardDataBuilder
            {
                CardID = ID,
                NameKey = "GiveIncantArmor_Name_Key",
                OverrideDescriptionKey = "GiveIncantArmor_Description_Key",
                Cost = 2,
                Rarity = CollectableRarity.Common,
                CardType = CardType.Spell,
                TargetsRoom = true,
                Targetless = false,
                ClanID = VanillaClanIDs.Stygian,
                AssetPath = "ClanAssets/MareALee.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "GiveIncantArmor_Lore_Key"
                },
                LinkedClass = CustomClassManager.GetClassDataByID(VanillaClanIDs.Stygian),

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectAddTempCardUpgradeToUnits).AssemblyQualifiedName,
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = true,

                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            TriggerUpgrades = new List<CharacterTriggerData>
                            {
                                new CharacterTriggerDataBuilder
                                {
                                    Trigger = CharacterTriggerData.Trigger.CardSpellPlayed,
                                    DescriptionKey = "AddIncantArmor_TriggerKey",
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        {
                                            EffectStateName = "CardEffectAddStatusEffect",
                                            TargetMode = TargetMode.Self,

                                            ParamStatusEffects = new StatusEffectStackData[]
                                            {
                                                new StatusEffectStackData
                                                {
                                                    statusId = VanillaStatusEffectIDs.Armor,
                                                    count = 2
                                                }
                                            }
                                        }
                                    }
                                }.Build()
                            }
                        }.Build()
                    },
                }
            }.BuildAndRegister();
        }
    }
}
*/