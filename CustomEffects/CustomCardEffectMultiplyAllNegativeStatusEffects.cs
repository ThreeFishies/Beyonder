using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomEffects
{
    // Token: 0x020004B3 RID: 1203
    public sealed class CustomCardEffectMultiplyAllNegativeStatusEffects : CardEffectBase
    {
        // Token: 0x06002B7B RID: 11131 RVA: 0x000AAEF6 File Offset: 0x000A90F6
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            List<CharacterState.StatusEffectStack> list = new List<CharacterState.StatusEffectStack>();
            for (int i = cardEffectParams.targets.Count - 1; i >= 0; i--)
            {
                CharacterState characterState = cardEffectParams.targets[i];
                characterState.GetStatusEffects(out list, false);
                foreach (CharacterState.StatusEffectStack statusEffectStack in list)
                {
                    if ((statusEffectStack.State.GetDisplayCategory() == StatusEffectData.DisplayCategory.Negative) && statusEffectStack.State.IsStackable())
                    {
                        string statusId = statusEffectStack.State.GetStatusId();
                        int paramInt = cardEffectState.GetParamInt();
                        int count = statusEffectStack.Count;
                        int num = count * paramInt;
                        if (num > 0)
                        {
                            int num2 = num - count;
                            if (num2 > 0)
                            {
                                CharacterState.AddStatusEffectParams addStatusEffectParams = new CharacterState.AddStatusEffectParams
                                {
                                    sourceRelicState = cardEffectParams.sourceRelic,
                                    sourceCardState = cardEffectParams.playedCard,
                                    cardManager = cardEffectParams.cardManager,
                                    sourceIsHero = (cardEffectState.GetSourceTeamType() == Team.Type.Heroes),
                                    fromEffectType = typeof(CardEffectMultiplyAllStatusEffects)
                                };
                                characterState.AddStatusEffect(statusId, num2, addStatusEffectParams);
                            }
                        }
                    }
                }
            }
            yield break;
        }
    }
}