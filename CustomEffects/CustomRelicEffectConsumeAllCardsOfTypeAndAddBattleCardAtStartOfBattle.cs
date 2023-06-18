//CustomRelicEffectConsumeAllCardsOfTypeAndAddBattleCardAtStartOfBattle

using System;
using System.Collections;
using System.Collections.Generic;
using Trainworks.Constants;
using Void.Init;

namespace CustomEffects
{
    public class CustomRelicEffectConsumeAllCardsOfTypeAndAddBattleCardAtStartOfBattle : RelicEffectAddCardBase, IRelicEffect, IStartOfCombatRelicEffect
    {
        private CardType cardType { get; set; }
        private SubtypeData subType { get; set; }

        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData) 
        {
            cardType = relicEffectData.GetParamCardType();
            this.getAllCardsInPool = true;
            this.subType = relicEffectData.GetParamCharacterSubtype();

            base.Initialize(relicState, relicData, relicEffectData);
        }

        public IEnumerator ApplyEffect(RelicEffectParams relicEffectParams)
        {
            if (relicEffectParams.cardManager == null) 
            {
                yield break;
            }

            List<CardState> allCardsInDeck = relicEffectParams.cardManager.GetAllCards();

            foreach (CardState card in allCardsInDeck) 
            {
                if (this.cardType == card.GetCardType()) 
                {
                    bool flag = relicEffectParams.relicManager.GetRelicEffect<RelicEffectMonstersAreAllSubtypes>() != null;
                    bool flag2 = card.IsSpawnerCard() && !relicEffectParams.monsterManager.DoesCharacterPassSubtypeCheck(card.GetSpawnCharacterData(), subType);

                    if (flag || flag2)
                    {
                        relicEffectParams.cardManager.MoveToStandByPile(card, false, true, new RemoveFromStandByCondition(() => CardPile.ExhaustedPile));
                    }
                }
            }

            if (this.cardPool == null) 
            {
                yield break;
            }

            foreach (CardData card1 in this.cardPool.GetAllChoices()) 
            {
                if (relicEffectParams.cardManager.GetHand().Count < relicEffectParams.cardManager.GetMaxHandSize())
                {
                    relicEffectParams.cardManager.AddCard(card1, CardPile.HandPile, 1, 1, true, false, null);
                }
                else
                {
                    relicEffectParams.cardManager.AddCard(card1, CardPile.DeckPileTop, 1, 1, true, false, null);
                }
            }

            yield break;
        }

        public bool TestEffect(RelicEffectParams relicEffectParams)
        {
            return true;
        }
    }
}