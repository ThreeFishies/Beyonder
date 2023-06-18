using ShinyShoe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Trainworks.Constants;
using Trainworks.Managers;
using UnityEngine;
using static CardManager;

namespace CustomEffects
{
    // Token: 0x020000A2 RID: 162
    public sealed class CustomCardEffectDropAllButOneUnlessBoneDog : CardEffectBase
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

        public bool HasBoneDog(List<CardState> hand) 
        { 
            foreach (CardState card in hand) 
            {
                if (card != null && card.GetCardDataID() == VanillaCardIDs.BoneDogsFavor) 
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator HandleDiscardables(List<CardState> hand, CardEffectParams cardEffectParams, CardEffectState cardEffectState) 
        {
            List<CardState> cardsToDiscard = new List<CardState>();
            if (HasBoneDog(hand)) { yield break; }

            List<CardState> units = new List<CardState>();
            foreach (CardState card in hand) 
            {
                if (card.IsSpawnerCard())
                {
                    units.Add(card);
                }
                else 
                { 
                    cardsToDiscard.Add(card);
                }
            }

            if ((units.Count > 1) && ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager))
            {
                yield return cardManager.SelectCardFromHand(new HandSelectionUI.Params
                {
                    descriptionKey = locKey,
                    cardChosenCallback = new HandSelectionUI.CardStateChosenDelegate(this.HandleCardChosen),
                    filterCallback = ((CardState checkCard) => this.CardFilterFunc(checkCard, cardEffectParams.playedCard, cardEffectParams.relicManager)),
                    selectionErrorType = CardSelectionBehaviour.SelectionError.Invalid,
                    instantApplyDelay = cardEffectParams.saveManager.GetBalanceData().GetAnimationTimingData().cardDrawAnimationDuration
                });

                //units.Shuffle(RngId.Battle);
                //units.RemoveAt(0);
                //cardsToDiscard.Clear();
            }
            else 
            {
                yield return DiscardDiscardables(cardsToDiscard, cardEffectParams, cardEffectState);
            }

            yield break;
        }

        private bool CardFilterFunc(CardState checkCard, CardState playedCard, RelicManager relicManager)
        {
            return (checkCard.GetCardType() != CardType.Monster) && (checkCard != playedCard);
        }

        private IEnumerator HandleCardChosen(CardState chosenCard, CardManager cardManager)
        {
            List<CardState> hand = cardManager.GetHand(true);
            if (chosenCard != null)
            {
                hand.Remove(chosenCard);
            }

            yield return DiscardDiscardables(hand, null, null);
            yield break;
        }

        private IEnumerator DiscardDiscardables(List<CardState> discardables, CardEffectParams cardEffectParams, CardEffectState cardEffectState) 
        {
            CardManager cardManager;
            AllGameData allGameData;

            if (cardEffectParams == null)
            {
                ProviderManager.TryGetProvider<CardManager>(out cardManager);
                allGameData = ProviderManager.SaveManager.GetAllGameData();
            }
            else 
            {
                cardManager = cardEffectParams.cardManager;
                allGameData = cardEffectParams.allGameData;
            }

            foreach (CardState cardToDiscard in discardables)
            {
                CardManager.DiscardCardParams discardCardParams = new CardManager.DiscardCardParams();

                float effectDelay = 0f;

                yield return CoreUtil.WaitForSecondsOrBreak(effectDelay);
                if (discardMode == CardEffectDiscardHand.DiscardMode.Discard)
                {
                    discardCardParams.discardCard = cardToDiscard;
                    discardCardParams.triggeredByCard = true;
                    discardCardParams.triggeredCard = this.sourceCardState;
                    discardCardParams.wasPlayed = false;
                    yield return cardManager.DiscardCard(discardCardParams, false);
                }
                else if (discardMode == CardEffectDiscardHand.DiscardMode.Consume)
                {
                    cardManager.MoveToStandByPile(cardToDiscard, false, true, new RemoveFromStandByCondition(() => CardPile.ExhaustedPile, null), discardCardParams, HandUI.DiscardEffect.Exhausted);
                    this.numConsumed++;
                }
                effectDelay += allGameData.GetBalanceData().GetAnimationTimingData().handDiscardAnimationDelay;
                //cardToDiscard = null;
            }
            yield break;
        }

        // Token: 0x0600073D RID: 1853 RVA: 0x00021EEE File Offset: 0x000200EE
        public override void Setup(CardEffectState cardEffectState)
        {
            base.Setup(cardEffectState);
            this.sourceCardState = cardEffectState.GetParentCardState();
            this.discardMode = (CardEffectDiscardHand.DiscardMode)cardEffectState.GetParamInt();
        }

        // Token: 0x0600073E RID: 1854 RVA: 0x00021F03 File Offset: 0x00020103
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            this.numConsumed = 0;
            List<CardState> hand = cardEffectParams.cardManager.GetHand(true);
            if (cardEffectParams.playedCard != null)
            {
                hand.Remove(cardEffectParams.playedCard);
            }

            yield return HandleDiscardables(hand, cardEffectParams, cardEffectState);

            //List<CardState>.Enumerator enumerator = default(List<CardState>.Enumerator);
            yield break;
            //yield break;
        }

        // Token: 0x0600073F RID: 1855 RVA: 0x00021F20 File Offset: 0x00020120
        public int GetNumCardsConsumed()
        {
            return this.numConsumed;
        }

        private readonly string locKey = "ScreenDeck_Select_CustomCardEffectDropAllButOneUnlessBoneDog";

        // Token: 0x0400044B RID: 1099
        private int numConsumed;

        // Token: 0x0400044C RID: 1100
        private CardState sourceCardState;

        private CardEffectDiscardHand.DiscardMode discardMode;
    }
}