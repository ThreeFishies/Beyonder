using System;
using System.Collections;
using System.Collections.Generic;
using Void.Artifacts;
using Void.Mania;

namespace CustomEffects
{
    // Token: 0x020000B1 RID: 177
    public sealed class CustomCardEffectKillConditional : CardEffectBase
    {
        // Token: 0x0600078E RID: 1934 RVA: 0x00022AC0 File Offset: 0x00020CC0
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            foreach (CharacterState target in cardEffectParams.targets)
            {
                if (!base.IsTargetValid(cardEffectState, target, true))
                {
                    return false;
                }
            }

            if (BlackLight.HasIt()) 
            {
                int currentMania = ManiaManager.GetCurrentMania();
                int target = cardEffectState.GetParamInt();
                if (currentMania < 0) { currentMania *= -1; }
                if (target < 0) { target *= -1; }
                if (currentMania >= target ) { return true; }
                return false;
            }

            if (cardEffectState.GetParamBool()) 
            {
                if (ManiaManager.GetCurrentMania() < cardEffectState.GetParamInt()) 
                {
                    return false;
                }
            }

            if (!cardEffectState.GetParamBool())
            {
                if (ManiaManager.GetCurrentMania() > cardEffectState.GetParamInt())
                {
                    return false;
                }
            }

            return true;
        }

        // Token: 0x0600078F RID: 1935 RVA: 0x00022B20 File Offset: 0x00020D20
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            /*
            if (cardEffectState.GetParamBool())
            {
                if (ManiaManager.GetCurrentMania() < cardEffectState.GetParamInt())
                {
                    yield break;
                }
            }

            if (!cardEffectState.GetParamBool())
            {
                if (ManiaManager.GetCurrentMania() > cardEffectState.GetParamInt())
                {
                    yield break;
                }
            }
            */

            foreach (CharacterState characterState in cardEffectParams.targets)
            {
                yield return characterState.Sacrifice(null, false, false);
            }
            //List<CharacterState>.Enumerator enumerator = default(List<CharacterState>.Enumerator);
            yield break;
            //yield break;
        }
    }
}