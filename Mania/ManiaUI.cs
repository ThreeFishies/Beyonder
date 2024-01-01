using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using System.Text;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ParticleSystemJobs;
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
using Steamworks;
using BepInEx.Logging;
using ShinyShoe;

namespace Void.Mania
{
    public static class ManiaUI
    {
        public static EnergyUI UI;
        public static Image image;
        public static TMP_Text label;
        //public static ParticleSystem maniaVfx;
        public static Color sanity = Color.white;
        public static Color highMania = new Color(1.0f, 0.6f, 0.2f, 1.0f);
        public static Color panic = new Color(1.0f, 0.4f, 0.0f, 1.0f);
        public static Color lowMania = new Color(1.0f, 0.2f, 0.6f, 1.0f);
        public static Color shock = new Color(1.0f, 0.0f, 0.4f, 1.0f);
        public static Color blackout = new Color(0.4f, 0.2f, 0.2f, 1.0f);
        public static Sprite maniaSane;
        public static Sprite maniaInsanePanic;
        public static Sprite maniaInsaneShock;

        public static void Init(BattleHud hud)
        {
            ManiaManager.Mania = 0;
            ManiaManager.maniaLevel = ManiaLevel.Default;
            for (int ii = ManiaManager.coroutines.Count - 1; ii > -1; ii--)
            {
                ManiaManager.coroutines[ii] = null;
            }
            ManiaManager.coroutines.Clear();
            ManiaManager.triggerQueueDatas.Clear();
            ManiaManager.chutzpahAssociatedTriggers.Clear();
            ManiaManager.chutzpahCounts.Clear();
            ManiaManager.chutzpahTargets.Clear();

            if (maniaSane == null)
            {
                maniaSane = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/ManiaMeterBase.png"));
                maniaInsanePanic = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/ManiaMeterPanic.png"));
                maniaInsaneShock = CustomAssetManager.LoadSpriteFromPath(Path.Combine(Beyonder.BasePath, "ClanAssets/ManiaMeterNeurosis.png"));
            }

            //Beyonder.Log("Line 55.");

            EnergyUI baseEnergyUI = AccessTools.Field(typeof(BattleHud), "energyUI").GetValue(hud) as EnergyUI;

            //Beyonder.Log("Line 59.");

            if (UI != null) 
            {
                GameObject.DestroyImmediate(UI);
            }

            UI = GameObject.Instantiate<EnergyUI>(baseEnergyUI, hud.transform);

            //ParticleSystem source = AccessTools.Field(typeof(EnergyUI), "gainEmberVfx").GetValue(baseEnergyUI) as ParticleSystem;
            //maniaVfx = ParticleSystem.Instantiate(source);

            //Beyonder.Log("Line 66.");

            //Why declare this obsolete when the replacement can't be modified?
            //maniaVfx.startColor = Color.magenta;

            //AccessTools.Field(typeof(EnergyUI), "gainEmberVfx").SetValue(UI, maniaVfx);

            //Beyonder.Log("Line 73.");

            label = AccessTools.Field(typeof(EnergyUI), "count").GetValue(UI) as TMP_Text;

            //Beyonder.Log("Line 77.");

            Component.DestroyImmediate(UI.GetComponent<Graphic2DInvisRaycastTarget>());
            image = UI.gameObject.AddComponent<Image>();
            //image.sprite = maniaSane;

            image.rectTransform.sizeDelta = new Vector2() { x = 0.5f, y = 0.5f };

            //Beyonder.Log("Line 81.");

            LocalizedTooltipProvider tooltipProvider = UI.gameObject.GetComponent<LocalizedTooltipProvider>();
            Component.DestroyImmediate(tooltipProvider);
            TooltipProviderComponent tooltipProvider1 = UI.gameObject.AddComponent<TooltipProviderComponent>();

            //Don't add any more tooltips or they won't fit.
            tooltipProvider1.ClearTooltips();
            //tooltipProvider.SetTooltip("Beyonder_Mechanic_Mania_TooltipKey", "Beyonder_Mechanic_Mania_TooltipText", TooltipDesigner.TooltipDesignType.Keyword);
            tooltipProvider1.AddTooltip(new TooltipContent("Beyonder_Mechanic_Mania_TooltipKey".Localize(), "Beyonder_Mechanic_Mania_TooltipText".Localize(), TooltipDesigner.TooltipDesignType.Keyword, "Mania"));
            tooltipProvider1.AddTooltip(new TooltipContent("Beyonder_Mechanic_Manic_Tips_TooltipKey".Localize(), "Beyonder_Mechanic_Manic_Tips_TooltipBody".Localize(), TooltipDesigner.TooltipDesignType.Default, "Manic"));
            tooltipProvider1.AddTooltip(new TooltipContent("Trigger_Beyonder_OnHysteria_CardText".Localize(), "Trigger_Beyonder_OnHysteria_TooltipText".Localize(), TooltipDesigner.TooltipDesignType.Keyword, "Hysteria"));
            tooltipProvider1.AddTooltip(new TooltipContent("Trigger_Beyonder_OnAnxiety_CardText".Localize(), "Trigger_Beyonder_OnAnxiety_TooltipText".Localize(), TooltipDesigner.TooltipDesignType.Keyword, "Anxiety"));
            tooltipProvider1.AddTooltip(new TooltipContent("Beyonder_Mechanic_Insanity_TooltipKey".Localize(), "Beyonder_Mechanic_Insanity_TooltipText".Localize(), TooltipDesigner.TooltipDesignType.Keyword, "Insanity"));
            tooltipProvider1.AddTooltip(new TooltipContent("StatusEffect_Beyonder_panic_CardText".Localize(), "StatusEffect_Beyonder_panic_CardTooltipText".Localize(), TooltipDesigner.TooltipDesignType.Negative, "Panic"));
            tooltipProvider1.AddTooltip(new TooltipContent("StatusEffect_Beyonder_shock_CardText".Localize(), "StatusEffect_Beyonder_shock_CardTooltipText".Localize(), TooltipDesigner.TooltipDesignType.Negative, "Neurosis"));

            Set_Text();
            Set_Color();
            Set_Image();

            UpdateUI();

            image.SetNativeSize();

            if (Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("rawsome.modster-train.move-battle-ui"))
            {
                image.rectTransform.AdjustPosition((-95.0f + 102.64f + 27.0f), (-20.0f + 53.39f - 9.0f), (0.0f));
            }
            else
            {
                image.rectTransform.AdjustPosition((-95.0f + 102.64f), (-20.0f + 53.39f), (0.0f));
            }
        }

