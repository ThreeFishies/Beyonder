/*
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
    public sealed class CustomCardEffectUpdateDynamicMagicPowerOnHit : CardEffectBase
    {
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (ProviderManager.SaveManager == null || ProviderManager.SaveManager.PreviewMode)
            {
                yield break;
            }

            if (cardEffectParams.selfTarget != null) 
            {
                if (cardEffectParams.selfTarget.GetRoomStateModifiers().Count > 0) 
                {
                    foreach (IRoomStateModifier roomStateModifier in cardEffectParams.selfTarget.GetRoomStateModifiers()) 
                    {
                        CustomRoomStateDynamicMagicalPowerModifier dynamicMagicalPowerModifier = roomStateModifier as CustomRoomStateDynamicMagicalPowerModifier;

                        dynamicMagicalPowerModifier?.Update();
                    }
                }
            }

            yield break;
        }
    }
}
*/