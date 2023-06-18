using System;
using System.Collections;
using System.Collections.Generic;
using ShinyShoe;
using ShinyShoe.Logging;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;

namespace CustomEffects
{
    // Token: 0x02000099 RID: 153
    public sealed class CustomCardEffectBumpPreviewConditional : CardEffectBase, ICardEffectStatuslessTooltip
    {
        //public static List<CharacterState> PokedTargets = new List<CharacterState>();
        public static Dictionary<CharacterState,int> PokedTargetsDict = new Dictionary<CharacterState,int>();
        public static bool PlayedCardIsBumper = false;
        public static int thisEffectIndex = 0;
        public int thisIndex = 0;

        public bool ShouldPrieview(CardEffectState cardEffectState, CardEffectParams cardEffectParams) 
        {
            if (!cardEffectParams.saveManager.PreviewMode) 
            {
                return true;
            }

            if (PlayedCardIsBumper) 
            {
                return false;
            }

            if (cardEffectParams.targets != null && cardEffectParams.targets.Count > 0) 
            {
                foreach (CharacterState target in cardEffectParams.targets) 
                {
                    if (PokedTargetsDict.ContainsKey(target))
                    {
                        if (PokedTargetsDict[target] == this.thisIndex) 
                        {
                            return true;
                        }                        
                        return false;
                    }
                    else 
                    { 
                        PokedTargetsDict.Add(target, this.thisIndex);
                    }
                }
            }

            return true;
        }


        // Token: 0x06000705 RID: 1797 RVA: 0x0002167E File Offset: 0x0001F87E
        public override void Setup(CardEffectState cardEffectState)
        {
            this.cachedState = cardEffectState;
            base.Setup(cardEffectState);
            this.thisIndex = thisEffectIndex;
            thisEffectIndex++;
        }

        // Token: 0x06000706 RID: 1798 RVA: 0x0002168E File Offset: 0x0001F88E
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (!ShouldPrieview(cardEffectState, cardEffectParams)) 
            {
                yield break;
            }
            yield return this.Bump(cardEffectParams, cardEffectState.GetSourceCardEffectData().GetParamInt());
            yield break;
        }

