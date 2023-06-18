using System;
using System.Collections;
using System.Collections.Generic;
using Void.Mania;

namespace CustomEffects
{

    // Token: 0x020000A2 RID: 162
    public sealed class CustomCardEffectDiscardHandByEntropic : CardEffectBase
    {
        // Token: 0x17000064 RID: 100
        // (get) Token: 0x0600073B RID: 1851 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanPlayAfterBossDead
        {
            get
            {
                return false;
            }
        }

        // Token: 0x17000065 RID: 101
        // (get) Token: 0x0600073C RID: 1852 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return false;
            }
        }

        // Token: 0x0600073D RID: 1853 RVA: 0x00021EEE File Offset: 0x000200EE
        public override void Setup(CardEffectState cardEffectState)
        {
            base.Setup(cardEffectState);
            this.sourceCardState = cardEffectState.GetParentCardState();
        }

        // Token: 0x0600073E RID: 1854 RVA: 0x00021F03 File Offset: 0x00020103
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            this.numConsumed = 0;
            CardEffectDiscardHand.DiscardMode discardMode = (CardEffectDiscardHand.DiscardMode)cardEffectState.GetParamInt();
            List<CardState> hand = cardEffectParams.cardManager.GetHand(true);
            if (cardEffectParams.playedCard != null)
            {
                hand.Remove(cardEffectParams.playedCard);
            }
            float effectDelay = 0f;
            CardManager.DiscardCardParams discardCardParams = new CardManager.DiscardCardParams();
            foreach (CardState cardToDiscard in hand)
            {
                if (cardToDiscard.HasTrait(typeof(BeyonderCardTraitEntropic))) 
                {
                    continue;
                }
                yield return CoreUtil.WaitForSecondsOrBreak(effectDelay);
                if (discardMode == CardEffectDiscardHand.DiscardMode.Discard)
                {
                    discardCardParams.discardCard = cardToDiscard;
                    discardCardParams.triggeredByCard = true;
                    discardCardParams.triggeredCard = this.sourceCardState;
                    discardCardParams.wasPlayed = false;
                    yield return cardEffectParams.cardManager.DiscardCard(discardCardParams, false);
                }
                else if (discardMode == CardEffectDiscardHand.DiscardMode.Consume)
                {
                    cardEffectParams.cardManager.MoveToStandByPile(cardToDiscard, false, true, new RemoveFromStandByCondition(() => CardPile.ExhaustedPile, null), discardCardParams, HandUI.DiscardEffect.Exhausted);
                    this.numConsumed++;
                }
                effectDelay += cardEffectParams.allGameData.GetBalanceData().GetAnimationTimingData().handDiscardAnimationDelay;
                //cardToDiscard = null;
            }
            //List<CardState>.Enumerator enumerator = default(List<CardState>.Enumerator);
            yield break;
            //yield break;
        }

        // Token: 0x0600073F RID: 1855 RVA: 0x00021F20 File Offset: 0x00020120
        public int GetNumCardsConsumed()
        {
            return this.numConsumed;
        }

        // Token: 0x0400044B RID: 1099
        private int numConsumed;

        // Token: 0x0400044C RID: 1100
        private CardState sourceCardState;

        // Token: 0x02000BFA RID: 3066
        public enum DiscardMode
        {
            // Token: 0x04003F82 RID: 16258
            Discard,
            // Token: 0x04003F83 RID: 16259
            Consume
        }
    }
}