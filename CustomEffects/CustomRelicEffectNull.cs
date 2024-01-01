using System;
using System.Collections;
using System.Collections.Generic;
using Trainworks.Constants;
using Trainworks.Managers;
using Void.Init;

namespace CustomEffects
{
    // Token: 0x020002E3 RID: 739
    public sealed class CustomRelicEffectNULL : RelicEffectBase, ICardGeneratingRelicEffect
    {
        public List<CardData> GetGeneratedCardDataForTooltip()
        {
            return new List<CardData>
            {
                CustomCardManager.GetCardDataByID(VanillaCardIDs.VengefulShard)
            };
        }

        public List<CardState> GetGeneratedCardStateForTooltip(out bool showTitle)
        {
            showTitle = true;

            return new List<CardState>
            {
                new CardState(CustomCardManager.GetCardDataByID(VanillaCardIDs.VengefulShard), null, null, true)
            };
        }
    }
}