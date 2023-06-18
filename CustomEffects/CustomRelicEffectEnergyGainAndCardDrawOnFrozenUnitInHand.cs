//CustomRelicEffectEnergyGainAndCardDrawOnFrozenUnitInHand

using System;
using System.Collections;
using System.Collections.Generic;
using Void.Init;

namespace CustomEffects
{

    // Token: 0x020002B3 RID: 691
    public sealed class CustomRelicEffectEnergyGainAndCardDrawOnFrozenUnitInHand : RelicEffectBase, IStartOfPlayerTurnAfterDrawRelicEffect
    {
        // Token: 0x170001E2 RID: 482
        // (get) Token: 0x06001769 RID: 5993 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return false;
            }
        }

        // Token: 0x0600176A RID: 5994 RVA: 0x0005BE4B File Offset: 0x0005A04B
        public override string GetActivatedDescription()
        {
            return string.Format(base.GetActivatedDescription(), this.energy);
        }

        // Token: 0x0600176B RID: 5995 RVA: 0x0005BE63 File Offset: 0x0005A063
        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, relicData, relicEffectData);
            this.energy = relicEffectData.GetParamInt();
            this.teamFilter = relicEffectData.GetParamSourceTeam();
            this.cardType = relicEffectData.GetParamCardType();
            this.cardTrait = relicEffectData.GetTargetTrait();
        }

        public bool TestEffect(RelicEffectParams relicEffectParams)
        {
            List<CardState> hand = relicEffectParams.cardManager.GetHand();

            foreach (CardState card in hand) 
            {
                if (card.GetCardType() == this.cardType) 
                {
                    foreach (CardTraitData trait in card.GetTraits())
                    {
                        //Beyonder.Log($"Testing Cardtrait: {trait.GetTraitStateName()} against: {this.cardTrait} for card: {card.GetTitle()}");

                        if (trait.GetTraitStateName() == this.cardTrait) 
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public IEnumerator ApplyEffect(RelicEffectParams relicEffectParams)
        {
            if (relicEffectParams.saveManager.PreviewMode)
            {
                yield break;
            }

            base.NotifyRoomRelicTriggered(new RelicEffectParams
            {
                relicManager = relicEffectParams.relicManager,
                playerManager = relicEffectParams.playerManager,
                roomManager = relicEffectParams.roomManager,
                saveManager = relicEffectParams.saveManager
            });
            relicEffectParams.playerManager.AddEnergy(this.energy);
            relicEffectParams.cardManager.DrawCards(1, null, CardType.Invalid);
            yield break;
        }

        // Token: 0x04000C6D RID: 3181
        private int energy;

        // Token: 0x04000C6E RID: 3182
        private Team.Type teamFilter;

        private CardType cardType;

        private string cardTrait;
    }
}