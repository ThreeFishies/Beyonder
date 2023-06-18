//CustomRelicEffectSpawnUnitOnAllFloorsOnUnitSpawned

using System;
using System.Collections;
using System.Collections.Generic;
using Trainworks.Constants;
using UnityEngine;

using Void.Init;

namespace CustomEffects
{

    // Token: 0x0200030E RID: 782
    public sealed class CustomRelicEffectSpawnUnitOnAllFloorsOnUnitSpawned : RelicEffectBase, IRelicEffect, ICharacterGeneratingRelicEffect
    {
        // Token: 0x060018B5 RID: 6325 RVA: 0x0005E93D File Offset: 0x0005CB3D
        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
        {
            base.Initialize(relicState, relicData, relicEffectData);
            this.targetTeam = relicEffectData.GetParamSourceTeam();
            this.paramCharacters = relicEffectData.GetParamCharacters();
            this.paramCardPool = relicEffectData.GetParamCardPool();
        }

        // Token: 0x060018B6 RID: 6326 RVA: 0x0005E96C File Offset: 0x0005CB6C
        public bool TestEffect(RelicEffectParams relicEffectParams)
        {
            for (int ii = 0; ii < relicEffectParams.roomManager.GetNumRooms(); ii++)
            {
                if (this.targetTeam == Team.Type.Monsters && relicEffectParams.roomManager.GetRoom(ii).GetIsPyreRoom())
                {
                    continue;
                }
                if (!relicEffectParams.roomManager.GetRoom(ii).IsRoomEnabled())
                {
                    continue;
                }
                if (this.targetTeam == Team.Type.Monsters && relicEffectParams.roomManager.GetRoom(ii).GetFirstEmptyMonsterPoint() != null)
                {
                    return true;
                }
                if (this.targetTeam == Team.Type.Heroes && relicEffectParams.roomManager.GetRoom(ii).GetFirstEmptyHeroPoint() != null)
                {
                    return true;
                }
            }

            return false;
        }

