using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ShinyShoe;
using Trainworks.Managers;
using Trainworks.Utilities;
using Trainworks.Builders;
using UnityEngine;
using Void.Init;
using System.IO;

//Note: This is now redundant with Trainwork's Mutator builder. Please use that instead of copying this internal builder.
namespace Void.Builders
{
    /// <summary>
    /// Mutators are custom modifiers that can be selected before starting a new run.
    /// Players, by default, can pick up to three.
    /// They function as relics are are built much the same way.
    /// </summary>
    public class MutatorDataBuilder
    {
        /// <summary>
        /// The ID overwrites the base GameObject ID. This needs to be unique.
        /// </summary>
        public string ID;

        /// <summary>
        /// Subdirectory and filename.
        /// ex: "Mutators/Sprite/MTR_GroupHug.png"
        /// <br></br>
        /// Icon should be 76x76 and color-coded so it can be visually identified as positive or negative.
        /// </summary>
        public string IconPath = "";

        /// <summary>
        /// Localization key.
        /// It's worth noting that the mutators list is sorted by the alphabetical order of the localized name.
        /// </summary>
        public string NameKey = "";

        /// <summary>
        /// Localization key.
        /// This describes how the mutator functions.
        /// </summary>
        public string DescriptionKey = "";

        /// <summary>
        /// The determines how the mutator behaves.
        /// <br></br>
        /// The existing Trainworks 'RelicEffectDataBuilder' can be used here; just remember to call the Build() function as it won't happen recursively.
        /// <br></br>
        /// This field defaults to 'RelicEffectNull'.
        /// </summary>
        public List<RelicEffectData> Effects = new List<RelicEffectData>
        {
            new RelicEffectDataBuilder
            {
                RelicEffectClassName = "RelicEffectNull"
            }.Build(),
        };

        /// <summary>
        /// Localization key(s).
        /// Mutators don't normally have lore tooltips, but if you define them, they will appear.
        /// </summary>
        public List<string> RelicLoreTooltipKeys = new List<string>() { };

        /// <summary>
        /// Tooltip design style for mutators. <br></br>
        /// Defaults to Herzal.
        /// </summary>
        public RelicData.RelicLoreTooltipStyle RelicLoreTooltipStyle = RelicData.RelicLoreTooltipStyle.Herzal; //Ponies.PonyRelicTooltip.GetEnum();

        /// <summary>
        /// Localization key.
        /// Some relic effects cause text to appear over units when activated that describes the effect.
        /// <br></br>
        /// Ex: '+5 [attack]'
        /// </summary>
        public string RelicActivatedKey = "";

        /// <summary>
        /// This isn't relavent to mutators. Should be left at 'false'.
        /// </summary>
        public bool DivineVariant = false;

        /// <summary>
        /// This typically ranges from -10 to +10.
        /// <br></br>
        /// Positive numbers indicate beneficial mutators, while negative numbers indicate harmful ones.
        /// <br></br>
        /// When a player clicks the 'randomize mutators' button, one positive, one negative, and one other mutator will be chosen. The total combined boon value will be -5 to -2.
        /// </summary>
        public int BoonValue = 0;

        /// <summary>
        /// When set to 'true', this mutator will never be picked randomly.
        /// </summary>
        public bool DisableInDailyChallenges = false;

        /// <summary>
        /// This determines whether the player needs the DLC to be active to use this mutator.
        /// <br></br>
        /// Defaults to 'DLC.None'.
        /// </summary>
        public DLC RequiredDLC = DLC.None;

        /// <summary>
        /// Tags are metadata that can place the mutator into different categories.
        /// <br></br>
        /// They are mutually exclusive, and no two tags will ever be the same when mutators are selected randomly.
        /// <br></br>
        /// ex: 'friendlyeffect'
        /// </summary>
        public List<String> Tags = new List<String> { };

        /// <summary>
        /// This creates a new MutatorData object.
        /// </summary>
        /// <returns>The newly-created MutatorData object.</returns>
        public MutatorData Build()
        {
            string fullIconPath = Path.Combine(Path.GetDirectoryName(Beyonder.Instance.Info.Location), IconPath);
            MutatorData mutatorData = ScriptableObject.CreateInstance<MutatorData>();
            if (!ID.IsNullOrEmpty())
            {
                AccessTools.Field(typeof(GameData), "id").SetValue(mutatorData, ID);
            }
            AccessTools.Field(typeof(MutatorData), "nameKey").SetValue(mutatorData, NameKey);
            AccessTools.Field(typeof(MutatorData), "descriptionKey").SetValue(mutatorData, DescriptionKey);
            Sprite icon = CustomAssetManager.LoadSpriteFromPath(fullIconPath);
            AccessTools.Field(typeof(MutatorData), "icon").SetValue(mutatorData, icon);
            AccessTools.Field(typeof(MutatorData), "effects").SetValue(mutatorData, Effects);
            AccessTools.Field(typeof(MutatorData), "relicLoreTooltipKeys").SetValue(mutatorData, RelicLoreTooltipKeys);
            AccessTools.Field(typeof(MutatorData), "relicLoreTooltipStyle").SetValue(mutatorData, RelicLoreTooltipStyle);
            AccessTools.Field(typeof(MutatorData), "relicActivatedKey").SetValue(mutatorData, RelicActivatedKey);
            AccessTools.Field(typeof(MutatorData), "divineVariant").SetValue(mutatorData, DivineVariant);
            AccessTools.Field(typeof(MutatorData), "boonValue").SetValue(mutatorData, BoonValue);
            AccessTools.Field(typeof(MutatorData), "disableInDailyChallenges").SetValue(mutatorData, DisableInDailyChallenges);
            AccessTools.Field(typeof(MutatorData), "requiredDLC").SetValue(mutatorData, RequiredDLC);
            AccessTools.Field(typeof(MutatorData), "tags").SetValue(mutatorData, Tags);
            return mutatorData;
        }

        /// <summary>
        /// This adds a MutatorData object to AllGameData.
        /// </summary>
        /// <param name="mutatorData"></param>
        /// <returns>False if the data has already been added or is null, true otherwise.</returns>
        public bool Register(MutatorData mutatorData)
        {
            if (mutatorData == null) { return false; }

            AllGameData allGameData = ProviderManager.SaveManager.GetAllGameData();
            List<MutatorData> mutatorDatas = (List<MutatorData>)AccessTools.Field(typeof(AllGameData), "mutatorDatas").GetValue(allGameData);

            if (!mutatorDatas.Contains(mutatorData))
            {
                mutatorDatas.Add(mutatorData);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Builds the MutatorData and adds it to AllGameData.
        /// </summary>
        /// <returns>The newly-built MutatorData.</returns>
        public MutatorData BuildAndRegister()
        {
            MutatorData mutatorData = this.Build();
            this.Register(mutatorData);
            return mutatorData;
        }
    }
}
