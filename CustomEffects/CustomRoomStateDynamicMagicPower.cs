using System;
using System.Collections.Generic;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using Trainworks.BuildersV2;
using Void.Init;
using HarmonyLib;
using Trainworks.ManagersV2;

namespace CustomEffects
{

    // Token: 0x0200036A RID: 874
    public sealed class CustomRoomStateDynamicMagicalPowerModifier : RoomStateModifierBase, IRoomStateRoomSelectedModifier, IRoomStateSpawnPointsChangedModifier, IRoomStateCardManagerModifier
    {
        private CharacterState self;

        private int last_MagicPower = 0;

        //private static int NUMNUM = -1;

        //public override void Initialize(RoomModifierData roomModifierData, RoomManager roomManager)
        //{
        //    NUMNUM++;
        //    base.Initialize(roomModifierData, roomManager);
        //}

        // Token: 0x17000267 RID: 615
        // (get) Token: 0x06001B92 RID: 7058 RVA: 0x0000C623 File Offset: 0x0000A823
        public bool CanApplyInPreviewMode
        {
            get
            {
                return false;
            }
        }

        public void SetSelf(CharacterState character) 
        { 
            self = character;
        }

        /*
        private CharacterState GetSelf()
        {
            Beyonder.Log("Looking for self.");

            if (!ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager)) { return self; }

            List<CharacterState> characterStates = new List<CharacterState>();

            if (roomManager == null) { return self; }

            for (int ii = roomManager.GetNumRooms() - 1; ii >= 0; ii--) 
            {
                roomManager.GetRoom(ii).AddCharactersToList(characterStates, Team.Type.Heroes | Team.Type.Monsters, false);
            }

            foreach (CharacterState characterState in characterStates) 
            {
                if (characterState.GetRoomStateModifiers().Count > 0) 
                {
                    foreach (IRoomStateModifier roomStateModifier in characterState.GetRoomStateModifiers()) 
                    {
                        if (roomStateModifier == this) 
                        {
                            Beyonder.Log("Self Found.");
                            self = characterState;
                            break;
                        }
                    }
                    if (self != null) 
                    {
                        break;
                    }
                }
            }

            return self;
        }
        */

        private int GetMagicPowerValue(bool ignorePreviewStatus = false) 
        {
            //Beyonder.Log("Line 66");

            if (self == null)
            {
                return 0;
            }

            //Beyonder.Log("Line 73");
            if (!ignorePreviewStatus && (self.PreviewMode || (ProviderManager.SaveManager != null && ProviderManager.SaveManager.PreviewMode)))
            {
                return last_MagicPower;
            }

            if (!self.IsAlive)
            {
                return 0;
            }

            //Beyonder.Log("Line 80");

            if (self.HasStatusEffect(VanillaStatusEffectIDs.Phased)) 
            {
                return 0;
            }

            //Beyonder.Log("Line 87");

            int value = 0;

            //Can't use self.GetAttackDamage() as this references RoomModifiers and softlocks the game.
            value += (int)AccessTools.Field(typeof(PrimaryAbilityDisplay), "cachedAttack").GetValue(self.GetCharacterUI().GetPrimaryAbilityDisplay());

            if (value < 0) { value = 0; }

            if (self.HasStatusEffect(VanillaStatusEffectIDs.Multistrike))
            {
                value *= (1 + self.GetStatusEffectStacks(VanillaStatusEffectIDs.Multistrike));
            }

            value += self.GetHP();

            //Beyonder.Log("Line 100: Value is "+value);

            return value;
        }

        // Token: 0x06001B93 RID: 7059 RVA: 0x00068130 File Offset: 0x00066330
        public void RoomSelectionChanged(bool roomSelected, CardManager cardManager)
        {
            if (roomSelected)
            {
                this.AddMagicPowerModifierToCardsInHand(cardManager);
                return;
            }
            this._cardsWeHaveModified.ClearTrackingAndResetAllMagicPower();
        }

        // Token: 0x06001B94 RID: 7060 RVA: 0x00068148 File Offset: 0x00066348
        public void SpawnPointChanged(CharacterState characterState, SpawnPoint prevPoint, SpawnPoint newPoint, CardManager cardManager)
        {
            bool flag = newPoint != null && newPoint.GetRoomOwner().GetSelected();
            bool flag2 = prevPoint != null && prevPoint.GetRoomOwner().GetSelected();
            if (flag)
            {
                this.AddMagicPowerModifierToCardsInHand(cardManager);
                return;
            }
            if (flag2 || (characterState != null && characterState.IsDead))
            {
                this._cardsWeHaveModified.ClearTrackingAndResetAllMagicPower();
            }
        }

        // Token: 0x06001B95 RID: 7061 RVA: 0x0006819C File Offset: 0x0006639C
        public void CardDiscarded(CardState cardState)
        {
            this._cardsWeHaveModified.ClearMagicPower(cardState);
        }

        // Token: 0x06001B96 RID: 7062 RVA: 0x000681AA File Offset: 0x000663AA
        public void CardDrawn(CardState cardState)
        {
            this.ApplyMagicPower(cardState);
        }

        // Token: 0x06001B97 RID: 7063 RVA: 0x000681B4 File Offset: 0x000663B4
        private void AddMagicPowerModifierToCardsInHand(CardManager cardManager)
        {
            //base.GetParamInt();
            foreach (CardState cardState in cardManager.GetHand(false))
            {
                if (!this._cardsWeHaveModified.HaveWeAddedMagicPowerToThisCardAlready(cardState))
                {
                    this.ApplyMagicPower(cardState);
                    if (cardManager != null)
                    {
                        cardManager.RefreshCardInHand(cardState, true);
                    }
                }
            }
        }

