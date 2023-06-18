using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000085 RID: 133

namespace CustomEffects
{
    public sealed class CustomCardEffectAddStatusEffectsFromSelf : CardEffectBase
    {
        // Token: 0x06000696 RID: 1686 RVA: 0x00020C4C File Offset: 0x0001EE4C
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            foreach (CharacterState target in cardEffectParams.targets)
            {
                if (target == cardEffectParams.selfTarget) 
                {
                    continue;
                }

                CardUpgradeState cardUpgradeState = new CardUpgradeState();
                cardUpgradeState.Setup();
                List<CharacterState.StatusEffectStack> list = new List<CharacterState.StatusEffectStack>();
                cardEffectParams.selfTarget.GetStatusEffects(out list, false);
                foreach (CharacterState.StatusEffectStack statusEffectStack in list)
                {
                    if (statusEffectStack.State.GetDisplayCategory() == (StatusEffectData.DisplayCategory)cardEffectState.GetParamInt())
                    {
                        cardUpgradeState.AddStatusEffectUpgradeStacks(statusEffectStack.State.GetStatusId(), statusEffectStack.Count);
                    }
                }
                yield return target.ApplyCardUpgrade(cardUpgradeState, false);
                /*
                if (!cardEffectParams.saveManager.PreviewMode)
                {
                    CardUpgradeState cardUpgradeState2 = new CardUpgradeState();
                    cardUpgradeState2.Setup();
                    CardEffectAddStatsFromSelf.ApplyCardUpgradeToTarget(target, cardUpgradeState2, cardEffectParams.cardManager);
                }
                */
                //target = null;
            }
            //List<CharacterState>.Enumerator enumerator = default(List<CharacterState>.Enumerator);
            yield break;
            //yield break;
        }
    }
}