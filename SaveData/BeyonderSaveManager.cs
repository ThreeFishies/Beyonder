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

namespace Void.Init
{
    [Serializable]
    public struct RunSetupData
    {
        [SerializeField]
        public string RunID;
        [SerializeField] 
        public bool ForceSeed; //For potential use later. Mostly testing.
        [SerializeField]
        public string BeyonderVersion; //Data can potentially change between versions.
 
        [SerializeField]
        public int LocoMotiveConductorPath;
        [SerializeField]
        public int LocoMotiveHorrorPath;
        [SerializeField]
        public int LocoMotiveFormlessPath;

        [SerializeField]
        public int EpidemialInnumerablePath;
        [SerializeField]
        public int EpidemialContagiousPath;
        [SerializeField]
        public int EpidemialSoundlessPath;

        [SerializeField]
        public List<int> VboonIndexes;
        [SerializeField]
        public List<int> VbaneIndexes;
        [SerializeField]
        public List<int> UboonIndexes;
        [SerializeField]
        public List<int> UbaneIndexes;

        public LocoMotivePathData LocoMotivePathData 
        { 
            get 
            {
                return new LocoMotivePathData
                {
                    ConductorTreePathID = LocoMotiveConductorPath,
                    FormlessTreePathID = LocoMotiveFormlessPath,
                    HorrorTreePathID = LocoMotiveHorrorPath
                };
            } 
            set
            {
                //LocoMotivePathData = value;
                LocoMotiveConductorPath = value.ConductorTreePathID;
                LocoMotiveFormlessPath = value.FormlessTreePathID;
                LocoMotiveHorrorPath = value.HorrorTreePathID;
            } 
        }
        public EpidemialPathData EpidemialPathData 
        { 
            get 
            {
                return new EpidemialPathData
                {
                    ContagiousTreePathID = EpidemialContagiousPath,
                    InnumerableTreePathID = EpidemialInnumerablePath,
                    SoundlessTreePathID = EpidemialSoundlessPath
                };
            } 
            set
            {
                //EpidemialPathData = value;
                EpidemialSoundlessPath = value.SoundlessTreePathID;
                EpidemialContagiousPath = value.ContagiousTreePathID;
                EpidemialInnumerablePath = value.InnumerableTreePathID;
            } 
        }
        public BoonsBanesData BoonsBanesData 
        { 
            get 
            {
                return new BoonsBanesData
                {
                    VBanes = VbaneIndexes,
                    VBoons = VboonIndexes,
                    UBanes = UbaneIndexes,
                    UBoons = UboonIndexes
                };
            }
            set 
            {
                //BoonsBanesData = value;
                VboonIndexes = value.VBoons;
                VbaneIndexes = value.VBanes;
                UboonIndexes = value.UBoons;
                UbaneIndexes = value.UBanes;
            }
        }

