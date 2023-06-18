//CustomRelicEffectHealFriendlyUnitsOnRelocate

using System;
using System.Collections;
using Void.Init;
using Void.Artifacts;

namespace CustomEffects
{

    // Token: 0x020002C0 RID: 704
    public sealed class CustomRelicEffectHealFriendlyUnitsOnRelocate : RelicEffectBase, ICharacterActionRelicEffect, IRelicEffect
    {
        // Token: 0x170001E5 RID: 485
        // (get) Token: 0x0600179E RID: 6046 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanShowNotifications
        {
            get
            {
                return false;
            }
        }

        // Token: 0x0600179F RID: 6047 RVA: 0x0005C41E File Offset: 0x0005A61E
        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, relicData, relicEffectData);
            this.healAmount = relicEffectData.GetParamInt();
            this.excludeCharacterSubtypes = relicEffectData.GetParamExcludeCharacterSubtypes();
            this.teamType = relicEffectData.GetParamSourceTeam();
        }

        // Token: 0x060017A0 RID: 6048 RVA: 0x0005A2FC File Offset: 0x000584FC
        public bool TestCharacterTriggerEffect(CharacterTriggerRelicEffectParams relicEffectParams)
        {
            return relicEffectParams.trigger == PurloinedHeavensSeal.GetOnRelocate() && !relicEffectParams.characterState.IsDestroyed && relicEffectParams.characterState.GetTeamType() == this.teamType;
        }

        // Token: 0x060017A1 RID: 6049 RVA: 0x0005C441 File Offset: 0x0005A641
        public IEnumerator ApplyCharacterTriggerEffect(CharacterTriggerRelicEffectParams relicEffectParams)
        {
            foreach (SubtypeData subtypeData in this.excludeCharacterSubtypes)
            {
                if (relicEffectParams.characterState.GetHasSubtype(subtypeData))
                {
                    yield break;
                }
            }
            base.NotifyRelicTriggered(relicEffectParams.relicManager, relicEffectParams.characterState);
            yield return relicEffectParams.characterState.ApplyHeal(this.healAmount, true, null, this._srcRelicState, false);
            yield break;
        }

        // Token: 0x04000C83 RID: 3203
        private int healAmount;

        private Team.Type teamType;

        // Token: 0x04000C84 RID: 3204
        private SubtypeData[] excludeCharacterSubtypes;
    }
}