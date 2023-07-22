using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Void.Triggers;
using Void.Mania;
using static CharacterState;
using Void.Spells;
using Void.Artifacts;
using Void.Init;
using static UnityEngine.GraphicsBuffer;
using Trainworks.Builders;
using Trainworks.Managers;

namespace Void.Mania
{
    // Token: 0x0200008D RID: 141
    public sealed class CustomCardEffectManic : CardEffectBase
    {
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            int Affliction = 0;
            int Compulsion = 0;
            bool isTherapeutic = false;

            if (cardEffectParams == null) { yield break; }
            if (cardEffectParams.playedCard == null) { yield break; }
            if (cardEffectParams.playedCard.GetTraitStates().Count > 0) 
            {
                foreach (CardTraitState trait in cardEffectParams.playedCard.GetTraitStates()) 
                {
                    if (trait is BeyonderCardTraitAfflictive) 
                    {
                        Affliction = trait.GetParamInt();
                    }
                    if (trait is BeyonderCardTraitCompulsive)
                    {
                        Compulsion = trait.GetParamInt();
                    }
                    if (trait is BeyonderCardTraitTherapeutic) 
                    {
                        isTherapeutic = true;
                    }
                }
            }

            if (Affliction > 0) 
            {
                yield return ManiaManager.Affliction(Affliction);
            }
            if (Compulsion > 0) 
            {
                yield return ManiaManager.Compulsion(Compulsion);
            }
            if (isTherapeutic) 
            {
                yield return ManiaManager.Therapy();
            }

            yield break;
        }

        public static IEnumerator AssertCardManic(CardState card) 
        {
            if (card == null) { yield break; }

            int last = card.GetEffectStates().Count - 1;

            if (last >= 0)
            {
                for (int ii = 0; ii <= last; ii++)
                {
                    if (card.GetEffectStates()[ii].GetCardEffect() is CustomCardEffectManic)
                    {
                        if (ii == last)
                        {
                            yield break;
                        }
                        else 
                        {
                            Beyonder.Log("Deleting trait: " + card.GetEffectStates()[ii].GetCardEffect().GetType().Name);
                            card.GetEffectStates().RemoveAt(ii);
                            ii--;
                            last--;
                        }
                    }
                }
            }

            CardEffectData Manic = new CardEffectDataBuilder
            {
                EffectStateName = typeof(CustomCardEffectManic).AssemblyQualifiedName
            }.Build();

            CardEffectState ManicState = new CardEffectState();
            ManicState.Setup(Manic,card);
            card.GetEffectStates().Add(ManicState);

            yield break;
        }
    }
}