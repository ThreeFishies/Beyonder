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
    public static class UndretchBoons
    {
        public static List<CardUpgradeData> Build()
        {
            List<CardUpgradeData> boons = new List<CardUpgradeData>();

            //Boon 00 (Anxiety: Heal 5 and damage enemy units equal to amount healed.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_00",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Boon_00_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = typeof(CustomCardEffectHealAndDamageRelative).AssemblyQualifiedName,
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamInt = 5,
                                ParamMultiplier = 2.0f,
                            }
                        }
                    }
                }
            }.Build());

            /*
            //Boon 00 (-8/-8 and Sweep)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_00",
                BonusDamage = -8,
                BonusHP = -8,

                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = VanillaStatusEffectIDs.Sweep,
                        count = 1
                    }
                }
            }.Build());
            */

            //Boon 01 (Chronic 3)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_01",

                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectChronic.statusId,
                        count = 5
                    }
                }
            }.Build());

            //Boon 02 (Anxiety: +4 health)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_02",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Boon_02_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,

                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                {
                                    UpgradeTitleKey = "Undretch_Boon_02_Anxiety_Effect",
                                    BonusHP = 6,
                                    SourceSynthesisUnit = new CharacterDataBuilder { CharacterID = "DummyNULL" }.Build()
                                }.Build()
                            }
                        }
                    }
                }

            }.Build());

            //Boon 03 (Anxiety: +1 Stealth)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_03",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Boon_03_Description_Key",
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
                                        statusId = VanillaStatusEffectIDs.Stealth,
                                        count = 1,
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Boon 04 (Anxiety: +1 Chronic to friendly units)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_04",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Boon_04_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddStatusEffect",
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,

                                ParamStatusEffects = new StatusEffectStackData[]
                                {
                                    new StatusEffectStackData
                                    {
                                        statusId = StatusEffectChronic.statusId,
                                        count = 1,
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Boon 05 (Anxiety: +1 Ember)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_05",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        DescriptionKey = "Undretch_Boon_05_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectGainEnergy",
                                ParamInt = 1,
                            }
                        }
                    }
                }
            }.Build());

            //Boon 06 (Revenge: Apply Jitters 3 to enemy units for each Mania below 0)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_06",

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = CharacterTriggerData.Trigger.OnHit,
                        DescriptionKey = "Undretch_Boon_06_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = typeof(CustomCardEffectAddStatusEffectPerMania).AssemblyQualifiedName,
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Heroes,
                                ParamInt = 3,
                                ParamBool = false, //Anxiety

                                ParamStatusEffects = new StatusEffectStackData[]
                                {
                                    new StatusEffectStackData
                                    {
                                        statusId = StatusEffectJitters.statusId,
                                        count = 3
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Boon 07 (+1 cost and +16/+35)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_07",
                CostReduction = -1,
                BonusDamage = 16,
                BonusHP = 35
            }.Build());

            //Boon 08 (-1 Size)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_08",
                BonusSize = -1,
            }.Build());

            //Boon 09 (-1 Cost and Stalker)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Undretch_Boon_09",
                CostReduction = 1,

                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName,
                    }
                }
            }.Build());

            return boons;
        }
    }
}