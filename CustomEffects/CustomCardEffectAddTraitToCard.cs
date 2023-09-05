using HarmonyLib;
using ShinyShoe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Void.Init;
using Void.Spells;

namespace CustomEffects
{
    // Token: 0x020000A9 RID: 169
    public sealed class CustomCardEffectAddTraitToCard : CardEffectBase, ICardEffectStatuslessTooltip
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

        public CardSelectionBehaviour.SelectionError GetSelectionError() 
        {
            if (cardTraits.Count > 0)
            {
                if (cardTraits[0] != null) 
                {
                    if (cardTraits[0].GetTraitStateName() == "CardTraitRetain") 
                    {
                        return Beyonder.HoldoverCardSelectionError.GetEnum();
                    }
                    if (cardTraits[0].GetTraitStateName() == typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName) 
                    {
                        return Beyonder.EntropicCardSelectionError.GetEnum();
                    }
                }
            }

            return CardSelectionBehaviour.SelectionError.Invalid;
        }

        // Token: 0x06000767 RID: 1895 RVA: 0x000226D2 File Offset: 0x000208D2
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if ((cardPile == CardPile.HandPile) && (allCards == false))
            {
                if (cardEffectParams.cardManager.GetHand(false).Count > 0 && this.CanAddTrait(cardEffectParams))
                {
                    yield return cardEffectParams.cardManager.SelectCardFromHand(new HandSelectionUI.Params
                    {
                        descriptionKey = locKey,
                        cardChosenCallback = new HandSelectionUI.CardStateChosenDelegate(this.HandleCardChosen),
                        filterCallback = ((CardState checkCard) => this.CardFilterFunc(checkCard, cardEffectParams.playedCard, cardEffectParams.relicManager)),
                        selectionErrorType = GetSelectionError(),
                        instantApplyDelay = cardEffectParams.saveManager.GetBalanceData().GetAnimationTimingData().cardDrawAnimationDuration
                    });
                }
            }
            if (allCards == true)
            {
                List<CardState> allCards = cardEffectParams.cardManager.GetAllCards();
                foreach (CardState card in allCards)
                {
                    bool flag = true;
                    if (!filters.IsNullOrEmpty())
                    {
                        foreach (CardUpgradeMaskData filter in filters)
                        {
                            flag &= filter.FilterCard<CardState>(card, cardEffectParams.relicManager);
                        }
                    }
                    if (flag)
                    {
                        yield return HandleFilteredCard(card, cardEffectParams.cardManager);
                    }
                }
            }
            yield break;
        }

        private IEnumerator HandleFilteredCard(CardState card, CardManager cardManager)
        {
            if (cardTraits.IsNullOrEmpty() || cardTraits.Count < 1 || card == null)
            {
                yield break;
            }

            foreach (CardTraitData cardTraitData in cardTraits)
            {
                AddTraitToCardNoDuplicates(cardManager, card, cardTraitData, true, false);
            }

            if (cardManager.IsCardInHand(card))
            {
                yield return new WaitForSeconds(cardManager.GetHandCardVfxDuration(HandUI.HandVFX.Freeze));
            }
            yield break;
        }

        // Token: 0x06000768 RID: 1896 RVA: 0x000226E8 File Offset: 0x000208E8
        private IEnumerator HandleCardChosen(CardState chosenCard, CardManager cardManager)
        {
            if (cardTraits.IsNullOrEmpty() || cardTraits.Count < 1)
            {
                yield break;
            }

            foreach (CardTraitData cardTraitData in cardTraits)
            {
                //cardManager.AddTemporaryTraitToCard(chosenCard, cardTraitData, true, false);
                AddTraitToCardNoDuplicates(cardManager, chosenCard, cardTraitData, true, false);
            }
            yield return new WaitForSeconds(cardManager.GetHandCardVfxDuration(HandUI.HandVFX.Freeze));
            yield break;
        }

        private void AddTraitToCardNoDuplicates(CardManager cardManager, CardState card, CardTraitData cardTraitData, bool refreshCard = true, bool ignoreUpgrades = false) 
        {
            if (card.GetTraitStates().Count > 0)
            {
                for (int ii = 0; ii < card.GetTraitStates().Count; ii++)
                {
                    if (card.GetTraitStates()[ii].GetTraitStateName() == cardTraitData.GetTraitStateName())
                    {
                        //Beyonder.Log("Skipping duplicate trait: " + cardTraitData.GetTraitStateName());
                        return;
                    }
                }
            }
            else
            {
                //Beyonder.Log("No card traits detected.");
            }
            //Beyonder.Log("Adding trait: " + cardTraitData.GetTraitStateName());
            cardManager.AddTemporaryTraitToCard(card, cardTraitData, true, false);
        }

        // Token: 0x06000769 RID: 1897 RVA: 0x00022700 File Offset: 0x00020900
        private bool CardFilterFunc(CardState checkCard, CardState playedCard, RelicManager relicManager)
        {
            //bool flag = checkCard.HasTrait(typeof(CardTraitFreeze));
            bool flag = true;
            if (cardTraits.IsNullOrEmpty())
            {
                return flag;
            }
            if (!filters.IsNullOrEmpty())
            {
                foreach (CardUpgradeMaskData filter in filters)
                {
                    flag &= !filter.FilterCard<CardState>(checkCard, relicManager);
                }
            }

            /*
            List<string> excludedTraits = AccessTools.Field(typeof(CardUpgradeMaskData), "excludedCardTraits").GetValue(filters[0]) as List<string>;
            Beyonder.Log("______________");
            Beyonder.Log("Excluded trait:");
            Beyonder.Log(excludedTraits[0]);
            Beyonder.Log(typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName);
            Beyonder.Log(excludedTraits.Contains(typeof(BeyonderCardTraitEntropic).AssemblyQualifiedName).ToString());
            Beyonder.Log("______________");
            //Beyonder.Log($"checkcard {checkCard.GetTitleKey().Localize()} == playedcard {playedCard.GetTitleKey().Localize()}: {checkCard == playedCard} || flag {flag}");
            */
            return (checkCard == playedCard) || flag;
        }
        public override void Setup(CardEffectState cardEffectState)
        {
            base.Setup(cardEffectState);
            CardUpgradeData cardUpgradeData = cardEffectState.GetParamCardUpgradeData();

            if (cardUpgradeData != null)
            {
                cardTraits = cardUpgradeData.GetTraitDataUpgrades();
                filters = cardUpgradeData.GetFilters();
            }

            cardPile = (CardPile)cardEffectState.GetParamInt();
            allCards = cardEffectState.GetParamBool();
            locKey = cardEffectState.GetParamStr();
            //hintKey = cardEffectState.GetParamSubtype();
        }

        // Token: 0x0600076A RID: 1898 RVA: 0x00022724 File Offset: 0x00020924
        public string GetTooltipBaseKey(CardEffectState cardEffectState)
        {
            return string.Empty;
        }

        // Token: 0x0600076B RID: 1899 RVA: 0x0002272C File Offset: 0x0002092C
        private bool CanAddTrait(CardEffectParams cardEffectParams)
        {
            foreach (CardState checkCard in cardEffectParams.cardManager.GetHand(false))
            {
                if (!this.CardFilterFunc(checkCard, cardEffectParams.playedCard, cardEffectParams.relicManager))
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x0400045C RID: 1116
        private bool allCards = false;
        private CardPile cardPile = CardPile.None;
        private string locKey = "ScreenDeck_Select_CustomCardEffectAddTraitToCard";
        //private string hintKey = string.Empty;
        private List<CardData> cardDatas = new List<CardData>();
        private List<CardTraitData> cardTraits = new List<CardTraitData>();
        private List<CardUpgradeMaskData> filters = new List<CardUpgradeMaskData>();
    }
}