        public static void UpdateUI()
        {
            if (UI == null) { return; }
            if (!ProviderManager.SaveManager.IsInBattle() || ProviderManager.SaveManager.GetGameSequence() == SaveData.GameSequence.BattlePostCombatRewards)
            {
                UI.gameObject.SetActive(false);
                ManiaManager.Mania = 0;
                ManiaManager.maniaLevel = ManiaLevel.Default;
                for (int ii = ManiaManager.coroutines.Count - 1; ii > -1; ii--)
                {
                    ManiaManager.coroutines[ii] = null;
                }
                ManiaManager.coroutines.Clear();
                ManiaManager.triggerQueueDatas.Clear();
                ManiaManager.chutzpahAssociatedTriggers.Clear();
                ManiaManager.chutzpahCounts.Clear();
                ManiaManager.chutzpahTargets.Clear();
                return;
            }

            if (ManiaManager.Mania != 0 || IsBeyonder())
            {
                Set_Text();
                Set_Color();
                Set_Image();

                UI.gameObject.SetActive(true);
            }
            else 
            {
                UI.gameObject.SetActive(false);
            }
        }

        public static bool IsBeyonder() 
        {
            if (Void.Mutators.MadnessWithin.HasIt()) 
            {
                return true;
            }

            if (ProviderManager.SaveManager.GetMainClass() != null && ProviderManager.SaveManager.GetMainClass() == Beyonder.BeyonderClanData) 
            {
                return true;
            }

            return ProviderManager.SaveManager.GetSubClass() != null && ProviderManager.SaveManager.GetSubClass() == Beyonder.BeyonderClanData;
        }

        public static void Set_Image()
        {
            if (image == null) 
            {
                Beyonder.Log("Failed to update ManiaUI because image is null.");
                return;
            }

            if (maniaSane == null || maniaInsanePanic == null || maniaInsaneShock == null) 
            {
                Beyonder.Log("Failed to update ManiaUI because an asset is missing.");
                return;
            }

            ManiaLevel maniaLevel = ManiaManager.GetCurrentManiaLevel();

            switch (maniaLevel)
            {
                case ManiaLevel.Default:
                    image.sprite = maniaSane;
                    break;

                case ManiaLevel.High:
                    image.sprite = maniaSane;
                    break;

                case ManiaLevel.Neurosis:
                    image.sprite = maniaInsaneShock;
                    //maniaVfx.Play();
                    break;

                case ManiaLevel.Low:
                    image.sprite = maniaSane;
                    break;

                case ManiaLevel.Panic:
                    image.sprite = maniaInsanePanic;
                    //maniaVfx.Play();
                    break;

                case ManiaLevel.BlackoutHigh:
                    image.sprite = maniaInsanePanic;
                    //maniaVfx.Play();
                    break;

                case ManiaLevel.BlackoutLow:
                    image.sprite = maniaInsaneShock;
                    //maniaVfx.Play();
                    break;

                default:
                    image.sprite = maniaSane;
                    break;
            }

        }

        public static void Set_Text()
        {
            int tempMania = ManiaManager.Mania + (ManiaManager.PreviewHysteria ? 1 : 0) + (ManiaManager.PreviewAnxiety ? -1 : 0);
            tempMania *= (ManiaManager.PreviewSanity ? 0 : 1);

            label.SetTextSafe(tempMania.ToString(), true);
        }

        public static void Set_Color()
        {
            ManiaLevel maniaLevel = ManiaManager.GetCurrentManiaLevel();

            switch (maniaLevel)
            {
                case ManiaLevel.Default:
                    label.color = sanity;
                    break;

                case ManiaLevel.High:
                    label.color = highMania;
                    break;

                case ManiaLevel.Neurosis:
                    label.color = shock;
                    break;

                case ManiaLevel.Low:
                    label.color = lowMania;
                    break;

                case ManiaLevel.Panic:
                    label.color = panic;
                    break;

                case ManiaLevel.BlackoutHigh:
                    label.color = blackout;
                    break;

                case ManiaLevel.BlackoutLow:
                    label.color = blackout;
                    break;

                default:
                    label.color = sanity;
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(BattleHud), "SetActive")]
    public static class InitManiaUI
    {
        public static void Postfix(ref BattleHud __instance, bool active) 
        {
            if (active) 
            {
                ManiaUI.Init(__instance);
            }
            else if (ManiaUI.UI != null && ManiaUI.UI.gameObject != null && ManiaUI.image != null)
            { 
                ManiaUI.UI.gameObject.SetActive(false);
            }
        }
    }
}