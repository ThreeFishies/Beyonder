using System;
using System.Collections;
using System.Collections.Generic;
using ShinyShoe.Logging;
using Void.Status;
using Void.Init;
using UnityEngine;
using Trainworks.Managers;

namespace CustomEffects
{
    public class CustomRelicEffectEmbraceTheMadness : RelicEffectBase, IRelicEffect, IStartOfCombatRelicEffect
    {
        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, relicData, relicEffectData);
            this.Min = relicEffectData.GetParamMinInt();
            this.Max = relicEffectData.GetParamMaxInt();
            this.ParamInt = relicEffectData.GetParamInt();
            this.useIntRange = relicEffectData.GetUseIntRange();
            this.manicToDeck = new CustomCardEffectApplyManicToDeck
            {
                overrideMania = true,
                overrideValue = this.ParamInt
            };
        }

        public IEnumerator ApplyEffect(RelicEffectParams relicEffectParams)
        {
            CardEffectParams cardEffectParams = new CardEffectParams()
            {
                saveManager = ProviderManager.SaveManager,
                cardManager = ProviderManager.CombatManager.GetCardManager(),
                playedCard = null,
            };

            this.manicToDeck.overrideValue = GetBaseMania();

            yield return this.manicToDeck.ApplyEffect(null, cardEffectParams);
        }

        public bool TestEffect(RelicEffectParams relicEffectParams)
        {
            return (ProviderManager.SaveManager != null && ProviderManager.CombatManager != null && ProviderManager.CombatManager.GetCardManager() != null);
        }

        private int GetBaseMania() 
        {
            if (this.useIntRange) 
            {
                return RandomManager.Range(this.Min, this.Max, RngId.Battle);
            }

            return this.ParamInt;
        }

        private int Min = 0;
        private int Max = 0;
        private bool useIntRange = false;
        private int ParamInt = 0;
        private CustomCardEffectApplyManicToDeck manicToDeck = null;
    }
}