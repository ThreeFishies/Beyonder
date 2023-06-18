using System;
using System.Collections;
using System.Collections.Generic;
using ShinyShoe.Logging;
using Void.Init;

namespace CustomEffects
{
    // Token: 0x020000F6 RID: 246
    public sealed class CustomCardTriggerEffectDebuffCharacterDamage : ICardTriggerEffect
    {
        // Token: 0x060008BB RID: 2235 RVA: 0x000250DC File Offset: 0x000232DC
        public IEnumerator ApplyTriggerEffect(CardTriggerState triggerEffectState, CardTriggerEventParams triggerParams)
        {
            Beyonder.Log("Attempting to reduce damage.");
            //Log.Assert(triggerParams.playedCard != null, LogGroups.Gameplay, "Cannot run CardTriggerEffectBuffCharacterDamage without a played card.", Array.Empty<object>());
            int paramInt = triggerEffectState.GetParamInt();
            this.BuffCharacter(paramInt, triggerEffectState, triggerParams, true);
            triggerParams.playedCard.GetCardStateModifiers().IncrementAdditionalDamage(-paramInt);
            triggerParams.playedCard.UpdateDamageText();
            yield break;
        }

        // Token: 0x060008BC RID: 2236 RVA: 0x000250F9 File Offset: 0x000232F9
        public IEnumerator ApplyTriggerEffectPreviewMode(CardTriggerState triggerEffectState, CardTriggerEventParams triggerParams)
        {
            Beyonder.Log("Attempting to reduce damage preview.");
            this.BuffCharacter(triggerEffectState.GetParamInt(), triggerEffectState, triggerParams, false);
            yield break;
        }

        // Token: 0x060008BD RID: 2237 RVA: 0x00025118 File Offset: 0x00023318
        private void BuffCharacter(int additionalAmount, CardTriggerState triggerEffectState, CardTriggerEventParams triggerParams, bool showNotification)
        {
            Beyonder.Log("Reducing damage.");
            this.affectedCharacters.Clear();
            triggerParams.monsterManager.AddCharactersSpawnedByCardToList(this.affectedCharacters, triggerParams.playedCard);
            foreach (CharacterState characterState in this.affectedCharacters)
            {
                characterState.DebuffDamage(additionalAmount, null, false);
                if (showNotification)
                {
                    characterState.ShowNotification(string.Format("CardUpgrade_AttackUnDamage".Localize(null), -additionalAmount), PopupNotificationUI.Source.General, null);
                }
            }
        }

        // Token: 0x04000472 RID: 1138
        private List<CharacterState> affectedCharacters = new List<CharacterState>();
    }
}