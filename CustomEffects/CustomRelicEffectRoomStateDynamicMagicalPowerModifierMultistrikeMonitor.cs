//CustomRelicEffectRoomStateDynamicMagicalPowerModifierMultistrikeMonitor
using System;
using System.Collections;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using UnityEngine;
using Void.Init;

namespace CustomEffects
{
    public sealed class CustomRelicEffectRoomStateDynamicMagicalPowerModifierMultistrikeMonitor : RelicEffectBase //IOnStatusEffectAddedRelicEffect
    {
        private CardUpgradeData _cardUpgradeData;

        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
        {
            _cardUpgradeData = relicEffectData.GetParamCardUpgradeData();
            base.Initialize(relicState, relicData, relicEffectData);
        }

        /*
        int IOnStatusEffectAddedRelicEffect.GetModifiedStatusEffectStacksFromMultiplier(StatusEffectStackData statusEffectStackData, CharacterState onCharacter)
        {
            return statusEffectStackData.count;
        }

        int IOnStatusEffectAddedRelicEffect.GetStatusEffectStacksToAdd(StatusEffectStackData statusEffectStackData, CharacterState onCharacter)
        {
            return 0;

            if (!(ProviderManager.SaveManager != null && !ProviderManager.SaveManager.PreviewMode)) 
            {
                return 0;
            }

            if (onCharacter == null || onCharacter.IsDead)
            {
                return 0;
            }

            if (!(statusEffectStackData.statusId == VanillaStatusEffectIDs.Multistrike || statusEffectStackData.statusId == VanillaStatusEffectIDs.Phased)) 
            {
                return 0;
            }

            if (onCharacter.GetRoomStateModifiers().Count > 0)
            {
                foreach (IRoomStateModifier roomStateModifier in onCharacter.GetRoomStateModifiers())
                {
                    CustomRoomStateDynamicMagicalPowerModifier dynamicMagicalPowerModifier = roomStateModifier as CustomRoomStateDynamicMagicalPowerModifier;
                    dynamicMagicalPowerModifier?.Update();
                }
            }

            return 0;
        }

        void IOnStatusEffectAddedRelicEffect.OnStatusEffectAddedApplyAdder(OnStatusEffectAddedRelicEffectParams relicEffectParams)
        {
        }

        void IOnStatusEffectAddedRelicEffect.OnStatusEffectAddedApplyMultiplier(OnStatusEffectAddedRelicEffectParams relicEffectParams)
        {
        }

        void IOnStatusEffectAddedRelicEffect.OnStatusEffectRemoved(OnStatusEffectAddedRelicEffectParams relicEffectParams)
        {
            return;

            if (!(ProviderManager.SaveManager != null && !ProviderManager.SaveManager.PreviewMode))
            {
                return;
            }

            if (relicEffectParams.characterState == null) { return; }

            if (!(relicEffectParams.statusId == VanillaStatusEffectIDs.Multistrike || relicEffectParams.statusId == VanillaStatusEffectIDs.Phased))
            {
                return;
            }

            if (relicEffectParams.characterState.GetRoomStateModifiers().Count > 0)
            {
                foreach (IRoomStateModifier roomStateModifier in relicEffectParams.characterState.GetRoomStateModifiers())
                {
                    CustomRoomStateDynamicMagicalPowerModifier dynamicMagicalPowerModifier = roomStateModifier as CustomRoomStateDynamicMagicalPowerModifier;
                    dynamicMagicalPowerModifier?.Update();
                }
            }
        }
        */

        public override IEnumerator OnCharacterAdded(CharacterState character, CardState fromCard, RelicManager relicManager, SaveManager saveManager, PlayerManager playerManager, RoomManager roomManager, CombatManager combatManager, CardManager cardManager)
        {
            if (character != null && character.GetTeamType() == Team.Type.Monsters && !character.IsPyreHeart()) 
            {
                CardUpgradeState cardUpgradeState = new CardUpgradeState();
                cardUpgradeState.Setup(this._cardUpgradeData, false);
                yield return character.ApplyCardUpgrade(cardUpgradeState);
                if (character.GetRoomStateModifiers().Count > 0) 
                {
                    foreach (IRoomStateModifier roomStateModifier in character.GetRoomStateModifiers()) 
                    {
                        CustomRoomStateDynamicMagicalPowerModifier powerModifier = roomStateModifier as CustomRoomStateDynamicMagicalPowerModifier;
                        powerModifier?.SetSelf(character);
                        powerModifier?.Update();
                    }
                }
            }

            yield break;
        }
    }
}