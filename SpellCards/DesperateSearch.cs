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
    public static class DesperateSearch
    {
        public static readonly string ID = "Beyonder_DesperateSearch_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/DesperateSearch.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Uncommon,
                NameKey = "Beyonder_Spell_DesperateSearch_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_DesperateSearch_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 0,
                TargetsRoom = false,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_DesperateSearch_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1,
                    }
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectDraw",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.None,
                        ParamInt = 10,
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectDropAllButOneUnlessBoneDog).AssemblyQualifiedName,
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.None,
                        TargetCardType = CardType.Monster,
                        ParamInt = (int)CardEffectDiscardHand.DiscardMode.Discard,

                        AdditionalTooltips = new AdditionalTooltipData[]
                        {
                            new AdditionalTooltipData
                            {
                                titleKey = "CardData_nameKey-fd46bcb23f8b055b-976d27dafa269524e921685a48bae1ed-v2",
                                descriptionKey = "Bone_Dog_Reference_Key",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = false,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Default
                            }
                        }
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }
}