//CustomCardEffectSecretStatusEffect
using System;
using System.Collections;
using System.Collections.Generic;
using ShinyShoe.Logging;
using Trainworks.Managers;
using Void.Artifacts;

namespace Equestrian.CustomEffects
{
    // Token: 0x02000087 RID: 135
    public class CustomCardEffectSecretStatusEffect : CardEffectBase
    {
        public override void Setup(CardEffectState cardEffectState)
        {
            GenderRevealID = cardEffectState.GetParamStr();
            base.Setup(cardEffectState);
        }

        // Token: 0x0600069B RID: 1691 RVA: 0x00020CB4 File Offset: 0x0001EEB4
        public static StatusEffectStackData GetStatusEffectStack(CardEffectData cardEffectData, CardEffectState cardEffectState, CharacterState selfTarget, bool isTest = false)
        {
            StatusEffectStackData[] paramStatusEffects = cardEffectData.GetParamStatusEffects();
            cardEffectData.GetParamBool();
            if (paramStatusEffects == null || paramStatusEffects.Length == 0)
            {
                Log.Error(LogGroups.Gameplay, "cardEffectData.GetParamStatusEffects() yielded no results.");
                return null;
            }
            StatusEffectStackData statusEffectStackData = paramStatusEffects[0];
            if (paramStatusEffects.Length > 1)
            {
                RngId rngId = isTest ? RngId.BattleTest : RngId.Battle;
                statusEffectStackData = paramStatusEffects.RandomElement(rngId);
            }
            if (cardEffectState != null && cardEffectState.GetUseStatusEffectStackMultiplier() && selfTarget != null)
            {
                statusEffectStackData = statusEffectStackData.Copy();
                int statusEffectStacks = selfTarget.GetStatusEffectStacks(cardEffectState.GetStatusEffectStackMultiplier());
                statusEffectStackData.count *= statusEffectStacks;
            }
            return statusEffectStackData;
        }

        // Token: 0x0600069C RID: 1692 RVA: 0x00020D38 File Offset: 0x0001EF38
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            return true;
        }

        private bool HasGenderReveal() 
        {
            if (MemoryJewel.TryGetClassData(out ClassData _))
            {
                if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.GetMutatorCount() > 0) 
                {
                    foreach (MutatorState mutator in ProviderManager.SaveManager.GetMutators()) 
                    {
                        if (mutator.GetRelicDataID() == this.GenderRevealID) 
                        {
                            return true;
                        }   
                    }
                }
            }
            return false;
        }

        // Token: 0x0600069D RID: 1693 RVA: 0x00020DF4 File Offset: 0x0001EFF4
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (!HasGenderReveal()) { yield break; }

            StatusEffectStackData statusEffectStack = CardEffectAddStatusEffect.GetStatusEffectStack(cardEffectState.GetSourceCardEffectData(), cardEffectState, cardEffectParams.selfTarget, false);
            if (statusEffectStack == null)
            {
                yield break;
            }
            int intInRange = cardEffectState.GetIntInRange();
            CharacterState.AddStatusEffectParams addStatusEffectParams = new CharacterState.AddStatusEffectParams
            {
                sourceRelicState = cardEffectParams.sourceRelic,
                sourceCardState = cardEffectParams.playedCard,
                cardManager = cardEffectParams.cardManager,
                sourceIsHero = (cardEffectState.GetSourceTeamType() == Team.Type.Heroes)
            };
            for (int i = cardEffectParams.targets.Count - 1; i >= 0; i--)
            {
                CharacterState characterState = cardEffectParams.targets[i];
                RngId rngId = cardEffectParams.saveManager.PreviewMode ? RngId.BattleTest : RngId.Battle;
                if (intInRange == 0 || RandomManager.Range(0, 100, rngId) < intInRange)
                {
                    int count = statusEffectStack.count;
                    characterState.AddStatusEffect(statusEffectStack.statusId, count, addStatusEffectParams);
                }
            }
            yield break;
        }

        // Token: 0x0600069E RID: 1694 RVA: 0x00020E0A File Offset: 0x0001F00A
        public override void GetTooltipsStatusList(CardEffectState cardEffectState, ref List<string> outStatusIdList)
        {
            //CardEffectAddStatusEffect.GetTooltipsStatusList(cardEffectState.GetSourceCardEffectData(), ref outStatusIdList);
        }

        // Token: 0x0600069F RID: 1695 RVA: 0x00020E18 File Offset: 0x0001F018
        public static void GetTooltipsStatusList(CardEffectData cardEffectData, ref List<string> outStatusIdList)
        {
            //foreach (StatusEffectStackData statusEffectStackData in cardEffectData.GetParamStatusEffects())
            //{
            //    outStatusIdList.Add(statusEffectStackData.statusId);
            //}
        }

        private string GenderRevealID;
    }
}