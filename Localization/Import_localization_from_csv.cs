using System;
using HarmonyLib;
using I2.Loc;
using Trainworks.Managers;

namespace Void.Localization
{
	internal class localization
	{
		[HarmonyPatch(typeof(LocalizationManager), "UpdateSources")]
		private class RegisterLocalizationStrings
		{
			private static void Postfix()
			{
				CustomLocalizationManager.ImportCSV("Localization/InfiniteVoid.csv", ',');
			}
		}
	}
}