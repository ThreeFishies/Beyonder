using System;
using System.Collections;
using Trainworks.Managers;
using HarmonyLib;
using UnityEngine;

namespace CustomEffects
{

    // Token: 0x020000AC RID: 172
    public sealed class CustomCardEffectAdjustHandSize : CardEffectBase
    {
        public static int AdjustAmount = 0;
        // Token: 0x17000074 RID: 116
        // (get) Token: 0x06000779 RID: 1913 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanPlayAfterBossDead
        {
            get
            {
                return false;
            }
        }

        // Token: 0x17000075 RID: 117
        // (get) Token: 0x0600077A RID: 1914 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return true;
            }
        }

        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            return true;
        }

        // Token: 0x0600077B RID: 1915 RVA: 0x000228B3 File Offset: 0x00020AB3
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (cardEffectParams.saveManager.PreviewMode) 
            {
                yield break;
            }

            int intInRange = cardEffectState.GetIntInRange();
            AdjustAmount += intInRange;
            cardEffectParams.combatManager.DispatchCardDrawNextTurnSignal();
            yield break;
        }
    }

    [HarmonyPatch(typeof(CombatManager), "GetStartOfTurnCards")]
    public static class AdjustHandSizePatch1 
    { 
        public static void Postfix(ref bool includeTemporaryEffects, ref int __result) 
        {
            if (CustomCardEffectAdjustHandSize.AdjustAmount != 0 && includeTemporaryEffects)
            {
                int maxHand = ProviderManager.SaveManager.GetBalanceData().GetMaxHandSize();
                __result = Mathf.Clamp(__result + CustomCardEffectAdjustHandSize.AdjustAmount, 0, maxHand);
            }
        }
    }

    [HarmonyPatch(typeof(CombatManager), "StartCombat")]
    public static class AdjustHandSizePatch2
    {
        public static void Prefix()
        {
            CustomCardEffectAdjustHandSize.AdjustAmount = 0;
        }
    }
}