using System;
using System.Collections;
using System.Collections.Generic;
using Trainworks.Builders;
using UnityEngine;
using Void.Champions;
using Void.Init;

namespace CustomEffects 
{ 
    // Token: 0x0200009C RID: 156
    public sealed class CustomCardEffectCopyUnitsAndApplyUpgrade : CardEffectBase
    {
        public int numCopiesOfEachUnit;
        public CharacterState newMonster;
        public CardUpgradeData cardUpgradeData;

        private bool GetUpgradeState(out CardUpgradeState cardUpgradeState) 
        {
            cardUpgradeState =  new CardUpgradeState();

            if (cardUpgradeData == null) 
            {
                return false;
            }

            cardUpgradeState.Setup(cardUpgradeData, false);
            return true;
        }

        // Token: 0x0600071B RID: 1819 RVA: 0x00021A18 File Offset: 0x0001FC18
        public override void Setup(CardEffectState cardEffectState)
        {
            base.Setup(cardEffectState);
            numCopiesOfEachUnit = cardEffectState.GetParamInt();
            cardUpgradeData = new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "EpidemialDuplicatedUnitDeathTrigger",
                TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                {
                    new CharacterTriggerDataBuilder
                    {
                        Trigger = CharacterTriggerData.Trigger.PostCombat,
                        DescriptionKey = "Beyonder_Champ_Epidemial_Innumerable_C_Death_DescriptionKey",
                        EffectBuilders = new List<CardEffectDataBuilder>
                        {
                            new CardEffectDataBuilder
                            {
                                EffectStateName = "CardEffectKill",
                                TargetMode = TargetMode.Self,
                                TargetTeamType = Team.Type.Monsters,
                            }
                        },
                    }
                }
            }.Build();
        }

        // Token: 0x0600071C RID: 1820 RVA: 0x00021094 File Offset: 0x0001F294
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            List<CharacterState> targets = new List<CharacterState>();
            foreach (CharacterState unit in cardEffectParams.targets)
            {
                if (unit.GetSourceCharacterData() == Epidemial.card.GetSpawnCharacterData())
                {
                }
                else if (unit.IsDead || unit.IsDestroyed || unit.IsSacrifice)
                {
                }
                else
                {
                    targets.Add(unit);
                }
            }

            return targets.Count > 0;
        }

        private CharacterState chosenOne;

        // Token: 0x0600071D RID: 1821 RVA: 0x00021A2D File Offset: 0x0001FC2D
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            int numSpawns = 0;
            chosenOne = null;

            foreach (CharacterState unit in cardEffectParams.targets)
            {
                if (unit.GetSourceCharacterData() == Epidemial.card.GetSpawnCharacterData())
                {
                }
                else if (unit.IsDead || unit.IsDestroyed || unit.IsSacrifice)
                {
                }
                else 
                {
                    if (chosenOne == null || unit.GetHP() < chosenOne.GetHP())
                    {
                        chosenOne = unit;
                    }
                }
            }

            if (chosenOne == null)
            {
                yield break;
            }
            CharacterState copyUnit = chosenOne;

            //SpawnPoint spawnPoint = copyUnit.GetSpawnPoint(false);
            SpawnPoint spawnPoint = cardEffectParams.roomManager.GetRoom(copyUnit.GetCurrentRoomIndex()).GetFirstEmptyMonsterPoint();
            if (spawnPoint != null)
            {
                int num = 0;
                for (int i = 0; i < numCopiesOfEachUnit; i = num + 1)
                {
                    newMonster = null;
                    yield return cardEffectParams.monsterManager.CreateMonsterState(copyUnit.GetSourceCharacterData(), copyUnit.GetSpawnerCard(), cardEffectParams.selectedRoom, delegate (CharacterState character)
                    {
                        newMonster = character;
                        //Beyonder.Log("New Monster: " + newMonster.GetSourceCharacterData().GetNameKey().Localize());
                        if (newMonster != null)
                        {
                            CharacterHelper.CopyCharacterStats(newMonster, copyUnit);
                            newMonster.AddStatusEffect("cardless", 1);
                        }
                    }, SpawnMode.BackSlot, null, null, false, null, null, false);
                    if (newMonster != null)
                    {
                        if (GetUpgradeState(out CardUpgradeState cardUpgradeState))
                        {
                            //Beyonder.Log("New Monster (again): " + newMonster.GetSourceCharacterData().GetNameKey().Localize());
                            yield return newMonster.ApplyCardUpgrade(cardUpgradeState);
                        }
                        num = numSpawns;
                        numSpawns = num + 1;
                    }
                    num = i;
                }
                spawnPoint = null;
            }
            
		    if (numSpawns > 1 && !cardEffectParams.saveManager.PreviewMode)
		    {
			    yield return cardEffectParams.roomManager.GetRoomUI().CenterCharacters(cardEffectParams.GetSelectedRoom(), false, false, false);
            }
            yield break;
	    }
    }
}