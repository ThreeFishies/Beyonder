//CustomCardEffectAddSpawnerBattleCard
using System;
using System.Collections;
using System.Collections.Generic;
using Trainworks.Managers;
using UnityEngine;

namespace CustomEffects
{

    // Token: 0x02000082 RID: 130
    public sealed class CustomCardEffectAddSpawnerBattleCard : CardEffectBase
    {
        // Token: 0x17000049 RID: 73
        // (get) Token: 0x0600068A RID: 1674 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanPlayAfterBossDead
        {
            get
            {
                return false;
            }
        }

        // Token: 0x1700004A RID: 74
        // (get) Token: 0x0600068B RID: 1675 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return false;
            }
        }

        // Token: 0x0600068C RID: 1676 RVA: 0x00020BB4 File Offset: 0x0001EDB4
        public static void GetCardText(CardEffectData cardEffectData, out string outText)
        {
            outText = "CardEffectAddBattleCard_CardText".Localize(null);
        }

        // Token: 0x0600068D RID: 1677 RVA: 0x00020BC3 File Offset: 0x0001EDC3
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            int num = Mathf.Max(1, cardEffectState.GetAdditionalParamInt());
            int num2 = num;
            int num3 = cardEffectParams.cardManager.GetMaxHandSize() - cardEffectParams.cardManager.GetNumCardsInHand();
            if (num3 > 0)
            {
                num2 = Mathf.Min(num2, num3);
            }
            for (int i = 0; i < num; i++)
            {
                if (cardEffectState != null && cardEffectParams != null && cardEffectParams.cardManager != null)
                {
                    CardPile paramInt = (CardPile)cardEffectState.GetParamInt();
                    this.toProcessCards.Clear();

                    if (cardEffectParams.selfTarget != null && cardEffectParams.selfTarget.GetSpawnerCard() != null) 
                    {
                        string cardDataID = cardEffectParams.selfTarget.GetSpawnerCard().GetCardDataID();
                        this.toProcessCards.Add(CustomCardManager.GetCardDataByID(cardDataID));
                    }
                    
                    //if (cardEffectState.GetFilteredCardListFromPool(cardEffectParams.relicManager, ref this.toProcessCards))
                    if (this.toProcessCards.Count > 0)
                    {
                        if (cardEffectState.GetFilterBasedOnMainSubClass())
                        {
                            ClassData mainClass = cardEffectParams.saveManager.GetMainClass();
                            ClassData subClass = cardEffectParams.saveManager.GetSubClass();
                            for (int j = this.toProcessCards.Count - 1; j >= 0; j--)
                            {
                                if (this.toProcessCards[j].GetLinkedClass() != null && this.toProcessCards[j].GetLinkedClass() != mainClass && this.toProcessCards[j].GetLinkedClass() != subClass)
                                {
                                    this.toProcessCards.RemoveAt(j);
                                }
                            }
                        }
                        int index = RandomManager.Range(0, this.toProcessCards.Count, RngId.Battle);
                        CardData cardData = this.toProcessCards[index];
                        int num4 = 0;
                        int num5 = 0;
                        this.toProcessCharacters.Clear();
                        RoomState selectedRoom = cardEffectParams.GetSelectedRoom();
                        CharacterState selfTarget = cardEffectParams.selfTarget;
                        if (selfTarget != null)
                        {
                            selectedRoom.AddCharactersToList(this.toProcessCharacters, selfTarget.GetTeamType(), false);
                        }
                        else
                        {
                            num4 = i;
                            num5 = num2;
                        }
                        for (int k = 0; k < this.toProcessCharacters.Count; k++)
                        {
                            CharacterState characterState = this.toProcessCharacters[k];
                            foreach (CharacterTriggerState characterTriggerState in characterState.GetTriggers())
                            {
                                List<CardEffectState> effects = characterTriggerState.GetEffects(true);
                                bool flag = false;
                                using (List<CardEffectState>.Enumerator enumerator2 = effects.GetEnumerator())
                                {
                                    while (enumerator2.MoveNext())
                                    {
                                        if (enumerator2.Current.GetCardEffect() is CardEffectAddBattleCard)
                                        {
                                            if (selfTarget == characterState)
                                            {
                                                num4 = num5;
                                            }
                                            flag = true;
                                            num5++;
                                            break;
                                        }
                                    }
                                }
                                if (flag)
                                {
                                    break;
                                }
                            }
                        }
                        if (num4 == 0 && num5 == 0)
                        {
                            num4 = i;
                            num5 = num2;
                        }
                        CardManager.AddCardUpgradingInfo addCardUpgradingInfo = new CardManager.AddCardUpgradingInfo();
                        if (cardEffectState.GetParamCardUpgradeData() != null)
                        {
                            addCardUpgradingInfo.upgradeDatas.Add(cardEffectState.GetParamCardUpgradeData());
                        }
                        addCardUpgradingInfo.tempCardUpgrade = true;
                        addCardUpgradingInfo.upgradingCardSource = cardEffectState.GetParentCardState();
                        if (cardEffectState.GetCopyModifiersFromSource())
                        {
                            addCardUpgradingInfo.ignoreTempUpgrades = cardEffectState.GetIgnoreTempModifiersFromSource();
                            addCardUpgradingInfo.copyModifiersFromCard = cardEffectParams.playedCard;
                            if (cardEffectParams.selfTarget != null && cardEffectParams.selfTarget.GetSpawnerCard() != null)
                            {
                                addCardUpgradingInfo.copyModifiersFromCard = cardEffectParams.selfTarget.GetSpawnerCard();
                            }
                        }
                        cardEffectParams.cardManager.AddCard(cardData, paramInt, num4, num5, false, false, addCardUpgradingInfo);
                    }
                }
            }
            yield break;
        }

        // Token: 0x04000431 RID: 1073
        private List<CardData> toProcessCards = new List<CardData>();

        // Token: 0x04000432 RID: 1074
        private List<CharacterState> toProcessCharacters = new List<CharacterState>();
    }
}