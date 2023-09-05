using ShinyShoe;
using System;
using Trainworks.Managers;
using Void.Init;
using System.Collections.Generic;
using HarmonyLib;

namespace CustomEffects
{

    // Token: 0x0200036E RID: 878
    public sealed class CustomRoomStateImpGangRiseUp : RoomStateModifierBase, IRoomStateDamageModifier, IRoomStateModifier, ILocalizationParamInt, ILocalizationParameterContext
    {
        // Token: 0x06001BAF RID: 7087 RVA: 0x0006848F File Offset: 0x0006668F
        public override void Initialize(RoomModifierData roomModifierData, RoomManager roomManager)
        {
            base.Initialize(roomModifierData, roomManager);
            this.subtype = roomModifierData.GetParamSubtype();
            this.statusEffects = roomModifierData.GetParamStatusEffects();
            if (this.statusEffects.Length > 0)
            {
                this.statusID = this.statusEffects[0].statusId;
                this.additionalStrikes = this.statusEffects[0].count;
            }
        }

        // Token: 0x06001BB0 RID: 7088 RVA: 0x00067EB7 File Offset: 0x000660B7
        public int GetModifiedDamage(Damage.Type damageType, CharacterState attackerState, bool requestingForCharacterStats)
        {
            //Beyonder.Log("Line 29");
            if (requestingForCharacterStats)
            {
                //Beyonder.Log("Line 32");
                //DebugLogRoomContext(attackerState);
                ModifyStatusByContext(attackerState);
            }
            return 0;
        }

        private void DebugLogRoomContext(CharacterState monster)
        {
            if (monster == null)
            {
                Beyonder.Log("Monster does not exist!");
                return;
            }

            if (!monster.GetRoomStateModifiers().Contains(this))
            {
                //Beyonder.Log($"Ignoring. {monster.name} is not target unit.");
                return;
            }

            if (monster.GetTeamType() != Team.Type.Monsters)
            {
                Beyonder.Log($"No context. {monster.name} is not a monster.");
                return;
            }

            if (!monster.IsAlive)
            {
                Beyonder.Log($"No context. {monster.name} is not alive.");
                return;
            }

            if (monster.GetSpawnPoint() == null)
            {
                Beyonder.Log($"No context. {monster.name} has no spawn point.");
                return;
            }

            RoomState owner = monster.GetSpawnPoint().GetRoomOwner();

            if (owner == null)
            {
                Beyonder.Log($"No context. {monster.name} has no room owner.");
                return;
            }

            int floor = owner.GetRoomIndex();
            int monsterCount = owner.GetNumCharacters(Team.Type.Monsters);
            List<CharacterState> buddyList = new List<CharacterState>();
            owner.AddCharactersToList(buddyList, Team.Type.Monsters, false);
            string buddies = "";

            foreach (CharacterState unit in buddyList)
            {
                if (unit == monster)
                {
                    buddies += $", {monster.name}[*]";
                }
                else
                {
                    buddies += $", {monster.name}";
                }
            }
            if (buddies.Length > 3)
            {
                buddies = buddies.Substring(2);
            }

            Beyonder.Log($"Floor: {floor} | Monsters: {monsterCount} | Units: {buddies}");
        }

        public void OnStacksAdded(string statusId, int count)
        {
            if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.PreviewMode) 
            {
                return;
            }

            if (statusId == this.statusID)
            {
                this.baseValue += count;
            }
        }

        public void OnStacksRemoved(string statusId, int count)
        {
            if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.PreviewMode)
            {
                return;
            }

