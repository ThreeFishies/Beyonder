using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Void.Init;
using Trainworks.Managers;
using Trainworks.Constants;
using Trainworks.Builders;
using Void.Chaos;
using Void.Enhancers;
using ShinyShoe.Logging;

namespace Void.HarmonyPatches
{
    //This patch changes the behaviour of rerolling items in the magic shop. Minor upgrades are now forced to reroll into a different option when Beyonder is present.
    //This will prevent the player from getting two -1 compulsive upgrades in a row, for example, allowing for better consistency and value from magic shops.
    [HarmonyPatch(typeof(SaveManager), "RerollMerchantGoodsAtCurrentDistance")]
    public static class RerollMerchantGoodsAtCurrentDistancePatch
    {
        public static bool IsBeyonder()
        {
            if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.HasMainClass()) 
            {
                if (ProviderManager.SaveManager.GetMainClass().GetID() == Beyonder.BeyonderClanData.GetID()) 
                {
                    //Beyonder.Log("Beyonder detected.");
                    return true;
                }
                if (ProviderManager.SaveManager.GetSubClass().GetID() == Beyonder.BeyonderClanData.GetID())
                {
                    //Beyonder.Log("Beyonder detected.");
                    return true;
                }
            }

            //Beyonder.Log("Not Beyonder.");
            return false;
        }

        /*
        public static bool IsMagicShop(List<MerchantGoodState> excludedItems) 
        {
            for (int i = excludedItems.Count - 1; i >= 0; i--)
            {
                EnhancerRewardData enhancerRewardData;
                if ((enhancerRewardData = (excludedItems[i].RewardData as EnhancerRewardData)) != null && enhancerRewardData.GetRarity() == CollectableRarity.Common)
                {
                    //Beyonder.Log("Common EnhancerID: " + enhancerRewardData.GetRelicData().GetID() + $" ({enhancerRewardData.name})");

                    if (enhancerRewardData.GetRelicData().GetID() == Voidstone.Enhancer.GetID() || enhancerRewardData.GetRelicData().GetID() == "7301ebb3-4d99-46b9-b4bd-aeabf530fd0a")
                    {
                        //Beyonder.Log("Magic Shop Detected.");
                        return true;
                    }
                }
            }

            //Beyonder.Log("Not a Magic Shop reroll.");
            return false;
        }
        */

        public static bool Prefix(ref SaveManager __instance, ref RewardState.Location location, ref List<MerchantGoodState> excludedItems)
        {
            if (!Beyonder.IsInit)
            {
                return true;
            }

            //This version does not remove the common Beyonder magic enhancers from excluded items, forcing a different option to appear on a reroll.
            if (IsBeyonder()) 
            {
                //Beyonder.Log("Magic Shop Reroll");

                //__instance.ActiveSaveData.ClearMerchantGoodStates(location, true);
                //List<string> methds = AccessTools.GetMethodNames(typeof(SaveManager));
                //foreach (string item in methds)
                //{
                //    Beyonder.Log(item);
                //}

                for (int i = excludedItems.Count - 1; i >= 0; i--)
                {
                    EnhancerRewardData enhancerRewardData;
                    if ((enhancerRewardData = (excludedItems[i].RewardData as EnhancerRewardData)) != null && enhancerRewardData.GetRarity() == CollectableRarity.Common)
                    {
                        if (enhancerRewardData.GetRelicData().GetID() == Voidstone.Enhancer.GetID() || enhancerRewardData.GetRelicData().GetID() == Veilstone.Enhancer.GetID())
                        {
                            //exclude Beyonder upgrades from reappering after a reroll.
                        }
                        else
                        {
                            //Normal upgrades can be seen again.
                            excludedItems.RemoveAt(i);
                        }
                    }
                }

                SaveData activeSaveData = AccessTools.Method(typeof(SaveManager), "get_ActiveSaveData").Invoke(__instance, new object[] { }) as SaveData;
                activeSaveData.ClearMerchantGoodStates(location, true);

                Log.Verbose(LogGroups.Gameflow, string.Format("Reroll merchant items at {0}", location));

                //__instance.GenerateMerchantGoodsAtDistance(location.Distance, location.Branch, location.Index, false, excludedItems, true);
                AccessTools.Method(typeof(SaveManager), "GenerateMerchantGoodsAtDistance", new Type[] { typeof (int), typeof(int), typeof(int), typeof(bool), typeof(List<MerchantGoodState>), typeof(bool) }).Invoke(__instance, new object[] { location.Distance, location.Branch, location.Index, false, excludedItems, true });

                return false;
            }

            return true;
        }
    }
}