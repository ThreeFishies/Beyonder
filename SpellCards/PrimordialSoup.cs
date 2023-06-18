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
    class PrimordialSoup
    {
        public static readonly string ID = "Beyonder_PrimordialSoup_" + Beyonder.GUID;
        public static CardData Card;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_PrimordialSoup_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_PrimordialSoup_Description_Key",
                Cost = 3,
                Rarity = CollectableRarity.Rare,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                TargetsRoom = true,
                Targetless = false,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/PrimordialSoup.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_PrimordialSoup_Lore_Key"
                },
                LinkedClass = Beyonder.BeyonderClanData,
                UnlockLevel = 0,

                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitExhaustState",
                        //TraitStateName = typeof(CardTraitExhaustState).AssemblyQualifiedName,
                        //TraitStateType = typeof(CardTraitExhaustState),
                    }
                },

                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Monsters,
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
                                    statusId = StatusEffectChronic.statusId,
                                    count = 10
                                },
                            }
                        }.Build(),
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectDrawAdditionalNextTurn",
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Monsters,
                        ShouldTest = false,
                        ParamInt = 3
                    }
                }
            }.BuildAndRegister();
        }
    }
}
