using System;


namespace CustomEffects
{
    // Token: 0x020002E3 RID: 739
    public sealed class CustomRelicEffectGiveGoldAtStartOfRun : RelicEffectBase, IStartOfRunRelicEffect
    {
        private int goldToGive;

        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, relicData, relicEffectData);
            goldToGive = relicEffectData.GetIntInRange();
        }

        public void ApplyEffect(RelicEffectParams relicEffectParams)
        {
            relicEffectParams.saveManager.AdjustGold(goldToGive);
            base.NotifyRelicTriggered(relicEffectParams.relicManager);
        }
    }
}