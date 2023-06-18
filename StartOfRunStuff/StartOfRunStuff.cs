using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Builders;
using Trainworks.Managers;
using Trainworks.Constants;
using System.Linq;
using UnityEngine;
using Trainworks.Utilities;
using Void.Init;
using CustomEffects;
using Void.Clan;
using Void.Unit;
using Void.Triggers;
using Void.Status;
using Steamworks;
using TMPro;
using Void.Monsters;
using Void.Champions;
using Void.Artifacts;
using Void.Spells;
using ShinyShoe.Loading;

namespace Void.Chaos.NewRun
{
    [HarmonyPatch(typeof(SaveManager), "SetupRun")]
    public static class StartOfRunStuff
    {
        public static void Prefix(SaveManager __instance)
        {
            //Beyonder.Log("Main class: " +__instance.GetStartingConditions().Class);
            //Beyonder.Log("Sub class: " + __instance.GetStartingConditions().Subclass);

            if (!StartNewRunFromRunHistory.replayedRun)
            {
                LoadingScreen.AddTask(new LoadChaos(LoadingScreen.DisplayStyle.FullScreen, null));
            }
            else
            {
                LoadingScreen.AddTask(new ReplayChaos(LoadingScreen.DisplayStyle.FullScreen, null));
            }

            StartNewRunFromRunHistory.replayedRun = false;

            if (StatusEffectChronic.cardsTriggeredOn != null)
            {
                StatusEffectChronic.cardsTriggeredOn.Clear();
            }
        }

        public static void Postfix(SaveManager __instance)
        {
            //__instance.AddCardToDeck(SoundlessSwarm.Card);
            //__instance.AddCardToDeck(Phleghmbuyoancy.Card);
            //__instance.AddCardToDeck(SuctionCups.Card);
            //__instance.AddCardToDeck(MouthInMouth.Card);
            //__instance.AddCardToDeck(Malevolence.Card);
            //__instance.AddCardToDeck(DisembodiedMaw.Card);
            //__instance.AddCardToDeck(SpikeFromBeyond.Card);
            //__instance.AddCardToDeck(PostItNoteOfForbiddenKnowledge.Card);
            //__instance.AddCardToDeck(SupplementalDeadBrain.Card);
            //__instance.AddCardToDeck(CustomCardManager.GetCardDataByID(VanillaCardIDs.AdaptiveMutation));
            //__instance.AddCardToDeck(MentalDisorder.Card);
            //__instance.AddCardToDeck(CopingMechanism.Card);
            //__instance.AddCardToDeck(Microaggression.Card);
            //__instance.AddCardToDeck(IntoTheBeyond.Card);
            //__instance.AddCardToDeck(LookingStars.Card);
            //__instance.AddCardToDeck(DarkRecipe.Card);
            //__instance.AddCardToDeck(HairyPotty.Card);
            //__instance.AddCardToDeck(FurryBeholder.Card);
            //__instance.AddCardToDeck(Vexation.Card);
            //__instance.AddCardToDeck(Deathwood.Card);
            //__instance.AddCardToDeck(MassHysteria.Card);
            //__instance.AddCardToDeck(EyeballsForDays.Card);
            //__instance.AddCardToDeck(Chutzpah.Card);
            //__instance.AddCardToDeck(ApostleoftheVoid.Card);
            //__instance.AddCardToDeck(Seizure.Card);
            //__instance.AddCardToDeck(Paranoia.Card);
            //__instance.AddCardToDeck(CerebralDetonation.Card);
            //__instance.AddCardToDeck(PacingRut.Card);
            //__instance.AddCardToDeck(HeebieJeebies.Card);
            //__instance.AddCardToDeck(DesperateSearch.Card);
            //__instance.AddCardToDeck(CustomCardManager.GetCardDataByID(VanillaCardIDs.BoneDogsFavor));
            //__instance.AddCardToDeck(BasketCase.Card);
            //__instance.AddCardToDeck(EmbraceTheMadness.Card);
            //__instance.AddCardToDeck(EntropicStorm.Card);
            //__instance.AddCardToDeck(ExistentialDread.Card);
            //__instance.AddCardToDeck(PrimordialSoup.Card);
            //__instance.AddCardToDeck(Sociopathy.Card);
            //__instance.AddRelic(BrainBleach.Artifact);
            //__instance.AddRelic(BrainClamps.Artifact);
            //__instance.AddRelic(BedMonster.Artifact);
            //__instance.AddRelic(VialOfBlackEyedBlood.Artifact);
            //__instance.AddRelic(LivingEntropy.Artifact);
            //__instance.AddRelic(LingeringChaos.Artifact);
            //__instance.AddRelic(BloodyTentacles.Artifact);
            //__instance.AddRelic(UnstableEnergy.Artifact);
            //__instance.AddRelic(CustomCollectableRelicManager.GetRelicDataByID(VanillaCollectableRelicIDs.AbandonedAntumbra));
            //__instance.AddRelic(RadioactiveWaste.Artifact);
            //__instance.AddRelic(EyevoryEyedol.Artifact);
            //__instance.AddRelic(BlackLight.Artifact);
            //__instance.AddRelic(CustomCollectableRelicManager.GetRelicDataByID(VanillaCollectableRelicIDs.TheFirstHellpact));
            //__instance.AddRelic(GenieImp.Artifact);
            //__instance.AddRelic(FasciatedKernels.Artifact);
            //__instance.AddRelic(MalickasGift.Artifact);
            //__instance.AddRelic(ShallowGraves.Artifact);
            //__instance.AddCardToDeck(CustomCardManager.GetCardDataByID("8cd475e4-6e39-4306-b544-026bf30f4eac"));
            //__instance.AddRelic(ShadowPuppeteer.Artifact);
            //__instance.AddRelic(BadEggs.Artifact);
            //__instance.AddRelic(CustomCollectableRelicManager.GetRelicDataByID(VanillaCollectableRelicIDs.TraitorsQuill));

            //if (MemoryJewel.Artifact != null) 
            //{ 
            //    __instance.AddRelic(MemoryJewel.Artifact);
            //}
            //if (PurloinedHeavensSeal.Artifact != null) { __instance.AddRelic(PurloinedHeavensSeal.Artifact); }
            //if (Preservatives.Artifact != null) { __instance.AddRelic(Preservatives.Artifact); }
            //if (ScourgeMagnet.Artifact != null) { __instance.AddRelic(ScourgeMagnet.Artifact); }
            /*
            if (UnSeeingEye.HasIt()) 
            {
                Beyonder.Log("Unseeing Eye detected. Applying effects.");

                //if (!ProviderManager.TryGetProvider<CardManager>(out CardManager cardManager)) 
                //{
                //    Beyonder.Log("Failed to apply Unseeing Eye. Card Manager not found.");
                //    return;
                //}

                if (__instance.GetDeckState() != null && __instance.GetDeckState().Count > 0) 
                {
                    List<CardState> deck = __instance.GetDeckState();

                    BeyonderAddManicTraitToAlliedClanStarterCards.Initialize();

                    for (int ii = 0; ii < deck.Count; ii++)
                    {
                        if (BeyonderAddManicTraitToAlliedClanStarterCards.ApplyCardStateModifiers(deck[ii], __instance))
                        {
                            Beyonder.Log("Applied Unseeing Eye to card: " + deck[ii].GetTitleKey().LocalizeEnglish());
                        }
                        else 
                        {
                            Beyonder.Log("Skipping card: " + deck[ii].GetTitleKey().LocalizeEnglish());
                        }
                    }
                }
                else 
                {
                    Beyonder.Log("Failed to apply Unseeing Eye. No cards found in deck.");
                }
            }
            */
        }
    }

