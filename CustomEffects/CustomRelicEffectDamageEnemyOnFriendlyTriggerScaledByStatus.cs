//CustomRelicEffectDamageEnemyOnFriendlyTriggerScaledByStatus

using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomEffects
{

    // Token: 0x020002A1 RID: 673
    public sealed class CustomRelicEffectDamageEnemyOnFriendlyTriggerScaledByStatus : RelicEffectBase, ICharacterActionRelicEffect, IRelicEffect, ITriggerRelicEffect, IStatusEffectRelicEffect
    {
        // Token: 0x170001D3 RID: 467
        // (get) Token: 0x060016EF RID: 5871 RVA: 0x0001C727 File Offset: 0x0001A927
        public override bool CanApplyInPreviewMode
        {
            get
            {
                return true;
            }
        }

        // Token: 0x170001D4 RID: 468
        // (get) Token: 0x060016F0 RID: 5872 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanShowNotifications
        {
            get
            {
                return true;
            }
        }

        // Token: 0x060016F1 RID: 5873 RVA: 0x0005A378 File Offset: 0x00058578
        public bool TestCharacterTriggerEffect(CharacterTriggerRelicEffectParams relicEffectParams)
        {
            CharacterState characterState = relicEffectParams.characterState;
            if (relicEffectParams.trigger != this.trigger || characterState.GetTeamType() != this.sourceTeam)
            {
                return false;
            }
            foreach (StatusEffectStackData statusEffectStackData in this.statusEffects)
            {
                if (!characterState.HasStatusEffect(statusEffectStackData.statusId))
                {
                    return false;
                }
                else
                {
                    this.statusMultiplier = characterState.GetStatusEffectStacks(statusEffectStackData.statusId);
                }
            }
            if (this.trigger == CharacterTriggerData.Trigger.OnDeath && characterState.IsAlive)
            {
                return false;
            }
            if (!characterState.GetCharacterManager().DoesCharacterPassSubtypeCheck(characterState, this.subtypeData))
            {
                return false;
            }
            SpawnPoint spawnPoint = characterState.GetSpawnPoint(true);
            int roomIndex = (spawnPoint != null) ? spawnPoint.GetRoomOwner().GetRoomIndex() : relicEffectParams.roomManager.GetSelectedRoom();
            TargetHelper.CollectTargetsData data = new TargetHelper.CollectTargetsData
            {
                targetMode = this.targetMode,
                targetModeStatusEffectsFilter = new List<string>(),
                targetModeHealthFilter = CardEffectData.HealthFilter.Both,
                targetTeamType = this.sourceTeam.GetOppositeTeam(),
                roomIndex = roomIndex,
                heroManager = relicEffectParams.heroManager,
                monsterManager = relicEffectParams.monsterManager,
                roomManager = relicEffectParams.roomManager,
                inCombat = false,
                isTesting = true
            };
            this._targets.Clear();
            TargetHelper.CollectTargets(data, ref this._targets);
            return this._targets.Count > 0;
        }

        // Token: 0x060016F2 RID: 5874 RVA: 0x0005A4C6 File Offset: 0x000586C6
        public IEnumerator ApplyCharacterTriggerEffect(CharacterTriggerRelicEffectParams relicEffectParams)
        {
            List<CharacterState> list = new List<CharacterState>(this._targets);
            foreach (CharacterState characterState in list)
            {
                base.NotifyRelicTriggered(relicEffectParams.relicManager, characterState);
                yield return relicEffectParams.combatManager.ApplyDamageToTarget((this.damageAmount * this.statusMultiplier), characterState, new CombatManager.ApplyDamageToTargetParameters
                {
                    relicState = this._srcRelicState,
                    vfxAtLoc = this._srcRelicEffectData.GetAppliedVfx(),
                    showDamageVfx = true
                });
            }
            //List<CharacterState>.Enumerator enumerator = default(List<CharacterState>.Enumerator);
            yield break;
            //yield break;
        }

        // Token: 0x060016F3 RID: 5875 RVA: 0x0005A4DC File Offset: 0x000586DC
        public override void Initialize(RelicState relicState, RelicData srcRelicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, srcRelicData, relicEffectData);
            this.trigger = relicEffectData.GetParamTrigger();
            this.sourceTeam = relicEffectData.GetParamSourceTeam();
            this.damageAmount = relicEffectData.GetParamInt();
            this.statusEffects = relicEffectData.GetParamStatusEffects();
            this.subtypeData = relicEffectData.GetParamCharacterSubtype();
            this.targetMode = relicEffectData.GetTargetMode();
            this.statusMultiplier = 0;
        }

        // Token: 0x060016F4 RID: 5876 RVA: 0x0005A53A File Offset: 0x0005873A
        public StatusEffectStackData[] GetStatusEffects()
        {
            return this.statusEffects;
        }

        // Token: 0x060016F5 RID: 5877 RVA: 0x0005A542 File Offset: 0x00058742
        public CharacterTriggerData.Trigger GetTrigger()
        {
            return this.trigger;
        }

        // Token: 0x060016F6 RID: 5878 RVA: 0x0000C623 File Offset: 0x0000A823
        public bool HideTriggerTooltip()
        {
            return false;
        }

        // Token: 0x04000C12 RID: 3090
        private CharacterTriggerData.Trigger trigger;

        // Token: 0x04000C13 RID: 3091
        private Team.Type sourceTeam;

        private int statusMultiplier;

        // Token: 0x04000C14 RID: 3092
        private int damageAmount;

        // Token: 0x04000C15 RID: 3093
        private StatusEffectStackData[] statusEffects;

        // Token: 0x04000C16 RID: 3094
        private SubtypeData subtypeData;

        // Token: 0x04000C17 RID: 3095
        private TargetMode targetMode;

        // Token: 0x04000C18 RID: 3096
        private List<CharacterState> _targets = new List<CharacterState>();
    }
}