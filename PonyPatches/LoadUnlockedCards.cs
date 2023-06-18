using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using Trainworks.Constants;
using Trainworks.Builders;
using Void.Init;
using Void.Spells;
using Void.Monsters;

namespace Equestrian.HarmonyPatches
{
    [HarmonyPatch(typeof(UnlockScreen), "GetClanLevelUpUnlockItems")]
    public static class LoadCardsForLevelUP
    {
        static void Postfix(ref SaveManager saveManager, ref ClassData classData, ref int newLevel) 
        { 
            if (Beyonder.BeyonderClanData.GetID() == classData.GetID()) 
            {
                CardPool cardsToShow = UnityEngine.ScriptableObject.CreateInstance<CardPool>();

                Beyonder.Log("Level up to level: " + newLevel);

                CardPoolBuilder cardPoolBuilder = new CardPoolBuilder() 
                { 
                    CardPoolID = "BeyonderClanNewLevel_" + newLevel,
                    CardIDs = new List<string> { }
                };

                cardsToShow = cardPoolBuilder.BuildAndRegister();
                var cardDataList = (Malee.ReorderableArray<CardData>)AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(cardsToShow);                

                if (newLevel == 1) //You start at class level 1 so this is not needed.
                {
                }
                if (newLevel == 2)
                {
                    cardDataList.Add(Phleghmbuyoancy.Card);
                }
                if (newLevel == 3)
                {
                    cardDataList.Add(PostItNoteOfForbiddenKnowledge.Card);
                }
                if (newLevel == 4)
                {
                    cardDataList.Add(Chutzpah.Card);
                    cardDataList.Add(MassHysteria.Card);
                }
                if (newLevel == 5)
                {
                    cardDataList.Add(EyeballsForDays.Card);
                }
                if (newLevel == 6)
                {
                    cardDataList.Add(Deathwood.Card);
                }
                if (newLevel == 7)
                {
                    cardDataList.Add(PacingRut.Card);
                }
                if (newLevel == 8)
                {
                    cardDataList.Add(DarkRecipe.Card);
                }
                if (newLevel == 9)
                {
                    cardDataList.Add(ApostleoftheVoid.Card);
                    cardDataList.Add(EmbraceTheMadness.Card);
                }
                if (newLevel == 10)
                {
                    cardDataList.Add(ExistentialDread.Card);
                    cardDataList.Add(BasketCase.Card);
                }

                FixArt.TryYetAnotherFix(cardsToShow, ShinyShoe.Loading.LoadingScreen.DisplayStyle.Spinner);
            }
        }
    }
}