        // Token: 0x060018B7 RID: 6327 RVA: 0x0005E98C File Offset: 0x0005CB8C
        public override IEnumerator OnCharacterAdded(CharacterState character, CardState fromCard, RelicManager relicManager, SaveManager saveManager, PlayerManager playerManager, RoomManager roomManager, CombatManager combatManager, CardManager cardManager)
        {
            SpawnPoint spawnPoint = null;

            for (int roomIndex = roomManager.GetNumRooms() - 1; roomIndex >= 0; roomIndex--)
            {
                if (this.targetTeam == Team.Type.Monsters && roomManager.GetRoom(roomIndex).GetIsPyreRoom())
                {
                    continue;
                }

                if (this.targetTeam == Team.Type.Monsters)
                {
                    spawnPoint = roomManager.GetRoom(roomIndex).GetFirstEmptyMonsterPoint();
                }
                else 
                {
                    spawnPoint = roomManager.GetRoom(roomIndex).GetFirstEmptyHeroPoint();
                }

                if (spawnPoint == null) 
                {
                    continue;
                }

                yield return roomManager.GetRoomUI().SetSelectedRoom(roomIndex, false);
                int soundGateId = combatManager.IgnoreDuplicateSounds(false);
                if (this.targetTeam.HasFlag(Team.Type.Heroes))
                {
                    foreach (CharacterData heroData in this.paramCharacters)
                    {
                        CharacterState characterState = null;
                        yield return combatManager.GetHeroManager().SpawnHeroInRoom(heroData, roomIndex, characterState);
                        if (characterState != null)
                        {
                            roomManager.GetRoom(roomIndex).UpdateSpawnPointPositions(Team.Type.Heroes, -1, false, true);
                            characterState.ShowNotification(string.Empty, PopupNotificationUI.Source.General, this._srcRelicState);
                            yield return CoreUtil.WaitForSecondsOrBreak(combatManager.ActiveTiming.RelicEffectSpawnUnitStartOfCombatSuccessiveUnitsDelay);
                            //yield return combatManager.RunTriggerQueue();
                            //yield return new WaitUntil(() => !combatManager.IsRunningTriggerQueue);
                        }
                        characterState = null;
                    }
                    //List<CharacterData>.Enumerator enumerator = default(List<CharacterData>.Enumerator);
                }
                else if (this.targetTeam.HasFlag(Team.Type.Monsters))
                {
                    foreach (CharacterData monsterData in this.paramCharacters)
                    {
                        CharacterState characterState = null;
                        yield return combatManager.GetMonsterManager().CreateMonsterState(monsterData, null, roomIndex, delegate (CharacterState _character)
                        {
                            characterState = _character;
                        }, SpawnMode.FrontSlot, null, null, false, null, null, true);
                        if (characterState != null)
				        {
                            characterState.ShowNotification(this.GetActivatedDescription().Localize(), PopupNotificationUI.Source.General, this._srcRelicState);
                            yield return CoreUtil.WaitForSecondsOrBreak(combatManager.ActiveTiming.RelicEffectSpawnUnitStartOfCombatSuccessiveUnitsDelay);

                            if (characterState.GetSourceCharacterData().GetID() == VanillaCharacterIDs.Transcendimp) 
                            {
                                Beyonder.Log("Transcendimp detected. Attempting to replay summon triggers.");
                                yield return combatManager.ReplayBattleTriggers(targetTeam, CharacterTriggerData.Trigger.OnSpawn, roomIndex, null, characterState, characterState.GetSpawnPoint());
                            }
                            //yield return combatManager.RunTriggerQueue();
                            //yield return new WaitUntil(() => !combatManager.IsRunningTriggerQueue);
                        }
                        characterState = null;
                    }
                    /*
                    foreach (CardData spawner in this.paramCardPool.GetAllChoices()) 
                    {
                        if (spawner.IsSpawnerCard()) 
                        {
                            CardState cardState = cardManager.AddCard(spawner, CardPile.None, 1, 1, true, false, null);
                            if (cardState != null) 
                            {
                                combatManager.GetMonsterManager().SetOverrideIgnoreCapacity(true);
                                cardManager.PlayAnyCard(cardState, spawnPoint, true, false);
                                combatManager.GetMonsterManager().SetOverrideIgnoreCapacity(false);

                                CharacterState characterState = spawnPoint.GetCharacterState();
                                if (characterState != null)
                                {
                                    characterState.ShowNotification(this.GetActivatedDescription().Localize(), PopupNotificationUI.Source.General, this._srcRelicState);
                                    yield return CoreUtil.WaitForSecondsOrBreak(combatManager.ActiveTiming.RelicEffectSpawnUnitStartOfCombatSuccessiveUnitsDelay);
                                }
                            }
                        }
                    }
                    */
                    //List<CharacterData>.Enumerator enumerator = default(List<CharacterData>.Enumerator);
                }
                yield return CoreUtil.WaitForSecondsOrBreak(combatManager.ActiveTiming.RelicEffectPostUnitSpawnPause);
                combatManager.ReleaseIgnoreDuplicateCuesHandle(soundGateId);
                //yield break;
            }
            yield break;
        }

        // Token: 0x060018B8 RID: 6328 RVA: 0x0001C7D4 File Offset: 0x0001A9D4
        public override string GetActivatedDescription()
        {
            return string.Empty;
        }

        // Token: 0x060018B9 RID: 6329 RVA: 0x0005E9A2 File Offset: 0x0005CBA2
        public CharacterData GetGeneratedCharacterDataForTooltip()
        {
            return this.paramCharacters.GetValueOrDefault(0);
        }

        // Token: 0x04000D19 RID: 3353
        private Team.Type targetTeam;

        // Token: 0x04000D1A RID: 3354
        private List<CharacterData> paramCharacters;

        private CardPool paramCardPool;
    }
}