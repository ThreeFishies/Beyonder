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
using Void.Champions;
using Void.Mania;
using Void.Spells;
using Void.Artifacts;
using Void.Triggers;
using Void.Monsters;
using Void.Enhancers;
using Void.Chaos;
using Void.CardPools;
using Void.Patches;
using System;
using Void.Init;
using PubNubAPI;
using UnityEngine.SceneManagement;

namespace Void.Tutorial
{
    [Serializable]
    public struct TutorialProgress 
    {
        [SerializeField]
        public bool HasSeenWelcome;
        [SerializeField]
        public bool HasSkippedTutorial;
        [SerializeField]
        public bool HasSeenIntro;
        [SerializeField]
        public bool HasSeenArtifact;
        [SerializeField]
        public bool HasSeenChampUpgrades;
        [SerializeField]
        public bool HasSeenFirstCombat;
        [SerializeField]
        public bool HasBeenInsane;
        [SerializeField]
        public bool HasSeenVictoryUI;
        [SerializeField]
        public bool HasSeenClanBanner;
        [SerializeField]
        public bool HasFinishedTutorial;

        //For whatever reason this doesn't work and the RunSetupData does not get saved when converting to .json.
        [SerializeField]
        public RunSetupData TutorialData;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public static class TutorialManager 
    {
        public static TutorialProgress CurrentProgress;
        public static bool isLoaded = false;
        public static bool dirty = false;

        public const string TutorialPath = "SaveData/Tutorial.json";
        public const string TutorialData = "TutorialRunSetupData.json";

        public static RunSetupData GetTutorialData() 
        {
            //int seed = 1091589910;
            int seed = 1390638784;
            //if (Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("mod.equestrian.clan.monstertrain"))
            //{
            //    seed = 123456;
            //    Beyonder.Log($"Adjusting tutorial seed for Equestrian clan: {seed}");
            //}
            //else 
            //{
            //    Beyonder.Log($"Using default tutorial seed: {seed}");
            //}

            RunSetupData runData = new RunSetupData
            {
                LocoMotiveHorrorPath = 0,
                LocoMotiveConductorPath = 0,
                LocoMotiveFormlessPath = 0,
                EpidemialContagiousPath = 0,
                EpidemialInnumerablePath = 0,
                EpidemialSoundlessPath = 0,
                VboonIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                VbaneIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                UboonIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                UbaneIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                BeyonderVersion = Beyonder.VERSION,
                ForceSeed = true,
                RunID = "TutorialRunID",
                StartingConditions = new StartingConditions { }
            };

            runData.StartingConditions.SetSeed(seed);
            runData.StartingConditions.SetIsBattleMode(false);
            runData.StartingConditions.SetFtue(false);
            runData.StartingConditions.SetBattleModeStartTime(DateTime.Now);
            runData.StartingConditions.SetBattleModeTimerScaler(0.0d);
            runData.StartingConditions.SetBattleWarmup(0);
            runData.StartingConditions.SetVersion("12923");
            runData.StartingConditions.SetMainClass("55bcaaa0-6e62-4b91-81c9-c44e93f9b0e6", 1, 0, false);
            runData.StartingConditions.SetSubclass("fd119fcf-c2cf-469e-8a5a-e9b0f265560d", 1, 0, false);
            runData.StartingConditions.SetAscensionLevel(1);
            runData.StartingConditions.SetSpChallengeId("");
            runData.StartingConditions.SetMutatorIds(new List<string> { "mod.beyonder.clan.monstertrain_FirstLaugh" });

            /*
            AccessTools.Field(typeof(StartingConditions), "seed").SetValue(runData.StartingConditions, 07734);
            AccessTools.Field(typeof(StartingConditions), "isBattleMode").SetValue(runData.StartingConditions, false);
            AccessTools.Field(typeof(StartingConditions), "isFtueRun").SetValue(runData.StartingConditions, false);
            AccessTools.Field(typeof(StartingConditions), "battleModeStartTime").SetValue(runData.StartingConditions, "");
            AccessTools.Field(typeof(StartingConditions), "battleModeTimerScalar").SetValue(runData.StartingConditions, 0.0d);
            AccessTools.Field(typeof(StartingConditions), "battleWarmup").SetValue(runData.StartingConditions, 0);
            AccessTools.Field(typeof(StartingConditions), "version").SetValue(runData.StartingConditions, "12923");
            AccessTools.Field(typeof(StartingConditions), "mainClassInfo").SetValue(runData.StartingConditions, new StartingConditions.ClassInfo("55bcaaa0-6e62-4b91-81c9-c44e93f9b0e6", 1, 0, false));
            AccessTools.Field(typeof(StartingConditions), "subclassInfo").SetValue(runData.StartingConditions, new StartingConditions.ClassInfo("fd119fcf-c2cf-469e-8a5a-e9b0f265560d", 1, 0, false));
            AccessTools.Field(typeof(StartingConditions), "ascensionLevel").SetValue(runData.StartingConditions, 1);
            AccessTools.Field(typeof(StartingConditions), "spChallengeId").SetValue(runData.StartingConditions, "");
            AccessTools.Field(typeof(StartingConditions), "covenants").SetValue(runData.StartingConditions, new List<string> { "a498b6c7-a094-48c4-8f99-8e3a2d2cc35b" });
            AccessTools.Field(typeof(StartingConditions), "mutators").SetValue(runData.StartingConditions, new List<string> { "mod.beyonder.clan.monstertrain_FirstLaugh" });
            */

            if (ProviderManager.SaveManager.IsDlcAvailableWhenStartingRun(ShinyShoe.DLC.Hellforged))
            {
                runData.StartingConditions.EnableDlc(ShinyShoe.DLC.Hellforged);
                //AccessTools.Field(typeof(StartingConditions), "enabledDlcs").SetValue(runData.StartingConditions, new List<ShinyShoe.DLC> { ShinyShoe.DLC.Hellforged });
            }
            else 
            {
                //AccessTools.Field(typeof(StartingConditions), "enabledDlcs").SetValue(runData.StartingConditions, new List<ShinyShoe.DLC> { });
            }

            Beyonder.Log("Tutorial Setup Data: " + runData.ToString());

            return runData;
        }

        public static void LoadProgress() 
        {
            if (isLoaded) { return; }

            string path = Path.Combine(Beyonder.BasePath, TutorialPath);

            bool oops = false;

            IL_56:

            if (!File.Exists(path) || oops)
            { 
                isLoaded = true;

                CurrentProgress = new TutorialProgress
                {
                    HasSkippedTutorial = false,
                    HasSeenWelcome = false,
                    HasSeenIntro = false,
                    HasSeenArtifact = false,
                    HasSeenChampUpgrades = false,
                    HasSeenFirstCombat = false,
                    HasBeenInsane = false,
                    HasSeenVictoryUI = false,
                    HasSeenClanBanner = false,
                    HasFinishedTutorial = false,
                    TutorialData = GetTutorialData(),
                };
 
                dirty = true;
                return;
            }

            try
            {
                string data = File.ReadAllText(path);
                CurrentProgress = JsonUtility.FromJson<TutorialProgress>(data);
                //Shouldn't need setup data if the tutorial has already started.
                //CurrentProgress.TutorialData = GetTutorialData();
                isLoaded = true;
            }
            catch
            {
                Beyonder.Log("Failed to load tutorial progress. Data has been reset.");
                oops = true;
                goto IL_56;  //You know you're tired when you willingly put goto statements in your code.
            }
        }

        public static bool SaveTutorialProgress() 
        {
            if (!dirty) { return true; }

            string path = Path.Combine(Beyonder.BasePath, TutorialPath);

            try
            {
                File.WriteAllText(path, CurrentProgress.ToString());
                dirty = false;
                return true;
            }
            catch 
            {
                Beyonder.Log("Failed to save tutorial progress.");
                return false;
            }
        }

        public static void TryDoWelcome() 
        {
            if (CurrentProgress.HasSeenWelcome || CurrentProgress.HasSkippedTutorial) 
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager)) 
            {
                return;
            }

