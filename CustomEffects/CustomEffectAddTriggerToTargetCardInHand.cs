/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEffects
{
    // Token: 0x020000A9 RID: 169
    public sealed class CustomEffectAddTriggerToTargetCardInHand : CardEffectBase, ICardEffectStatuslessTooltip
    {
        // Token: 0x1700006E RID: 110
        // (get) Token: 0x06000764 RID: 1892 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanPlayAfterBossDead
        {
            get
            {
                return false;
            }
        }

        // Token: 0x1700006F RID: 111
        // (get) Token: 0x06000765 RID: 1893 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return false;
            }
        }

        // Token: 0x06000766 RID: 1894 RVA: 0x0001C727 File Offset: 0x0001A927
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            return true;
        }

        // Token: 0x06000767 RID: 1895 RVA: 0x000226D2 File Offset: 0x000208D2
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            bool okay = false;

            if (cardEffectState.GetParamCardUpgradeData() != null) 
            { 
                if (cardEffectState.GetParamCardUpgradeData().GetCardTriggerUpgrades() != null) 
                {
                    TriggerToAdd = new CardTriggerEffectState();
                    TriggerToAdd.Setup(cardEffectState.GetParamCardUpgradeData().GetCardTriggerUpgrades()[0], cardEffectParams.playedCard, cardEffectParams.saveManager);
                    okay = true;
                }
            }

            if (!okay) 
            {
                yield break;
            }

            if (cardEffectParams.cardManager.GetHand(false).Count > 0 && this.CanFreezeHand(cardEffectParams))
            {
                yield return cardEffectParams.cardManager.SelectCardFromHand(new HandSelectionUI.Params
                {
                    descriptionKey = "ScreenDeck_Select_CardEffectFreezeCard",
                    cardChosenCallback = new HandSelectionUI.CardStateChosenDelegate(this.HandleCardChosen),
                    filterCallback = ((CardState checkCard) => this.CardFilterFunc(checkCard, cardEffectParams.playedCard)),
                    selectionErrorType = CardSelectionBehaviour.SelectionError.InvalidTarget,
                    instantApplyDelay = cardEffectParams.saveManager.GetBalanceData().GetAnimationTimingData().cardDrawAnimationDuration
                });
            }
            yield break;
        }

        // Token: 0x06000768 RID: 1896 RVA: 0x000226E8 File Offset: 0x000208E8
        private IEnumerator HandleCardChosen(CardState chosenCard, CardManager cardManager)
        {
            //CardTraitData cardTraitData = new CardTraitData();
            //cardTraitData.Setup("CardTraitFreeze");
            //cardManager.AddTemporaryTraitToCard(chosenCard, cardTraitData, true, false);

            chosenCard

            yield return new WaitForSeconds(cardManager.GetHandCardVfxDuration(HandUI.HandVFX.Freeze));
            cardManager.RefreshHandCards();
            yield break;
        }

        // Token: 0x06000769 RID: 1897 RVA: 0x00022700 File Offset: 0x00020900
        private bool CardFilterFunc(CardState checkCard, CardState playedCard)
        {
            //bool flag = checkCard.HasTrait(typeof(CardTraitFreeze));
            bool flag = checkCard.HasTriggerType(addTriggerType);
            return checkCard == playedCard || flag;
        }

        // Token: 0x0600076A RID: 1898 RVA: 0x00022724 File Offset: 0x00020924
        public string GetTooltipBaseKey(CardEffectState cardEffectState)
        {
            return "CardEffectFreezeCard";
        }

        // Token: 0x0600076B RID: 1899 RVA: 0x0002272C File Offset: 0x0002092C
        private bool CanFreezeHand(CardEffectParams cardEffectParams)
        {
            foreach (CardState checkCard in cardEffectParams.cardManager.GetHand(false))
            {
                if (!this.CardFilterFunc(checkCard, cardEffectParams.playedCard))
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x0400045C RID: 1116
        private CardTriggerEffectState TriggerToAdd = new CardTriggerEffectState();
        private CardTriggerType addTriggerType = CardTriggerType.OnUnplayed;
        private List<CardData> cardDatas = new List<CardData>();
    }
}*/