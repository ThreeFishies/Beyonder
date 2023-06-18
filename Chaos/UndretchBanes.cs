using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Builders;
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

            //Bane 00 (-8/-10)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Bane_00",
                BonusDamage = -8,
                BonusHP = -10
            }.Build());

            //Bane 01 (+1 Size and +16 Health)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Bane_01",
                BonusSize = 1,
                BonusHP = 16
            }.Build());

            //Bane 02 (+1 Cost)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Bane_02",
                CostReduction = -1
            }.Build());

            //Bane 03 (Anxiety: -1 Attack)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Bane_03",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Bane_03_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,

                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                { 
                                    UpgradeTitleKey = "Undretch_Bane_03_Anxiety_Trigger",
                                    BonusDamage = -1,
                                    SourceSynthesisUnit = new CharacterDataBuilder{ CharacterID = "DummyNULL" }.Build(),
                                }.Build(),
                            }
                        }
                    }
                }
            }.Build());

            //Bane 04 (Hysteria: -1 Health)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Bane_04",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Bane_04_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,

                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                {
                                    UpgradeTitleKey = "Undretch_Bane_04_Hysteria_Trigger",
                                    BonusHP = -1,
                                    SourceSynthesisUnit = new CharacterDataBuilder{ CharacterID = "DummyNULL" }.Build(),
                                }.Build(),
                            }
                        }
                    }
                }
            }.Build());

            //Bane 05 (Hysteria: Jitters 1)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Bane_05",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Bane_05_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddStatusEffect",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                
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

            //Bane 06 (Hysteria: Ascend the front enemy unit)
            banes.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Bane_06",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Bane_06_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = typeof(CustomCardEffectBumpPreviewConditional).AssemblyQualifiedName,
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
                UpgradeTitleKey = "Undretch_Bane_07",
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
                UpgradeTitleKey = "Undretch_Bane_08",
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
                UpgradeTitleKey = "Undretch_Bane_09",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = CharacterTriggerData.Trigger.OnSpawn,
                        DescriptionKey = "Undretch_Bane_09_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            { 
                                EffectStateName = "CardEffectDrawAdditionalNextTurn",
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
