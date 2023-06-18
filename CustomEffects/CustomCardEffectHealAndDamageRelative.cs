using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEffects
{
    // Token: 0x020000AF RID: 175
    public sealed class CustomCardEffectHealAndDamageRelative : CardEffectHeal
    {
        // Token: 0x17000078 RID: 120
        // (get) Token: 0x06000785 RID: 1925 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanRandomizeTargetMode
        {
            get
            {
                return false;
            }
        }

        // Token: 0x06000786 RID: 1926 RVA: 0x00021094 File Offset: 0x0001F294
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            return cardEffectParams.targets.Count > 0;
        }

        // Token: 0x06000787 RID: 1927 RVA: 0x0002291E File Offset: 0x00020B1E
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            int healAmount = base.GetHealAmount(cardEffectState);
            float DamageMultiplier = cardEffectState.GetParamMultiplier();
            CharacterState target = cardEffectParams.targets[0];
            int oldHp = target.GetHP();
            yield return target.ApplyHeal(healAmount, true, cardEffectParams.playedCard, null, false);
            int num = Mathf.Max(Mathf.RoundToInt((float)(target.GetHP() - oldHp) * DamageMultiplier), 0);
            if (num > 0)
            {
                this.CollectTargets(target, cardEffectParams, false);
                if (this.toProcessCharacters.Count > 0)
                {
                    foreach (CharacterState target2 in this.toProcessCharacters)
                    {
                        yield return cardEffectParams.combatManager.ApplyDamageToTarget(num, target2, new CombatManager.ApplyDamageToTargetParameters
                        {
                            playedCard = cardEffectParams.playedCard,
                            finalEffectInSequence = cardEffectParams.finalEffectInSequence,
                            vfxAtLoc = cardEffectState.GetAppliedVFX(),
                            showDamageVfx = cardEffectParams.allowPlayingDamageVfx
                        });
                    }
                }
            }
            yield break;
        }

        // Token: 0x06000788 RID: 1928 RVA: 0x0002293C File Offset: 0x00020B3C
        private void CollectTargets(CharacterState target, CardEffectParams cardEffectParams, bool isTesting = false)
        {
            this.toProcessCharacters.Clear();
            this.collectTargetsData.Reset(TargetMode.Room);
            this.collectTargetsData.targetTeamType = target.GetTeamType().GetOppositeTeam();
            this.collectTargetsData.roomIndex = target.GetCurrentRoomIndex();
            this.collectTargetsData.inCombat = false;
            this.collectTargetsData.heroManager = cardEffectParams.heroManager;
            this.collectTargetsData.monsterManager = cardEffectParams.monsterManager;
            this.collectTargetsData.cardManager = cardEffectParams.cardManager;
            this.collectTargetsData.roomManager = cardEffectParams.roomManager;
            this.collectTargetsData.combatManager = cardEffectParams.combatManager;
            this.collectTargetsData.isTesting = isTesting;
            TargetHelper.CollectTargets(this.collectTargetsData, ref this.toProcessCharacters);
            this.collectTargetsData.Reset(TargetMode.Room);
        }

        // Token: 0x0400045E RID: 1118
        private List<CharacterState> toProcessCharacters = new List<CharacterState>();

        // Token: 0x0400045F RID: 1119
        private TargetHelper.CollectTargetsData collectTargetsData;
    }
}