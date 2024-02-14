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

// Undretch
namespace Void.Chaos
{
    public static class UndretchBanes
    {
        public static List<CardUpgradeData> Build()
        {
            List<CardUpgradeData> banes = new List<CardUpgradeData>();

            CardUpgradeData Undretch_Bane_03_Anxiety_Trigger = new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_03_Anxiety_Trigger",
                BonusDamage = -1,
            }.Build();

            CardUpgradeData Undretch_Bane_04_Hysteria_Trigger = new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_04_Hysteria_Trigger",
                BonusHP = -1,
            }.Build();

            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(Undretch_Bane_03_Anxiety_Trigger, true);
            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(Undretch_Bane_04_Hysteria_Trigger, true);

            //Bane 00 (-8/-10)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_00",
                BonusDamage = -6,
                BonusHP = -8
            }.Build());

            //Bane 01 (+1 Size and +16 Health)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_01",
                BonusSize = 1,
                BonusHP = 19
            }.Build());

            //Bane 02 (+1 Cost)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_02",
                CostReduction = -1,
                XCostReduction = -1,
                BonusDamage = 2,
                BonusHP = 1
            }.Build());

            //Bane 03 (Anxiety: -1 Attack)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_03",
                BonusHP = 3,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Bane_03_Trigger_ID",
                        DescriptionKey = "Undretch_Bane_03_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddTempCardUpgradeToUnits),
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = Undretch_Bane_03_Anxiety_Trigger
                            }
                        }
                    }
                }
            }.Build());

            //Bane 04 (Hysteria: -1 Health)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_04",
                BonusDamage = 3,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Bane_04_Trigger_ID",
                        DescriptionKey = "Undretch_Bane_04_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddTempCardUpgradeToUnits),
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = Undretch_Bane_04_Hysteria_Trigger, 

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

            //Bane 05 (Hysteria: Jitters 1)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_05",
                BonusDamage = 1,
                BonusHP = 2,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Bane_05_Trigger_ID",
                        DescriptionKey = "Undretch_Bane_05_Description_Key",
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

            //Bane 06 (Hysteria: Ascend the front enemy unit)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_06",
                BonusHP = 10,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Bane_06_Trigger_ID",
                        DescriptionKey = "Undretch_Bane_06_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CustomCardEffectBumpPreviewConditional),
                                TargetMode = TargetMode.FrontInRoom,
                                TargetTeamType = Team.Type.Heroes,
                                ParamInt = 1 //up by 1 floor
                            }
                        }
                    }
                }
            }.Build());

            //Bane 07 (Mutated)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_07",
                BonusDamage = -1,
                BonusHP = 3,

                StatusEffectUpgrades = new List<StatusEffectStackData> 
                { 
                    new StatusEffectStackData
                    { 
                        statusId = StatusEffectMutated.statusId,
                        count = 1
                    }
                }
            }.Build());

            //Bane 08 (Jitters 6)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_08",
                BonusDamage = 2,

                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectJitters.statusId,
                        count = 6
                    }
                }
            }.Build());

            //Bane 09 (Summon: -2 draw next turn)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Bane_09",
                BonusDamage = 3,
                BonusHP = 7,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = CharacterTriggerData.Trigger.OnSpawn,
                        TriggerID = "Undretch_Bane_09_Trigger_ID",
                        DescriptionKey = "Undretch_Bane_09_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            { 
                                EffectStateType = typeof(CardEffectDrawAdditionalNextTurn),
                                ParamInt = -2
                            }
                        }
                    }
                }
            }.Build());

            return banes;
        }
    }
}
