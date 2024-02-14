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

namespace Void.Chaos
{
    public static class VeilritchBoons
    {
        public static List<CardUpgradeData> Build() 
        { 
            List<CardUpgradeData> boons = new List<CardUpgradeData>();

            CardUpgradeData Veilritch_Boon_05_OnHysteria_Effect = new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_05_OnHysteria_Effect",
                BonusDamage = 6,
            }.Build();

            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(Veilritch_Boon_05_OnHysteria_Effect, true);

            //Boon 00 (Quick)
            boons.Add(new CardUpgradeDataBuilder 
            { 
                UpgradeID = "Veilritch_Boon_00",
                BonusDamage = 13,
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
                UpgradeID = "Veilritch_Boon_01",
                BonusDamage = -2,
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
                UpgradeID = "Veilritch_Boon_02",
                BonusDamage = 4,
                BonusHP = -1,
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
                UpgradeID = "Veilritch_Boon_03",
                BonusDamage = 28,
                BonusHP = 24,
                CostReduction = -1,
                XCostReduction = -1,
            }.Build());

            //Whoops. Don't need this twice.
            /*
            //Boon 04 (+1 cost and +25/+26)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_04",
                BonusDamage = 25,
                BonusHP = 26,
                CostReduction = -1
            }.Build());
            */

            //Boon 04 (Hysteria: +6 attack.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_05",
                BonusDamage = 6,

                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                { 
                    new CharacterTriggerDataBuilder
                    { 
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        TriggerID =  "Veilritch_Boon_05_Trigger_ID",
                        DescriptionKey = "Veilritch_Boon_05_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            { 
                                EffectStateType = typeof(CardEffectAddTempCardUpgradeToUnits),
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                                ParamCardUpgradeData = Veilritch_Boon_05_OnHysteria_Effect,
                            }
                        }
                    }
                }
            }.Build());

            //Boon 05 (Hysteria: +18 Jitters to the front enemy unit.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_06",
                BonusDamage = 2,
                BonusHP = 1,
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        TriggerID = "Veilritch_Boon_06_Trigger_ID",
                        DescriptionKey = "Veilritch_Boon_06_Description_Key",
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
                                        statusId = StatusEffectJitters.statusId,
                                        count = 18
                                    }
                                }
                            }
                        }
                    }
                }
            }.Build());

            //Boon 06 (Hysteria: Deal 30 damage to the front enemy unit.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_07",
                BonusHP = 3,
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                        TriggerID = "Veilritch_Boon_07_Trigger_ID",
                        DescriptionKey = "Veilritch_Boon_07_Description_Key",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateType = typeof(CardEffectDamage),
                                TargetMode = TargetMode.FrontInRoom,
                                TargetTeamType = Team.Type.Heroes,
                                ParamInt = 30
                            }
                        }
                    }
                }
            }.Build());

            //Boon 07 (+15 attack per mania above 0.)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_08",
                StatusEffectUpgrades = new List<StatusEffectStackData>
                {
                    new StatusEffectStackData
                    {
                        statusId = StatusEffectFormless.statusId,
                        count = 1
                    }
                },

                RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>
                {
                    new RoomModifierDataBuilder
                    {
                        RoomModifierID = "Veilritch_Boon_08_RoomModifier_ID",
                        DescriptionKey = "Veilritch_Boon_08_Description_Key",
                        ParamStatusEffects = new List<StatusEffectStackData> { },
                        RoomModifierClassType = typeof(CustomRoomStateSelfDamagePerMania),
                        ParamInt = 15, //+15 above 0, -15 below 0
                        ExtraTooltipTitleKey = "",
                        ExtraTooltipBodyKey = ""
                    }
                }
            }.Build());

            AccessTools.Field(typeof(RoomModifierData), "descriptionKeyInPlay").SetValue(boons[7].GetRoomModifierUpgrades()[0], "Veilritch_Boon_08_Description_Key_In_Play");

            //Boon 08 (-1 size)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_09",
                BonusSize = -1
            }.Build());

            //Boon 09 (-1 cost and Stalker)
            boons.Add(new CardUpgradeDataBuilder
            {
                UpgradeID = "Veilritch_Boon_10",
                CostReduction = 1,
                XCostReduction = 1,
                BonusDamage = 3,

                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                { 
                    new CardTraitDataBuilder
                    { 
                        TraitStateType = typeof(BeyonderCardTraitStalkerState)
                    }
                }
            }.Build());

            return boons;
        }
    }
}