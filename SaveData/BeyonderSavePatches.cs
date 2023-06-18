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
using System.Linq;
using Spine;

namespace Void.Init
{
    [HarmonyPatch(typeof(RunHistoryUI), "ShowRunSummary")]
    public static class LoadRunHistoryDataPatch1
    {
        public static bool DataChanged = false;
        public static bool DataFailed = false;
        
        public static void Prefix(RunAggregateData runData, ref ScreenManager ___screenManager)
        {
            //Beyonder.Log("Loading Run History for ID: " + runData.GetID());
            if (!Beyonder.IsInit) { return; }
            if (ProviderManager.SaveManager.GetRunType() != RunType.None && runData.GetID() == ProviderManager.SaveManager.GetRunId())
            {
                return;
            }

            //Beyonder.Log("Line 45");
            DataFailed = false;

            if (BeyonderSaveManager.LoadDataForRunID(runData.GetID()))
            {
                Beyonder.Log($"Loading data for runID: {runData.GetID()}");
                DataChanged = true;
            }
            else 
            {
                Beyonder.Log($"No data found for runID: {runData.GetID()}");

                if (runData.GetMainClassID() == Beyonder.BeyonderClanData.GetID() || runData.GetSubClassID() == Beyonder.BeyonderClanData.GetID()) 
                {
                    DataFailed = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(RunSummaryScreen), "Initialize")]
    public static class NagDialogOnLackOfRunData 
    {
        public static void Postfix(ref RunAggregateData ___runData, ref ScreenManager ___screenManager)
        {
            if (LoadRunHistoryDataPatch1.DataFailed)
            {
                string message = string.Format("Beyonder_RunHistory_DataMissing".Localize(), ___runData.GetID());

                ___screenManager.ShowNotificationDialog(message, null, Dialog.Anchor.Center);

                LoadRunHistoryDataPatch1.DataFailed = false;
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuScreen), "SetScreenActive")]
    public static class LoadRunHistoryScreenPatch2 
    {
        public static void Prefix()
        {
            if (!Beyonder.IsInit) { return; }

            if (LoadRunHistoryDataPatch1.DataChanged)
            {
                Beyonder.Log("Refreshing Current Run Data.");
                BeyonderSaveManager.LoadActiveRunData();
                LoadRunHistoryDataPatch1.DataChanged = false;
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuScreen), "Initialize")]
    public static class StartNewRunFromForcedSeed 
    {
        public static void Postfix()
        {
            if (!Beyonder.IsInit) { return; }
            if (!ProviderManager.TryGetProvider<ScreenManager>(out ScreenManager screenManager)) { return; }

            if (BeyonderSaveManager.CurrentRunSetupData.ForceSeed)
            {
                screenManager.ShowConfirmationDialog("Beyonder_Forced_Seed_Confirmation_Dialog".Localize(), delegate 
                {
                    StartNewRunFromRunHistory.replayedRun = true;
                    StartNewRunFromRunHistory.setupData = BeyonderSaveManager.CurrentRunSetupData;
                    BeyonderSaveManager.CurrentRunSetupData.ForceSeed = false;
                    StartNewRunFromRunHistory.StartNewRun();
                }, delegate
                {
                    BeyonderSaveManager.CurrentRunSetupData.ForceSeed = false;
                }, Dialog.Anchor.Center, true);
            }
        }
    }
    
    //For testing only. Debugging dynamic sysnthesis data.
    //Actually, this can be a fallback mechanism if the run history is tampered with.

    [HarmonyPatch(typeof(AllGameData), "FindCardUpgradeData")]
    public static class CheckLoadingDetails 
    {
        public static void Postfix(string id, ref CardUpgradeData __result, ref AllGameData __instance)
        {
            if (__result == null && id != string.Empty)
            {
                Beyonder.Log("Seeking upgrade: " + id);
                Beyonder.Log("Current result: NULL");

                //Vexation
                if (id.StartsWith("VexationEssenceBase_"))
                {
                    Beyonder.Log("Vexation Essence detected.");

                    string[] elements = id.Replace("_merge_", "|").Split('|');

                    if (elements.Length > 1)
                    {
                        int vBoon = ChaosManager.FindValue(ref ChaosManager.VBoonsData, elements[1]);

                        Beyonder.Log($"vBoon: {elements[1]} is {vBoon}.");

                        if (vBoon != -1)
                        {
                            ChaosManager.SetIndex("Vboons", Vexation.EssenceVboonIndex, vBoon);

                            __result = Vexation.GetSynthesis();

                            Beyonder.Log($"New result: Vexation Synthesis {ChaosManager.Vboons[Vexation.EssenceVboonIndex]}.x");
                            return;
                        }
                    }
                }

                //Malevolence
                if (id.StartsWith("MalevolenceEssenceBase_"))
                {
                    Beyonder.Log("Malevolence Essence detected.");

                    string[] elements = id.Replace("_merge_", "|").Split('|');

                    if (elements.Length > 1)
                    {
                        int vBane = ChaosManager.FindValue(ref ChaosManager.VBanesData, elements[1]);

                        Beyonder.Log($"vBane: {elements[1]} is {vBane}.");

                        if (vBane != -1)
                        {
                            ChaosManager.SetIndex("Vbanes", Malevolence.EssenceVbaneIndex, vBane);

                            __result = Malevolence.GetSynthesis();

                            Beyonder.Log($"New result: Malevolence Synthesis x.{ChaosManager.Vbanes[Malevolence.EssenceVbaneIndex]}");
                            return;
                        }
                    }
                }

                //HairyPotty
                if (id.StartsWith("HairyPottyEssenceBase_"))
                {
                    Beyonder.Log("Hairy Potty Essence detected.");

                    string[] elements = id.Replace("_merge_", "|").Split('|');

                    if (elements.Length > 1)
                    {
                        int uBoon = ChaosManager.FindValue(ref ChaosManager.UBoonsData, elements[1]);

                        Beyonder.Log($"uBoon: {elements[1]} is {uBoon}.");

                        if (uBoon != -1)
                        {
                            ChaosManager.SetIndex("Uboons", HairyPotty.EssenceUboonIndex, uBoon);

                            __result = HairyPotty.GetSynthesis();

                            Beyonder.Log($"New result: Hairy Potty Synthesis {ChaosManager.Uboons[HairyPotty.EssenceUboonIndex]}.x");
                            return;
                        }
                    }
                }

                //FurryBeholder
                if (id.StartsWith("FurryBeholderEssenceBase_"))
                {
                    Beyonder.Log("Furry Beholder Essence detected.");

                    string[] elements = id.Replace("_merge_", "|").Split('|');

                    if (elements.Length > 1)
                    {
                        int uBane = ChaosManager.FindValue(ref ChaosManager.UBanesData, elements[1]);

                        Beyonder.Log($"uBane: {elements[1]} is {uBane}.");

                        if (uBane != -1)
                        {
                            ChaosManager.SetIndex("Ubanes", FurryBeholder.EssenceUbaneIndex, uBane);

                            __result = FurryBeholder.GetSynthesis();

                            Beyonder.Log($"New result: Furry Beholder Synthesis x.{ChaosManager.Ubanes[FurryBeholder.EssenceUbaneIndex]}");
                            return;
                        }
                    }
                }

                //Veilritch_Boon_
                if (id.StartsWith("Veilritch_Boon_"))
                {
                    string[] elements = id.Replace("_merge_", "|").Split('|');

                    if (elements.Length > 3)
                    {
                        Beyonder.Log("Apostile of the Void Essence detected.");

                        int vBoon = ChaosManager.FindValue(ref ChaosManager.VBoonsData, elements[0]);
                        int vBane = ChaosManager.FindValue(ref ChaosManager.VBanesData, elements[1]);
                        int uBoon = ChaosManager.FindValue(ref ChaosManager.UBoonsData, elements[2]);
                        int uBane = ChaosManager.FindValue(ref ChaosManager.UBanesData, elements[3]);

                        Beyonder.Log($"vBoon: {elements[0]} is {vBoon}.");
                        Beyonder.Log($"vBane: {elements[1]} is {vBane}.");
                        Beyonder.Log($"uBoon: {elements[2]} is {uBoon}.");
                        Beyonder.Log($"uBane: {elements[3]} is {uBane}.");

                        if (vBoon != -1 && vBane != -1 && uBoon != -1 && uBane != -1)
                        {
                            ChaosManager.SetIndex("Vboons", ApostleoftheVoid.EssenceVboonIndex, vBoon);
                            ChaosManager.SetIndex("Vbanes", ApostleoftheVoid.EssenceVbaneIndex, vBane);
                            ChaosManager.SetIndex("Uboons", ApostleoftheVoid.EssenceUboonIndex, uBoon);
                            ChaosManager.SetIndex("Ubanes", ApostleoftheVoid.EssenceUbaneIndex, uBane);

                            __result = ApostleoftheVoid.GetSynthesis();

                            Beyonder.Log($"New result: Apostle of the Void Synthesis {ChaosManager.Vboons[ApostleoftheVoid.EssenceVboonIndex]}.{ChaosManager.Vbanes[ApostleoftheVoid.EssenceVbaneIndex]}.{ChaosManager.Uboons[ApostleoftheVoid.EssenceUboonIndex]}.{ChaosManager.Ubanes[ApostleoftheVoid.EssenceUbaneIndex]}");
                            return;
                        }
                    }
                    else if (elements.Length > 1)
                    {
                        Beyonder.Log("Formless Horror Essence detected.");

                        int vBoon = ChaosManager.FindValue(ref ChaosManager.VBoonsData, elements[0]);
                        int vBane = ChaosManager.FindValue(ref ChaosManager.VBanesData, elements[1]);

                        Beyonder.Log($"vBoon: {elements[0]} is {vBoon}.");
                        Beyonder.Log($"vBane: {elements[1]} is {vBane}.");

                        if (vBoon != -1 && vBane != -1)
                        {
                            ChaosManager.SetIndex("Vboons", FormlessHorror.EssenceVboonIndex, vBoon);
                            ChaosManager.SetIndex("Vbanes", FormlessHorror.EssenceVbaneIndex, vBane);

                            __result = FormlessHorror.GetSynthesis();

                            Beyonder.Log($"New result: Formless Horror Synthesis {ChaosManager.Vboons[FormlessHorror.EssenceVboonIndex]}.{ChaosManager.Vbanes[FormlessHorror.EssenceVbaneIndex]}");
                            return;
                        }
                    }
                }

                //Undretch_Boon_
                if (id.StartsWith("Veilritch_Boon_"))
                {
                    string[] elements = id.Replace("_merge_", "|").Split('|');

                    if (elements.Length > 1)
                    {
                        Beyonder.Log("Soundless Swarm Essence detected.");

                        int uBoon = ChaosManager.FindValue(ref ChaosManager.UBoonsData, elements[2]);
                        int uBane = ChaosManager.FindValue(ref ChaosManager.UBanesData, elements[3]);

                        Beyonder.Log($"uBoon: {elements[2]} is {uBoon}.");
                        Beyonder.Log($"uBane: {elements[3]} is {uBane}.");

                        if (uBoon != -1 && uBane != -1)
                        {
                            ChaosManager.SetIndex("Uboons", SoundlessSwarm.EssenceUboonIndex, uBoon);
                            ChaosManager.SetIndex("Ubanes", SoundlessSwarm.EssenceUbaneIndex, uBane);

                            __result = SoundlessSwarm.GetSynthesis();

                            Beyonder.Log($"New result: Apostle of the Void Synthesis {ChaosManager.Uboons[SoundlessSwarm.EssenceUboonIndex]}.{ChaosManager.Ubanes[SoundlessSwarm.EssenceUbaneIndex]}");
                            return;
                        }
                    }
                }
            
                Beyonder.Log("Essence fallback attempt failed.");
            }
            else
            {
                //Beyonder.Log("Result: " + __result.GetUpgradeTitleKey());
            }
        }
    }
}