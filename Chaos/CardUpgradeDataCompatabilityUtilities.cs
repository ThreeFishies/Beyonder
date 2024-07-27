using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using Trainworks.ManagersV2;
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
using Void.Builders;
using CustomEffects;
using RunHistory;
using Malee;
using Void.Spells;
using System;
using System.Xml.Linq;
using Trainworks.Utilities;
using Void.Monsters;

namespace Void.Chaos.BackCompatability
{ 
    public static class CardUpgradeDataBackwardsCompatability
    {
        public static Dictionary<string,string> GUIDtoUpgradeTitleKey = new Dictionary<string,string>();
        public static List<string> IgnorePool = new List<string> { };

        public static void Unregister(string UpgradeTitleKey) 
        {
            if (ProviderManager.SaveManager == null) 
            { 
                return; 
            }

            string guid = GUIDGenerator.GenerateDeterministicGUID(UpgradeTitleKey);
            CardUpgradeData dataToRemove = ProviderManager.SaveManager.GetAllGameData().FindCardUpgradeData(guid);

            if (dataToRemove != null) 
            {
                CustomUpgradeManager.CustomUpgradeData.Remove(guid);
                ProviderManager.SaveManager.GetAllGameData().GetAllCardUpgradeData().Remove(dataToRemove);
            }
        }

        private static bool didSoundlessSwarm = false;
        private static bool didFormlessHorror = false;
        private static bool didHairyPotty = false;
        private static bool didFurryBeholder = false;
        private static bool didVexation = false;
        private static bool didMalevolence = false;
        private static bool didApostileoftheVoid = false;
        private static bool builtIgnorePool = false;

        public static bool ShouldIgnore(string guid) 
        {
            if (!builtIgnorePool)
            {
                BuildIgnorePool();
            }

            if (IgnorePool.Contains(guid))
            {
                return true;
            }

            return false;
        }

        public static string GetUpgradeTitleKey(string guid) 
        {
            if (!GUIDtoUpgradeTitleKey.ContainsKey(guid) && !didSoundlessSwarm)
            {
                AddGUIDStoDictionary(ChaosManager.UBoonsData, ChaosManager.UBanesData);
                didSoundlessSwarm = true;
            }

            if (!GUIDtoUpgradeTitleKey.ContainsKey(guid) && !didFormlessHorror)
            {
                AddGUIDStoDictionary(ChaosManager.VBoonsData, ChaosManager.VBanesData);
                didFormlessHorror = true;
            }

            if (!GUIDtoUpgradeTitleKey.ContainsKey(guid) && !didHairyPotty)
            {
                AddGUIDStoDictionary(HairyPotty.Synthesis, ChaosManager.UBoonsData);
                didHairyPotty = true;
            }

            if (!GUIDtoUpgradeTitleKey.ContainsKey(guid) && !didFurryBeholder)
            {
                AddGUIDStoDictionary(FurryBeholder.Synthesis, ChaosManager.UBanesData);
                didFurryBeholder = true;
            }

            if (!GUIDtoUpgradeTitleKey.ContainsKey(guid) && !didVexation)
            {
                AddGUIDStoDictionary(Vexation.Synthesis, ChaosManager.VBoonsData);
                didVexation = true;
            }

            if (!GUIDtoUpgradeTitleKey.ContainsKey(guid) && !didMalevolence)
            {
                AddGUIDStoDictionary(Malevolence.Synthesis, ChaosManager.VBanesData);
                didMalevolence = true;
            }

            if (!GUIDtoUpgradeTitleKey.ContainsKey(guid) && !didApostileoftheVoid)
            {
                AddGUIDStoDictionary(ChaosManager.VBoonsData, ChaosManager.VBanesData, ChaosManager.UBoonsData, ChaosManager.UBanesData);
                didApostileoftheVoid = true;
            }

            if (GUIDtoUpgradeTitleKey.ContainsKey(guid)) 
            { 
                return GUIDtoUpgradeTitleKey[guid];
            }

            return guid;
        }

        /// <summary>
        /// This is meant for Soundless Swarm and Formless Horror. Order matters.
        /// </summary>
        /// <param name="list1">Boons List</param>
        /// <param name="list2">Banes List</param>
        private static void AddGUIDStoDictionary(List<CardUpgradeData> list1, List<CardUpgradeData> list2) 
        {
            if (list1.IsNullOrEmpty() || list2.IsNullOrEmpty()) 
            {
                Beyonder.Log("Improper use of method: AddGUIDStoDictionary(list1, list2). Paramaters cannot be empty.");
                return;
            }

            foreach (CardUpgradeData data in list1) 
            {
                foreach (CardUpgradeData data2 in list2) 
                { 
                    string UpgradeTitleKey = data.GetUpgradeTitleKey() + "_merge_" + data2.GetUpgradeTitleKey();
                    if (GUIDtoUpgradeTitleKey.ContainsValue(UpgradeTitleKey))
                    {
                        Beyonder.Log($"Attempted to duplicate GUID for {UpgradeTitleKey}");
                    }
                    else
                    {
                        GUIDtoUpgradeTitleKey.Add(GUIDGenerator.GenerateDeterministicGUID(UpgradeTitleKey), UpgradeTitleKey);
                    }
                }
            }
        }

