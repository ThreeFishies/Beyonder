using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using System.Collections;
using UnityEngine;
using StateMechanic;
using Trainworks.AssetConstructors;
using Trainworks.Builders;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using Trainworks.Enums.MTTriggers;
using Void.Init;
using TMPro;

namespace Void.Triggers
{
	public static class Trigger_Beyonder_OnHysteria
	{
        public const string IDName = "Trigger_Beyonder_OnHysteria";
        public static CharacterTrigger OnHysteriaCharTrigger = new CharacterTrigger(IDName + "_Char");
        public static CardTrigger OnHysteriaCardTrigger = new CardTrigger(IDName);
        
        static Trigger_Beyonder_OnHysteria()
		{
			CustomTriggerManager.AssociateTriggers(OnHysteriaCardTrigger, OnHysteriaCharTrigger);
		}
	}
}