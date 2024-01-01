using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Builders;
using Trainworks.Managers;
using Trainworks.Constants;
using System.Linq;
using UnityEngine;
using Trainworks.Utilities;
using Void.Init;
using CustomEffects;
using Void.Clan;
using Void.Unit;
using Void.Triggers;
using Void.Status;
using Steamworks;
using Void.Monsters;
using System.Dynamic;
using I2.Loc;
using System.Security.Cryptography;

namespace Void.Chaos
{
    public static class ChaosLocalizationManager 
    {
        public static Dictionary<string, CardUpgradeData> Failures = new Dictionary<string, CardUpgradeData>();
        public static Dictionary<string,string> Queue = new Dictionary<string,string>();

        //A bulk upload should go faster than multiple individual ones.
        public static void ProcessQueue() 
        {
            if (!Beyonder.IsInit || (Queue.Count <= 0 && Failures.Count <=0))
            {
                return;
            }

            RedoFailures();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Key;Type;Desc;Plural;Group;Descriptions;English [en-US];French [fr-FR];German [de-DE];Russian;Portuguese (Brazil);Chinese [zh-CN]\n");

            foreach (KeyValuePair<string,string> pair in Queue) 
            {
                stringBuilder.Append(pair.Key + ";");
                stringBuilder.Append("Text" + ";");
                stringBuilder.Append(string.Empty + ";");
                stringBuilder.Append(string.Empty + ";");
                stringBuilder.Append(string.Empty + ";");
                stringBuilder.Append(string.Empty + ";");
                stringBuilder.Append(pair.Value + ";");
                stringBuilder.Append(pair.Value + ";");
                stringBuilder.Append(pair.Value + ";");
                stringBuilder.Append(pair.Value + ";");
                stringBuilder.Append(pair.Value + ";");
                stringBuilder.Append(pair.Value + "\n");

                //Beyonder.Log("Registering localization for key: " + pair.Key + "   Value: " + pair.Value);
            }

            foreach (string category in LocalizationManager.Sources[0].GetCategories(OnlyMainCategory: true))
            {
                LocalizationManager.Sources[0].Import_CSV(category, stringBuilder.ToString(), eSpreadsheetUpdateMode.AddNewTerms, ';');
            }

            Queue.Clear();
        }

        public static void RedoFailures() 
        {
            if (!Beyonder.IsInit || Failures.Count <= 0 || !ChaosManager.IsReady())
            {
                return;
            }

            foreach (KeyValuePair<string,CardUpgradeData> data in Failures) 
            {
                ChaosManager.GenerateDescription(data.Key, data.Value, true);
            }

            Failures.Clear();
        }
    }
}