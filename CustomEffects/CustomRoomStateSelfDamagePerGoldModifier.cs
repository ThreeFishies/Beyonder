using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Void.Triggers;
using Void.Mania;
using Void.Artifacts;
using Trainworks.Managers;

namespace CustomEffects
{
    public sealed class CustomRoomStateSelfDamagePerGoldModifier : RoomStateModifierBase, IRoomStateDamageModifier, IRoomStateModifier, ILocalizationParamInt, ILocalizationParameterContext
    {
        // Token: 0x06001BAF RID: 7087 RVA: 0x0006848F File Offset: 0x0006668F
        public override void Initialize(RoomModifierData roomModifierData, RoomManager roomManager)
        {
            base.Initialize(roomModifierData, roomManager);
            this.goldPerDamagePoint = roomModifierData.GetParamInt();
        }

        // Token: 0x06001BB0 RID: 7088 RVA: 0x00067EB7 File Offset: 0x000660B7
        public int GetModifiedDamage(Damage.Type damageType, CharacterState attackerState, bool requestingForCharacterStats)
        {
            if (requestingForCharacterStats)
            {
                return this.GetDynamicInt(attackerState);
            }
            return 0;
        }

        // Token: 0x06001BB1 RID: 7089 RVA: 0x000684A8 File Offset: 0x000666A8
        public override int GetDynamicInt(CharacterState characterContext)
        {
            if (ProviderManager.SaveManager == null) 
            {
                return 0;
            }

            if (characterContext.GetRoomStateModifiers().Contains(this) && characterContext.GetSpawnPoint(false) != null)
            {
                int currentGold = ProviderManager.SaveManager.GetGold();

                return currentGold / this.goldPerDamagePoint;
            }
            return 0;
        }

        // Token: 0x04000E64 RID: 3684
        private int goldPerDamagePoint;
    }
}