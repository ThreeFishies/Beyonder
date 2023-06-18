using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using ShinyShoe.Loading;
using System.Collections;
using ShinyShoe;
using ShinyShoe.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Trainworks.Constants;
using Void.Init;

namespace Equestrian.HarmonyPatches
{
    //The art has been fixed.
    //This function is called when the "Give Pony" button is added to the UI.
    //A short "loading" circle appears at the start of battle while this runs.
	public static class FixArt
	{
        public static int cardsToLoad = 0;

        public static void TryYetAnotherFix(CardPool cardPool, LoadingScreen.DisplayStyle displayStyle = LoadingScreen.DisplayStyle.FullScreen) 
        {
            Beyonder.Log("Loading from Card Pool: " + cardPool.name);

            TryYetAnotherFix(cardPool.GetAllChoices(), displayStyle);
        }

        public static void TryYetAnotherFix(List<CharacterData> characters, LoadingScreen.DisplayStyle displayStyle = LoadingScreen.DisplayStyle.FullScreen, Action doneCallbak = null) 
        {
            LoadingScreen.AddTask
            (
            new LoadAdditionalCharacters
                (
                    characters, displayStyle, doneCallbak
                )
            );
        }

        public static void TryYetAnotherFix(List<CardData> cards, LoadingScreen.DisplayStyle displayStyle = LoadingScreen.DisplayStyle.FullScreen, Action doneCallbak = null)
		{
            var saveManager = Trainworks.Managers.ProviderManager.SaveManager;
            CardState card = new CardState();

            //Ponies.Log("Loading art and character assets for ponies.");
            Beyonder.Log("Loading additional card assets. " + cards.Count + " cards to load.");

            cardsToLoad += cards.Count;

            //foreach (CardData pony in MyCardPools.GivePonyCardPool.GetAllChoices())
            foreach (CardData pony in cards)
            {
                LoadingScreen.AddTask
                (
                    new LoadAdditionalCards
                    (
                        CustomCardManager.GetCardDataByID(pony.GetID()), loadSpawnedCharacters: true, displayStyle,
                        delegate
                        {
                            //Ponies.Log("Loaded " + pony.GetName() + ".");

                            cardsToLoad--;

                            //card = saveManager.AddCardToDeck
                            //(
                            //    CustomCardManager.GetCardDataByID(pony.GetID()), null, applyExistingRelicModifiers: true, applyExtraCopiesMutator: false, showAnimation: false, true, false
                            //);
                            //saveManager.RemoveCardFromDeck(card);
                        }
                    )
                );
            }
        }
	}
}