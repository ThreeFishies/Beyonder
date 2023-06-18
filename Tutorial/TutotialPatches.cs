using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
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
using Void.CardPools;
using Void.Patches;
using System;
using Void.Init;

namespace Void.Tutorial
{
    [HarmonyPatch(typeof(MainMenuScreen), "Initialize")]
    public static class StartTutorial 
    {
        private static bool loadEquestrianArtifact = true;
        public static void Postfix() 
        {
            if (Beyonder.IsInit) 
            { 
                TutorialManager.TryDoWelcome();
            }
            if (loadEquestrianArtifact) 
            {
                loadEquestrianArtifact = false;

                MemoryJewel.BuildAndRegister(); //Equestrian (Must be loaded later due to Equestrian's late initialization.)
                Beyonder.Log("[Equestrian] Memory Jewel");
            }
        }
    }

    [HarmonyPatch(typeof(MapScreen), "Initialize")]
    public static class DisplayIntro 
    {
        public static void Postfix() 
        {
            if (Beyonder.IsInit)
            {
                TutorialManager.TryDoIntro();
            }
        }
    }

    [HarmonyPatch(typeof(RelicDraftScreen), "Initialize")]
    public static class DisplayRelicBlurb 
    {
        public static void Postfix() 
        {
            if (Beyonder.IsInit)
            {
                TutorialManager.TryDoArtifact();
            }
        }
    }

    [HarmonyPatch(typeof(ChampionUpgradeScreen), "Initialize")]
    public static class ChampionUpgradeScreenMessages 
    {
        public static void Postfix() 
        {
            if (Beyonder.IsInit)
            {
                TutorialManager.TryDoChampUpgradeMessages();
            }
        }
    }

    [HarmonyPatch(typeof(CombatFtue), "TryMonsterIntroCoroutine")]
    public static class DoFirstCombatMessages 
    {
        public static void Postfix()
        {
            if (Beyonder.IsInit)
            {
                TutorialManager.TryDoFirstCombatStart();
            }
        }
    }

    [HarmonyPatch(typeof(VictoryUI), "Show")]
    public static class VictoryUIMessages 
    {
        public static void Postfix()
        {
            if (Beyonder.IsInit)
            {
                TutorialManager.TryDoVictoryMessages();
            }
        }
    }

    [HarmonyPatch(typeof(MapNodeUI), "TryTriggerNode")]
    public static class BeyonderBannerMessages
    {
        public static void Postfix(ref MapNodeUI __instance)
        {
            if (Beyonder.IsInit)
            {
                if (__instance.GetData().GetTooltipTitle() == "name_beyonder_banner".Localize(null))
                {
                    TutorialManager.TryDoBeyonderBannerMessages();
                }
            }
        }
    }
}