using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.BuildersV2;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Void.Unit;
using Void.Clan;
using Void.Status;
using Void.Champions;
using Void.Mania;
using Void.Spells;
using Void.Artifacts;
using Void.Triggers;
using Void.Monsters;
using Void.Enhancers;
using Void.Chaos;

namespace Void.CardPools
{ 
    public static class BeyonderCardPools
    {
        public static CardPool MutationCards;
        public static CardPool UnTherapeutic;
        public static CardPool MadnessWithinCardPool;
        public static CardPool CaveStoryCardPool;

        public static void BuildCardPools() 
        {
            MutationCards = new CardPoolBuilder
            {
                CardPoolID = "BeyonderMutationCardsCardPool",
                Cards = new List<CardData>
                {
                    CustomCardManager.GetCardDataByID(VanillaCardIDs.AdaptiveMutation),
                    Phleghmbuyoancy.Card,
                    SuctionCups.Card,
                    MouthInMouth.Card,
                    DisembodiedMaw.Card,
                    SpikeFromBeyond.Card,
                    SupplementalDeadBrain.Card,
                }
            }.Build();

            UnTherapeutic = new CardPoolBuilder
            {
                CardPoolID = "BeyonderUnTherapeuticCardsCardPool",
                Cards = new List<CardData>
                {
                    EmbraceTheMadness.Card,
                    EntropicStorm.Card,
                    EyeballsForDays.Card,
                    LookingStars.Card,
                    MindScar.Card,
                    OcularInfection.Card,
                    Paranoia.Card,
                    Seizure.Card
                }
            }.Build();

            MadnessWithinCardPool = new CardPoolBuilder 
            {
                CardPoolID = "MadnessWithinCardPool",
                Cards = new List<CardData> 
                { 
                    PostItNoteOfForbiddenKnowledge.Card,
                    PostItNoteOfForbiddenKnowledge.Card,
                    SupplementalDeadBrain.Card
                }
            }.Build();

            CaveStoryCardPool = new CardPoolBuilder
            {
                CardPoolID = "CaveStoryCardPool",
                Cards = new List<CardData>
                {
                    CaveofaThousandEyes.Card
                }
            }.Build();
        }
    }
}