using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using System.Collections;
using System;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Trainworks.Enums.MTTriggers;
using Void.Init;
using Void.Triggers;
using Void.Artifacts;
using Void.Status;
using ShinyShoe;

namespace Void.Mania
{
    [HarmonyPatch(typeof(CardTooltipContainer), "AddTooltipsCardUpgrade")]
    public static class AddCardTraitTooltipsToUnitSynthesis 
    {
        public static void Postfix(ref CardTooltipContainer __instance, ref CardUpgradeState upgradeState)
        {
            if (upgradeState.GetTraitDataUpgrades().IsNullOrEmpty())
            {
                return;
            }
            else 
            {
                foreach (CardTraitData cardTraitData in upgradeState.GetTraitDataUpgrades())
                {
                    Type type = TypeNameCache.GetType(cardTraitData.GetTraitStateName());
                    CardTraitState cardTraitState = (CardTraitState)Activator.CreateInstance(type);
                    cardTraitState.Setup(cardTraitData, null, false);

                    if (TooltipContainer.TraitSupportedInTooltips(cardTraitState.GetType().Name))
                    {
                        TooltipUI tooltipUI = __instance.InstantiateTooltip(cardTraitState.GetCardTooltipId(), TooltipDesigner.TooltipDesignType.Keyword, false);
                        if (tooltipUI != null)
                        {
                            tooltipUI.InitCardTrait(cardTraitState);
                        }
                        cardTraitState.CreateAdditionalTooltips(__instance);
                    }
                    if (cardTraitState.GetParamStatusEffects() != null)
                    {
                        foreach (StatusEffectStackData statusEffectStackData in cardTraitState.GetParamStatusEffects())
                        {
                            TooltipUI tooltipUI2 = __instance.InstantiateTooltipStatusEffect(statusEffectStackData.statusId, StatusEffectManager.Instance);
                            if (tooltipUI2 != null)
                            {
                                tooltipUI2.InitStatusEffect(statusEffectStackData.statusId, StatusEffectManager.Instance, TooltipUI.TitleStyle.Normal);
                            }
                        }
                    }
                }
            }
        }
    }
}