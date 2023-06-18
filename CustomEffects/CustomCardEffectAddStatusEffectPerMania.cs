using System;
using System.Collections;
using System.Collections.Generic;
using Void.Artifacts;
using Void.Mania;

namespace CustomEffects
{

    // Token: 0x02000089 RID: 137
    public sealed class CustomCardEffectAddStatusEffectPerMania : CardEffectAddStatusEffect
    {
        // Token: 0x060006A5 RID: 1701 RVA: 0x00020E69 File Offset: 0x0001F069
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            StatusEffectStackData[] paramStatusEffects = cardEffectState.GetSourceCardEffectData().GetParamStatusEffects();
            int paramInt = cardEffectState.GetParamInt();
            bool paramBool = cardEffectState.GetParamBool();
            StatusEffectStackData statusEffectStackData = paramStatusEffects[0];
            if (statusEffectStackData == null)
            {
                yield break;
            }

            int num = ManiaManager.GetCurrentMania();
            num *= paramInt;
            num *= paramBool ? 1 : -1;

            if (num < 0 && BlackLight.HasIt()) 
            {
                num *= -1;
            }

            if (num <= 0) 
            {
                yield break;
            }

            for (int i = cardEffectParams.targets.Count - 1; i >= 0; i--)
            {
                CharacterState characterState2 = cardEffectParams.targets[i];
                CharacterState.AddStatusEffectParams addStatusEffectParams = new CharacterState.AddStatusEffectParams
                {
                    sourceRelicState = cardEffectParams.sourceRelic,
                    sourceIsHero = (cardEffectState.GetSourceTeamType() == Team.Type.Heroes),
                    fromEffectType = typeof(CustomCardEffectAddStatusEffectPerMania)
                };
                characterState2.AddStatusEffect(statusEffectStackData.statusId, num, addStatusEffectParams);
            }
            yield break;
        }

        // Token: 0x04000434 RID: 1076
        private List<CharacterState> charsInTargetRoom = new List<CharacterState>();
    }
}