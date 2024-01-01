/*
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
using Trainworks.BuildersV2; //Swapping to the new Trainworks builder.
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Trainworks.Interfaces;
using Trainworks.Constants;
using PubNubAPI;
using Void.Init;
using Void.Artifacts;
using static RotaryHeart.Lib.DataBaseExample;

namespace Void.Status
{
    //[HarmonyPatch(typeof(StatusEffectData), "GetIcon")]
    public static class FixDisplayIconOnUnits 
    {
        public static bool init = false;
        public static Sprite ChronicBig;
        public static Sprite FormlessBig;
        public static Sprite JittersBig;
        public static Sprite MutatedBig;
        public static Sprite PanicBig;
        public static Sprite ShockBig;
        public static Sprite SoundlessBig;

        public static void Init() 
        {
            if (init) { return; }

            ChronicBig = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/StatusIconsBig/chronic.png"));
            FormlessBig = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/StatusIconsBig/formless.png"));
            JittersBig = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/StatusIconsBig/jitters.png"));
            MutatedBig = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/StatusIconsBig/mutated.png"));
            PanicBig = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/StatusIconsBig/panic.png"));
            ShockBig = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/StatusIconsBig/shock.png"));
            SoundlessBig = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/StatusIconsBig/soundless.png"));
            ChronicBig.name = "chronic";
            FormlessBig.name = "formless";
            JittersBig.name = "jitters";
            MutatedBig.name = "mutated";
            PanicBig.name = "panic";
            ShockBig.name = "shock";
            SoundlessBig.name = "soundless";

            AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(StatusEffectChronic.data, ChronicBig);
            AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(StatusEffectFormless.data, FormlessBig);
            AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(StatusEffectJitters.data, JittersBig);
            AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(StatusEffectMutated.data, MutatedBig);
            AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(StatusEffectPanic.data, PanicBig);
            AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(StatusEffectShock.data, ShockBig);
            AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(StatusEffectSoundless.data, SoundlessBig);

            init = true;
        }

        /*
        public static void Postfix(ref StatusEffectData __instance, ref Sprite __result) 
        {
            if (__instance.GetStatusId() == StatusEffectChronic.statusId) 
            {
                __result = ChronicBig; 
                return;
            }
            if (__instance.GetStatusId() == StatusEffectFormless.statusId)
            {
                __result = FormlessBig;
                return;
            }
            if (__instance.GetStatusId() == StatusEffectJitters.statusId)
            {
                __result = JittersBig;
                return;
            }
            if (__instance.GetStatusId() == StatusEffectMutated.statusId)
            {
                __result = MutatedBig;
                return;
            }
            if (__instance.GetStatusId() == StatusEffectPanic.statusId)
            {
                __result = PanicBig;
                return;
            }
            if (__instance.GetStatusId() == StatusEffectShock.statusId)
            {
                __result = ShockBig;
                return;
            }
            if (__instance.GetStatusId() == StatusEffectSoundless.statusId)
            {
                __result = SoundlessBig;
                return;
            }
        }*//*
    }
}*/