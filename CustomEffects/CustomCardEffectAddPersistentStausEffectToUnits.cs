using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomEffects
{

    // Token: 0x0200008D RID: 141
    public sealed class CustomCardEffectAddPersistentStausEffectToUnits : CardEffectBase, ICardEffectTipTooltip
    {
        // Token: 0x060006BB RID: 1723 RVA: 0x00021094 File Offset: 0x0001F294
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            return cardEffectParams.targets.Count > 0;
        }

        // Token: 0x060006BC RID: 1724 RVA: 0x000210A4 File Offset: 0x0001F2A4
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            foreach (CharacterState target in cardEffectParams.targets)
            {
                CardUpgradeState upgradeState = new CardUpgradeState();
                upgradeState.Setup(cardEffectState.GetParamCardUpgradeData(), false);
                if (cardEffectParams.playedCard != null && !upgradeState.IsUnitSynthesisUpgrade())
                {
                    foreach (CardTraitState cardTraitState in cardEffectParams.playedCard.GetTraitStates())
                    {
                        cardTraitState.OnApplyingCardUpgradeToUnit(cardEffectParams.playedCard, target, upgradeState, cardEffectParams.cardManager);
                    }
                }
                if (cardEffectParams.playedCard != null && upgradeState.GetStatusEffectUpgrades().Count > 0)
                {
                    foreach (StatusEffectStackData statusEffectStackData in upgradeState.GetStatusEffectUpgrades()) 
                    {
                        foreach (CardTraitState cardTraitState in cardEffectParams.playedCard.GetTraitStates())
                        {
                            //This should work with Doublestack and Entropic
                            statusEffectStackData.count = cardTraitState.GetModifiedStatusEffectStacks(statusEffectStackData);
                        }
                    }
                }
                bool attackDamage = upgradeState.GetAttackDamage() != 0;
                int additionalHP = upgradeState.GetAdditionalHP();
                int additionalSize = upgradeState.GetAdditionalSize();
                string text = attackDamage ? this.GetAttackNotificationText(upgradeState) : null;
                string text2 = (additionalHP != 0) ? this.GetHPNotificationText(upgradeState) : null;
                string text3 = (additionalSize != 0) ? this.GetSizeNotificationText(upgradeState) : null;
                string[] array = new string[]
                {
                text,
                text2,
                text3
                };
                int num = array.Count((string n) => n != null);
                string text4 = string.Empty;
                if (num == 3)
                {
                    text4 = string.Format("TextFormat_SpacedItems3".Localize(null), array[0], array[1], array[2]);
                }
                else if (num == 2)
                {
                    List<string> list = (from n in array
                                         where !string.IsNullOrEmpty(n)
                                         select n).ToList<string>();
                    text4 = string.Format("TextFormat_SpacedItems".Localize(null), list[0], list[1]);
                }
                else if (num == 1)
                {
                    text4 = (from n in array
                             where !string.IsNullOrEmpty(n)
                             select n).ToList<string>()[0];
                }
                if (text4 != null)
                {
                    base.NotifyHealthEffectTriggered(cardEffectParams.saveManager, cardEffectParams.popupNotificationManager, text4, target.GetCharacterUI());
                }
                yield return target.ApplyCardUpgrade(upgradeState, false);
                CardState spawnerCard = target.GetSpawnerCard();
                if (spawnerCard != null && !cardEffectParams.saveManager.PreviewMode && (target.GetSourceCharacterData() == spawnerCard.GetSpawnCharacterData() || spawnerCard.GetSpawnCharacterData() == null))
                {
                    CardAnimator.CardUpgradeAnimationInfo type = new CardAnimator.CardUpgradeAnimationInfo(spawnerCard, upgradeState, CardPile.None, CardPile.DeckPileTop, 1, 1);
                    CardAnimator.DoAddRecentCardUpgrade.Dispatch(type);
                    spawnerCard.GetTemporaryCardStateModifiers().AddUpgrade(upgradeState, null);
                    spawnerCard.UpdateCardBodyText(null);
                    CardManager cardManager = cardEffectParams.cardManager;
                    if (cardManager != null)
                    {
                        cardManager.RefreshCardInHand(spawnerCard, true);
                    }
                }
                upgradeState = null;
            }
            yield break;
        }

        // Token: 0x060006BD RID: 1725 RVA: 0x000210C1 File Offset: 0x0001F2C1
        private string GetAttackNotificationText(CardUpgradeState upgradeState)
        {
            return CardEffectBuffDamage.GetNotificationText(upgradeState.GetAttackDamage());
        }

        // Token: 0x060006BE RID: 1726 RVA: 0x000210D0 File Offset: 0x0001F2D0
        private string GetHPNotificationText(CardUpgradeState upgradeState)
        {
            int additionalHP = upgradeState.GetAdditionalHP();
            if (additionalHP >= 0)
            {
                return CardEffectBuffMaxHealth.GetNotificationText(additionalHP);
            }
            return CardEffectDebuffMaxHealth.GetNotificationText(Mathf.Abs(additionalHP));
        }

        // Token: 0x060006BF RID: 1727 RVA: 0x000210FA File Offset: 0x0001F2FA
        private string GetSizeNotificationText(CardUpgradeState upgradeState)
        {
            return string.Format("SizeNotificationText".Localize(null), string.Format("TextFormat_Add".Localize(null), upgradeState.GetAdditionalSize()));
        }

        // Token: 0x060006C0 RID: 1728 RVA: 0x00021127 File Offset: 0x0001F327
        public override void GetTooltipsStatusList(CardEffectState cardEffectState, ref List<string> outStatusIdList)
        {
            CardEffectAddTempCardUpgradeToUnits.GetTooltipsStatusList(cardEffectState.GetSourceCardEffectData(), ref outStatusIdList);
        }

        // Token: 0x060006C2 RID: 1730 RVA: 0x00021198 File Offset: 0x0001F398
        public string GetTipTooltipKey(CardEffectState cardEffectState)
        {
            if (cardEffectState.GetParamCardUpgradeData() != null && cardEffectState.GetParamCardUpgradeData().HasUnitStatUpgrade())
            {
                return "TipTooltip_StatChangesStick";
            }
            return null;
        }
    }
}