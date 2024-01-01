/*
using HarmonyLib;
using Trainworks.Managers;
using Void.Init;
using Void.Monsters;

[HarmonyPatch(typeof(SaveManager), "IsStoryEventValid")]
public class SeeStoryRun
{
    private static void Prefix(ref SaveManager __instance, ref StoryEventData storyEventData, ref int runsStartedModifier)
    {
        string beyonderClassID = Beyonder.BeyonderClanData.GetID();

        if (__instance.GetMainClass().GetID() == beyonderClassID)
        {
            if (__instance.GetClassLevel(beyonderClassID) > 9)
            {
                //Ponies.Log("Runs for Equestrian: " + __instance.GetTrackedValue(MetagameSaveData.TrackedValue.StartedRuns));

                runsStartedModifier += 10;

                //Ponies.Log("Tweaking started runs by 10 for Equestrian clan.");
                //Ponies.Log("Story event name: " + storyEventData.name);
            }
        }
    }

    public static void Postfix(ref bool __result, ref StoryEventData storyEventData) 
    {
        //This creates a list of the cavern events you'll encounter on your journey. Please note that the second event in the list is never used.
        if (__result)
        {
            Beyonder.Log($"Story: {storyEventData.name}");
        }
    }    
}

[HarmonyPatch(typeof(SaveManager), "TrackCardWins")]
[HarmonyBefore("mod.clan.helper.monstertrain")]
public static class AllowCaveofaThousandEyesCardMastery
{
    public static bool shouldMasterCaveofaThousandEyes = false;

    private static void Prefix(ref SaveManager __instance, ref bool divineWin, ref MetagameSaveData ___metagameSaveData, ref AllGameData ___allGameData)
    {
        if (!Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("mod.clan.helper.monstertrain"))
        {
            return;
        }

        foreach (CardState cardState in __instance.GetCachedDeckState())
        {
            if (cardState.GetCardDataID() == CaveofaThousandEyes.Card.GetID())
            {
                ___metagameSaveData.TrackCardWin(cardState, divineWin, ___allGameData);
            }
        }

        return;
    }
}
*/