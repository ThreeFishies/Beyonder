using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Void.Init;
using Trainworks.Managers;
using Trainworks.Constants;
using Trainworks.Builders;
using Void.Chaos;

namespace Void.HarmonyPatches
{
    //Because localization can't be completed during the loading process, wait until the main menu appears to finish
    [HarmonyPatch(typeof(MainMenuScreen), "TryTriggerButton")]
    public static class InitializeChaosLocalizationPatch
    {
        public static void Prefix() 
        {
            if (Beyonder.IsInit) 
            {
                ChaosLocalizationManager.ProcessQueue();
            }
        }
    }
}