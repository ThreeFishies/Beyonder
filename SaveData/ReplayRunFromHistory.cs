using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Void.Unit;
using Void.Clan;
using Void.Status;
using Void.Init;
using Void.Triggers;
using Void.Champions;
using Void.Spells;
using Void.Monsters;
using CustomEffects;
using RunHistory;
using Malee;
using Void.Chaos;
using System;
using PubNubAPI;
using I2.Loc.SimpleJSON;
using ShinyShoe;
using System.Collections;
using UnityEngine.SceneManagement;
using Void.Mania;
using Void.Artifacts;

namespace Void.Init
{
    public static class StartNewRunFromRunHistory
    {
        public static RunSetupData setupData = new RunSetupData();
        public static Coroutine _coroutine = null;
        public static bool replayedRun = false;

        public static RunSetupData GetRunSetupDataFromRunAggregateData(RunAggregateData run)
        {
            if (BeyonderSaveManager.RunHistoryData.ContainsKey(run.GetID()))
            {
                setupData = BeyonderSaveManager.RunHistoryData[run.GetID()];
                return BeyonderSaveManager.RunHistoryData[run.GetID()];
            }

            RunSetupData currentData = BeyonderSaveManager.GetRunSetupData();

            currentData.RunID = run.GetID();
            currentData.StartingConditions = run.GetStartingConditions();

            setupData = currentData;

            return currentData;
        }

        public static void ConfirmDeleteRun()
        {
            string content = "ScreenMainMenu_ConfirmAbandonGame";

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            if (ProviderManager.SaveManager.HasRun())
            {
                screenManager.ShowConfirmationDialog(content.Localize(), delegate
                {
                    StartNewRun();
                }, null, Dialog.Anchor.Center, false);
            }
            else
            {
                StartNewRun();
            }
        }

        public static void StartNewRun()
        {
            _coroutine = GlobalMonoBehavior.Inst.StartCoroutine(StartNewRunCoroutine());
        }

        public static IEnumerator StartNewRunCoroutine()
        {
            if (!ProviderManager.TryGetProvider<GameStateManager>(out GameStateManager gameStateManager))
            {
                Beyonder.Log("Failed to locate GameStateManager. Can't start a new run.");
                yield break;
            }
            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                Beyonder.Log("Failed to locate ScreenManager. Can't start a new run.");
                yield break;
            }

            replayedRun = true;

            screenManager.ReturnToMainMenu();
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "main_menu" && SceneManager.GetActiveScene().isLoaded);
            yield return null;
            yield return null;
            yield return null;

            RunType runType = RunType.Class;

            if (setupData.StartingConditions.Mutators.Count > 0 && setupData.StartingConditions.SpChallengeId == "")
            {
                runType = RunType.Custom;
            }

            LocoMotive.SetupTrees(setupData.LocoMotivePathData.ConductorTreePathID, setupData.LocoMotivePathData.HorrorTreePathID, setupData.LocoMotivePathData.FormlessTreePathID);
            Epidemial.SetupTrees(setupData.EpidemialPathData.InnumerableTreePathID, setupData.EpidemialPathData.ContagiousTreePathID, setupData.EpidemialPathData.SoundlessTreePathID);
            ChaosManager.LoadFromData(setupData.BoonsBanesData);
            LocoMotive.BuildTreeForNewRun(RngId.NonDeterministic, false);
            Epidemial.BuildTreeForNewRun(RngId.NonDeterministic, false);
            ChaosManager.UpdateStartingUpgrades(setupData.BoonsBanesData);

            Beyonder.Log("Attempting to start a new run from RunSetupData: " + setupData.ToString());

            ProviderManager.SaveManager.Cheat_ForceSeed(setupData.StartingConditions.Seed);

            gameStateManager.StartRun(runType, "", setupData.StartingConditions, setupData.StartingConditions.IsFtueRun);

            ProviderManager.SaveManager.Cheat_ForceSeed(0);

            yield break;
        }
    }

    [HarmonyPatch(typeof(RunSummaryScreen), "HandleCreateChallengeButton")]
    public static class HandleReplayRunButton
    {
        public static bool Prefix(RunAggregateData ___runData)
        {
            StartNewRunFromRunHistory.GetRunSetupDataFromRunAggregateData(___runData);
            StartNewRunFromRunHistory.ConfirmDeleteRun();

            return false;
        }
    }

    [HarmonyPatch(typeof(RunSummaryDetailsUI), "Setup")]
    public static class SetupReplayRunButton
    {
        public static void Postfix(GameObject ___generateChallengeButton) 
        {
            //___createChallengeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "RunSummaryScreen_ReplayRunButton".Localize();

            //Beyonder.Log("Challenge Button: " + ___generateChallengeButton.name);

            foreach (Component gameObject in ___generateChallengeButton.GetComponentsInChildren<Component>()) 
            {
                //Beyonder.Log($"Game Object: {gameObject.GetType().Name} Name: {gameObject.name}");

                if (gameObject.GetType().Name == "TextMeshProUGUI" && gameObject.name == "Challenge button label")
                {
                    //Beyonder.Log($"Old text: {(gameObject as TMPro.TextMeshProUGUI).text}");
                    (gameObject as TMPro.TextMeshProUGUI).text = "RunSummaryScreen_ReplayRunButton".Localize();
                    //Beyonder.Log($"New text: {(gameObject as TMPro.TextMeshProUGUI).text}");
                }
            }

            //___generateChallengeButton.gameObject.SetActive(false);
        }
    }
}