using System;
using System.IO;
using HarmonyLib;
using ShinyShoe;
using UnityEngine;
using BepInEx;
using Void.Init;
using Trainworks.Managers;
using Void.Champions;

namespace Void.Clan
{
	[HarmonyPatch(typeof(ClassSelectionScreen), "RefreshCharacters")]
	public static class TheVoidIsAllPresent
	{
		private static GameObject LocoMotiveSpite = new GameObject();
		private static GameObject EpidemialSprite = new GameObject();
		private static GameObject subClan = new GameObject();
		private static bool isSetup = false;

		private static void Postfix(ref ClassSelectionUI ___mainClassSelectionUI, ref ClassSelectionIconUI ___subClassSelectionUI, ref ChampionData ____currentlySelectedChampion) 
		{
			//Ponies.Log("Updating selected characters:");
            if (!isSetup) 
			{ 
				SetupSprites();
			}

			//Ponies.Log("Mare a Lee Instance ID: " + mareALee.GetInstanceID().ToString());

			if (LocoMotiveSpite.GetInstanceID() == 0) 
			{
				//Ponies.Log("Attempting to refresh clan sprites.");

				LocoMotiveSpite = null;
				EpidemialSprite = null;
				subClan = null;

				LocoMotiveSpite = new GameObject();
				EpidemialSprite = new GameObject();
				subClan = new GameObject();

				SetupSprites();
			}

			//Ponies.Log("Mare a Lee Instance ID: " + mareALee.GetInstanceID().ToString());

			if (___mainClassSelectionUI.currentClass.isRandom) 
			{
				//Ponies.Log("Random main clan selected.");
				LocoMotiveSpite.SetActive(false);
				EpidemialSprite.SetActive(false);
			}
			else if (____currentlySelectedChampion.championCardData == LocoMotive.card)
			{
				//Ponies.Log("Champion Mare a Lee selected.");
				LocoMotiveSpite.SetActive(true);
				EpidemialSprite.SetActive(false);
			}
			else if (____currentlySelectedChampion.championCardData == Epidemial.card)
			{
				//Ponies.Log("Champion Tantabus selected.");
				LocoMotiveSpite.SetActive(false);
				EpidemialSprite.SetActive(true);
			}
			else 
			{
				//Ponies.Log("Non-Equestrian champion selected.");
				LocoMotiveSpite.SetActive(false);
				EpidemialSprite.SetActive(false);
			}

			if (___subClassSelectionUI.currentClass.isRandom) 
			{
				//Ponies.Log("Random sub-class selected.");
				subClan.SetActive(false);
			}
			else if (___subClassSelectionUI.currentClass.classData == Beyonder.BeyonderClanData)
			{
				//Ponies.Log("Equestrian sub-class selected.");
				subClan.SetActive(true);
			}
			else 
			{
				//Ponies.Log("Non-Equestrian sub-class selected.");
				subClan.SetActive(false);
			}
		}

		private static void SetupSprites() 
		{
			SpriteRenderer oohLaLa = LocoMotiveSpite.AddComponent<SpriteRenderer>();
			oohLaLa.sprite = Mod_Sprites_Setup.LoadNewSprite(Path.Combine(Path.GetDirectoryName(Beyonder.Instance.Info.Location), "ClanAssets"), "LocoMotiveSprite.png");
			oohLaLa.sortingLayerID = 0;
			oohLaLa.transform.SetPosition(-7.0f, -5.0f, 0.0f);
			SpriteRenderer wakkaWakka = EpidemialSprite.AddComponent<SpriteRenderer>();
			wakkaWakka.sprite = Mod_Sprites_Setup.LoadNewSprite(Path.Combine(Path.GetDirectoryName(Beyonder.Instance.Info.Location), "ClanAssets"), "EpidemialSprite.png");
			wakkaWakka.sortingLayerID = 0;
			wakkaWakka.transform.SetPosition(-5.0f, -5.0f, 0.0f);
			SpriteRenderer oogaOoga = subClan.AddComponent<SpriteRenderer>();
			oogaOoga.sprite = Mod_Sprites_Setup.LoadNewSprite(Path.Combine(Path.GetDirectoryName(Beyonder.Instance.Info.Location), "ClanAssets"), "BeyonderClanSprite.png");
			oogaOoga.sortingLayerID = 0;
			oogaOoga.transform.SetPosition(-12.0f,-2.0f,0.0f);

			LocoMotiveSpite.SetActive(false);
			EpidemialSprite.SetActive(false);
			subClan.SetActive(false);

			isSetup = true;
		}
	}
}