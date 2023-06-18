using Trainworks.Managers;
using Trainworks.Builders;
using Trainworks.Constants;
using Void.Init;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using HarmonyLib;
using ShinyShoe;
using UnityEngine;
using CustomEffects;
using Void.Builders;

namespace Void.Mutators
{
    public static class FirstLaugh
    {
        public static string ID = Beyonder.GUID + "_FirstLaugh";
        public static MutatorData mutatorData;

        public static void BuildAndRegister()
        {
            mutatorData = new MutatorDataBuilder
            {
                ID = ID,
                NameKey = "Beyonder_Mutator_FirstLaugh_Name_Key",
                DescriptionKey = "Beyonder_Mutator_FirstLaugh_Description_Key",
                RelicActivatedKey = "Beyonder_Mutator_FirstLaugh_Activated_Key",
                RelicLoreTooltipKeys = new List<string>()
                {
                    "Beyonder_Mutator_FirstLaugh_Lore_Key"
                },
                DisableInDailyChallenges = false,
                DivineVariant = false,
                BoonValue = 5,
                RequiredDLC = DLC.None,
                IconPath = "Mutators/Sprite/MTR_FirstLaugh.png",
                Tags = new List<string>
                {
                    "pyrefragile"
                },
                Effects = new List<RelicEffectData>
                {
                    new RelicEffectDataBuilder
                    {
                        RelicEffectClassName = typeof(CustomRelicEffectNoPyreDamageEarly).AssemblyQualifiedName,
                        ParamSourceTeam = Team.Type.Monsters,
                        ParamTargetMode = TargetMode.Room,
                        ParamBool = false,
                        ParamCharacterSubtype = "SubtypesData_Pyre",
                        ParamInt = 2,
                    }.Build(),
                }
            }.BuildAndRegister();
        }
    }
}