        // Token: 0x06000707 RID: 1799 RVA: 0x000216AB File Offset: 0x0001F8AB
        public IEnumerator Bump(CardEffectParams cardEffectParams, int bumpAmount)
        {
            this.ascendingCharacters = (this.ascendingCharacters ?? new Dictionary<CharacterState, CharacterState.SpawnPointAscensionData>());
            this.ascendingCharacters.Clear();
            this.ascendAttemptCharacters = (this.ascendAttemptCharacters ?? new List<CharacterState>());
            this.ascendAttemptCharacters.Clear();
            List<CharacterState> characters = new List<CharacterState>(cardEffectParams.targets);
            RoomManager roomManager = cardEffectParams.roomManager;
            HeroManager heroManager = cardEffectParams.heroManager;
            CombatManager combatManager = cardEffectParams.combatManager;
            int i = 0;
            while (i < characters.Count)
            {
                CharacterState target2 = characters[i];
                Team.Type teamType2 = target2.GetTeamType();
                SpawnPoint oldSpawnPoint = target2.GetSpawnPoint(false);
                CardEffectBump.BumpError bumpError = CardEffectBump.BumpError.None;
                SpawnPoint newSpawnPoint = null;
                if (target2.HasStatusEffect("immobile") || target2.IsHellforgedBoss())
                {
                    bumpError = CardEffectBump.BumpError.ImmobileCharacter;
                    goto IL_1A9;
                }
                if (target2.HasStatusEffect("rooted"))
                {
                    target2.RemoveStatusEffect("rooted", false, 1, true, cardEffectParams.sourceRelic, null);
                    this.ascendAttemptCharacters.Add(target2);
                }
                else
                {
                    if (!target2.IsPyreHeart())
                    {
                        newSpawnPoint = this.FindBumpSpawnPoint(target2, bumpAmount, roomManager, out bumpError);
                        goto IL_1A9;
                    }
                    goto IL_1A9;
                }
            IL_31F:
                int num = i;
                i = num + 1;
                continue;
            IL_1A9:
                if (bumpError != CardEffectBump.BumpError.None)
                {
                    target2.ShowNotification(CardEffectBump.GetErrorMessage(bumpError), PopupNotificationUI.Source.General, null);
                }
                if (newSpawnPoint != null)
                {
                    if (target2.IsOuterTrainBoss())
                    {
                        yield return heroManager.ForceMoveBoss(target2, oldSpawnPoint.GetRoomOwner(), newSpawnPoint.GetRoomOwner(), CustomCardEffectBumpPreviewConditional.GetBumpDirection(bumpAmount));
                    }
                    else if (!cardEffectParams.saveManager.PreviewMode)
                    {
                        oldSpawnPoint.SetCharacterState(null);
                        newSpawnPoint.SetCharacterState(target2);
                        RoomState roomOwner = oldSpawnPoint.GetRoomOwner();
                        if (roomOwner != null)
                        {
                            roomOwner.UpdateSpawnPointPositions(teamType2, -1, false, false);
                        }
                        RoomState roomOwner2 = newSpawnPoint.GetRoomOwner();
                        if (roomOwner2 != null)
                        {
                            roomOwner2.UpdateSpawnPointPositions(teamType2, -1, false, false);
                        }
                        target2.SetSpawnPoint(newSpawnPoint, false, false, null, 0f);
                    }
                    this.ascendingCharacters.Add(target2, new CharacterState.SpawnPointAscensionData(oldSpawnPoint, newSpawnPoint));
                }
                else if (CustomCardEffectBumpPreviewConditional.IsAttempt(bumpError))
                {
                    this.ascendAttemptCharacters.Add(target2);
                }
                if (!cardEffectParams.saveManager.PreviewMode)
                {
                    target2.GetCharacterUI().SetHighlightVisible(false, SelectionStyle.DEFAULT);
                }
                target2 = null;
                oldSpawnPoint = null;
                newSpawnPoint = null;
                goto IL_31F;
            }
            if (cardEffectParams.saveManager.PreviewMode)
            {
                foreach (KeyValuePair<CharacterState, CharacterState.SpawnPointAscensionData> keyValuePair in this.ascendingCharacters)
                {
                    CharacterState key = keyValuePair.Key;
                    CharacterState.SpawnPointAscensionData value = keyValuePair.Value;
                    SpawnPoint oldSpawnPoint2 = value.oldSpawnPoint;
                    int previewBumpAmount = value.newSpawnPoint.GetRoomOwner().GetRoomIndex() - oldSpawnPoint2.GetRoomOwner().GetRoomIndex();
                    key.SetPreviewBumpAmount(previewBumpAmount);
                }
                yield break;
            }
            foreach (KeyValuePair<CharacterState, CharacterState.SpawnPointAscensionData> keyValuePair2 in this.ascendingCharacters)
            {
                CharacterState key2 = keyValuePair2.Key;
                CharacterState.SpawnPointAscensionData value2 = keyValuePair2.Value;
                SpawnPoint oldSpawnPoint3 = value2.oldSpawnPoint;
                SpawnPoint newSpawnPoint2 = value2.newSpawnPoint;
                Team.Type teamType3 = key2.GetTeamType();
                if (!key2.IsOuterTrainBoss())
                {
                    RoomState roomOwner3 = oldSpawnPoint3.GetRoomOwner();
                    if (roomOwner3 != null)
                    {
                        roomOwner3.UpdateSpawnPointPositions(teamType3, -1, false, true);
                    }
                    RoomState roomOwner4 = newSpawnPoint2.GetRoomOwner();
                    if (roomOwner4 != null)
                    {
                        roomOwner4.UpdateSpawnPointPositions(teamType3, -1, false, true);
                    }
                }
            }
            roomManager.AllowEnchantmentUpdates = false;
            int targetIndex = 0;
            foreach (KeyValuePair<CharacterState, CharacterState.SpawnPointAscensionData> keyValuePair3 in this.ascendingCharacters)
            {
                CharacterState target2 = keyValuePair3.Key;
                CharacterState.SpawnPointAscensionData value3 = keyValuePair3.Value;
                SpawnPoint newSpawnPoint = value3.oldSpawnPoint;
                SpawnPoint oldSpawnPoint = value3.newSpawnPoint;
                target2.ChatterClearedSignal.Dispatch(true);
                if (target2.HasStatusEffect("relentless"))
                {
                    yield return roomManager.GetRoomUI().SetSelectedRoom(oldSpawnPoint.GetRoomOwner().GetRoomIndex(), false);
                }
                if (!target2.IsOuterTrainBoss())
                {
                    target2.MoveUpDownTrain(oldSpawnPoint, targetIndex, newSpawnPoint.GetRoomOwner().GetRoomIndex(), null, true);
                    int num = targetIndex;
                    targetIndex = num + 1;
                    target2 = null;
                    newSpawnPoint = null;
                    oldSpawnPoint = null;
                }
            }
            //Dictionary<CharacterState, CharacterState.SpawnPointAscensionData>.Enumerator enumerator2 = default(Dictionary<CharacterState, CharacterState.SpawnPointAscensionData>.Enumerator);
            foreach (KeyValuePair<CharacterState, CharacterState.SpawnPointAscensionData> keyValuePair4 in this.ascendingCharacters)
            {
                CharacterState target = keyValuePair4.Key;
                if (!target.IsOuterTrainBoss())
                {
                    yield return new WaitUntil(() => target.IsMovementDone);
                }
            }
            //enumerator2 = default(Dictionary<CharacterState, CharacterState.SpawnPointAscensionData>.Enumerator);
            if (!cardEffectParams.saveManager.PreviewMode)
            {
                cardEffectParams.saveManager.IncrementRunStat(RunStat.StatType.UnitsBumped, this.ascendingCharacters.Count, null);
            }
            roomManager.AllowEnchantmentUpdates = true;
            foreach (KeyValuePair<CharacterState, CharacterState.SpawnPointAscensionData> keyValuePair5 in this.ascendingCharacters)
            {
                CharacterState target2 = keyValuePair5.Key;
                Team.Type teamType = target2.GetTeamType();
                CharacterState.SpawnPointAscensionData value4 = keyValuePair5.Value;
                SpawnPoint oldSpawnPoint = value4.oldSpawnPoint;
                SpawnPoint newSpawnPoint = value4.newSpawnPoint;
                if (target2.HasStatusEffect("relentless"))
                {
                    yield return target2.DoAttackAnimation(cardEffectParams.combatManager.GetBalanceData().GetAnimationTimingData());
                    int roomIndex = oldSpawnPoint.GetRoomOwner().GetRoomIndex();
                    i = newSpawnPoint.GetRoomOwner().GetRoomIndex();
                    int num;
                    for (int roomIdx = roomIndex; roomIdx < i; roomIdx = num + 1)
                    {
                        yield return roomManager.GetRoom(roomIdx).DestroyRoom();
                        num = roomIdx;
                    }
                }
                if (!target2.IsOuterTrainBoss())
                {
                    if (teamType == Team.Type.Heroes)
                    {
                        yield return heroManager.PostAscensionDescensionSingularCharacterTrigger(target2, CustomCardEffectBumpPreviewConditional.GetBumpDirection(bumpAmount), false);
                    }
                    target2 = null;
                    oldSpawnPoint = null;
                    newSpawnPoint = null;
                }
            }
            //enumerator2 = default(Dictionary<CharacterState, CharacterState.SpawnPointAscensionData>.Enumerator);
            foreach (CharacterState characterState in this.ascendAttemptCharacters)
            {
                if (characterState.GetTeamType() == Team.Type.Heroes)
                {
                    yield return heroManager.PostAscensionDescensionSingularCharacterTrigger(characterState, CustomCardEffectBumpPreviewConditional.GetBumpDirection(bumpAmount), true);
                }
            }
            //List<CharacterState>.Enumerator enumerator3 = default(List<CharacterState>.Enumerator);
            yield break;
            //yield break;
        }

