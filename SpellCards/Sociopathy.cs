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
    public static class Sociopathy
    {
        public static readonly string ID = "Sociopathy_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/Sociopath.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Rare,
                NameKey = "Beyonder_Spell_Sociopathy_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_Sociopathy_Description_Key",
                UnlockLevel = 1,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 1,
                TargetsRoom = true,
                Targetless = false,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_Sociopathy_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        ParamInt = 1,
                    },
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = "CardTraitScalingAddStatusEffect",
                        ParamInt = 1,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamCardType = CardStatistics.CardTypeTarget.Any,
                        ParamUseScalingParams = true,
                        ParamTeamType = Team.Type.Monsters,
                        ParamTrackedValue = Beyonder.trackAlliesSacrificed.GetEnum(),
                        ParamStatusEffects = new StatusEffectStackData[]
                        { 
                            new StatusEffectStackData
                            { 
                                statusId = StatusEffectChronic.statusId,
                                count = 1,
                            }
                        }
                    }
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    /*
                    new CardEffectDataBuilder 
                    {
                        EffectStateName = "CardEffectNULL",
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Monsters
                    },
                    new CardEffectDataBuilder
                    { 
                        EffectStateName = "CardEffectKill",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Monsters,
                        TargetModeStatusEffectsFilter = new string[] 
                        {
                            "BEYONDER_FILTER_BY_EXCLUDE_LAST_TARGET"
                        }
                    },*/
                    new CardEffectDataBuilder
                    {
                        EffectStateName = typeof(CustomCardEffectScaleDamageBySacrificeAllies).AssemblyQualifiedName,
                        TargetMode = TargetMode.DropTargetCharacter,
                        TargetTeamType = Team.Type.Monsters,
                        ParamInt = 14,
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeTitleKey = "BlankSociopathyUpgradeDataForTooltipHealthPurposes"
                        }.Build(),
                    },
                    new CardEffectDataBuilder
                    { 
                        EffectStateName = "CardEffectAddStatusEffect",
                        TargetMode = TargetMode.LastTargetedCharacters,
                        TargetTeamType = Team.Type.Monsters,
                        ParamStatusEffects = new StatusEffectStackData[]
                        {
                            new StatusEffectStackData
                            {
                                statusId = StatusEffectChronic.statusId,
                                count = 0
                            }
                        }
                    }
                }
            }.BuildAndRegister();

            return Card;
        }
    }



    [HarmonyPatch(typeof(TargetHelper), "CheckTargetFiltered")]
    public static class BeyonderFilterByExcludeLastTargetedCharacter
    {
        public static bool Prefix(CharacterState target, List<string> targetModeStatusEffectsFilter, CardEffectData.HealthFilter targetModeHealthFilter, bool targetIgnoreBosses, bool targetIgnorePyre, bool inCombat, bool ignoreDead, SubtypeData targetSubtype, ref bool __result)
        {
            if (!Beyonder.IsInit) { return true; }

            if (targetModeStatusEffectsFilter.IsNullOrEmpty())
            {
                return true;
            }
            if (targetModeStatusEffectsFilter[0] == "BEYONDER_FILTER_BY_EXCLUDE_LAST_TARGET")
            {
                //Beyonder.Log("Sociopathy Detected");

                __result = CheckTargetFiltered(target, targetModeStatusEffectsFilter, targetModeHealthFilter, targetIgnoreBosses, targetIgnorePyre, inCombat, ignoreDead, targetSubtype);

                return false;
            }

            return true;
        }

        // TargetHelper
        // Token: 0x06002D74 RID: 11636 RVA: 0x000B0F98 File Offset: 0x000AF198
        private static bool CheckTargetFiltered(CharacterState target, List<string> targetModeStatusEffectsFilter, CardEffectData.HealthFilter targetModeHealthFilter, bool targetIgnoreBosses, bool targetIgnorePyre, bool inCombat, bool ignoreDead, SubtypeData targetSubtype)
        {
            if (ignoreDead && target.IsDead)
            {
                return true;
            }
            if (CustomCardEffectScaleDamageBySacrificeAllies.LastTarget != null && CustomCardEffectScaleDamageBySacrificeAllies.LastTarget == target)
            {
                //CustomCardEffectScaleDamageBySacrificeAllies.LastTarget = null;
                return true;
            }
            List<CharacterState.StatusEffectStack> list;
            target.GetStatusEffects(out list, false);
            using (List<CharacterState.StatusEffectStack>.Enumerator enumerator2 = list.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (!enumerator2.Current.State.GetUnitIsTargetable(inCombat))
                    {
                        return true;
                    }
                }
            }
            return !TargetHelper.TargetPassesHealthFilter(target, targetModeHealthFilter) || (targetIgnoreBosses && (target.IsMiniboss() || target.IsOuterTrainBoss())) || (targetIgnorePyre && target.IsPyreHeart()) || (targetSubtype != null && !targetSubtype.IsNone && !target.GetCharacterManager().DoesCharacterPassSubtypeCheck(target, targetSubtype));
        }
    }

}