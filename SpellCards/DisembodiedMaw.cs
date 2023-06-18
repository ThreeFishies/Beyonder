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
using HarmonyLib;

namespace Void.Spells
{
    class DisembodiedMaw
    {
        public static readonly string ID = "Beyonder_DisembodiedMaw_" + Beyonder.GUID;
        public static CardData Card;
        public static bool VfxFixed = false;

        public static void BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                NameKey = "Beyonder_Spell_DisembodiedMaw_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_DisembodiedMaw_Description_Key",
                Cost = 2,
                Rarity = CollectableRarity.Uncommon,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                TargetsRoom = true,
                Targetless = false,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                AssetPath = "SpellCards/Assets/DisembodiedMaw.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                CardLoreTooltipKeys = new List<string>
                {
                    "Beyonder_Spell_DisembodiedMaw_Lore_Key"
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
                        TraitStateName = "CardTraitIgnoreArmor",
                    },
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
                        EffectStateName = "CardEffectDamage",
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Heroes | Team.Type.Monsters,
                        ShouldTest = true,
                        ParamInt = 60,
                        //AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.MementoMori).GetEffects()[0].GetAppliedVFX()
                        //AppliedVFX = ProviderManager.CombatManager.FeedingVFX,
                    },
                }
            }.BuildAndRegister();
        }
    }

    [HarmonyPatch(typeof(CombatManager), "StartCombat")]
    public static class FixDisembodiedMawAppliedVFX 
    {
        public static void Prefix(ref CombatManager __instance)
        {
            if (!Beyonder.IsInit) { return; }
            if (DisembodiedMaw.VfxFixed) { return; }

            AccessTools.Field(typeof(CardEffectData), "appliedVFX").SetValue(DisembodiedMaw.Card.GetEffects()[1], __instance.FeedingVFX);
            AccessTools.Field(typeof(CardEffectData), "appliedVFX").SetValue(DarkRecipe.Card.GetEffects()[0], __instance.FeedingVFX);

            DisembodiedMaw.VfxFixed = true;
        }
    }
}
