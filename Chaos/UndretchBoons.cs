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
    public static class UndretchBoons
    {
        public static List<CardUpgradeData> Build()
        {
            List<CardUpgradeData> boons = new List<CardUpgradeData>();

            CardUpgradeData Undretch_Boon_02_Anxiety_Effect = new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_02_Anxiety_Effect",
                BonusHP = 6,
            }.Build();

            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(Undretch_Boon_02_Anxiety_Effect, true);

            //Boon 00 (Anxiety: Heal 5 and damage enemy units equal to 2 x amount healed.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_00",
                BonusDamage = -2,
                BonusHP = 5,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Boon_00_Trigger_ID",
                        DescriptionKey = "Undretch_Boon_00_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CustomCardEffectHealAndDamageRelative),
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
                UpgradeID = "Undretch_Boon_00",
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

            //Boon 01 (Chronic 5)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_01",
                BonusDamage = 2,
                BonusHP = 2,

                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectChronic.statusId,
                        count = 5
                    }
                }
            }.Build());

            //Boon 02 (Anxiety: +6 health)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_02",
                BonusHP = 6,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Boon_02_Trigger_ID",
                        DescriptionKey = "Undretch_Boon_02_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddTempCardUpgradeToUnits),
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = Undretch_Boon_02_Anxiety_Effect
                            }
                        }
                    }
                }

            }.Build());

            //Boon 03 (Anxiety: +1 Stealth)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_03",
                BonusDamage = 4,
                BonusHP = -2,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Boon_03_Trigger_ID",
                        DescriptionKey = "Undretch_Boon_03_Description_Key",
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
                UpgradeID = "Undretch_Boon_04",
                BonusDamage = 1,
                BonusHP = 2,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Boon_04_Trigger_ID",
                        DescriptionKey = "Undretch_Boon_04_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectAddStatusEffect),
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Monsters,

                                ParamStatusEffects = new List<StatusEffectStackData>
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
                UpgradeID = "Undretch_Boon_05",
                BonusHP = 2,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                        TriggerID = "Undretch_Boon_05_Trigger_ID",
                        DescriptionKey = "Undretch_Boon_05_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectGainEnergy),
                                ParamInt = 1,
                            }
                        }
                    }
                }
            }.Build());

            //Boon 06 (Revenge: Apply Jitters 3 to enemy units for each Mania below 0)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_06",

                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectSoundless.statusId,
                        count = 1
                    }
                },

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = CharacterTriggerData.Trigger.OnHit,
                        TriggerID = "Undretch_Boon_06_Trigger_ID",
                        DescriptionKey = "Undretch_Boon_06_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CustomCardEffectAddStatusEffectPerMania),
                                TargetMode = TargetMode.Room,
                                TargetTeamType = Team.Type.Heroes,
                                ParamInt = 3,
                                ParamBool = false, //Anxiety

                                ParamStatusEffects = new List<StatusEffectStackData>
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
                UpgradeID = "Undretch_Boon_07",
                CostReduction = -1,
                XCostReduction = -1,
                BonusDamage = 16,
                BonusHP = 36
            }.Build());

            //Boon 08 (-1 Size)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_08",
                BonusSize = -1,
            }.Build());

            //Boon 09 (-1 Cost and Stalker)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Undretch_Boon_09",
                CostReduction = 1,
                XCostReduction = 1,
                BonusHP = 3,

                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateType = typeof(BeyonderCardTraitStalkerState),
                    }
                }
            }.Build());

            return boons;
        }
    }
}