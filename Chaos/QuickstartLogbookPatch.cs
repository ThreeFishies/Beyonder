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
using Void.Chaos;

//After quick-restarting, the logbook does not properly refresh to reflect the new, shuffled attributes of Beyonder units. This patch should force the unit cards to refresh.
namespace Void.Patches
{
    [HarmonyPatch(typeof(CompendiumSectionCards), "Open")]
    public static class QuickRestartLogbookPatch
    {
        public static void Prefix(ref bool ___needToRefreshPage, ref List<CardState> ___allCards)
        {
            if (ChaosManager.ShouldRefreshLogbook) 
            { 
                ChaosManager.ShouldRefreshLogbook = false;
                ___needToRefreshPage = true;

                if (___allCards != null) 
                {
                    List<int> Replacements = new List<int>();

                    for (int ii = ___allCards.Count - 1; ii >= 0; ii--) 
                    { 
                        if (___allCards[ii].GetLinkedClassID() == Beyonder.BeyonderClanData.GetID() && ___allCards[ii].IsSpawnerCard() && ___allCards[ii].GetRarityType() == CollectableRarity.Uncommon) 
                        {
                            Replacements.Add(ii);
                            //Beyonder.Log("Refreshing card: " + ___allCards[ii].GetTitle());
                        }
                    }

                    if (Replacements.Count > 0) 
                    {
                        foreach (int jj in Replacements) 
                        {
                            CardData source = CustomCardManager.GetCardDataByID(___allCards[jj].GetCardDataID());
                            ___allCards[jj] = new CardState(source, null, null, true);
                        }
                    }
                }
            }
        }
    }
}