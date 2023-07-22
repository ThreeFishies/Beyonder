using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Void.Mania;
using Void.Init;

namespace CustomEffects
{
    public sealed class CustomCardEffectApplyManicToDeck : CardEffectBase
    {
        public static bool IsCardManic(CardState card) 
        {
            if (card == null) { return false; }
            if (card.HasTrait(typeof(BeyonderCardTraitAfflictive)) || card.HasTrait(typeof(BeyonderCardTraitCompulsive)) || card.HasTrait(typeof(BeyonderCardTraitTherapeutic)))
            {
                return true;
            }
            return false;
        }

        public static int GetNumAfflictive(int numCardsToManic, CardState playedCard) 
        {
            int afflictiveBase = 2;
            int compulsiveBase = 2;

            int currentMania = ManiaManager.GetCurrentMania(playedCard);
            ManiaLevel maniaLevel = ManiaManager.GetCurrentManiaLevel(false, currentMania);

            if (maniaLevel == ManiaLevel.Panic || maniaLevel == ManiaLevel.BlackoutHigh) 
            {
                Beyonder.Log("Insane: All afflictive.");
                return numCardsToManic;
            }

            if (maniaLevel == ManiaLevel.Neurosis || maniaLevel == ManiaLevel.BlackoutLow) 
            {
                Beyonder.Log("Insane: All compulsive.");
                return 0;
            }

            if (currentMania > 0)
            {
                afflictiveBase += currentMania;
            }
            else 
            { 
                compulsiveBase -= currentMania;
            }

            float cardsraw = ((float)afflictiveBase / ((float)compulsiveBase + (float)afflictiveBase)) * (float)numCardsToManic;

            //Beyonder.Log($"CurrentMania: {currentMania}, NumCardsToManic: {numCardsToManic}, AfflictiveBase: {afflictiveBase}, CompulsiveBase: {compulsiveBase}, Cardsraw: {cardsraw}");
            //Beyonder.Log("Final value: " + (int)Math.Round(cardsraw, 0));

            return (int)Math.Round(cardsraw, 0);
        }

        private IEnumerator HandleCards(List<CardState> cards, CardManager cardManager, CardState playedCard)
        {
            if (cardManager == null) 
            { 
                yield break; 
            }

            if (cards.IsNullOrEmpty()) 
            {
                yield break;
            }

            int cardsToAfflictive = GetNumAfflictive(cards.Count, playedCard);

            cards.Shuffle(RngId.Battle);

            CardTraitData afflictiveTraitData = new CardTraitData();
            afflictiveTraitData.Setup(typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName);
            afflictiveTraitData.SetParamInt(1);
            CardTraitData compulsiveTraitData = new CardTraitData();
            compulsiveTraitData.Setup(typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName);
            compulsiveTraitData.SetParamInt(1);

            int ii = 0;

            foreach (CardState card in cards)
            {
                if (ii < cardsToAfflictive)
                {
                    cardManager.AddTemporaryTraitToCard(card, afflictiveTraitData, true, false);
                    //yield return new WaitForSeconds(cardManager.GetHandCardVfxDuration(HandUI.HandVFX.Freeze));
                }
                else
                {
                    cardManager.AddTemporaryTraitToCard(card, compulsiveTraitData, true, false);
                    //yield return new WaitForSeconds(cardManager.GetHandCardVfxDuration(HandUI.HandVFX.Freeze));
                }
                ii++;
            }
            yield break;
        }

        private bool GatherCardsToManic(CardEffectState cardEffectState, CardManager cardManager, out List<CardState> cards) 
        {
            cards = new List<CardState> { };

            if (cardManager == null) 
            {
                return false;
            }

            List<CardState> allCards = cardManager.GetAllCards();

            if (allCards.IsNullOrEmpty()) 
            {
                return false;
            }

            foreach (CardState card in allCards) 
            {
                if (((cardEffectState.GetTargetCardType() == CardType.Invalid) || (cardEffectState.GetTargetCardType() == card.GetCardType())) && !IsCardManic(card))
                { 
                    cards.Add(card);
                }
            }

            return true;
        }

        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (cardEffectParams.saveManager.PreviewMode) 
            {
                yield break;
            }

            if (GatherCardsToManic(cardEffectState, cardEffectParams.cardManager, out List<CardState> cardsToManic)) 
            {
                yield return HandleCards(cardsToManic, cardEffectParams.cardManager, cardEffectParams.playedCard);
            }

            cardEffectParams.cardManager.RefreshHandCards();
            yield return new WaitForSeconds(cardEffectParams.cardManager.GetHandCardVfxDuration(HandUI.HandVFX.Freeze));

            yield break;
        }
    }
}