            if (statusId == this.statusID)
            {
                this.baseValue -= count;
                if (this.baseValue < 0)
                {
                    this.baseValue = 0;
                }
            }
        }

        private bool AdjustStatusEffectStack(CharacterState character, int target)
        {
            character.GetStatusEffects(out List<CharacterState.StatusEffectStack> statusEffects, true);

            if (statusEffects.Count > 0)
            {
                foreach (CharacterState.StatusEffectStack statusEffectStack in statusEffects)
                {
                    if (statusEffectStack.State.GetStatusId() == this.statusID)
                    {
                        if (!this.hasCachedBaseValue)
                        {
                            this.baseValue = statusEffectStack.Count;
                            this.hasCachedBaseValue = true;
                            statusEffectStack.Count += target;
                        }
                        else
                        {
                            statusEffectStack.Count = target;
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        private void ModifyStatusByContext(CharacterState unit)
        {
            //Beyonder.Log("Line 40");
            if (unit != null && unit.IsAlive && unit.GetRoomStateModifiers().Contains(this) && unit.GetCharacterUI().isActiveAndEnabled)
            {
                //Beyonder.Log("Line 43");
                int dynamicValue = this.GetDynamicInt(unit);

                AdjustStatusEffectStack(unit, dynamicValue + this.baseValue);
            }
        }

        // Token: 0x06001BB1 RID: 7089 RVA: 0x000684A8 File Offset: 0x000666A8
        public override int GetDynamicInt(CharacterState characterContext)
        {
            //Beyonder.Log("Line 73");
            if (FloorIsPureUnitsOfType(characterContext))
            {
                int Multiplier = 1;

                if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.GetMutatorCount() > 0)
                {
                    //Beyonder.Log("Line 106");
                    foreach (MutatorState mutator in ProviderManager.SaveManager.GetMutators())
                    {
                        //Beyonder.Log("Line 109");
                        if (mutator.GetRelicDataID() == "459c7ec3-9c35-4298-8873-bda051605210") // Duality
                        {
                            //Beyonder.Log("Line 112");
                            Multiplier = 2;
                        }
                    }
                }

                //Beyonder.Log("Line 76");
                RoomState roomOwner = characterContext.GetSpawnPoint(false).GetRoomOwner();
                if (roomOwner != null)
                {
                    //Beyonder.Log("Line 80");
                    return (roomOwner.GetNumCharacters(Team.Type.Monsters) - 1) * this.additionalStrikes * Multiplier;
                }
            }
            //Beyonder.Log("Line 84");
            return 0;
        }

        private bool FloorIsPureUnitsOfType(CharacterState unit)
        {
            //Beyonder.Log("Line 90");
            if (unit != null && unit.GetTeamType() == Team.Type.Monsters && unit.GetSpawnPoint(false) != null && unit.GetRoomStateModifiers().Contains(this))
            {
                //Beyonder.Log("Line 93");
                if (!ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager)) { return false; }

                //Beyonder.Log("Line 96");
                RoomState owner = roomManager.GetRoom(unit.GetCurrentRoomIndex());

                if (owner == null) { return false; }
                //Beyonder.Log("Line 100");
                if (owner.GetNumCharacters(Team.Type.Monsters) < 2) { return false; }
                //Beyonder.Log("Line 102");

                if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.GetMutatorCount() > 0)
                {
                    //Beyonder.Log("Line 106");
                    foreach (MutatorState mutator in ProviderManager.SaveManager.GetMutators())
                    {
                        //Beyonder.Log("Line 109");
                        if (mutator.GetRelicDataID() == "3894bbe3-6ad6-42ed-9f63-6b47371b4583") // Hivemind
                        {
                            //Beyonder.Log("Line 112");
                            return true;
                        }
                    }
                }

                List<CharacterState> monsters = new List<CharacterState>();
                owner.AddCharactersToList(monsters, Team.Type.Monsters, false);

                //Beyonder.Log("Line 121");
                foreach (CharacterState monster in monsters)
                {
                    //Beyonder.Log("Line 124");
                    if (monster.GetSubtypes().Count > 0)
                    {
                        //Beyonder.Log("Line 127");
                        for (int ii = 0; ii < monster.GetSubtypes().Count; ii++)
                        {
                            //Beyonder.Log("Line 130");
                            if (monster.GetSubtypes()[ii].Key != subtype.Key && monster.GetSubtypes()[ii].Key != "SubtypesData_Chosen")
                            {
                                //Beyonder.Log("Line 133");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //Beyonder.Log("Line 140");
                        return false;
                    }
                }
                //Beyonder.Log("Line 144");

                return true;
            }

            //Beyonder.Log("Line 149");
            return false;
        }

        // Token: 0x04000E64 RID: 3684
        private int additionalStrikes;
        private SubtypeData subtype;
        private StatusEffectStackData[] statusEffects;
        private string statusID;
        //private int currentStatusStackCount = 0;
        private bool hasCachedBaseValue = false;
        private int baseValue = 0;
    }

    [HarmonyPatch(typeof(CharacterState), "AddStatusEffect", new Type[] { typeof(string), typeof(int), typeof(CharacterState.AddStatusEffectParams) })]
    public static class AdjustBaseValueOnStacksAddedPatch 
    {
        public static void Postfix(ref string statusId, ref int numStacks, ref CharacterState __instance)
        {
            if (__instance.IsAlive && __instance.GetRoomStateModifiers().Count > 0 && __instance.GetSpawnPoint() != null && __instance.GetSpawnPoint().GetRoomOwner() != null && __instance.GetTeamType() == Team.Type.Monsters)
            {
                foreach (IRoomStateModifier modifier in __instance.GetRoomStateModifiers()) 
                {
                    if (modifier is CustomRoomStateImpGangRiseUp) 
                    {
                        CustomRoomStateImpGangRiseUp impGangRiseUp = modifier as CustomRoomStateImpGangRiseUp;
                        impGangRiseUp.OnStacksAdded(statusId, numStacks);
                        return;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharacterState), "RemoveStatusEffect")]
    public static class AdjustBaseValueOnStacksRemovedPatch
    {
        public static void Postfix(ref string statusId, ref bool removeAtEndOfTurn, ref int numStacks, ref CharacterState __instance)
        {
            if (!removeAtEndOfTurn && __instance.IsAlive && __instance.GetRoomStateModifiers().Count > 0 && __instance.GetSpawnPoint() != null && __instance.GetSpawnPoint().GetRoomOwner() != null && __instance.GetTeamType() == Team.Type.Monsters)
            {
                foreach (IRoomStateModifier modifier in __instance.GetRoomStateModifiers())
                {
                    if (modifier is CustomRoomStateImpGangRiseUp)
                    {
                        CustomRoomStateImpGangRiseUp impGangRiseUp = modifier as CustomRoomStateImpGangRiseUp;
                        impGangRiseUp.OnStacksRemoved(statusId, numStacks);
                        return;
                    }
                }
            }
        }
    }
}