        /// <summary>
        /// This is meant for Furry Beholder, Hairy Potty, Vexation and Malevolence. Order matters.
        /// </summary>
        /// <param name="list">Boons/Banes List</param>
        /// <param name="base">Unit's base essence data</param>
        /// <param name="BaseIsFirst">Base essence should always be first</param>
        private static void AddGUIDStoDictionary(CardUpgradeData @base, List<CardUpgradeData> list, bool BaseIsFirst = true)
        {
            if (@base == null || list.IsNullOrEmpty())
            {
                Beyonder.Log("Improper use of method: AddGUIDStoDictionary(base, list, order). Paramaters cannot be null or empty.");
                return;
            }

            foreach (CardUpgradeData data in list)
            {
                string UpgradeTitleKey = "";

                if (BaseIsFirst)
                {
                    UpgradeTitleKey = @base.GetUpgradeTitleKey() + "_merge_" + data.GetUpgradeTitleKey();
                }
                else 
                {
                    UpgradeTitleKey = data.GetUpgradeTitleKey() + "_merge_" + @base.GetUpgradeTitleKey();
                }

                if (GUIDtoUpgradeTitleKey.ContainsValue(UpgradeTitleKey))
                {
                   Beyonder.Log($"Attempted to duplicate GUID for {UpgradeTitleKey}");
                }
                else
                {
                    GUIDtoUpgradeTitleKey.Add(GUIDGenerator.GenerateDeterministicGUID(UpgradeTitleKey), UpgradeTitleKey);
                }
            }
        }

        /// <summary>
        /// Meant for Apostle of the Void
        /// </summary>
        /// <param name="list1">Vboons</param>
        /// <param name="list2">Vbanes</param>
        /// <param name="list3">Uboons</param>
        /// <param name="list4">Ubanes</param>
        private static void AddGUIDStoDictionary(List<CardUpgradeData> list1, List<CardUpgradeData> list2, List<CardUpgradeData> list3, List<CardUpgradeData> list4)
        {
            if (list1.IsNullOrEmpty() || list2.IsNullOrEmpty() || list3.IsNullOrEmpty() || list4.IsNullOrEmpty())
            {
                Beyonder.Log("Improper use of method: AddGUIDStoDictionary(list1, list2, list3, list4). Paramaters cannot be empty.");
                return;
            }

            foreach (CardUpgradeData data in list1)
            {
                foreach (CardUpgradeData data2 in list2)
                {
                    foreach (CardUpgradeData data3 in list3)
                    {
                        foreach (CardUpgradeData data4 in list4)
                        {
                            //Omitting the sanity check due to the sheer number of entries (10,000) that need to be generated.
                            string UpgradeTitleKey = data.GetUpgradeTitleKey() + "_merge_" + data2.GetUpgradeTitleKey() + "_merge_" + data3.GetUpgradeTitleKey() + "_merge_" + data4.GetUpgradeTitleKey();
                            //if (GUIDtoUpgradeTitleKey.ContainsValue(UpgradeTitleKey))
                            //{
                            //    Beyonder.Log($"Attempted to duplicate GUID for {UpgradeTitleKey}");
                            //}
                            //else
                            //{
                            GUIDtoUpgradeTitleKey.Add(GUIDGenerator.GenerateDeterministicGUID(UpgradeTitleKey), UpgradeTitleKey);
                            //}
                        }
                    }
                }
            }
        }

        private static void BuildIgnorePool() 
        { 
            IgnorePool.Clear();
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Conductor_0_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Conductor_1_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Conductor_2_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Horror_0_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Horror_1_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Horror_2_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Formless_0_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Formless_1_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_LocoMotive_Formless_2_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Innumerable_0_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Innumerable_1_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Innumerable_2_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Contagious_0_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Contagious_1_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Contagious_2_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Soundless_0_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Soundless_1_TitleKey"));
            IgnorePool.Add(GUIDGenerator.GenerateDeterministicGUID("Beyonder_Champ_Epidemial_Soundless_2_TitleKey"));
        }
    }
}