        // Token: 0x06000708 RID: 1800 RVA: 0x000216C8 File Offset: 0x0001F8C8
        private SpawnPoint FindBumpSpawnPoint(CharacterState target, int bumpAmount, RoomManager roomManager, out CardEffectBump.BumpError bumpError)
        {
            SpawnPoint spawnPoint;
            RoomState roomOwner = (spawnPoint = target.GetSpawnPoint(false)).GetRoomOwner();
            if (roomOwner == null)
            {
                bumpError = CardEffectBump.BumpError.NoRoom;
                return null;
            }
            int roomIndex = roomOwner.GetRoomIndex();
            bumpError = CardEffectBump.BumpError.None;
            int num = roomManager.GetNumRooms() - 1;
            bumpAmount = Mathf.Clamp(bumpAmount, -num, num);
            CardEffectBump.BumpDirection bumpDirection = CustomCardEffectBumpPreviewConditional.GetBumpDirection(bumpAmount);
            for (int i = 1; i <= Mathf.Abs(bumpAmount); i++)
            {
                int roomIndex2 = Mathf.Clamp(roomIndex + i * (int)bumpDirection, 0, num);
                RoomState room = roomManager.GetRoom(roomIndex2);
                if (!room.IsRoomEnabled())
                {
                    bumpError = CardEffectBump.BumpError.DestroyedRoom;
                    break;
                }
                SpawnPoint spawnPoint2 = null;
                if (target.IsOuterTrainBoss())
                {
                    if (room.GetIsPyreRoom() && target.GetBossState().GetCurrentAttackPhase() != BossState.AttackPhase.Relentless)
                    {
                        bumpError = CardEffectBump.BumpError.BossFurnaceRoom;
                        break;
                    }
                    spawnPoint2 = room.GetOuterTrainSpawnPoint();
                }
                else if (target.GetTeamType() == Team.Type.Heroes)
                {
                    spawnPoint2 = room.GetFirstEmptyHeroPoint();
                }
                else if (target.GetTeamType() == Team.Type.Monsters)
                {
                    if (room.GetIsPyreRoom())
                    {
                        bumpError = CardEffectBump.BumpError.FurnaceRoom;
                        break;
                    }
                    spawnPoint2 = room.GetFirstEmptyMonsterPoint();
                }
                if (spawnPoint2 == null)
                {
                    bumpError = CardEffectBump.BumpError.FullRoom;
                    break;
                }
                spawnPoint = spawnPoint2;
            }
            if (((spawnPoint != null) ? spawnPoint.GetRoomOwner().GetRoomIndex() : roomIndex) == roomIndex)
            {
                bumpError = CardEffectBump.BumpError.SameRoom;
                return null;
            }
            return spawnPoint;
        }

