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
using Void.Init;
using Void.Triggers;
using CustomEffects;
using RunHistory;
using Void.Spells;
using Void.Artifacts;

namespace Void.Artifacts
{
    public static class ScourgeMagnet
    {
        public static CollectableRelicData Artifact;
        public static string ID = "ScourgeMagnet_" + Beyonder.GUID;

        public static bool TryGetClassData(out ClassData clan)
        {
            clan = null;

            if (!Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("com.name.package.succclan-mod"))
            {
                return false;
            }

            clan = CustomClassManager.GetClassDataByID("Succubus");

            if (clan == null)
            {
                Beyonder.LogError("Succubus detected but failed to find class data.");
                return false;
            }

            return true;
        }

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
            if (!TryGetClassData(out ClassData clan))
            {
                return null;
            }

            Artifact = new CollectableRelicDataBuilder
            {
                CollectableRelicID = ID,
                ClanID = clan.GetID(),
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_ScourgeMagnet_Name_Key",
                DescriptionKey = "Malicka_Artifact_ScourgeMagnet_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_ScourgeMagnet_Lore_Key"
                },
                IconPath = "ArtifactAssets/ScourgeMagnet.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                //LinkedClass = clan,
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "EmptyString-0000000000000000-00000000000000000000000000000000-v2",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(RelicEffectAddTempUpgrade),
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamExcludeCharacterSubtypes = new string[] {},
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        { 
                            UpgradeID = "MagneticForBLightCards",
                            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                            { 
                                new CardTraitDataBuilder
                                { 
                                    TraitStateType = typeof(CustomCardTraitMagnetizedState),
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardUpgradeMaskID = "ScourgeMagnetBlightFilter",
                                    CardType = CardType.Blight
                                }
                            }
                        }.Build(),
                        ParamTargetMode = TargetMode.FrontInRoom,
                        //AdditionalTooltips = new List<AdditionalTooltipData>
                        //{ 
                        //    new AdditionalTooltipData
                        //    { 
                        //        titleKey = "CardTraitMagneticState_CardText",
                        //        descriptionKey = "CardTraitMagneticState_TooltipText_Verbose",
                        //        isStatusTooltip = false,
                        //        statusId = "",
                        //        isTriggerTooltip = false,
                        //        trigger = CharacterTriggerData.Trigger.OnDeath,
                        //        isTipTooltip = false,
                        //        style = TooltipDesigner.TooltipDesignType.Keyword
                        //    }
                        //}
                    },
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassType = typeof(RelicEffectAddTempUpgrade),
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamCharacterSubtype = "SubtypesData_None",
                        ParamExcludeCharacterSubtypes = new string[] {},
                        ParamCardUpgradeData = new CardUpgradeDataBuilder
                        {
                            UpgradeID = "MagneticForScourgeCards",
                            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                            {
                                new CardTraitDataBuilder
                                {
                                    TraitStateType = typeof(CustomCardTraitMagnetizedState),
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            {
                                new CardUpgradeMaskDataBuilder
                                {
                                    CardUpgradeMaskID = "ScourgeMagnetJunkFilter",
                                    CardType = CardType.Junk
                                }
                            }
                        }.Build(),
                        ParamTargetMode = TargetMode.FrontInRoom,
                    },
                },

                RelicLoreTooltipStyle = RelicData.RelicLoreTooltipStyle.Malicka,
                RequiredDLC = ShinyShoe.DLC.Hellforged,
            }.BuildAndRegister();

            //AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            //AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}

namespace Void.HarmonyPatches 
{
    [HarmonyPatch(typeof(CardManager), "AddCard")]
    public static class MakeBlightScourgeBounceToHand 
    {
        public static void Postfix(ref CardState __result, ref CardManager __instance, ref CardPile targetPile) 
        {
            if (ScourgeMagnet.HasIt()) 
            {
                if (__result == null)
                {
                    return;
                }

                if (targetPile != CardPile.HandPile) 
                {
                    if (__result.GetCardType() == CardType.Blight || __result.GetCardType() == CardType.Junk) 
                    {
                        HandUI.DrawSource drawSource = HandUI.DrawSource.Deck;
                        switch (targetPile) 
                        {
                            case CardPile.DiscardPile:
                                drawSource = HandUI.DrawSource.Discard;
                                break;
                            case CardPile.EatenPile:
                                drawSource = HandUI.DrawSource.Eaten;
                                break;
                            case CardPile.ExhaustedPile:
                                drawSource = HandUI.DrawSource.Consume;
                                break;
                            default:
                                break;                                
                        };

                        __instance.DrawSpecificCard(__result, 0.1f, drawSource, null, 1, 1);
                    }
                }
            }
        }
    }
}