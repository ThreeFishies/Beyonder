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

namespace Void.Chaos
{
    public static class VeilritchBoons
    {
        public static List<CardUpgradeData> Build() 
        { 
            List<CardUpgradeData> boons = new List<CardUpgradeData>();

            //Boon 00 (Quick)
            boons.Add(new CardUpgradeDataBuilder 
            { 
                UpgradeTitleKey = "Veilritch_Boon_00",
                BonusDamage = 18,
                StatusEffectUpgrades = new List<StatusEffectStackData> 
                { 
                    new StatusEffectStackData
                    { 
                        statusId = VanillaStatusEffectIDs.Quick,
                        count = 1
                    }
                }
            }.Build());

            //Boon 01 (Multistrike 1)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_01",
                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectMultistrikeState.StatusId,
                        count = 1
                    }
                }
            }.Build());

            //Boon 02 (Trample)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_02",
                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectTrampleState.StatusId,
                        count = 1
                    }
                }
            }.Build());

            //Boon 03 (+1 cost and +25/+26)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_03",
                BonusDamage = 25,
                BonusHP = 26,
                CostReduction = -1
            }.Build());

            //Whoops. Don't need this twice.
            /*
            //Boon 04 (+1 cost and +25/+26)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_04",
                BonusDamage = 25,
                BonusHP = 26,
                CostReduction = -1
            }.Build());
            */

            //Boon 05 (Hysteria: +4 attack.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_05",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Veilritch_Boon_05_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            { 
                                EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = new CardUpgradeDataBuilder
                                { 
                                    UpgradeTitleKey = "Veilritch_Boon_05_OnHysteria_Effect",
                                    BonusDamage = 6,
                                    SourceSynthesisUnit = new CharacterDataBuilder{ CharacterID = "DummyNULL" }.Build(),
                                }.Build(),
                            }
                        }
                    }
                }
            }.Build());

            //Boon 06 (Hysteria: +10 Jitters to the front enemy unit.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_06",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Veilritch_Boon_06_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectAddStatusEffect",
                                TargetMode = TargetMode.FrontInRoom,
                                TargetTeamType = Team.Type.Heroes,
                                ParamStatusEffects = new StatusEffectStackData[]
                                { 
                                    new StatusEffectStackData
                                    { 
                                        statusId = StatusEffectJitters.statusId,
                                        count = 10
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Boon 07 (Hysteria: Deal 30 damage to the front enemy unit.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_07",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        DescriptionKey = "Veilritch_Boon_07_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectDamage",
                                TargetMode = TargetMode.FrontInRoom,
                                TargetTeamType = Team.Type.Heroes,
                                ParamInt = 30
                            }
                        }
                    }
                }
            }.Build());

            //Boon 08 (+15 attack per mania above 0.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_08",

                RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>
                {
                    new RoomModifierDataBuilder
                    {
                        DescriptionKey = "Veilritch_Boon_08_Description_Key",
                        ParamStatusEffects = new StatusEffectStackData[] { },
                        RoomStateModifierClassName = typeof(CustomRoomStateSelfDamagePerMania).AssemblyQualifiedName,
                        ParamInt = 15, //+15 above 0, -15 below 0
                        ExtraTooltipTitleKey = "",
                        ExtraTooltipBodyKey = ""
                    }
                }
            }.Build());

            AccessTools.Field(typeof(RoomModifierData), "descriptionKeyInPlay").SetValue(boons[7].GetRoomModifierUpgrades()[0], "Veilritch_Boon_08_Description_Key_In_Play");

            //Boon 09 (-1 size)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_09",
                BonusSize = -1
            }.Build());

            //Boon 10 (-1 cost and Stalker)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Veilritch_Boon_10",
                CostReduction = 1,

                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateName = typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName
                    }
                }
            }.Build());

            return boons;
        }
    }
}