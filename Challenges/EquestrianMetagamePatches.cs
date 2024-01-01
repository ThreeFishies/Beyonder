using Trainworks.Managers;
using Trainworks.Builders;
using Trainworks.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using HarmonyLib;
using ShinyShoe;
using Equestrian.Metagame;
using UnityEngine;
using CustomEffects;
using Void.Init;

namespace Equestrian.HarmonyPatches 
{ 
    [HarmonyPatch(typeof(MetagameSaveData), "HasCompletedSpChallenge")]
    public static class spChallengeVictoryPatch
    { 
        public static void Postfix(ref bool __result, ref string id) 
        {
            if (!Beyonder.IsInit) { return; }

            if (PonyMetagame.IsPonyChallenge(id))
            {
                __result = PonyMetagame.HasSpChallengeWin(id);
            }
        }
    }

    [HarmonyPatch(typeof(MetagameSaveData), "HasCompletedSpChallengeWithDivineVictory")]
    public static class spChallengeDivineVictoryPatch
    {
        public static void Postfix(ref bool __result, ref string id)
        {
            if (!Beyonder.IsInit) { return; }

            if (PonyMetagame.IsPonyChallenge(id))
            {
                __result = PonyMetagame.HasSpChallengeWinDivine(id);
            }
        }
    }

    [HarmonyPatch(typeof(SaveManager), "TrackRunResults")]
    public static class CatchChallengeWins
    {
        [HarmonyBefore(new string[] { "mod.clan.helper.monstertrain" })]
        public static void Prefix(ref SaveManager __instance)
        {
            if (!Beyonder.IsInit) { return; }

            Beyonder.Log($"Tracking spChallenge ID: {__instance.GetSpChallengeId()}");

            if (PonyMetagame.IsPonyChallenge(__instance.GetSpChallengeId()))
            {
                Beyonder.Log("Challenge is Beyonder challenge.");
                SaveManager.VictoryType victoryType = __instance.GetVictoryType();
                bool victory = false;
                bool divine = false;

                if (victoryType > SaveManager.VictoryType.None)
                {
                    victory = true;

                    if (victoryType >= SaveManager.VictoryType.Hellforged)
                    {
                        divine = true;
                    }
                }

                PonyMetagame.UpdateChallenge(__instance.GetSpChallengeId(), victory, divine);
                Beyonder.Log("Updating Challenge Metadata.");
                PonyMetagame.SavePonyMetaFile();
                Beyonder.Log("Saving Challenge Metadata.");
            }
            else 
            {
                Beyonder.Log("Challenge is not a Beyonder challenge. Ignoring.");
            }
        }
    }
}