        public void Update(bool ignorePreviewStatus = false) 
        {
            if (!IsOnActiveFloor() || last_MagicPower == GetMagicPowerValue(ignorePreviewStatus)) { return; }

            this._cardsWeHaveModified.ClearTrackingAndResetAllMagicPower();
            if (!ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager)) { return; }
            this.AddMagicPowerModifierToCardsInHand(cardManager);
        }

        private bool IsOnActiveFloor() 
        {
            if (self == null || !ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager)) 
            { 
                return false;
            }

            if (self.GetSpawnPoint()?.GetRoomOwner()?.GetRoomIndex() == roomManager.GetSelectedRoom())
            { 
                return true;
            }

            return false;
        }

        // Token: 0x06001B98 RID: 7064 RVA: 0x00068228 File Offset: 0x00066428
        private void ApplyMagicPower(CardState cardState)
        {
            int power = this.GetMagicPowerValue();

            last_MagicPower = power;

            if (power <= 0) 
            { 
                return; 
            }

            CardUpgradeData cardUpgradeData = new CardUpgradeDataBuilder
            {
                UpgradeID = this.GetParamCardUpgradeData().GetID() + "_" + power,
                BonusDamage = power,
                BonusHeal = power,
            }.Build(false);

            if (cardState.GetCardType() == CardType.Spell)
            {
                this._cardsWeHaveModified.AddUpgradeToCard(cardState, cardUpgradeData);
            }
        }

        public override int GetDynamicInt(CharacterState characterContext)
        {
            return GetMagicPowerValue();
        }

        // Token: 0x04000E59 RID: 3673
        private CustomRoomStateDynamicMagicalPowerModifier.ModifiedCardTracker _cardsWeHaveModified = new CustomRoomStateDynamicMagicalPowerModifier.ModifiedCardTracker();

        // Token: 0x02000F1F RID: 3871
        private class ModifiedCardTracker
        {
            // Token: 0x06007110 RID: 28944 RVA: 0x00198D94 File Offset: 0x00196F94
            public void AddUpgradeToCard(CardState cardState, CardUpgradeData cardUpgradeData)
            {
                if (!this.HaveWeAddedMagicPowerToThisCardAlready(cardState))
                {
                    CardUpgradeState cardUpgradeState = null;
                    foreach (CardUpgradeState cardUpgradeState2 in cardState.GetTemporaryCardStateModifiers().GetCardUpgrades())
                    {
                        if (cardUpgradeState2.GetCardUpgradeDataId() == this.trackerId)
                        {
                            cardUpgradeState = cardUpgradeState2;
                            break;
                        }
                    }
                    if (cardUpgradeState == null)
                    {
                        cardUpgradeState = new CardUpgradeState();
                        cardUpgradeState.Setup(cardUpgradeData, false);
                        cardUpgradeState.SetCardUpgradeDataId(this.trackerId);
                        int magicPowerMultiplierFromTraits = cardState.GetMagicPowerMultiplierFromTraits();
                        if (magicPowerMultiplierFromTraits > 1 && cardUpgradeData.GetBonusDamage() == cardUpgradeState.GetAttackDamage())
                        {
                            cardUpgradeState.SetAttackDamage(cardUpgradeState.GetAttackDamage() * magicPowerMultiplierFromTraits);
                            cardUpgradeState.SetAdditionalHeal(cardUpgradeState.GetAdditionalHeal() * magicPowerMultiplierFromTraits);
                        }
                        cardState.GetTemporaryCardStateModifiers().AddUpgrade(cardUpgradeState, null);
                    }
                    this._modifiedCardMagicPower.Add(cardState, cardUpgradeState);
                }
                cardState.UpdateDamageText();
                cardState.UpdateCardBodyText(null);
            }

            // Token: 0x06007111 RID: 28945 RVA: 0x00198E84 File Offset: 0x00197084
            public void ClearTrackingAndResetAllMagicPower()
            {
                foreach (KeyValuePair<CardState, CardUpgradeState> keyValuePair in this._modifiedCardMagicPower)
                {
                    CardState key = keyValuePair.Key;
                    key.GetTemporaryCardStateModifiers().RemoveUpgrade(keyValuePair.Value);
                    key.UpdateDamageText();
                    key.UpdateCardBodyText(null);
                }
                this._modifiedCardMagicPower.Clear();
            }

            // Token: 0x06007112 RID: 28946 RVA: 0x00198F00 File Offset: 0x00197100
            public void ClearMagicPower(CardState cardState)
            {
                if (this._modifiedCardMagicPower.ContainsKey(cardState))
                {
                    cardState.GetTemporaryCardStateModifiers().RemoveUpgrade(this._modifiedCardMagicPower[cardState]);
                    cardState.UpdateDamageText();
                    cardState.UpdateCardBodyText(null);
                    this._modifiedCardMagicPower.Remove(cardState);
                }
            }

            // Token: 0x06007113 RID: 28947 RVA: 0x00198F4C File Offset: 0x0019714C
            public bool HaveWeAddedMagicPowerToThisCardAlready(CardState card)
            {
                return this._modifiedCardMagicPower.ContainsKey(card);
            }

            // Token: 0x06007114 RID: 28948 RVA: 0x00198F5A File Offset: 0x0019715A
            public CardUpgradeState GetCardUpgradeState(CardState card)
            {
                if (this._modifiedCardMagicPower.ContainsKey(card))
                {
                    return this._modifiedCardMagicPower[card];
                }
                return null;
            }

            // Token: 0x04004E1D RID: 19997
            private Dictionary<CardState, CardUpgradeState> _modifiedCardMagicPower = new Dictionary<CardState, CardUpgradeState>(20);

            // Token: 0x04004E1E RID: 19998
            private string trackerId = Guid.NewGuid().ToString(); //+"_"+NUMNUM;
        }
    }
}