        // Token: 0x06000709 RID: 1801 RVA: 0x000217F1 File Offset: 0x0001F9F1
        private static CardEffectBump.BumpDirection GetBumpDirection(CardEffectData cardEffectData)
        {
            if (cardEffectData.GetUseIntRange())
            {
                if (cardEffectData.GetParamMinInt() > 0)
                {
                    return CardEffectBump.BumpDirection.Up;
                }
                if (cardEffectData.GetParamMaxInt() < 0)
                {
                    return CardEffectBump.BumpDirection.Down;
                }
            }
            if (cardEffectData.GetParamInt() > 0)
            {
                return CardEffectBump.BumpDirection.Up;
            }
            return CardEffectBump.BumpDirection.Down;
        }

        // Token: 0x0600070A RID: 1802 RVA: 0x0002181D File Offset: 0x0001FA1D
        private static CardEffectBump.BumpDirection GetBumpDirection(int bumpAmount)
        {
            if (bumpAmount <= 0)
            {
                return CardEffectBump.BumpDirection.Down;
            }
            return CardEffectBump.BumpDirection.Up;
        }

        // Token: 0x0600070B RID: 1803 RVA: 0x00021826 File Offset: 0x0001FA26
        private static bool IsAttempt(CardEffectBump.BumpError bumpError)
        {
            return bumpError == CardEffectBump.BumpError.SameRoom || bumpError == CardEffectBump.BumpError.FurnaceRoom || bumpError == CardEffectBump.BumpError.DestroyedRoom || bumpError == CardEffectBump.BumpError.FullRoom || bumpError == CardEffectBump.BumpError.BossFurnaceRoom || bumpError == CardEffectBump.BumpError.ImmobileCharacter;
        }

