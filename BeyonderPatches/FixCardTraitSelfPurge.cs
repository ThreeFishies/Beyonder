
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
using Void.Status;

namespace Void.Patches
{
    [HarmonyPatch(typeof(CardManager), "PurgeCard")]
    public static class FixCardTraitSelfPurge 
    {
        public static void Prefix(ref CardManager __instance, ref CardState cardState) 
        {
            //When a card is played, it's temporarily placed in the discard pile, before the card itself takes effect.
            //Should the card effect cause the deck to reshuffle, this will move it to the draw pile.
            //If this card has the "Purge" trait, that will clear it from the master deck, and also remove it from the discard pile (or the consume pile).
            //So that means that if the card was moved to the draw pile, the purge effect will miss it.
            //Hence the need for this fix.
            __instance.GetDrawPile().Remove(cardState);
        }
    }
}