    [HarmonyPatch(typeof(UnitSynthesisMapping), "GetUpgradeData")]
    [HarmonyAfter("tools.modding.trainworks")] //Adding this in anticipation of a future Trainworks update. Can't have Trainworks undoing my hard work by reverting to default dummy data.
    public static class FindDynamicSynthesisData
    {
        public static void Postfix(ref CardUpgradeData __result, ref CharacterData characterData)
        {
            //This prevents a crash that can occur if this function is called before the Beyonder Clan is initialized.
            if (!Beyonder.IsInit) { return; }
            if (characterData == null) { return; }
            if (characterData.IsChampion()) { return; }

            //Veilrich
            if (characterData == FormlessHorror.Character) 
            {
                __result = FormlessHorror.GetSynthesis();
            }
            if (characterData == Malevolence.Character)
            {
                __result = Malevolence.GetSynthesis();
            }
            if (characterData == Vexation.Character)
            {
                __result = Vexation.GetSynthesis();
            }

            //Undretch
            if (characterData == SoundlessSwarm.Character) 
            { 
                __result = SoundlessSwarm.GetSynthesis();
            }
            if (characterData == HairyPotty.Character) 
            {
                __result = HairyPotty.GetSynthesis();
            }
            if (characterData == FurryBeholder.Character) 
            { 
                __result = FurryBeholder.GetSynthesis();
            }

            //ApostleoftheVoid
            if (characterData == ApostleoftheVoid.Character)
            {
                __result = ApostleoftheVoid.GetSynthesis();
            }

            //A bit of error handling
            if (__result == null) 
            {
                __result = FormlessHorror.Synthesis;
            }
        }
    }
}