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

namespace Void.Artifacts
{
    public static class ShallowGraves
    {
        public static CollectableRelicData Artifact;
        public static string ID = "ShallowGraves_" + Beyonder.GUID;

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
                ClanID = VanillaClanIDs.Hellhorned,
                RelicPoolIDs = new List<string> { VanillaRelicPoolIDs.MegaRelicPool },
                NameKey = "Malicka_Artifact_ShallowGraves_Name_Key",
                DescriptionKey = "Malicka_Artifact_ShallowGraves_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_ShallowGraves_Lore_Key"
                },
                IconPath = "ArtifactAssets/ShallowGraves.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 0,
                LinkedClass = CustomClassManager.GetClassDataByID(VanillaClanIDs.MeltingRemnant),
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "Malicka_Artifact_ShallowGraves_Activated_Key",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = typeof(CustomRelicEffectConsumeAllCardsOfTypeAndAddBattleCardAtStartOfBattle).AssemblyQualifiedName,
                        ParamCardType = CardType.Monster,
                        ParamInt = 1,
                        ParamCharacterSubtype = "SubtypesData_Champion_83f21cbe-9d9b-4566-a2c3-ca559ab8ff34",
                        ParamCardPool = new CardPoolBuilder
                        {
                            CardPoolID = "HallowedHallsOnlyCardPool",
                            CardIDs = new List<string>
                            {
                                "c6484604-b077-43ce-84a4-0179d2f36352"
                            }
                        }.Build(),
                        AdditionalTooltips = new AdditionalTooltipData[] 
                        {
                            new AdditionalTooltipData
                            { 
                                titleKey = "Malicka_Artifact_ShallowGraves_Tip_Title_Key",
                                descriptionKey = "Malicka_Artifact_ShallowGraves_Tip_Description_Key",
                                isStatusTooltip = false,
                                statusId = "",
                                isTipTooltip = true,
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                style = TooltipDesigner.TooltipDesignType.Default
                            }
                        }
                    }
                },
            }.BuildAndRegister();

            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(Artifact, RelicData.RelicLoreTooltipStyle.Malicka);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(Artifact, ShinyShoe.DLC.Hellforged);

            return Artifact;
        }
    }
}