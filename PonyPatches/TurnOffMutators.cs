using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using ShinyShoe.Loading;
using ShinyShoe;
using Void.Init;

namespace Equestrian.HarmonyPatches
{
	//Unknown mutators appear as ugly white boxes. If the clan mod is turned off, reset these settings to avoid that.
	[HarmonyPatch(typeof(ModSettingsScreen), "HandleModToggled")]
	public static class TurnOffMutators
	{
		public static void Prefix(ref ModDefinition modDef, ref bool enabled)
		{ 
			if (!Beyonder.IsInit || ProviderManager.SaveManager == null) { return; }

			if (!enabled && (modDef.ModName == "Beyonder Clan" || modDef.ModName == "Beyonder Dev"))
			{
				Beyonder.Log("Clearing mutator and expert challenge selections.");

				MetagameSaveData metagameSaveData = ProviderManager.SaveManager.GetMetagameSave();

				metagameSaveData.lastSelectedMutatorIDs.Clear();
				metagameSaveData.SetSpChallengeId(null);

				//Beyonder doesn't have a card mastery frame yet.
				//if (metagameSaveData.GetActiveMasteryFrameType() == Ponies.PonyFrame.GetEnum()) 
				//{
				//	Ponies.Log("Reset to default card mastery frame type.");
				//	metagameSaveData.SetActiveMasteryFrameType(MasteryFrameType.Default);
				//}

				ProviderManager.SaveManager.StartSavingMetagame("metagameSave");
			}
		}
	}
}