        // Token: 0x0600070C RID: 1804 RVA: 0x00021844 File Offset: 0x0001FA44
        public string GetTooltipBaseKey(CardEffectState cardEffectState)
        {
            string str = CustomCardEffectBumpPreviewConditional.GetBumpDirection(this.cachedState.GetSourceCardEffectData()).ToString();
            return "CardEffectBump_" + str;
        }

        // Token: 0x0600070E RID: 1806 RVA: 0x0002187C File Offset: 0x0001FA7C
        // Note: this type is marked as 'beforefieldinit'.
        static CustomCardEffectBumpPreviewConditional()
        {
            Dictionary<CardEffectBump.BumpError, string> dictionary = new Dictionary<CardEffectBump.BumpError, string>();
            dictionary[CardEffectBump.BumpError.None] = string.Empty;
            dictionary[CardEffectBump.BumpError.NoRoom] = "BumpError_NoRoom";
            dictionary[CardEffectBump.BumpError.SameRoom] = "BumpError_SameRoom";
            dictionary[CardEffectBump.BumpError.FurnaceRoom] = "BumpError_FurnaceRoom";
            dictionary[CardEffectBump.BumpError.DestroyedRoom] = "BumpError_DestroyedRoom";
            dictionary[CardEffectBump.BumpError.FullRoom] = "BumpError_FullRoom";
            dictionary[CardEffectBump.BumpError.BossFurnaceRoom] = "BumpError_BossFurnaceRoom";
            dictionary[CardEffectBump.BumpError.ImmobileCharacter] = "BumpError_ImmobileCharacter";
            CustomCardEffectBumpPreviewConditional.BumpErrorToMessageKey = dictionary;
        }

        // Token: 0x0400043D RID: 1085
        private static readonly Dictionary<CardEffectBump.BumpError, string> BumpErrorToMessageKey;

        // Token: 0x0400043E RID: 1086
        private Dictionary<CharacterState, CharacterState.SpawnPointAscensionData> ascendingCharacters;

        // Token: 0x0400043F RID: 1087
        private List<CharacterState> ascendAttemptCharacters;

        // Token: 0x04000440 RID: 1088
        private CardEffectState cachedState;
    }

    [HarmonyPatch(typeof(GameEffectHelper), "TestEffect")]
    public static class FindBumperEffect 
    {
        public static void Postfix(ref bool __result, ref CardEffectState effectState) 
        {
            if (effectState.GetCardEffect() is CardEffectBump && __result) 
            {
                CustomCardEffectBumpPreviewConditional.PlayedCardIsBumper = true;
            }
        }
    }

    [HarmonyPatch(typeof(CardManager), "PlayAnyCard")]
    public static class ResetConditionalBumperPreview 
    {
        public static void Postfix() 
        {
            CustomCardEffectBumpPreviewConditional.PlayedCardIsBumper = false;
            CustomCardEffectBumpPreviewConditional.PokedTargetsDict.Clear();
        }
    }

    [HarmonyPatch(typeof(CardSelectionBehaviour), "UnFocusCard")]
    public static class ResetConditionalBumperPreview2 
    {
        public static void Postfix()
        {
            CustomCardEffectBumpPreviewConditional.PlayedCardIsBumper = false;
            CustomCardEffectBumpPreviewConditional.PokedTargetsDict.Clear();
        }
    }
}