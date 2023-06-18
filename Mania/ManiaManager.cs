using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using System.Collections;
using System;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Trainworks.Enums.MTTriggers;
using Void.Init;
using Void.Triggers;
using Void.Artifacts;
using Void.Status;
using Void.Monsters;
using Void.Tutorial;
using ShinyShoe;
using UnityEngine.SceneManagement;
using CustomEffects;

namespace Void.Mania
{
    public enum ManiaLevel 
    { 
        Default = 0,
        High = 1,
        Panic = 2,
        BlackoutHigh = 3,
        Low = -1,
        Neurosis = -2,
        BlackoutLow = -3
    }

    public static class ManiaManager
    {
        public static int Mania = 0;
        //public static RelicManager relicManager;
        public static ManiaLevel maniaLevel = ManiaLevel.Default;
        public static int InsanityMultiplier = 2;
        public static int InsanityThreshold = 3;
        public static int BlackoutThreshold = 10;
        public static bool PreviewHysteria = false;
        public static bool PreviewAnxiety = false;
        public static bool PreviewSanity = false;
        public static bool SetSelectedRoomFlag = true;
        public static int SelectedRoomIndex = -1;
        public static List<CombatManager.TriggerQueueData> triggerQueueDatas = new List<CombatManager.TriggerQueueData>();
        public static List<CharacterState> chutzpahTargets = new List<CharacterState>();
        public static List<int> chutzpahCounts = new List<int>();
        public static List<CharacterTriggerData.Trigger> chutzpahAssociatedTriggers = new List<CharacterTriggerData.Trigger>();
        //public static CardManager.OnCardPlayedEvent OnCardPlayed = new CardManager.OnCardPlayedEvent(OnPlayedCard);
        public static Coroutine coroutine = null;

        /*
        public static void OnPlayedCard(CardState cardState, int roomIndex, SpawnPoint dropLocation, bool fromPlayedCard, CombatManager.ApplyPreEffectsVfxAction onPreEffectsFiredVfx, CombatManager.ApplyEffectsAction onEffectsFired) 
        {
            if (coroutine == null)
            {
                coroutine = GlobalMonoBehavior.Inst.StartCoroutine(OnPlayedCardCouroutine());
            }
            else 
            {
                Beyonder.Log("Attempted to run trigger queue while it was already running");
            }
        }
        */

        //This is called after a played card's effects are resolved.
        //Testing to see if delaying manic triggers fixes artifact issues.
        public static void RunTriggerQueue(CardState playedCard) 
        {
            //Beyonder.Log("Flushing queue after card: " + playedCard.GetTitle());

            if (coroutine == null)
            {
                coroutine = GlobalMonoBehavior.Inst.StartCoroutine(OnPlayedCardCouroutine());
            }
            else
            {
                Beyonder.Log("Attempted to run trigger queue while it was already running");
            }
        }

        public static IEnumerator OnPlayedCardCouroutine() 
        {
            int aa = triggerQueueDatas.Count;
            int bb = chutzpahTargets.Count;
            int cc = chutzpahCounts.Count;
            int dd = chutzpahAssociatedTriggers.Count;

            if (aa > 0 && (aa == bb && aa == cc && aa == dd))
            {
                if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager) || !ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
                {
                    yield break;
                }

                screenManager.GetScreen(screenManager.GetTopFullScreen()).SetScreenInteractable(false);
                roomManager.GetRoomUI().DisableScrolling();

                for (int ii = 0; ii < aa; ii++)
                {

                    if (triggerQueueDatas[0].character.GetCurrentRoomIndex() != roomManager.GetSelectedRoom()) 
                    {
                        roomManager.GetRoomUI().EnableScrolling();
                    }
                    //Beyonder.Log($"Character: {triggerQueueDatas[0].character.GetName()} Trigger: {triggerQueueDatas[0].trigger.ToString()} Count: {triggerQueueDatas[0].triggerCount}");

                    //ProviderManager.CombatManager.QueueTrigger(triggerQueueDatas[0]);
                    //CustomTriggerManager.QueueAndRunTrigger(triggerQueueDatas[0].trigger, triggerQueueDatas[0].character, triggerQueueDatas[0].canAttackOrHeal, triggerQueueDatas[0].canFireTriggers, triggerQueueDatas[0].fireTriggersData, triggerQueueDatas[0].triggerCount);
                    yield return triggerQueueDatas[0].character.FireTriggers(triggerQueueDatas[0].trigger, triggerQueueDatas[0].canAttackOrHeal, triggerQueueDatas[0].canFireTriggers, triggerQueueDatas[0].fireTriggersData, triggerQueueDatas[0].triggerCount, true);
                    ApplyChutzpahCardTrigger(chutzpahTargets[0], chutzpahCounts[0], chutzpahAssociatedTriggers[0]);
                    roomManager.GetRoomUI().DisableScrolling();

                    triggerQueueDatas.RemoveAt(0);
                    chutzpahTargets.RemoveAt(0);
                    chutzpahCounts.RemoveAt(0);
                    chutzpahAssociatedTriggers.RemoveAt(0);
                }

                screenManager.GetScreen(screenManager.GetTopFullScreen()).SetScreenInteractable(true);
                roomManager.GetRoomUI().EnableScrolling();

                //yield return ProviderManager.CombatManager.RunTriggerQueue();
                //yield return new WaitUntil(() => !ProviderManager.CombatManager.IsRunningTriggerQueue);

                //CustomTriggerManager.RunTriggerQueueRemote();
            }
            else if (aa > 0)
            {
                Beyonder.Log("Something went wrong when attempting to run trigger queue.");
            }
            triggerQueueDatas.Clear();
            chutzpahTargets.Clear();
            chutzpahCounts.Clear();
            chutzpahAssociatedTriggers.Clear();

