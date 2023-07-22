using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Void.Triggers;
using Void.Mania;
using Void.Init;
using static CharacterState;
using Void.Spells;
using Void.Artifacts;
using static UnityEngine.GraphicsBuffer;
using Trainworks.Builders;
using Trainworks.Managers;
using ShinyShoe.Audio;

namespace CustomEffects
{
    public sealed class CustomCardEffectPlaySoundCue : CardEffectBase
    {
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (ProviderManager.SaveManager == null || ProviderManager.SaveManager.PreviewMode) 
            {
                yield break;
            }

            if (cardEffectState.GetParamStr().IsNullOrEmpty()) 
            {
                Beyonder.Log("Tried to invoke CustomCardEffectPlaySoundCue withut a sound cue! Please set ParamStr.");
                yield break;
            }

            if (ProviderManager.TryGetProvider<SoundManager>(out SoundManager soundManager)) 
            {
                soundManager.PlaySfx(cardEffectState.GetParamStr(), AudioSfxPriority.Normal, false);
            }

            yield break;
        }
    }
}