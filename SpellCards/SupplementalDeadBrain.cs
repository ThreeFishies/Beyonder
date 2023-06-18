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
using Void.Triggers;
using Void.Status;

namespace Void.Spells
{
    class SupplementalDeadBrain
    {
        public static readonly string ID = "Beyonder_SupplementalDeadBrain_" + Beyonder.GUID;
        public static CardData Card;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_SupplementalDeadBrain_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_SupplementalDeadBrain_Description_Key",
                Cost = 2,
                Rarity = CollectableRarity.Uncommon,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                TargetsRoom = true,
                Targetless = false,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/SupplementalDeadBrain.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_SupplementalDeadBrain_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                UnlockLevel = 0,

                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitExhaustState",
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1
                    }
                },

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectAddStatusEffect",
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = true,

                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectMutated.statusId,
                                count = 1
                            }
                        }
                    },
                    new CardEffectDataBuilder
                    { 
                        EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = true,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        { 
                            UpgradeTitleKey = "SupplementalDeadBrainPersistentStatusEffects",
                            BonusDamage = 0,
                            BonusHeal = 0,
                            StatusEffectUpgrades = new List<StatusEffectStackData> 
                            { 
                                new StatusEffectStackData
                                { 
                                    statusId = StatusEffectMutated.statusId,
                                    count = 1
                                },
                                new StatusEffectStackData
                                {
                                    statusId = StatusEffectSoundless.statusId,
                                    count = 1
                                },
                                new StatusEffectStackData
                                {
                                    statusId = StatusEffectFormless.statusId,
                                    count = 1
                                },
                                new StatusEffectStackData
                                {
                                    statusId = VanillaStatusEffectIDs.Silenced,
                                    count = 1
                                },
                            }
                        }.Build(),
                    }
                }
            }.BuildAndRegister();
        }
    }
}