        [SerializeField]
        public StartingConditions StartingConditions;
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public static class BeyonderSaveManager
    {
        public static Dictionary<string,RunSetupData> RunHistoryData = new Dictionary<string,RunSetupData>();
        public const string CurrentRunPath = "SaveData/CurrentRun/ActiveRunData.json";
        public const string RunHistoryPath = "SaveData/RunHistory.json";

        public static RunSetupData CurrentRunSetupData = new RunSetupData { };

        public static RunSetupData GetRunSetupData() 
        {
            string runID = "";

            if (ProviderManager.SaveManager.GetRunType() != RunType.None) 
            {
                runID = ProviderManager.SaveManager.GetRunId();
            }

            //ChaosManager.LogChaos("Data Gathered for saving.");

            return new RunSetupData
            {
                RunID = runID,
                StartingConditions = ProviderManager.SaveManager.GetStartingConditions(),
                ForceSeed = false,
                BeyonderVersion = Beyonder.VERSION,
                LocoMotivePathData = LocoMotive.GetPathTreeData(),
                EpidemialPathData = Epidemial.GetPathTreeData(),
                BoonsBanesData = ChaosManager.GetData()
            };
        }

        public static void SaveData()
        {
            if (ProviderManager.SaveManager == null || !ProviderManager.SaveManager.HasMainClass()) 
            {
                Beyonder.Log("Unexpected use of BeyonderSaveManager.SaveData(). Can't save data without an active run.");
            }

            RunSetupData ActiveRun = GetRunSetupData();

            CurrentRunSetupData = ActiveRun;

            bool updateHistory = false;

            if (!RunHistoryData.ContainsKey(ActiveRun.RunID))
            {
                RunHistoryData.Add(ActiveRun.RunID, ActiveRun);
                updateHistory = true;
            }

            string path = Path.Combine(Beyonder.BasePath, CurrentRunPath);

            try
            {
                File.WriteAllText(path, ActiveRun.ToString());
                //File.WriteAllText(path, MiniJSON.Json.Serialize(ActiveRun));

                //Beyonder.Log(ActiveRun.ToString());
            }
            catch
            {
                Beyonder.LogError("Saving Active Run Data Failed.");
                return;
            }

            if (updateHistory)
            {
                path = Path.Combine(Beyonder.BasePath, RunHistoryPath);

                List<string> items = new List<string>();

                foreach (KeyValuePair<string, RunSetupData> pair in RunHistoryData) 
                {
                    items.Add(pair.Value.ToString());
                }

                try
                {
                    File.WriteAllLines(path, items);
                    //File.WriteAllText(path, MiniJSON.Json.Serialize(RunHistoryData));
                }
                catch
                {
                    Beyonder.LogError("Saving Run History Data Failed.");
                    return;
                }
            }
        }

        public static bool LoadDataFromFile() 
        {
            bool IsOkay = true;
            RunSetupData activeRun = new RunSetupData { };

            string path = Path.Combine(Beyonder.BasePath, CurrentRunPath);

            try
            {
                string data = File.ReadAllText(path);
                activeRun = JsonUtility.FromJson<RunSetupData>(data);
                //activeRun = (RunSetupData)MiniJSON.Json.Deserialize(data);
                CurrentRunSetupData = activeRun;
            }
            catch 
            {
                Beyonder.Log("Failed to load Active Save Data. Default data will be used instead.");
                IsOkay = false;
            }

            if (!IsOkay)
            {
                //return false;
            }
            else
            {
                LocoMotive.SetupTrees(activeRun.LocoMotivePathData.ConductorTreePathID, activeRun.LocoMotivePathData.HorrorTreePathID, activeRun.LocoMotivePathData.FormlessTreePathID);
                Epidemial.SetupTrees(activeRun.EpidemialPathData.InnumerableTreePathID, activeRun.EpidemialPathData.ContagiousTreePathID, activeRun.EpidemialPathData.SoundlessTreePathID);
                ChaosManager.LoadFromData(activeRun.BoonsBanesData);
            }

            path = Path.Combine(Beyonder.BasePath, RunHistoryPath);

            try
            {
                string[] data = File.ReadAllLines(path);
                if (data.Length > 0)
                { 
                    foreach (string line in data) 
                    { 
                        RunSetupData lineAsSetupData = JsonUtility.FromJson<RunSetupData>(line);
                        RunHistoryData.Add(lineAsSetupData.RunID, lineAsSetupData);
                    }
                }
                //RunHistoryData = JsonUtility.FromJson<Dictionary<string,RunSetupData>>(data);
                //RunHistoryData = (Dictionary<string, RunSetupData>)MiniJSON.Json.Deserialize(data);
            }
            catch
            {
                Beyonder.Log("Failed to load Run History Data.");
                IsOkay = false;
            }

            if (!IsOkay) 
            {
                RunHistoryData.Clear();
            }

            return IsOkay;
        }

        public static bool ImportFromFile(string path, out RunSetupData setupData)
        {
            setupData = new RunSetupData { };
            bool IsOkay = true;

            if (File.Exists(path))
            {
                try
                {
                    string data = File.ReadAllText(path);
                    setupData = JsonUtility.FromJson<RunSetupData>(data);
                }
                catch
                {
                    IsOkay = false;
                }
            }
            else 
            {
                return false;
            }

            IsOkay &= setupData.VbaneIndexes.Count == ChaosManager.VBanesData.Count;
            foreach (int index in setupData.VbaneIndexes) 
            {
                IsOkay &= (index >= 0 && index < ChaosManager.VBoonsData.Count);
            }
            IsOkay &= setupData.VboonIndexes.Count == ChaosManager.VBoonsData.Count;
            foreach (int index in setupData.VboonIndexes)
            {
                IsOkay &= (index >= 0 && index < ChaosManager.VBoonsData.Count);
            }
            IsOkay &= setupData.UbaneIndexes.Count == ChaosManager.UBanesData.Count;
            foreach (int index in setupData.UbaneIndexes)
            {
                IsOkay &= (index >= 0 && index < ChaosManager.UBanesData.Count);
            }
            IsOkay &= setupData.UboonIndexes.Count == ChaosManager.UBoonsData.Count;
            foreach (int index in setupData.UboonIndexes)
            {
                IsOkay &= (index >= 0 && index < ChaosManager.UBoonsData.Count);
            }
            IsOkay &= setupData.EpidemialContagiousPath >= 0;
            IsOkay &= setupData.EpidemialContagiousPath < 3;
            IsOkay &= setupData.EpidemialInnumerablePath >= 0;
            IsOkay &= setupData.EpidemialInnumerablePath < 3;
            IsOkay &= setupData.EpidemialSoundlessPath >= 0;
            IsOkay &= setupData.EpidemialSoundlessPath < 3;
            IsOkay &= setupData.LocoMotiveConductorPath >= 0;
            IsOkay &= setupData.LocoMotiveConductorPath < 3;
            IsOkay &= setupData.LocoMotiveFormlessPath >= 0;
            IsOkay &= setupData.LocoMotiveFormlessPath < 3;
            IsOkay &= setupData.LocoMotiveHorrorPath >= 0;
            IsOkay &= setupData.LocoMotiveHorrorPath < 3;
            //IsOkay &= setupData.StartingConditions.IsFtueRun != true;
            IsOkay &= setupData.StartingConditions.AscensionLevel == setupData.StartingConditions.Covenants.Count;
            IsOkay &= setupData.StartingConditions.AscensionLevel <= ProviderManager.SaveManager.GetMetagameSave().GetMaxAscensionLevel();
            IsOkay &= setupData.StartingConditions.AscensionLevel >= 0;
            //IsOkay &= setupData.StartingConditions.IsBattleMode != true;
            IsOkay &= setupData.StartingConditions.Seed > 0;
            IsOkay &= CustomClassManager.GetClassDataByID(setupData.StartingConditions.Class) != null;
            if (IsOkay)
            {
                IsOkay &= ProviderManager.SaveManager.GetMetagameSave().GetClassUnlocked(CustomClassManager.GetClassDataByID(setupData.StartingConditions.Class), ProviderManager.SaveManager);
                IsOkay &= ProviderManager.SaveManager.GetMetagameSave().GetLevel(setupData.StartingConditions.Class) >= setupData.StartingConditions.ClassLevel;
            }
            IsOkay &= CustomClassManager.GetClassDataByID(setupData.StartingConditions.Subclass) != null;
            if (IsOkay)
            {
                IsOkay &= ProviderManager.SaveManager.GetMetagameSave().GetClassUnlocked(CustomClassManager.GetClassDataByID(setupData.StartingConditions.Subclass), ProviderManager.SaveManager);
                IsOkay &= ProviderManager.SaveManager.GetMetagameSave().GetLevel(setupData.StartingConditions.Subclass) >= setupData.StartingConditions.SubclassLevel;
            }
            IsOkay &= (setupData.StartingConditions.MainChampionIndex == 0 || setupData.StartingConditions.MainChampionIndex == 1);
            IsOkay &= (setupData.StartingConditions.SubChampionIndex == 0 || setupData.StartingConditions.SubChampionIndex == 1);
            if (setupData.StartingConditions.SpChallengeId != string.Empty) 
            {
                IsOkay &= ProviderManager.SaveManager.GetAllGameData().FindSpChallengeData(setupData.StartingConditions.SpChallengeId) != null;
            }
            if (setupData.StartingConditions.Covenants.Count > 0) 
            {
                foreach (string covenant in setupData.StartingConditions.Covenants)
                { 
                    IsOkay &= ProviderManager.SaveManager.GetAllGameData().FindCovenantData(covenant) != null;
                }
            }
            if (setupData.StartingConditions.Mutators.Count > 0) 
            {
                foreach (string mutator in setupData.StartingConditions.Mutators) 
                { 
                    IsOkay &= ProviderManager.SaveManager.GetAllGameData().FindMutatorData(mutator) != null;
                }            
            }
            setupData.ForceSeed = true;
            setupData.StartingConditions.SetIsBattleMode(false);
            setupData.StartingConditions.SetFtue(false);

            return IsOkay;
        }

        public static bool LoadDataForRunID(string RunID)
        {
            if (RunID.IsNullOrEmpty() || !RunHistoryData.ContainsKey(RunID)) 
            {
                Beyonder.Log($"Run ID is not in history file: {RunID}");
                return false;
            }

            Beyonder.Log("Loading RunSetupData: " + RunHistoryData[RunID].ToString());

            //Beyonder.Log("Line 271 (BeyonderSaveManager)");
            LocoMotive.BuildTreeFromData(RunHistoryData[RunID].LocoMotivePathData);
            //Beyonder.Log("Line 273 (BeyonderSaveManager)");
            Epidemial.BuildTreeFromData(RunHistoryData[RunID].EpidemialPathData);
            //Beyonder.Log("Line 275 (BeyonderSaveManager)");
            ChaosManager.UpdateStartingUpgrades(RunHistoryData[RunID].BoonsBanesData);
            //Beyonder.Log("Line 277 (BeyonderSaveManager)");

            return true;
        }

        public static bool LoadActiveRunData() 
        { 
            if (ProviderManager.SaveManager.GetRunType() == RunType.None)
            {
                return false;
            }
            string RunID = ProviderManager.SaveManager.GetRunId();
            return LoadDataForRunID(RunID);
        }
    }
}