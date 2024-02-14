
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
using Void.Init;
using Void.Triggers;
using CustomEffects;
using RunHistory;
using Void.Spells;
using System.Reflection;
using Void.Chaos;

namespace Void.Artifacts
{
    public static class FasciatedKernels
    {
        public static CollectableRelicData Artifact;
        public static string ID = "FasciatedKernels_" + Beyonder.GUID;

        public static bool HasIt()
        {
            if (Artifact != null)
            {
                return ProviderManager.SaveManager.GetHasRelic(Artifact);
            }
            return false;
        }

        public static CollectableRelicData BuildAndRegister()
        {
            Artifact = new CollectableRelicDataBuilder
            {
                CollectableRelicID = ID,
                ClanID = VanillaClanIDs.Awoken,
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_FasciatedKernels_Name_Key",
                DescriptionKey = "Malicka_Artifact_FasciatedKernels_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_FasciatedKernels_Lore_Key"
                },
                IconPath = "ArtifactAssets/FasciatedKernels.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = CustomClassManager.GetClassDataByID(VanillaClanIDs.Awoken),
                Rarity = CollectableRarity.Common,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = "RelicEffectNull",
                    }
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }

    [HarmonyPatch(typeof(SynthesisScreen), "PopulateGoods")]
    public static class FasciatedKernelsCostIncreasePatch 
    {
        public static int defaultCrystals = 25;

        public static void Prefix(ref List<MerchantGoodState> goods) 
        {
            for (int i = 0; i < goods.Count; i++)
            {
                MerchantGoodState merchantGoodState = goods[i];
                if (merchantGoodState != null && merchantGoodState.RewardData is UnitSynthesisRewardData)
                {
                    //if (defaultCrystals == -1) 
                    //{
                    //    defaultCrystals = merchantGoodState.Crystals;
                    //}

                    if (FasciatedKernels.HasIt())
                    {
                        AccessTools.Field(typeof(MerchantGoodState), "crystals").SetValue(merchantGoodState, defaultCrystals + 15);
                    }
                    else 
                    {
                        if (merchantGoodState.Crystals != defaultCrystals) 
                        {
                            AccessTools.Field(typeof(MerchantGoodState), "crystals").SetValue(merchantGoodState, defaultCrystals);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(UnitSynthesisRewardData), "GrantReward")]
    public static class CacheFasciatedKernelsDuplicateState
    {
        public static bool ShouldDuplicate = false;

        public static void Prefix() 
        {
            if (FasciatedKernels.HasIt()) 
            {
                ShouldDuplicate = true;
            }
        }
    }

    [HarmonyPatch(typeof(CardState), "Upgrade")]
    public static class GrantFasciatedKernelsDuplicateState
    {
        public static void Prefix(ref CardState __instance, CardUpgradeState upgradeState, SaveManager saveManager, bool ignoreUpgradeAnimation)
        {
            if (CacheFasciatedKernelsDuplicateState.ShouldDuplicate)
            {
                //Filtering for synthesis upgrades should stop it from picking up on starting upgrades.
                if (upgradeState == null || __instance == null || !upgradeState.IsUnitSynthesisUpgrade())
                {
                    return;
                }

                CacheFasciatedKernelsDuplicateState.ShouldDuplicate = false;

                Beyonder.Log($"Fasciated Kernels detected. Attempting to duplicate: {upgradeState.GetAssetName()} - unit synthesis of: {upgradeState.GetSourceSynthesisUnit().GetName()} to unit: {__instance.GetTitle()}.");

                CardUpgradeState upgradeState1 = new CardUpgradeState();
                CardUpgradeData upgradeData = ProviderManager.SaveManager.GetAllGameData().FindCardUpgradeData(upgradeState.GetCardUpgradeDataId());
                if (upgradeData != null)
                {
                    upgradeState1.Setup(upgradeData);

                    __instance.Upgrade(upgradeState1, saveManager, ignoreUpgradeAnimation);
                }
                else 
                {
                    Beyonder.Log($"Failed to locate base data for {upgradeState.GetAssetName()}.");
                }
            }
        }
    }

    [HarmonyPatch(typeof(DeckScreen), "SetCardUpgradeData")]
    public static class PreviewFasciatedKernelsDuplicateState 
    {
        public static void Prefix(ref CardUpgradeData setCardUpgradeData, ref DeckScreen.Params ___setupParams) 
        {
            if (FasciatedKernels.HasIt() && ___setupParams != null && ___setupParams.mode == DeckScreen.Mode.SynthesisSelectionUpgrade && setCardUpgradeData != null)
            {
                setCardUpgradeData = ChaosManager.MergeUpgrades(setCardUpgradeData, setCardUpgradeData, setCardUpgradeData.GetSourceSynthesisUnit(), setCardUpgradeData.GetUpgradeDescriptionKey());
            }
        }
    }
}