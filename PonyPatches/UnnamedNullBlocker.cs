using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using Trainworks.Constants;
using Trainworks.Builders;
using ShinyShoe.Loading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Void.Init;

namespace Equestrian.HarmonyPatches
{
    //Add checks for null where appropriate.
    [HarmonyPatch(typeof(CardEffectFloorRearrange), "ApplyEffect")]
    public static class DidTheUnnamedRelicEverWorkOrWasItAlwaysBrokenEhWhoCaresLetsFixIt
    {
        /// <summary>
        /// This patch literally exists to fix a bug with a broken Arcadian relic called the "Unnamed Relic".<br></br>
        /// Without this fix, combat instantly breaks when playing a unit while you have that relic.
        /// </summary>
        /// <param name="cardEffectParams"></param>
        /// <returns></returns>
        public static bool Prefix(ref CardEffectParams cardEffectParams)
        {
            //Ponies.Log("line 28.");

            bool isOkay = true;

            if (cardEffectParams == null) { isOkay = false; }
            //Ponies.Log("line 33.");
            if (isOkay && cardEffectParams.targets == null) { isOkay = false; }
            //Ponies.Log("line 35.");
            if (isOkay && cardEffectParams.targets.Count == 0) { isOkay = false; }
            //Ponies.Log("line 37.");
            if (isOkay && cardEffectParams.targets[0] == null) { isOkay = false; }
            //Ponies.Log("line 39.");
            if (isOkay && cardEffectParams.targets[0].GetSpawnPoint(false) == null) { isOkay = false; }
            //Ponies.Log("line 41.");
            if (isOkay && cardEffectParams.targets[0].GetSpawnPoint(false).GetRoomOwner() == null) { isOkay = false; }
            //Ponies.Log("line 43.");

            //if (!isOkay && Beyonder.IsInit)
            //{
                //This error happens a LOT with the Unnamed Relic in use.
                //This fix seems to work, though.
                //Ponies.Log("Null detected. Handling unnamed error.");
            //}

            return isOkay;
        }
    }
}