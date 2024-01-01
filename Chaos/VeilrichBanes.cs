using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.BuildersV2;
using Trainworks.Managers;
using Trainworks.Constants;
using System.Linq;
using UnityEngine;
using Trainworks.Utilities;
using Void.Init;
using CustomEffects;
using Void.Clan;
using Void.Unit;
using Void.Triggers;
using Void.Status;
using Steamworks;
using System.Collections;
using Spine;

namespace Void.Chaos
{
    public static class VeilritchBanes
    {
        public static List<CardUpgradeData> Build()
        {
            List<CardUpgradeData> banes = new List<CardUpgradeData>();

            CardUpgradeData Veilritch_Bane_04_Hysteria_Effect = new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_04_Hysteria_Effect",
                BonusHP = -1,
            }.Build();

            CardUpgradeData Veilritch_Bane_05_Hysteria_Effect = new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_05_Hysteria_Effect",
                BonusDamage = -1,
            }.Build();

            CardUpgradeData Veilritch_Bane_08_Rally_Effect = new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_08_Rally_Effect",
                BonusHP = -1,
            }.Build();

            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(Veilritch_Bane_04_Hysteria_Effect, true);
            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(Veilritch_Bane_05_Hysteria_Effect, true);
            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(Veilritch_Bane_08_Rally_Effect, true);

            //Bane 00 (-10/-8)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_00",
                BonusDamage = -10,
                BonusHP = -8
            }.Build());

            //Bane 01 (+1 Size and +16 Attack)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_01",
                BonusDamage = 16,
                BonusSize = 1
            }.Build());

            //Don't need duplicates
            /*
            //Bane 02 (+1 Size and +16 Attack)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_02",
                BonusDamage = 16,
                BonusSize = 1
            }.Build());
            */

            //Bane 03 (+1 Cost)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_03",
                CostReduction = -1,
                XCostReduction = -1,
            }.Build());

            //Bane 04 (Hysteria: -1 Health)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_04",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        TriggerID = "Veilritch_Bane_04_Trigger_ID",
                        DescriptionKey = "Veilritch_Bane_04_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            { 
                                EffectStateType = typeof(CardEffectAddTempCardUpgradeToUnits),
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = Veilritch_Bane_04_Hysteria_Effect,

                                AdditionalTooltips = new List<AdditionalTooltipData>
                                {
                                    new AdditionalTooltipData
                                    {
                                        titleKey = string.Empty,
                                        descriptionKey = "TipTooltip_CanReduceHealthToZero",
                                        isStatusTooltip = false,
                                        statusId = "",
                                        isTipTooltip = true,
                                        isTriggerTooltip = false,
                                        trigger = CharacterTriggerData.Trigger.OnDeath,
                                        style = TooltipDesigner.TooltipDesignType.Default
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Bane 05 (Anxiety: -1 Attack)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_05",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Veilritch_Bane_05_Trigger_ID",
                        DescriptionKey = "Veilritch_Bane_05_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddTempCardUpgradeToUnits),
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = Veilritch_Bane_05_Hysteria_Effect
                            }
                        }
                    }
                }
            }.Build());

            //Bane 06 (Anxiety: Jitters 2 to self.)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_06",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Veilritch_Bane_06_Trigger_ID",
                        DescriptionKey = "Veilritch_Bane_06_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddStatusEffect),
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,

                                ParamStatusEffects = new List<StatusEffectStackData>
                                { 
                                    new StatusEffectStackData
                                    { 
                                        statusId = StatusEffectJitters.statusId,
                                        count = 1
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Bane 07 (Anxiety: Stealth 1 to the front enemy unit.)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_07",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Veilritch_Bane_07_Trigger_ID",
                        DescriptionKey = "Veilritch_Bane_07_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddStatusEffect),
                                TargetMode = TargetMode.FrontInRoom,
                                TargetTeamType = Team.Type.Heroes,

                                ParamStatusEffects = new List<StatusEffectStackData>
                                {
                                    new StatusEffectStackData
                                    {
                                        statusId = VanillaStatusEffectIDs.Stealth,
                                        count = 1
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            /*
            //Bane08 (Revenge: Apply Jitters 1 to friendly units for each Mania below 0.)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_08",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = CharacterTriggerData.Trigger.OnHit,
                        DescriptionKey = "Veilritch_Bane_08_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = typeof(CustomCardEffectAddStatusEffectPerMania).AssemblyQualifiedName,
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamInt = 1,
                                ParamBool = false, //Anxiety

                                ParamStatusEffects = new StatusEffectStackData[]
                                {
                                    new StatusEffectStackData
                                    {
                                        statusId = StatusEffectJitters.statusId,
                                        count = 1
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());
            */

            //Bane08 (Revenge: Apply Jitters 1 to friendly units for each Mania below 0.)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_08",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = CharacterTriggerData.Trigger.CardMonsterPlayed,
                        TriggerID = "Veilritch_Bane_08_Trigger_ID",
                        DescriptionKey = "Veilritch_Bane_08_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddTempCardUpgradeToUnits),
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = Veilritch_Bane_08_Rally_Effect,
                                AdditionalTooltips = new List<AdditionalTooltipData>
                                {
                                    new AdditionalTooltipData
                                    {
                                        titleKey = string.Empty,
                                        descriptionKey = "TipTooltip_CanReduceHealthToZero",
                                        isStatusTooltip = false,
                                        statusId = "",
                                        isTipTooltip = true,
                                        isTriggerTooltip = false,
                                        trigger = CharacterTriggerData.Trigger.OnDeath,
                                        style = TooltipDesigner.TooltipDesignType.Default
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Boon 09 (Jitters 6)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_09",
                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectJitters.statusId,
                        count = 6
                    }
                }
            }.Build());

            //Boon 10 (Mutated)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Bane_10",
                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectMutated.statusId,
                        count = 1
                    }
                }
            }.Build());

            return banes;
        }
    }
}