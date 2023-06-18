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
    [HarmonyPatch(typeof(CardEffectBump), "FindBumpSpawnPoint")]
    public static class MakeFlashFeatherNotFlashFryTheGame
    {
        /// <summary>
        /// This patch exists to fix a bug with the Arcadian unit 'Flashfeather'.<br></br>
        /// If that unit is killed by a bomb or extinguish effect, combat will softlock without this fix. <br></br>
        /// You'll see the 'no room' bump error pop up briefly instead of breaking the combat coroutine.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="roomManager"></param>
        /// <param name="bumpError"></param>
        /// <returns></returns>
        public static bool Prefix(ref CharacterState target, ref RoomManager roomManager, ref CardEffectBump.BumpError bumpError)
        {
            bool isOkay = true;

            if (target == null) { isOkay = false; }
            if (roomManager == null) { isOkay = false; }
            if (isOkay && target.GetSpawnPoint(false) == null) { isOkay = false;}
            if (isOkay && target.GetSpawnPoint(false).GetRoomOwner() == null) { isOkay = false; }

            if (!isOkay) 
            { 
                bumpError = CardEffectBump.BumpError.NoRoom;
                Beyonder.Log("Null detected. Handling bump error.");
            }

            return isOkay;
        }
    }
}