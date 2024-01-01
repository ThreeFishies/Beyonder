using Trainworks.Managers;
using Trainworks.Builders;
using Trainworks.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using HarmonyLib;
using ShinyShoe;
using UnityEngine;
using CustomEffects;
using Void.Init;

namespace Equestrian.Metagame 
{
    public static class PonyMetagame 
    {
        public static List<spChallengeMeta> challengeMetas = new List<spChallengeMeta>();
        public static string metaFile = Path.Combine(Path.GetDirectoryName(Beyonder.Instance.Info.Location), "SaveData", "BeyonderMetagame.txt");
        public static string preamble = "This file keeps track of Beyonder expert challenges.";
        public static string spChallengeTag = "#spChallenge";
        public static string victoryTag = "victory";
        public static string divineTag = "divine";
        public static bool dirty = false;

        public static bool RegisterChallenge(string spChallengeID, bool victory = false, bool divine = false)
        {
            if (challengeMetas.Count > 0) 
            { 
                foreach (spChallengeMeta challengeMeta in challengeMetas) 
                { 
                    if (challengeMeta.spChallengeID == spChallengeID)
                    {
                        return false;
                    }
                }
            }

            challengeMetas.Add(new spChallengeMeta { spChallengeID = spChallengeID, victory = victory, divine = divine });

            return true;
        }

        public static bool IsPonyChallenge(string spChallengeID) 
        {
            //Ponies.Log($"Checking if {spChallengeID} is pony challenge.");

            foreach (spChallengeMeta meta in challengeMetas)
            {
                if (meta.spChallengeID == spChallengeID)
                {
                    //Ponies.Log("True");
                    return true;
                }
            }
            //Ponies.Log("False");
            return false;
        }

        public static void UpdateChallenge(string spChallengeID, bool victory, bool divine, bool fromFile = false)
        {
            bool foundChallenge = false;

            if (divine && !victory)
            {
                Beyonder.Log($"Error: Attempting to set a divine victory for spChallenge \"{spChallengeID}\" before a normal victory.");

                return;
            }

            Beyonder.Log($"Updating spChallenge {spChallengeID} with win: {victory} divine: {divine}.");

            if (challengeMetas.Count > 0)
            {
                for (int ii = 0; ii < challengeMetas.Count; ii++) 
                {
                    if (challengeMetas[ii].spChallengeID == spChallengeID)
                    {
                        foundChallenge = true;
                        challengeMetas[ii].victory |= victory;
                        challengeMetas[ii].divine |= divine;
                    }
                }
            }

            if (!foundChallenge)
            {
                Beyonder.Log($"Error: Can't update. spChallenge \"{spChallengeID}\" not found.");
            }
            else
            {
                dirty |= !fromFile;
            }
        }

        public static void LoadPonyMetaFile() 
        {
            if (!File.Exists(metaFile)) 
            {
                Beyonder.Log($"File {metaFile} not found. A new one will be created.");
                dirty = true;
                return;
            }

            string[] metaFileData = File.ReadAllLines(metaFile);

            if (metaFileData.Length == 0) 
            {
                Beyonder.Log($"File {metaFile} is empty. New data will be populated.");
                dirty = true;
                return;
            }

            bool isSpChallenge = false;
            string currentSpChallengeID = string.Empty;
            bool currentVictory = false;
            bool currentDivine = false;

            for (int ii = 0; ii < metaFileData.Length; ii++) 
            {
                if (metaFileData[ii].Trim().IsNullOrEmpty() && isSpChallenge)
                {
                    UpdateChallenge(currentSpChallengeID, currentVictory, currentDivine, true);
                    currentSpChallengeID = string.Empty;
                    currentVictory = false;
                    currentDivine = false;
                    isSpChallenge = false;
                }
                else if (metaFileData[ii].Trim().StartsWith("#") && isSpChallenge)
                {
                    UpdateChallenge(currentSpChallengeID, currentVictory, currentDivine, true);
                    currentSpChallengeID = string.Empty;
                    currentVictory = false;
                    currentDivine = false;
                    isSpChallenge = false;
                }

                if (metaFileData[ii].Trim() == spChallengeTag)
                {
                    isSpChallenge = true;
                }
                else if (metaFileData[ii].Trim() == victoryTag && isSpChallenge) 
                {
                    currentVictory = true;
                }
                else if (metaFileData[ii].Trim() == divineTag && isSpChallenge)
                {
                    currentDivine = true;
                }
                else if (!metaFileData[ii].IsNullOrEmpty() && isSpChallenge)
                {
                    currentSpChallengeID = metaFileData[ii].Trim();
                }
            }

            if (isSpChallenge)
            {
                UpdateChallenge(currentSpChallengeID, currentVictory, currentDivine, true);
            }
        }

        public static void SavePonyMetaFile() 
        {
            if (!dirty) { return; }

            List<string> metaData = new List<string> {};
            metaData.Add(preamble);
            metaData.Add(string.Empty);

            foreach (spChallengeMeta meta in challengeMetas) 
            {
                if (meta.victory) 
                {
                    metaData.Add(spChallengeTag);
                    metaData.Add(meta.spChallengeID);
                    metaData.Add(victoryTag);
                }
                if (meta.divine)
                {
                    metaData.Add(divineTag);
                }
                if (meta.victory)
                {
                    metaData.Add(string.Empty);
                }
            }

            if (File.Exists(metaFile))
            {
                try
                {
                    File.Delete(metaFile);
                }
                catch (Exception ffff)
                {
                    Beyonder.LogError("Unable to delete old meta file " + metaFile);
                    Beyonder.LogError("Error: " + ffff.Message);
                }
            }

            try
            {
                File.WriteAllLines(metaFile, metaData);
            }
            catch (Exception fffff)
            {
                Beyonder.LogError("Unable to save meta file " + metaFile);
                Beyonder.LogError("Error: " + fffff.Message);
            }

            dirty = false;
        }

        public static bool HasSpChallengeWin(string spChallengeID) 
        {
            //Ponies.Log($"Checking to see if {spChallengeID} has normal win.");

            foreach (spChallengeMeta meta in challengeMetas)
            {
                if (meta.spChallengeID == spChallengeID)
                {
                    if (meta.victory) 
                    {
                        //Ponies.Log("True");
                        return true;
                    }
                    else
                    {
                        //Ponies.Log("False");
                        return false;
                    }
                }
            }

            return false;
        }

        public static bool HasSpChallengeWinDivine(string spChallengeID)
        {
            //Ponies.Log($"Checking to see if {spChallengeID} has divine win.");

            foreach (spChallengeMeta meta in challengeMetas)
            {
                if (meta.spChallengeID == spChallengeID)
                {
                    if (meta.divine)
                    {
                        //Ponies.Log("True");
                        return true;
                    }
                    else
                    {
                        //Ponies.Log("False");
                        return false;
                    }
                }
            }

            return false;
        }
    }

    [Serializable]
    public class spChallengeMeta
    {
        public string spChallengeID { get; set; }
        public bool victory { get; set; }
        public bool divine { get; set; }
    }
}