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
        // Token: 0x06000852 RID: 2130 RVA: 0x00023E8E File Offset: 0x0002208E
        public override IEnumerator OnCardDiscarded(CardManager.DiscardCardParams discardCardParams, CardManager cardManager, RelicManager relicManager, CombatManager combatManager, RoomManager roomManager, SaveManager saveManager)
        {
            if (!discardCardParams.wasPlayed)
            {
                yield break;
            }
            int additionalCards = this.GetAdditionalCards(cardManager.GetCardStatistics(), false);

            //int maxHand = ProviderManager.SaveManager.GetBalanceData().GetMaxHandSize();

            while (additionalCards > 0)
            {
                cardManager.DrawCards(1, discardCardParams.discardCard, CardType.Invalid);
                yield return new WaitForSeconds(0.5f);
                yield return AdjustMania(cardManager.GetLastDrawnCard());
                yield return cardManager.DiscardCard(new CardManager.DiscardCardParams 
                { 
                    discardCard = cardManager.GetLastDrawnCard(),
                    effectDelay = discardCardParams.effectDelay,
                    wasPlayed = false,
                    triggeredByCard = true,
                    triggeredCard = discardCardParams.discardCard,
                    characterSummoned = discardCardParams.characterSummoned,
                    handDiscarded = discardCardParams.handDiscarded,
                    outSuppressTraitOnDiscard = discardCardParams.outSuppressTraitOnDiscard
                });

                additionalCards--;
            }
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