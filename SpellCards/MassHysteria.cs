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
    public static class MassHysteria
    {
        public static readonly string ID = "Beyonder_MassHysteria_" + Beyonder.GUID;
        public static CardData Card;

        public static CardData BuildAndRegister()
        {
            Card = new CardDataBuilder
            {
                CardID = ID,
                ClanID = Beyonder.BeyonderClanData.GetID(),
                LinkedClass = Beyonder.BeyonderClanData,
                AssetPath = "SpellCards/Assets/MassHysteria.png",
                CardPoolIDs = new List<string> { VanillaCardPoolIDs.MegaPool },
                Rarity = CollectableRarity.Common,
                NameKey = "Beyonder_Spell_MassHysteria_Name_Key",
                OverrideDescriptionKey = "Beyonder_Spell_MassHysteria_Description_Key",
                UnlockLevel = 4,
                CardType = CardType.Spell,
                CostType = CardData.CostType.Default,
                Cost = 1,
                TargetsRoom = true,
                Targetless = true,
                CardLoreTooltipKeys = new List<string> { "Beyonder_Spell_MassHysteria_Lore_Key" },
                TraitBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = "CardTraitScalingAddDamage",
                        ParamTrackedValue = Beyonder.ScalingByHysteria.GetEnum(),
                        ParamInt = 3,
                        ParamFloat = 1.0f,
                        ParamTeamType = Team.Type.None,
                        ParamUseScalingParams = true,
                        ParamEntryDuration = CardStatistics.EntryDuration.ThisTurn,
                        ParamStatusEffects = new StatusEffectStackData[] { },
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        ParamInt = 1,
                    },
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName
                    }
                },
                EffectBuilders = new List<CardEffectDataBuilder>
                {
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectPlayUnitTrigger",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                        ParamTrigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum()
                    },
                    new CardEffectDataBuilder
                    {
                        EffectStateName = "CardEffectDamage",
                        TargetMode = TargetMode.Room,
                        TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                        TargetModeStatusEffectsFilter = new string[]
                        { 
                            "BEYONDER_FILTER_BY_EXCLUDE_HYSTERIA_TRIGGER",
                        },
                        ParamInt = 0,

                        AppliedVFX = CustomCardManager.GetCardDataByID(VanillaCardIDs.MortalEntrapment).GetEffects()[0].GetAppliedVFX(),
                    },
                }

            }.BuildAndRegister();

            return Card;
        }
    }

    [HarmonyPatch(typeof(TargetHelper), "CheckTargetFiltered")]
    public static class BeyonderFilterByExcludeHysteriaTriggerPatch
    {
        public static bool Prefix(CharacterState target, List<string> targetModeStatusEffectsFilter, CardEffectData.HealthFilter targetModeHealthFilter, bool targetIgnoreBosses, bool targetIgnorePyre, bool inCombat, bool ignoreDead, SubtypeData targetSubtype, ref bool __result)
        { 
            if (!Beyonder.IsInit) { return true; }

            if (targetModeStatusEffectsFilter.IsNullOrEmpty()) 
            { 
                return true; 
            }
            if (targetModeStatusEffectsFilter[0] == "BEYONDER_FILTER_BY_EXCLUDE_HYSTERIA_TRIGGER") 
            {
                //Beyonder.Log("Mass Hysteria Detected");

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
            //foreach (string statusId in targetModeStatusEffectsFilter)
            //{
            //    if (!target.HasStatusEffect(statusId))
            //    {
            //        return true;
            //    }
            //}
            if (target.HasEffectTrigger(Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum())) 
            {
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

    [HarmonyPatch(typeof(TooltipContainer), "InstantiateTooltipStatusEffect")]
    public static class FixBrokenTooltips 
    { 
        public static bool Prefix(string statusId, ref TooltipUI __result) 
        {
            if (statusId == "BEYONDER_FILTER_BY_EXCLUDE_HYSTERIA_TRIGGER" || statusId == "BEYONDER_FILTER_BY_EXCLUDE_LAST_TARGET") 
            {
                __result = null;
                return false;
            }

            return true;
        }
    }
}