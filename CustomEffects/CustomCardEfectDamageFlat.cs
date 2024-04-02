using System;
using System.Collections;

namespace CustomEffects
{
    // Token: 0x0200009D RID: 157
    //This is literally an unmodified copy of CardEffectDamage.
    //It won't be modified by card upgrades, though, because those look for "CardEffectDamage" specifically and the name was changed.
    public sealed class CustomCardEffectFixedDamage : CardEffectBase
    {
        // Token: 0x17000063 RID: 99
        // (get) Token: 0x0600071F RID: 1823 RVA: 0x00021A43 File Offset: 0x0001FC43
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return this.canApplyInPreviewMode;
            }
        }

        // Token: 0x06000720 RID: 1824 RVA: 0x00021A4B File Offset: 0x0001FC4B
        public override void Setup(CardEffectState cardEffectState)
        {
            base.Setup(cardEffectState);
            this.canApplyInPreviewMode = (cardEffectState.GetTargetMode() != TargetMode.Pyre);
        }

        // Token: 0x06000721 RID: 1825 RVA: 0x00021A68 File Offset: 0x0001FC68
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            int intInRange = cardEffectState.GetIntInRange();
            bool flag = !cardEffectState.GetUseIntRange() || cardEffectState.GetParamMaxInt() > 0;
            bool flag2 = true;
            if (cardEffectState.GetTargetMode() == TargetMode.DropTargetCharacter)
            {
                flag2 = (cardEffectParams.targets.Count > 0);
            }
            return intInRange >= 0 && flag2 && flag;
        }

        // Token: 0x06000722 RID: 1826 RVA: 0x00021AB8 File Offset: 0x0001FCB8
        private int GetDamageAmount(CardEffectState cardEffectState, CharacterState selfTarget)
        {
            int num = cardEffectState.GetIntInRange();
            if (cardEffectState.GetUseStatusEffectStackMultiplier())
            {
                int statusEffectStacks = selfTarget.GetStatusEffectStacks(cardEffectState.GetStatusEffectStackMultiplier());
                num *= statusEffectStacks;
            }
            return num;
        }

        // Token: 0x06000723 RID: 1827 RVA: 0x00021AE6 File Offset: 0x0001FCE6
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            int damageAmount = this.GetDamageAmount(cardEffectState, cardEffectParams.selfTarget);
            int soundGateId = SoundManager.INVALID_SOUND_GATE;
            if (cardEffectState.GetTargetMode() == TargetMode.Room)
            {
                soundGateId = cardEffectParams.combatManager.IgnoreDuplicateSounds(true);
            }
            int num;
            for (int i = 0; i < cardEffectParams.targets.Count; i = num + 1)
            {
                CharacterState target = cardEffectParams.targets[i];
                yield return cardEffectParams.combatManager.ApplyDamageToTarget(damageAmount, target, new CombatManager.ApplyDamageToTargetParameters
                {
                    playedCard = cardEffectParams.playedCard,
                    finalEffectInSequence = cardEffectParams.finalEffectInSequence,
                    relicState = cardEffectParams.sourceRelic,
                    selfTarget = cardEffectParams.selfTarget,
                    vfxAtLoc = cardEffectState.GetAppliedVFX(),
                    showDamageVfx = cardEffectParams.allowPlayingDamageVfx
                });
                num = i;
            }
            cardEffectParams.combatManager.ReleaseIgnoreDuplicateCuesHandle(soundGateId);
            yield break;
        }

        // Token: 0x06000724 RID: 1828 RVA: 0x00021B04 File Offset: 0x0001FD04
        public bool WillEffectKillTarget(CharacterState target, CardState card, CardEffectState cardEffectState, out int resultantDamage)
        {
            int damageAmount = this.GetDamageAmount(cardEffectState, null);
            int num;
            resultantDamage = target.GetDamageToTarget(damageAmount, null, card, out num, Damage.Type.Default);
            return resultantDamage >= target.GetHP();
        }

        // Token: 0x06000725 RID: 1829 RVA: 0x00021B38 File Offset: 0x0001FD38
        public override string GetHintText(CardEffectState cardEffectState, CharacterState selfTarget)
        {
            int damageAmount = this.GetDamageAmount(cardEffectState, selfTarget);
            return "CardTraitScalingAddDamage_CurrentScaling_CardText".Localize(new LocalizedIntegers(new int[]
            {
            damageAmount
            }));
        }

        // Token: 0x04000443 RID: 1091
        private bool canApplyInPreviewMode;
    }
}