            coroutine = null;

            if (!SetSelectedRoomFlag && SelectedRoomIndex != -1)
            {
                SetSelectedRoomFlag = true;
                if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager) && (roomManager.GetSelectedRoom() != ManiaManager.SelectedRoomIndex))
                {
                    yield return roomManager.GetRoomUI().SetSelectedRoom(ManiaManager.SelectedRoomIndex);
                }
            }

            yield break;
        }

        public static int GetInsanityThreshold() 
        {
            int relicModifier = 0;

            if (BrainClamps.HasIt()) 
            {
                relicModifier += 2;
            }

            return InsanityThreshold + relicModifier;
        }

        public static int GetInsanityMultiplier() 
        {
            //Add relic effects here.
            if (TearInReality.HasIt()) 
            {
                return InsanityMultiplier + 1;
            }

            return InsanityMultiplier;
        }

        public static int GetCurrentMania() 
        { 
            return (PreviewSanity ? 0 : (Mania + (PreviewAnxiety ? -1 : 0) + (PreviewHysteria ? 1 : 0)));
        }

        public static ManiaLevel GetCurrentManiaLevel(bool UseCurrentMania = true, int ForMania = 0)
        {
            //if (!ProviderManager.SaveManager.PreviewMode)
            //{
            //    return maniaLevel;
            //}

            int currentMania = ForMania;

            if (UseCurrentMania)
            {
                currentMania = GetCurrentMania();
            }

            if (currentMania == 0)
            {
                return ManiaLevel.Default;
            }
            else if (currentMania > 0 && currentMania < GetInsanityThreshold())
            {
                return ManiaLevel.High;
            }
            else if (currentMania >= GetInsanityThreshold() && currentMania < BlackoutThreshold)
            {
                return ManiaLevel.Panic;
            }
            else if (currentMania >= BlackoutThreshold) 
            {
                return ManiaLevel.BlackoutHigh;
            }
            else if (currentMania < 0 && currentMania > -GetInsanityThreshold())
            {
                return ManiaLevel.Low;
            }
            else if (currentMania <= -GetInsanityThreshold() && currentMania > -BlackoutThreshold)
            {
                return ManiaLevel.Neurosis;
            }
            else if (currentMania <= -BlackoutThreshold)
            {
                return ManiaLevel.BlackoutLow;
            }

            return ManiaLevel.Default;
        }

        public static int GetEntopicScalingValue(bool useCurrentMania = true, int forManiaValueOf = 0) 
        {
            int temp_mania = (PreviewSanity ? 0 : (Mania + (PreviewAnxiety ? -1 : 0) + (PreviewHysteria ? 1 : 0)));

            if (!useCurrentMania) 
            {
                temp_mania = forManiaValueOf;
            }

            if (Math.Abs(temp_mania) >= GetInsanityThreshold()) 
            { 
                return GetInsanityMultiplier();
            }

            return 1;
        }

        /*
        public static int GetAnxietyScalingValue(bool Preview = true)
        {
            if (BlackLight.HasIt())
            {
                return GetBlackLightScalingValue(Preview);
            }

            if (!Preview)
            {
                return (Mania < 0) ? -Mania : 0;
            }

            int temp_mania = (PreviewSanity ? 0 : (Mania + (PreviewAnxiety ? -1 : 0) + (PreviewHysteria ? 1 : 0)));

            if (temp_mania < 0) 
            {
                return -temp_mania;
            }

            return 0;
        }

        public static int GetHysteriaScalingValue(bool Preview = true)
        {
            if (BlackLight.HasIt()) 
            {
                return GetBlackLightScalingValue(Preview);
            }

            if (!Preview)
            {
                return (Mania > 0) ? Mania : 0;
            }

            //Beyonder.Log($"PreviewSanity: {PreviewSanity}, PreviewAnxiety: {PreviewAnxiety}, PreviewHysteria: {PreviewHysteria}, Mania: {Mania}");

            int temp_mania = (PreviewSanity ? 0 : (Mania + (PreviewAnxiety ? -1 : 0) + (PreviewHysteria ? 1 : 0)));

            //Beyonder.Log($"TempMania: {temp_mania}.");

            if (temp_mania > 0)
            {
                return temp_mania;
            }

            return 0;
        }

        public static int GetBlackLightScalingValue(bool Preview = true) 
        {
            if (!Preview)
            {
                return (Mania < 0) ? -Mania : Mania;
            }

            int temp_mania = (PreviewSanity ? 0 : (Mania + (PreviewAnxiety ? -1 : 0) + (PreviewHysteria ? 1 : 0)));

            if (temp_mania < 0)
            {
                return -temp_mania;
            }

            return temp_mania;
        }
        */

        public static bool iiisiiinit = false;
        public static int expectedNumListeners = 0;
        
        public static void IIInit(CardManager cardManager) 
        {
            //disable this for testing purposes.
            //return;

            if (cardManager == null)
            {
                return;
            }

            if (!iiisiiinit) 
            {
                expectedNumListeners = cardManager.cardPlayedSignal.Count + 1;
            }

            if (cardManager.cardPlayedSignal.Count < expectedNumListeners)
            {
                cardManager.cardPlayedSignal.AddListener(new Action<CardState>(RunTriggerQueue));
            }

            iiisiiinit = true;
        }
        

        public static void Init()
        {
            //relicManager = ProviderManager.SaveManager.RelicManager;
            triggerQueueDatas.Clear();
            chutzpahTargets.Clear();
            chutzpahCounts.Clear();
            chutzpahAssociatedTriggers.Clear();

            //RegisterTriggers();
            //SetupCrazyStick();
            //SetupPanicButton();
            //SetupRocksFall();
        }

        //public static void RegisterTriggers() 
        //{
        //    Hysteria = new CharacterTrigger("Beyonder_Hysteria");
        //    Anxiety = new CharacterTrigger("Beyonder_Anxiety");
        //}

        public static IEnumerator EnqueueAllPossibleTargets(CharacterTriggerData.Trigger trigger, int count, bool applynow = false)
        {
            if (!ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
            {
                yield break;
            }

            if (ProviderManager.CombatManager == null) 
            {
                yield break;
            }

            triggerQueueDatas.Clear();
            chutzpahTargets.Clear();
            chutzpahCounts.Clear();
            chutzpahAssociatedTriggers.Clear();

            List<CharacterState> allTargets = new List<CharacterState>();

            //Run the selected room first
            roomManager.GetRoom(SelectedRoomIndex).AddCharactersToList(allTargets, Team.Type.Heroes);
            yield return ProcessListToQueue(allTargets, trigger, count, applynow);
            roomManager.GetRoom(SelectedRoomIndex).AddCharactersToList(allTargets, Team.Type.Monsters);
            yield return ProcessListToQueue(allTargets, trigger, count, applynow);

            //Then rooms from top to bottom.
            for (int ii = roomManager.GetNumRooms() - 1; ii >= 0; ii--)
            {
                //skipping selected room so it isn't run twice
                if (ii == SelectedRoomIndex) 
                {
                    continue;
                }

                //and other floors in prieview mode because they simply trigger instead of previewing
                if (ProviderManager.SaveManager.PreviewMode) 
                {
                    if (ii != SelectedRoomIndex) 
                    {
                        continue;
                    }
                }

                if (!roomManager.GetRoomEnabled(ii)) { continue; }

                if (roomManager.GetRoom(ii).GetIsPyreRoom())
                {
                    roomManager.GetRoom(ii).AddCharactersToList(allTargets, Team.Type.Heroes);
                    yield return ProcessListToQueue(allTargets, trigger, count, applynow);
                }
                else
                {
                    roomManager.GetRoom(ii).AddCharactersToList(allTargets, Team.Type.Heroes);
                    yield return ProcessListToQueue(allTargets, trigger, count, applynow);
                    roomManager.GetRoom(ii).AddCharactersToList(allTargets, Team.Type.Monsters);
                    yield return ProcessListToQueue(allTargets, trigger, count, applynow);
                }
            }

            if (applynow && SelectedRoomIndex != -1 && !SetSelectedRoomFlag) 
            {
                yield return roomManager.GetRoomUI().SetSelectedRoom(SelectedRoomIndex);
            }

            yield break;
        }

        private static IEnumerator ProcessListToQueue(List<CharacterState> allTargets, CharacterTriggerData.Trigger trigger, int count, bool applynow = false) 
        {
            foreach (CharacterState character in allTargets)
            {
                CombatManager.TriggerQueueData triggerQueueData = new CombatManager.TriggerQueueData()
                {
                    trigger = trigger,
                    canAttackOrHeal = true,
                    canFireTriggers = true,
                    triggerCount = count,
                    fireTriggersData = null,
                    character = character
                };

                if (ProviderManager.SaveManager.PreviewMode || applynow)
                {
                    //immediate before card effect ..?
                    //yield return ProviderManager.CombatManager.QueueAndRunTrigger(character, trigger, true, true, null, count);
                    //Prefer after card effects.

                    ProviderManager.CombatManager.QueueTrigger(triggerQueueData);
                    ApplyChutzpahCardTrigger(character, count, trigger);
                }
                else 
                { 
                    triggerQueueDatas.Add(triggerQueueData);
                    chutzpahTargets.Add(character);
                    chutzpahCounts.Add(count);
                    chutzpahAssociatedTriggers.Add(trigger);
                }
            }

            allTargets.Clear();
            yield break;
        }

        //This is no longer needed as the trigger queue was redesigned to reduce camera hopping between floors.
        /*
        public static CharacterState[] GatherAllPossibleTargets()
        {
            List<CharacterState> allTargets = new List<CharacterState> { };

            if (!ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
            {
                return allTargets.ToArray();
            }

            for (int ii = roomManager.GetNumRooms() - 1; ii >= 0; ii--) 
            {
                if (!roomManager.GetRoomEnabled(ii)) { continue; }

                if (roomManager.GetRoom(ii).GetIsPyreRoom())
                {
                    roomManager.GetRoom(ii).AddCharactersToList(allTargets, Team.Type.Heroes);
                }
                else 
                {
                    roomManager.GetRoom(ii).AddCharactersToList(allTargets, Team.Type.Heroes);
                    roomManager.GetRoom(ii).AddCharactersToList(allTargets, Team.Type.Monsters);
                }
            }

            return allTargets.ToArray();
        }
        */

        /// <summary>
        /// Compulsion reduces Mania, triggering Anxiety and Neurosis.
        /// </summary>
        /// <param name="paramInt">How much Mania is reduced by.</param>
        /// <returns></returns>
        public static IEnumerator Compulsion(int paramInt, bool applynow = false) 
        {
            if (paramInt == 0) { yield break; }

            if (ProviderManager.SaveManager.PreviewMode) 
            {
                PreviewAnxiety = true;
                PreviewHysteria = false;
                PreviewSanity = false;
            }
            else 
            {
                PreviewAnxiety = false;
                PreviewHysteria = false;
                PreviewSanity = false;
            }

            if (Mania - paramInt < 0) 
            { 
                int numTriggers = -Mathf.Max(Mania - paramInt, -paramInt);

                //This only seems to trigger Anxiety on the first unit, then no other triggers activate.
                //Try something else.
                //CustomTriggerManager.QueueAndRunTrigger<MonsterManager>(Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger, true, true, null, numTriggers);

                //if (!PreviewAnxiety)
                //{
                    SetSelectedRoomFlag = false;
                    if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager)) 
                    {
                        SelectedRoomIndex = roomManager.GetSelectedRoom();
                    }
                //CustomTriggerManager.QueueTrigger(Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger, GatherAllPossibleTargets(), true, true, null, numTriggers);
                //}

                //I'm curious if this will suppress the effect until later.
                //I... think it works...?
                //Not well enough :(
                //yield return null;
                //yield return null;
                //yield return null;
                //yield return null;
                //yield return null;
                //yield return null;

                yield return EnqueueAllPossibleTargets(Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(), numTriggers, applynow);

                if (BlackLight.HasIt()) 
                {
                    yield return EnqueueAllPossibleTargets(Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(), numTriggers, applynow);
                }

                maniaLevel = ManiaLevel.Low;
            }

            if (!PreviewAnxiety)
            {
                Mania -= paramInt;

                if(ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager))
                {
                    cardManager.RefreshHandCards();                
                }

                maniaLevel = GetCurrentManiaLevel(false, Mania);
            }

            if (Mania <= -GetInsanityThreshold())
            {
                //Shock
                maniaLevel = ManiaLevel.Neurosis;

                yield return TriggerInsanity(StatusEffectShock.statusId, 1);
            }

            if (Mania == 0 && !PreviewAnxiety && EyevoryEyedol.HasIt() && paramInt > 0)
            {
                if (ProviderManager.TryGetProvider<PlayerManager>(out PlayerManager playerManager))
                {
                    playerManager.AddEnergy(1);

                    if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
                    {
                        if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.RelicManager != null)
                        {
                            ProviderManager.SaveManager.RelicManager.ShowRelicActivated(EyevoryEyedol.Artifact.GetIcon(), EyevoryEyedol.Artifact.GetRelicActivatedKey().Localize(), roomManager.GetRoom(roomManager.GetSelectedRoom()));
                        }
                    }
                }
            }

            ManiaUI.UpdateUI();

            yield break;
        }

        /// <summary>
        /// Affliction increases Mania, triggering Hysteria and Panic.
        /// </summary>
        /// <param name="paramInt">How much Mania is increased by.</param>
        /// <returns></returns>
        public static IEnumerator Affliction(int paramInt, bool applynow = false)
        {
            if (paramInt == 0) { yield break; }

            if (ProviderManager.SaveManager.PreviewMode)
            {
                PreviewAnxiety = false;
                PreviewHysteria = true;
                PreviewSanity = false;
            }
            else 
            {
                PreviewAnxiety = false;
                PreviewHysteria = false;
                PreviewSanity = false;
            }

            if (Mania + paramInt > 0)
            {
                int numTriggers = Mathf.Min(Mania + paramInt, paramInt);

                //This only seems to trigger Hysteria on the first unit, then no other triggers activate.
                //Try something else.
                //CustomTriggerManager.QueueAndRunTrigger<MonsterManager>(Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger, true, true, null, numTriggers);

                //if (!PreviewHysteria)
                //{
                    //CharacterState[] targets = GatherAllPossibleTargets();

                    SetSelectedRoomFlag = false;
                    if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
                    {
                        SelectedRoomIndex = roomManager.GetSelectedRoom();
                    }
                //CustomTriggerManager.QueueTrigger(Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger, targets, true, true, null, numTriggers);
                //}

                //yield return null;
                //yield return null;
                //yield return null;
                //yield return null;
                //yield return null;
                //yield return null;

                yield return EnqueueAllPossibleTargets(Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(), numTriggers, applynow);

                if (BlackLight.HasIt())
                {
                    yield return EnqueueAllPossibleTargets(Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(), numTriggers, applynow);
                }

                maniaLevel = ManiaLevel.High;
            }

            if (!PreviewHysteria)
            {
                Mania += paramInt;

                if (ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager))
                {
                    cardManager.RefreshHandCards();
                }

                maniaLevel = GetCurrentManiaLevel(false, Mania);
            }

            if (Mania >= GetInsanityThreshold())
            {
                //Panic
                maniaLevel = ManiaLevel.Panic;

                yield return TriggerInsanity(StatusEffectPanic.statusId, 1);
            }

            if (Mania == 0 && !PreviewHysteria && EyevoryEyedol.HasIt() && paramInt > 0)
            {
                if (ProviderManager.TryGetProvider<PlayerManager>(out PlayerManager playerManager)) 
                {
                    playerManager.AddEnergy(1);

                    if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
                    {
                        if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.RelicManager != null)
                        {
                            ProviderManager.SaveManager.RelicManager.ShowRelicActivated(EyevoryEyedol.Artifact.GetIcon(), EyevoryEyedol.Artifact.GetRelicActivatedKey().Localize(), roomManager.GetRoom(roomManager.GetSelectedRoom()));
                        }
                    }
                }
            }

            ManiaUI.UpdateUI();

            yield break;
        }

        public static IEnumerator Therapy() 
        {
            if (ProviderManager.SaveManager.PreviewMode)
            {
                PreviewAnxiety = false;
                PreviewHysteria = false;
                PreviewSanity = true;
            }
            else
            {
                PreviewAnxiety = false;
                PreviewHysteria = false;
                PreviewSanity = false;
            }

            if (Mania != 0 && !PreviewSanity && EyevoryEyedol.HasIt()) 
            {
                if (ProviderManager.TryGetProvider<PlayerManager>(out PlayerManager playerManager))
                {
                    playerManager.AddEnergy(1);

                    if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
                    {
                        if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.RelicManager != null)
                        {
                            ProviderManager.SaveManager.RelicManager.ShowRelicActivated(EyevoryEyedol.Artifact.GetIcon(), EyevoryEyedol.Artifact.GetRelicActivatedKey().Localize(), roomManager.GetRoom(roomManager.GetSelectedRoom()));
                        }
                    }
                }
            }

            if (!PreviewSanity) 
            {
                Mania = 0;
                maniaLevel = ManiaLevel.Default;
                if (ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager))
                {
                    cardManager.RefreshHandCards();
                }
            }

            ManiaUI.UpdateUI();

            yield break;
        }

        public static void ResetManiaOnTurnBegin()
        {
            //if (ProviderManager.SaveManager.PreviewMode) 
            //{
            //    PreviewAnxiety = false;
            //    PreviewHysteria = false;
            //    PreviewSanity = true;
            //    return;
            //}

            PreviewAnxiety = false;
            PreviewHysteria = false;
            PreviewSanity = false;
            Mania = 0;

            maniaLevel = ManiaLevel.Default;

            AddTrackManicCardsPlayed.cardCount = 0;

            ManiaUI.UpdateUI();

            return;
        }

        //The 'Blackout' mechanic at +/-10 Mania was abandoned. It will be used at some point to unlock card mastery frames.

        //public static CardEffectData RocksFall;
        /*
        public static void SetupRocksFall()
        {
            RocksFall = new CardEffectDataBuilder
            {
                EffectStateName = "CardEffectKill",
                TargetMode = TargetMode.Room,
                TargetTeamType = Team.Type.Monsters,
            }.Build();
        }
        */

        public static void ApplyChutzpahCardTrigger(CharacterState target, int numTriggers, CharacterTriggerData.Trigger associatedTrigger)
        {
            if (target == null || target.GetTeamType() != Team.Type.Monsters) { return; }

            CardTrigger cardTrigger = null;

            if (associatedTrigger == Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum()) 
            {
                cardTrigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCardTrigger;
            }
            if (associatedTrigger == Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum())
            {
                cardTrigger = Trigger_Beyonder_OnHysteria.OnHysteriaCardTrigger;
            }

            if (cardTrigger == null) { return; }

            bool hasAnxietyCardTrigger = false;

            if ((target.GetSpawnerCard() != null) && (target.GetSpawnerCard().GetCardTriggers().Count > 0)) 
            {
                foreach (CardTriggerEffectData cardTrigger1 in target.GetSpawnerCard().GetCardTriggers()) 
                {
                    if (cardTrigger1.GetTrigger() == Trigger_Beyonder_OnAnxiety.OnAnxietyCardTrigger.GetEnum()) 
                    {
                        hasAnxietyCardTrigger = true;
                        break;
                    }
                }
            }

            if (target.GetSpawnerCard() != null && (target.GetSpawnerCard().GetCardDataID() == Chutzpah.Card.GetID() || hasAnxietyCardTrigger))
            {
                numTriggers = numTriggers * target.GetTriggerFireCount(associatedTrigger);
                if (numTriggers > 0)
                {
                    for (int ii = 0; ii < numTriggers; ii++)
                    {
                        CustomTriggerManager.ApplyCardTriggers(cardTrigger, target.GetSpawnerCard(), false, -1, true, null, delegate 
                        {
                        });
                    }
                    if (ProviderManager.TryGetProvider<SoundManager>(out SoundManager soundManager))
                    {
                        if (cardTrigger == Trigger_Beyonder_OnHysteria.OnHysteriaCardTrigger && (target.GetSpawnerCard().GetCardDataID() == Chutzpah.Card.GetID()))
                        {
                            soundManager.PlaySfx("Multiplayer_Emote_Angry");
                        }
                        else if (cardTrigger == Trigger_Beyonder_OnAnxiety.OnAnxietyCardTrigger && hasAnxietyCardTrigger)
                        {
                            soundManager.PlaySfx("Multiplayer_Emote_Sad");
                        }
                    }
                }
            }
        }

        public static IEnumerator TriggerInsanity(string statusId, int count) 
        {
            if (ProviderManager.SaveManager.PreviewMode) { yield break; }
            if (count <= 0 || statusId.IsNullOrEmpty()) { yield break; }

            if (!ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
            {
                yield break;
            }

            List<CharacterState> monsters = new List<CharacterState>();

            for (int ii = 0; ii < roomManager.GetNumRooms(); ii++) 
            { 
                if (roomManager.GetRoom(ii).GetIsPyreRoom() || roomManager.GetRoom(ii).IsDestroyedOrInactive()) 
                {
                    continue;
                }

                roomManager.GetRoom(ii).AddCharactersToList(monsters, Team.Type.Monsters);
            }

            if (monsters.Count > 0) 
            {
                ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager);

                CharacterState.AddStatusEffectParams statusEffectParams = new CharacterState.AddStatusEffectParams
                {
                    cardManager = cardManager,
                    fromEffectType = null,
                    overrideImmunity = false,
                    relicEffects = null,
                    sourceCardState = null,
                    sourceIsHero = false,
                    sourceRelicState = null,
                    spawnEffect = false,
                };

                foreach (CharacterState monster in monsters) 
                {
                    monster.AddStatusEffect(statusId, count, statusEffectParams);
                }
            }

            TutorialManager.TryDoInsanity();

            yield break;
        }
    }

    [HarmonyPatch(typeof(CombatManager), "SetCombatPhase")]
    public static class ManiaResetAtStartOfTurnPatch
    {
        public static void Prefix(ref CombatManager.Phase newPhase)
        {
            if (newPhase == CombatManager.Phase.MonsterTurn)
            {
                ManiaManager.ResetManiaOnTurnBegin();
            }
        }
    }

    [HarmonyPatch(typeof(StatusEffectsDisplayData),"GetIcon")]
    public static class BeyonderTriggerIcons 
    { 
        public static void Postfix(ref CharacterTriggerData.Trigger trigger, ref Sprite __result) 
        {
            //return;

            if (!Beyonder.IsInit) { return; }

            if (trigger == Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum()) 
            {
                __result = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath,"ClanAssets/OnAnxiety.png"));
            }

            if (trigger == Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum()) 
            {
                __result = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/OnHysteria.png"));
            }
        }
    }

    [HarmonyPatch(typeof(CardSelectionBehaviour), "UnFocusCard")]
    public static class ExitPreviewState 
    {
        public static void Postfix() 
        {
            ManiaManager.PreviewAnxiety = false;
            ManiaManager.PreviewHysteria = false;
            ManiaManager.PreviewSanity = false;
            CustomCardEffectScaleDamageBySacrificeAllies.NumAlliesSacrificed = 0;
            CustomCardEffectSacrificeAssertTarget.HPofSacrifice = 0;
            CustomCardEffectSacrificeAssertTarget.SizeofSacrifice = 0;

            ManiaUI.UpdateUI();
        }
    }
    
    [HarmonyPatch(typeof(CombatManager), "StartCombat")]
    public static class InitManiaManager 
    {
        public static void Postfix(ref CardManager ___cardManager) 
        {
            ManiaManager.IIInit(___cardManager);
        }
    }
    
    /*
    [HarmonyPatch(typeof(CardManager), "PlayAnyCard")]
    public static class PlayManicTriggersLastHopefully 
    {
        public static void Postfix(ref bool __result)
        {
            if (__result && ProviderManager.SaveManager != null && !ProviderManager.SaveManager.PreviewMode) 
            {
                ManiaManager.OnPlayedCard(null, 0, null, true, null, null);
            }
        }
    }
    */
}