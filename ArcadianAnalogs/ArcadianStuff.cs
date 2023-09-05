using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Void.Init;
using Trainworks.Builders;
using Trainworks.Constants;
using Trainworks.Managers;
using Trainworks.Enums;
using CustomEffects;
using Void.CardPools;
using HarmonyLib;

namespace Void.Arcadian
{ 
    public static class ArcadianCompatibility
    {
        public static bool ArcadianExists = false;

        public static bool isArcadian = false;
        public static bool isArcadianExile = false;
        public static bool isBeyonder = false;
        public static bool isBeyonderExile = false;

        public static ClassData ArcadianClan = null;
        public static CardData Analog = null;

        public static void Initialize()
        { 
            if (Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("ca.chronometry.disciple")) 
            {
                ArcadianExists = true;
                Beyonder.Log("Arcadian Clan exists. Add compatibility.");
            }

            if (ArcadianExists) 
            {
                ArcadianClan = CustomClassManager.GetClassDataByID("Chrono");

                if (ArcadianClan == null) 
                {
                    Beyonder.Log("Error: Unable to retrieve data for Arcadian Clan.");
                    ArcadianExists = false;
                    return;
                }

                Analog = CustomCardManager.GetCardDataByID("Analog");

                if (Analog == null)
                {
                    Beyonder.Log("Error: Unable to retrieve data for Arcadian spell Analog.");
                    ArcadianExists = false;
                    return;
                }
                else
                {
                    //AnalogMastery = ProviderManager.SaveManager.GetMetagameSave().GetMastery(Analog);
                }
                BeyonderAnalogBaseCompulsive.BuildAndRegister();
                Beyonder.Log("Beyonder Analog Base Compulsive");

                BeyonderAnalogExileAfflictive.BuildAndRegister();
                Beyonder.Log("Beyonder Analog Exile Afflictive");
            }
        }

        public static bool CheckRunStatus() 
        {
            SaveManager saveManager = ProviderManager.SaveManager;

            if (!ArcadianExists || saveManager == null)
            {
                return false;
            }

            isArcadian = false;
            isArcadianExile = false;
            isBeyonder = false;
            isBeyonderExile = false;

            if (saveManager.GetMainClass() != null)
            {
                if ((saveManager.GetMainClass().GetID() ==  Beyonder.BeyonderClanData.GetID()))
                {
                    isBeyonder = true;

                    if (saveManager.GetMainChampionIndex() > 0)
                    {
                        isBeyonderExile = true;
                    }
                }
                else if ((saveManager.GetSubClass().GetID() == Beyonder.BeyonderClanData.GetID()))
                {
                    isBeyonder = true;

                    if (saveManager.GetSubChampionIndex() > 0)
                    {
                        isBeyonderExile = true;
                    }
                }

                if ((saveManager.GetMainClass().GetID() == ArcadianClan.GetID()))
                {
                    isArcadian = true;

                    if (saveManager.GetMainChampionIndex() > 0)
                    {
                        isArcadianExile = true;
                    }
                }
                else if ((saveManager.GetSubClass().GetID() == ArcadianClan.GetID()))
                {
                    isArcadian = true;

                    if (saveManager.GetSubChampionIndex() > 0)
                    {
                        isArcadianExile = true;
                    }
                }
            }
            else
            {
                //Beyonder.Log("Attempting to init a run without a main class. Aborting.");
                return false;
            }

            return true;
        }

        public static bool InitRun(SaveManager saveManager) 
        {
            if (!CheckRunStatus()) 
            {
                return false;
            }

            //replace Analog with Equestrian Analog
            if (isArcadianExile && isBeyonder) 
            {
                CardData BeyonderAnalogReplacement = null;
                List<CardState> CardsToReplace = new List<CardState>();

                if (!isBeyonderExile) 
                {
                    BeyonderAnalogReplacement = CustomCardManager.GetCardDataByID(BeyonderAnalogBaseCompulsive.ID);
                }
                else 
                {
                    BeyonderAnalogReplacement = CustomCardManager.GetCardDataByID(BeyonderAnalogExileAfflictive.ID);
                }

                foreach (CardState card in saveManager.GetDeckState())
                {
                    if (card.GetCardDataID() == Analog.GetID())
                    {
                        CardsToReplace.Add(card);
                    }
                }

                if (CardsToReplace.Count > 0)
                {
                    for (int ii = 0; ii < CardsToReplace.Count; ii++) 
                    {
                        saveManager.AddCardToDeck(BeyonderAnalogReplacement, CardsToReplace[ii].GetCardStateModifiers(), false, false, false, false, true);
                    }
                }

                foreach (CardState replaceableAnalog in CardsToReplace)
                {
                    saveManager.RemoveCardFromDeck(replaceableAnalog);
                }
            }

            return true;
        }
    }
}