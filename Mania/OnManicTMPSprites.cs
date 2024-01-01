using HarmonyLib;
using System.IO;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;
using Void.Init;
using Void.Triggers;

namespace Void.Mania 
{
    public static class RegisterTMPSprites
    {
        private static bool isInit = false;
        public static Sprite OnAnxietyTMP;
        public static Sprite OnHysteriaTMP;

        public static void SetupSprites()
        {
            if (!isInit || OnAnxietyTMP == null)
            {
                string OnAnxietyTMPfullPath = Path.Combine(Beyonder.BasePath, "ClanAssets/OnAnxietyTMP.png");
                string OnHysteriaTMPfullPath = Path.Combine(Beyonder.BasePath, "ClanAssets/OnHysteriaTMP.png");
                OnAnxietyTMP = CustomAssetManager.LoadSpriteFromPath(OnAnxietyTMPfullPath);
                OnHysteriaTMP = CustomAssetManager.LoadSpriteFromPath(OnHysteriaTMPfullPath);
                OnAnxietyTMP.name = "OnAnxiety"; //CharacterTriggerData.GetKeywordText(Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(), false);
                OnHysteriaTMP.name = "OnHysteria"; //CharacterTriggerData.GetKeywordText(Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(), false);
                TMP_SpriteAssetUtils.AddTextIcon(OnAnxietyTMPfullPath, OnAnxietyTMP.name);
                TMP_SpriteAssetUtils.AddTextIcon(OnHysteriaTMPfullPath, OnHysteriaTMP.name);
                isInit = true;
            }
        }
    }
}