using System;
using ShinyShoe;
using Trainworks.Managers;
using UnityEngine;
using Void.Init;

namespace CustomEffects
{

    // Token: 0x0200029E RID: 670
    public sealed class CustomRelicEffectNoPyreDamageEarly : RelicEffectBase, ITowerDamageTakenModifiedRelicEffect, IRelicEffect
    {
        // Token: 0x170001CF RID: 463
        // (get) Token: 0x060016DE RID: 5854 RVA: 0x0001C727 File Offset: 0x0001A927
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return true;
            }
        }

        public MerchantData.Currency Currency { get; private set; }

        public bool ShouldImmuneDamage(RelicEffectParams relicEffectParams) 
        {
            if (!relicEffectParams.saveManager.HasMainClass() || relicEffectParams.saveManager.GetRunState() == null) 
            { 
                return false; 
            }

            if (relicEffectParams.saveManager.GetRunState().GetCurrentDistance() < this.validUntilRing)
            {
                return true;
            }

            return false;
        }

        // Token: 0x170001D0 RID: 464
        // (get) Token: 0x060016DF RID: 5855 RVA: 0x0005A1A2 File Offset: 0x000583A2
        // (set) Token: 0x060016E0 RID: 5856 RVA: 0x0005A1AA File Offset: 0x000583AA
        //public MerchantData.Currency Currency { get; private set; }

        // Token: 0x060016E1 RID: 5857 RVA: 0x0005A1B3 File Offset: 0x000583B3
        public override void Initialize(RelicState relicState, RelicData srcRelicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, srcRelicData, relicEffectData);
            this.Currency = MerchantData.Currency.Gold;
            this.validUntilRing = relicEffectData.GetParamInt();
        }

        // Token: 0x060016E2 RID: 5858 RVA: 0x0005A1DC File Offset: 0x000583DC
        public int ApplyModifiedDamage(int previousDamage, RelicEffectParams relicEffectParams)
        {
            int num = previousDamage;
            if (num <= 0)
            {
                return num;
            }
            SaveManager saveManager = relicEffectParams.saveManager;
            if (ShouldImmuneDamage(relicEffectParams))
            {
                base.NotifyRelicTriggered(relicEffectParams.relicManager, relicEffectParams.roomManager.GetPyreRoom());
                num = 0;
            }
            return num;
        }

        // Token: 0x060016E3 RID: 5859 RVA: 0x0005A2B2 File Offset: 0x000584B2
        public override string GetActivatedDescription()
        {
            if (ProviderManager.TryGetProvider<SoundManager>(out SoundManager soundManager)) 
            {
                soundManager.PlaySfx("Multiplayer_Emote_Lol");
            }
            return this._srcRelicState.GetRelicActivatedKey().Localize(null);
        }

        // Token: 0x04000C0E RID: 3086
        //private int amountToBlockDamage;
        private int validUntilRing;

        // Token: 0x04000C0F RID: 3087
        //private int lastDamageAmount;
    }
}