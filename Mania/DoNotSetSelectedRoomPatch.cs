using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using System.Collections;
using System;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Trainworks.Enums.MTTriggers;
using Void.Init;
using Void.Triggers;
using Void.Artifacts;
using Void.Status;
using Void.Monsters;
using ShinyShoe;

namespace Void.Mania
{
    //This seems bad. Game seems to use selected room to determine where triggered effects are applied.
    //So stopping the camera adjustment also breaks everything...
    //Maybe just cache the room for Offering spell purposes and swap back to it when needed.

    /*
    [HarmonyPatch(typeof(RoomUI), "SetSelectedRoom")]
    public static class NoDontSetSelectedRoom 
    {
        public static bool Prefix(ref int ___selectedRoom, int setSelectedRoom) 
        {
            if (!ManiaManager.SetSelectedRoomFlag) 
            {
                ___selectedRoom = setSelectedRoom;
            }

            return ManiaManager.SetSelectedRoomFlag;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(CardManager), "PlayCard")]
    public static class ResetSelectedRoom
    {
        public static Coroutine _coroutine = null;
        public static void Postfix()
        {
            //if (_coroutine != null)
            //{
            //    GlobalMonoBehavior.Inst.StopCoroutine(_coroutine);
            //    _coroutine = null;
            //}

            if (!ManiaManager.SetSelectedRoomFlag && ManiaManager.SelectedRoomIndex != -1)
            {
                ManiaManager.SetSelectedRoomFlag = true;
                //if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager) && (roomManager.GetSelectedRoom() != ManiaManager.SelectedRoomIndex))
                //{
                //    _coroutine = GlobalMonoBehavior.Inst.StartCoroutine(roomManager.GetRoomUI().SetSelectedRoom(ManiaManager.SelectedRoomIndex));
                //}
            }
        }
    }
    */
    /*
    [HarmonyPatch(typeof(CardTraitState), "OnCardDiscarded")]
    public static class SelectTreasureRoom 
    {
        public static void Prefix(ref CardTraitState __instance)
        {
            if (!ManiaManager.SetSelectedRoomFlag && __instance is CardTraitTreasure) 
            {
                if (ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
                {
                    //roomManager.GetRoomUI().SetSelectedRoom(ManiaManager.SelectedRoomIndex);
                    AccessTools.Field(typeof(RoomUI), "selectedRoom").SetValue(roomManager.GetRoomUI(), ManiaManager.SelectedRoomIndex);
                }
            }
        }
    }
    */
}