using System;
using System.Collections.Generic;
using Trainworks.Managers;

namespace CustomEffects
{

    // Token: 0x020000EF RID: 239
    public sealed class CustomCardTraitShowManicTargets : CardTraitState
    {
        // Token: 0x060008A2 RID: 2210 RVA: 0x00024DEC File Offset: 0x00022FEC
        private int GetNumCompulsiveTargets(CardState cardState, CardStatistics cardStatistics, out int NumAfflictive)
        {
            int num = 0;
            NumAfflictive = 0;

            foreach (CardEffectState cardEffectState in cardState.GetEffectStates())
            {
                if (cardEffectState.GetCardEffect() is CustomCardEffectApplyManicToDeck)
                {
                    CustomCardEffectApplyManicToDeck applyManicToDeck = cardEffectState.GetCardEffect() as CustomCardEffectApplyManicToDeck;

                    if (ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager))
                    {
                        applyManicToDeck.GatherCardsToManic(cardEffectState, cardManager, out List<CardState> cards);
                        num = cards.Count;
                        NumAfflictive = CustomCardEffectApplyManicToDeck.GetNumAfflictive(num, cardState);
                    }

                    break;
                }
            }
            return num - NumAfflictive;
        }

        // Token: 0x060008A3 RID: 2211 RVA: 0x00024F44 File Offset: 0x00023144
        public override string GetCurrentEffectText(CardStatistics cardStatistics, SaveManager saveManager, RelicManager relicManager)
        {
            if (cardStatistics != null && cardStatistics.GetIsInActiveBattle())
            {
                int numAfflictive = 0;
                int numCompulsive = 0;
                numCompulsive = this.GetNumCompulsiveTargets(base.GetCard(), cardStatistics, out numAfflictive);

                return string.Format("CustomCardTraitShowManicTargets_CardText".Localize(null), numAfflictive, numCompulsive);
            }
            return string.Empty;
        }
    }
}