            screenManager.ShowConfirmationDialog("Beyonder_Tutorial_Welcome_Message".Localize(), delegate
            {
                StartNewRunFromRunHistory.setupData = CurrentProgress.TutorialData;
                StartNewRunFromRunHistory.ConfirmDeleteRun();
            }, delegate 
            {
                CurrentProgress.HasSkippedTutorial = true;
                dirty = true;
                SaveTutorialProgress();
                screenManager.ShowNotificationDialog("Beyonder_Tutorial_Skipped_Message".Localize(), null, Dialog.Anchor.Center);
            }, Dialog.Anchor.Center, false);

            CurrentProgress.HasSeenWelcome = true;
            dirty = true;
            SaveTutorialProgress();
        }

        public static void TryDoIntro() 
        {
            if (CurrentProgress.HasSeenIntro || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data 
            { 
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_Intro_Message".Localize(),
                button1Text = "Beyonder_Tutorial_Button_OK_Text".Localize(),
                callbackClose = delegate 
                {
                    CurrentProgress.HasSeenIntro = true;
                    dirty = true;
                    SaveTutorialProgress();
                }
            });
        }

        public static void TryDoArtifact()
        {
            if (CurrentProgress.HasSeenArtifact || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data
            {
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_Artifact_Message".Localize(),
                button1Text = "Beyonder_Tutorial_Button_Loot_Text".Localize(),
                callbackClose = delegate
                {
                    CurrentProgress.HasSeenArtifact = true;
                    dirty = true;
                    SaveTutorialProgress();
                    TryFinishTutorial();
                }
            });
        }

