using System;
using System.Collections;
using System.Collections.Generic;
using Trainworks.Managers;
using UnityEngine;
using Void.Mania;
using Void.Init;
using Void.Triggers;

namespace CustomEffects
{
    // Token: 0x020000DD RID: 221
    public sealed class CustomCardTraitScalingAddCardsAndDropThem : CardTraitState
    {
        private int lastKnownSelectedRoom;

        // Token: 0x06000852 RID: 2130 RVA: 0x00023E8E File Offset: 0x0002208E
        public override IEnumerator OnCardDiscarded(CardManager.DiscardCardParams discardCardParams, CardManager cardManager, RelicManager relicManager, CombatManager combatManager, RoomManager roomManager, SaveManager saveManager)
        {
            if (!discardCardParams.wasPlayed)
            {
                yield break;
            }

            //Beyonder.Log("Starting " + discardCardParams.discardCard.GetTitleKey().Localize() + " effect. " + discardCardParams.discardCard.GetID());

            lastKnownSelectedRoom = -1;

            if (ManiaManager.SetSelectedRoomFlag == false)
            {
                lastKnownSelectedRoom = ManiaManager.SelectedRoomIndex;
            }
            else if (roomManager != null)
            {
                lastKnownSelectedRoom = roomManager.GetSelectedRoom();
            }

            int additionalCards = this.GetAdditionalCards(cardManager.GetCardStatistics(), false);

            bool hasReshuffledFlag = false;
            int deckToZeroCount = cardManager.GetDrawPile().Count;
            int cardsInDeck = cardManager.GetDrawPile().Count + cardManager.GetDiscardPile().Count;
            int cycledCardsCount = 0;
            bool shouldCycleFlag = false;

            if (cardsInDeck <= 0) 
            {
                Beyonder.Log("There are no cards left. Aborting Mental Disorder.");
                yield break;
            }

            if (additionalCards >= cardManager.GetDiscardPile().Count + cardManager.GetDrawPile().Count)
            {
                //Beyonder.Log("Warning. Deck may be shuffled twice or more! This may cause issues.", BepInEx.Logging.LogLevel.Warning);
                shouldCycleFlag = true;
            }

            while (additionalCards > 0)
            {
                cardManager.DrawCards(1, discardCardParams.discardCard, CardType.Invalid);
                deckToZeroCount--;

                if (deckToZeroCount < 0)
                {
                    hasReshuffledFlag = true;
                }

                cardsInDeck = cardManager.GetDrawPile().Count + cardManager.GetDiscardPile().Count;

                //Beyonder.Log("Cards left to drop: " + additionalCards);
                //Beyonder.Log("Deck size is: " + cardsInDeck);

                if (cardsInDeck <= 0) 
                {
                    Beyonder.Log("There are no cards left! Aborting Mental Disorder.");
                    yield break; 
                }

                yield return new WaitForSeconds(0.5f);
                if (cardManager.GetLastDrawnCard().HasTrait(typeof(CardTraitTreasure)))
                {
                    if (roomManager != null && lastKnownSelectedRoom != -1)
                    {
                        yield return roomManager.GetRoomUI().SetSelectedRoom(lastKnownSelectedRoom);
                    }
                }
                else 
                {
                    yield return AdjustMania(cardManager.GetLastDrawnCard());
                }

                CardState lastDrawnCard = cardManager.GetLastDrawnCard();

                yield return cardManager.DiscardCard(new CardManager.DiscardCardParams 
                { 
                    discardCard = lastDrawnCard,
                    effectDelay = discardCardParams.effectDelay,
                    wasPlayed = false,
                    triggeredByCard = true,
                    triggeredCard = discardCardParams.discardCard,
                    characterSummoned = discardCardParams.characterSummoned,
                    handDiscarded = discardCardParams.handDiscarded,
                    outSuppressTraitOnDiscard = discardCardParams.outSuppressTraitOnDiscard                    
                });

                if (shouldCycleFlag && hasReshuffledFlag)
                {
                    cardManager.GetDiscardBufferPile().Remove(lastDrawnCard);
                    cardManager.GetDiscardPile().Remove(lastDrawnCard);
                    cardManager.GetDrawPile().Insert(0,lastDrawnCard);
                    cycledCardsCount++;

                    if (cycledCardsCount >= cardsInDeck) 
                    {
                        cycledCardsCount = 0;
                        cardManager.GetDrawPile().Shuffle(RngId.Battle);
                        //Beyonder.Log("Simulating a reshuffle event.");
                    }

                    //Beyonder.Log("Cycling deck. " + lastDrawnCard.GetTitleKey().Localize() + " has been returned to the draw pile.");
                }

                additionalCards--;
            }

            //Beyonder.Log("Ending " + discardCardParams.discardCard.GetTitleKey().Localize() + " effect. " + discardCardParams.discardCard.GetID());

            yield break;
        }

        private IEnumerator AdjustMania(CardState droppedCard) 
        {
            //Beyonder.Log("Dropping Card: " + droppedCard.GetTitleKey().Localize());
            bool flag = false;

            if (droppedCard.HasTrait(typeof(BeyonderCardTraitCompulsive))) 
            {
                //Beyonder.Log("Dropped card is Compulsive.");
                yield return ManiaManager.Compulsion(1, true);
                if (ManiaManager.Mania < 0)
                {
                    flag = true;
                }
            }
            if (droppedCard.HasTrait(typeof(BeyonderCardTraitAfflictive)))
            {
                //Beyonder.Log("Dropped card is Afflictive.");
                yield return ManiaManager.Affliction(1, true);
                if (ManiaManager.Mania > 0)
                {
                    flag = true;
                }
            }
            if (droppedCard.HasTrait(typeof(BeyonderCardTraitTherapeutic)))
            {
                //Beyonder.Log("Dropped card is Therapeutic.");
                yield return ManiaManager.Therapy();
            }
            if (!flag) { yield break; }

            CustomTriggerManager.RunTriggerQueueRemote();
            
            if (ProviderManager.CombatManager != null)
            {
                do
                {
                    yield return new WaitForSeconds(0.1f);
                } while (ProviderManager.CombatManager.IsRunningTriggerQueue);
            }

            yield break;
        }

        // Token: 0x06000853 RID: 2131 RVA: 0x00023EAC File Offset: 0x000220AC
        private int GetAdditionalCards(CardStatistics cardStatistics, bool forPreviewText)
        {
            CardStatistics.StatValueData statValueData = base.StatValueData;
            statValueData.forPreviewText = forPreviewText;
            int statValue = cardStatistics.GetStatValue(statValueData);
            return base.GetParamInt() * statValue;
        }

        // Token: 0x06000854 RID: 2132 RVA: 0x0001C727 File Offset: 0x0001A927
        public override bool HasMultiWordDesc()
        {
            return true;
        }

        // Token: 0x06000855 RID: 2133 RVA: 0x00023ED8 File Offset: 0x000220D8
        public override string GetCurrentEffectText(CardStatistics cardStatistics, SaveManager saveManager, RelicManager relicManager)
        {
            if (cardStatistics != null && cardStatistics.GetStatValueShouldDisplayOnCardNow(base.StatValueData))
            {
                return string.Format("CardTraitScalingAddCards_CurrentScaling_CardText".Localize(null), this.GetAdditionalCards(cardStatistics, true));
            }
            return string.Empty;
        }
    }
}