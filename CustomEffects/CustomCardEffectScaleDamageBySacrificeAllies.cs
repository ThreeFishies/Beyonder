using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Trainworks.Builders;
using static CharacterState;
using Void.Init;

namespace CustomEffects
{

    // Token: 0x0200008D RID: 141
    public sealed class CustomCardEffectScaleDamageBySacrificeAllies : CardEffectBase, ICardEffectTipTooltip
    {
        public static CharacterState LastTarget = null;
        public static int NumAlliesSacrificed = 0;

        // Token: 0x060006BB RID: 1723 RVA: 0x00021094 File Offset: 0x0001F294
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            GetNumAlliesSacrificed(cardEffectState, cardEffectParams);
            return cardEffectParams.targets.Count > 0;
        }

        public static int GetNumAlliesSacrificed(CardEffectState cardEffectState = null, CardEffectParams cardEffectParams = null) 
        {
            if (cardEffectParams == null || cardEffectState == null) 
            { 
                return NumAlliesSacrificed;
            }

            int room = -1;
            NumAlliesSacrificed = 0;

            List<CharacterState> unluckySaps = new List<CharacterState>();
            if (!cardEffectParams.targets.IsNullOrEmpty() && cardEffectParams.targets[0] != null)
            {
                room = cardEffectParams.targets[0].GetCurrentRoomIndex();
                cardEffectParams.roomManager.GetRoom(room).AddCharactersToList(unluckySaps, cardEffectParams.targets[0].GetTeamType());
                NumAlliesSacrificed = unluckySaps.Count;
                if (unluckySaps.Contains(cardEffectParams.targets[0]))
                {
                    NumAlliesSacrificed--;
                }
                LastTarget = cardEffectParams.targets[0];
            }

            return NumAlliesSacrificed;
        }

        public CardUpgradeData GenerateParamCardUpgradeData(CardEffectState cardEffectState, CardEffectParams cardEffectParams) 
        {
            int damagePerAlly = cardEffectState.GetParamInt();
            NumAlliesSacrificed = GetNumAlliesSacrificed(cardEffectState, cardEffectParams);

            CardUpgradeData cardUpgradeData = new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "CustomCardEffectScaleDamageBySacrificeAlliesEffect_" + (damagePerAlly * NumAlliesSacrificed),
                BonusDamage = damagePerAlly * NumAlliesSacrificed,
            }.Build();

            return cardUpgradeData;
        }

        // Token: 0x060006BC RID: 1724 RVA: 0x000210A4 File Offset: 0x0001F2A4
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            foreach (CharacterState target in cardEffectParams.targets)
            {
                CardUpgradeState upgradeState = new CardUpgradeState();
                upgradeState.Setup(GenerateParamCardUpgradeData(cardEffectState, cardEffectParams), false);
                if (cardEffectParams.playedCard != null && !upgradeState.IsUnitSynthesisUpgrade())
                {
                    foreach (CardTraitState cardTraitState in cardEffectParams.playedCard.GetTraitStates())
                    {
                        cardTraitState.OnApplyingCardUpgradeToUnit(cardEffectParams.playedCard, target, upgradeState, cardEffectParams.cardManager);
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
                if (cardEffectState.GetParamStatusEffectStackData().Length > 0)
                {
                    AddStatusEffectParams addStatusEffectParams = new AddStatusEffectParams()
                    {
                        cardManager = cardEffectParams.cardManager,
                        fromEffectType = typeof(CustomCardEffectScaleDamageBySacrificeAllies),
                        overrideImmunity = false,
                        relicEffects = null,
                        sourceCardState = cardEffectParams.playedCard,
                        sourceIsHero = false,
                        sourceRelicState = null,
                        spawnEffect = false
                    };
                    //Beyonder.Log($"+{NumAlliesSacrificed} Chronic from Highlander.");
                    target.AddStatusEffect(cardEffectState.GetParamStatusEffectStackData()[0].statusId, cardEffectState.GetParamStatusEffectStackData()[0].count, addStatusEffectParams);
                }
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
                //target = null;
            }

            if (cardEffectParams.targets.Count > 0)
            {
                int room = cardEffectParams.targets[0].GetCurrentRoomIndex();
                List<CharacterState> allies = new List<CharacterState> { };
                cardEffectParams.roomManager.GetRoom(room).AddCharactersToList(allies, cardEffectParams.targets[0].GetTeamType(), false);

                foreach (CharacterState poorBastard in allies) 
                {
                    if (!cardEffectParams.targets.Contains(poorBastard)) 
                    {
                        yield return poorBastard.Sacrifice(cardEffectParams.playedCard);
                    }
                }
            }
            //List<CharacterState>.Enumerator enumerator = default(List<CharacterState>.Enumerator);
            yield break;
            //yield break;
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

        // Token: 0x060006C1 RID: 1729 RVA: 0x00021138 File Offset: 0x0001F338
        public static void GetTooltipsStatusList(CardEffectData cardEffectData, ref List<string> outStatusIdList)
        {
            foreach (StatusEffectStackData statusEffectStackData in cardEffectData.GetParamCardUpgradeData().GetStatusEffectUpgrades())
            {
                outStatusIdList.Add(statusEffectStackData.statusId);
            }
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