        public static void TryDoChampUpgradeMessages() 
        {
            if (CurrentProgress.HasSeenChampUpgrades || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data
            {
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_ChampUpgrade_Message_0".Localize(),
                button1Text = "Beyonder_Tutorial_Button_Funny_Text".Localize(),
                callbackClose = delegate
                {
                    screenManager.ShowDialog(new Dialog.Data
                    {
                        style = Dialog.Style.Normal,
                        content = "Beyonder_Tutorial_ChampUpgrade_Message_1".Localize(),
                        button1Text = "Beyonder_Tutorial_Button_OK_Text".Localize(),
                        callbackClose = delegate
                        {
                            CurrentProgress.HasSeenChampUpgrades = true;
                            dirty = true;
                            SaveTutorialProgress();
                            TryFinishTutorial();
                        }
                    });
                }
            });
        }

        public static void TryDoFirstCombatStart() 
        {
            if (CurrentProgress.HasSeenFirstCombat || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data
            {
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_FirstCombat_Message_0".Localize(),
                button1Text = "Beyonder_Tutorial_Button_Dashing_Text".Localize(),
                callbackClose = delegate
                {
                    screenManager.ShowDialog(new Dialog.Data
                    {
                        style = Dialog.Style.Normal,
                        content = "Beyonder_Tutorial_FirstCombat_Message_1".Localize(),
                        button1Text = "Beyonder_Tutorial_Button_GotIt_Text".Localize(),
                        callbackClose = delegate
                        {
                            screenManager.ShowDialog(new Dialog.Data
                            {
                                style = Dialog.Style.Normal,
                                content = "Beyonder_Tutorial_FirstCombat_Message_2".Localize(),
                                button1Text = "Beyonder_Tutorial_Button_OK_Text".Localize(),
                                callbackClose = delegate
                                {
                                    screenManager.ShowDialog(new Dialog.Data
                                    {
                                        style = Dialog.Style.Normal,
                                        content = "Beyonder_Tutorial_FirstCombat_Message_3".Localize(),
                                        button1Text = "Beyonder_Tutorial_Button_GotIt_Text".Localize(),
                                        callbackClose = delegate
                                        {
                                            screenManager.ShowDialog(new Dialog.Data
                                            {
                                                style = Dialog.Style.Normal,
                                                content = "Beyonder_Tutorial_FirstCombat_Message_4".Localize(),
                                                button1Text = "Beyonder_Tutorial_Button_WillDo_Text".Localize(),
                                                callbackClose = delegate
                                                {
                                                    CurrentProgress.HasSeenFirstCombat = true;
                                                    dirty = true;
                                                    SaveTutorialProgress();

                                                    if (ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager)) 
                                                    {
                                                        CardManager.AddCardUpgradingInfo addCardUpgradingInfo = new CardManager.AddCardUpgradingInfo
                                                        {
                                                            copyModifiersFromCard = null,
                                                            ignoreTempUpgrades = true,
                                                            tempCardUpgrade = true,
                                                            upgradingCardSource = null,
                                                            upgradeDatas = new List<CardUpgradeData> 
                                                            { 
                                                                new CardUpgradeDataBuilder
                                                                { 
                                                                    UpgradeTitleKey = "Tutorial_SearingMind_PurgeSample",
                                                                    CostReduction = 1,
                                                                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                                                                    { 
                                                                        new CardTraitDataBuilder
                                                                        { 
                                                                            TraitStateName = "CardTraitSelfPurge",
                                                                        }
                                                                    }
                                                                }.Build(),
                                                            }
                                                        };

                                                        cardManager.AddCard(MindScar.Card, CardPile.HandPile, 1, 1, false, false, addCardUpgradingInfo);
                                                        cardManager.AddCard(MindScar.Card, CardPile.HandPile, 1, 1, false, false, addCardUpgradingInfo);
                                                        cardManager.AddCard(MindScar.Card, CardPile.HandPile, 1, 1, false, false, addCardUpgradingInfo);
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
        }

        public static void TryDoInsanity()
        {
            if (CurrentProgress.HasBeenInsane || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data
            {
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_Insanity_Message_0".Localize(),
                button1Text = "Beyonder_Tutorial_Button_Help_Text".Localize(),
                callbackClose = delegate
                {
                    screenManager.ShowDialog(new Dialog.Data
                    {
                        style = Dialog.Style.Normal,
                        content = "Beyonder_Tutorial_Insanity_Message_1".Localize(),
                        button1Text = "Beyonder_Tutorial_Button_Really_Text".Localize(),
                        callbackClose = delegate
                        {
                            screenManager.ShowDialog(new Dialog.Data
                            {
                                style = Dialog.Style.Normal,
                                content = "Beyonder_Tutorial_Insanity_Message_2".Localize(),
                                button1Text = "Beyonder_Tutorial_Button_Noticed_Text".Localize(),
                                callbackClose = delegate
                                {
                                    screenManager.ShowDialog(new Dialog.Data
                                    {
                                        style = Dialog.Style.Normal,
                                        content = "Beyonder_Tutorial_Insanity_Message_3".Localize(),
                                        button1Text = "Beyonder_Tutorial_Button_OK_Text".Localize(),
                                        callbackClose = delegate
                                        {
                                            screenManager.ShowDialog(new Dialog.Data
                                            {
                                                style = Dialog.Style.Normal,
                                                content = "Beyonder_Tutorial_Insanity_Message_4".Localize(),
                                                button1Text = "Beyonder_Tutorial_Button_Yay_Text".Localize(),
                                                button2Text = "Beyonder_Tutorial_Button_Fun_Text".Localize(),
                                                defaultIsButton1 = true,
                                                callbackClose = delegate
                                                {
                                                    CurrentProgress.HasBeenInsane = true;
                                                    dirty = true;
                                                    SaveTutorialProgress();
                                                    TryFinishTutorial();
                                                }
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
        }

        public static void TryDoVictoryMessages()
        {
            if (CurrentProgress.HasSeenVictoryUI || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data
            {
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_Victory_Message_0".Localize(),
                button1Text = "Beyonder_Tutorial_Button_Neat_Text".Localize(),
                callbackClose = delegate
                {
                    screenManager.ShowDialog(new Dialog.Data
                    {
                        style = Dialog.Style.Normal,
                        content = "Beyonder_Tutorial_Victory_Message_1".Localize(),
                        button1Text = "Beyonder_Tutorial_Button_OK_Text".Localize(),
                        callbackClose = delegate
                        {
                            CurrentProgress.HasSeenVictoryUI = true;
                            dirty = true;
                            SaveTutorialProgress();
                        }
                    });
                }
            });
        }

        public static void TryDoBeyonderBannerMessages()
        {
            if (CurrentProgress.HasSeenClanBanner || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data
            {
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_Banner_Message_0".Localize(),
                button1Text = "Beyonder_Tutorial_Button_OK_Text".Localize(),
                callbackClose = delegate
                {
                    screenManager.ShowDialog(new Dialog.Data
                    {
                        style = Dialog.Style.Normal,
                        content = "Beyonder_Tutorial_Banner_Message_1".Localize(),
                        button1Text = "Beyonder_Tutorial_Button_Weird_Text".Localize(),
                        callbackClose = delegate
                        {
                            screenManager.ShowDialog(new Dialog.Data
                            {
                                style = Dialog.Style.Normal,
                                content = "Beyonder_Tutorial_Banner_Message_2".Localize(),
                                button1Text = "Beyonder_Tutorial_Button_OK_Text".Localize(),
                                callbackClose = delegate
                                {
                                    screenManager.ShowDialog(new Dialog.Data
                                    {
                                        style = Dialog.Style.Normal,
                                        content = "Beyonder_Tutorial_Banner_Message_3".Localize(),
                                        button1Text = "Beyonder_Tutorial_Button_GotIt_Text".Localize(),
                                        callbackClose = delegate
                                        {
                                            CurrentProgress.HasSeenClanBanner = true;
                                            dirty = true;
                                            SaveTutorialProgress();
                                            TryFinishTutorial();
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
        }

        public static void TryFinishTutorial()
        {
            if (CurrentProgress.HasFinishedTutorial || CurrentProgress.HasSkippedTutorial)
            {
                return;
            }

            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager))
            {
                return;
            }

            bool flag = CurrentProgress.HasBeenInsane && CurrentProgress.HasSeenClanBanner && CurrentProgress.HasSeenArtifact && CurrentProgress.HasSeenChampUpgrades;

            if (!flag) 
            {
                return;
            }

            screenManager.ShowDialog(new Dialog.Data
            {
                style = Dialog.Style.Normal,
                content = "Beyonder_Tutorial_IsOver_Message".Localize(),
                button1Text = "Beyonder_Tutorial_Button_Lol_Text".Localize(),
                callbackClose = delegate
                {
                    CurrentProgress.HasFinishedTutorial = true;
                    dirty = true;
                    SaveTutorialProgress();

                    if (ProviderManager.TryGetProvider<SoundManager>(out SoundManager soundManager))
                    {
                        soundManager.PlaySfx("Multiplayer_Emote_Lol");
                    }
                }
            });
        }
    }
} 