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
    public static class MemoryJewel
    {
        public static CollectableRelicData Artifact;
        public static string ID = "MemoryJewel_" + Beyonder.GUID;

        public static bool TryGetClassData(out ClassData clan)
        {
            clan = null;

            if (!Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("mod.equestrian.clan.monstertrain"))
            {
                return false;
            }

            clan = CustomClassManager.GetClassDataByID("Equestrian_Clan");

            if (clan == null)
            {
                Beyonder.LogError("Equestrian detected but failed to find class data.");
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
                NameKey = "Malicka_Artifact_MemoryJewel_Name_Key",
                DescriptionKey = "Malicka_Artifact_MemoryJewel_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_MemoryJewel_Lore_Key"
                },
                IconPath = "ArtifactAssets/MemoryJewel.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                LinkedClass = clan,
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "CollectableRelicData_relicActivatedKey-2636bcff7875433f-294d815b112c64b408a218944e56bb37-v2",

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = typeof(CustomRelicEffectEnergyGainAndCardDrawOnFrozenUnitInHand).AssemblyQualifiedName,
                        ParamInt = 1,
                        ParamCardType = CardType.Monster,
                        TargetCardTraitParam = typeof(CardTraitFreeze).AssemblyQualifiedName,
                        ParamSourceTeam = Team.Type.Monsters,
                        AdditionalTooltips = new AdditionalTooltipData[] 
                        {
                            new AdditionalTooltipData
                            { 
                                titleKey = string.Empty, //"Malicka_Artifact_MemoryJewel_Hint_Title_Key",
                                descriptionKey = "Malicka_Artifact_MemoryJewel_Hint_Description_Key",
                                isStatusTooltip = false,
                                statusId = "",
                                isTriggerTooltip = false,
                                trigger = CharacterTriggerData.Trigger.OnDeath,
                                isTipTooltip = true,
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