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
using Void.Champions;
using Void.Mania;
using Void.Spells;
using Void.Artifacts;
using Void.Triggers;
using Void.Monsters;
using Void.Enhancers;
using Void.Chaos;
using Void.CardPools;
using Void.Patches;
using Void.Mutators;
using Void.Tutorial;
using I2.Loc;

namespace Void.Init
{ 
    // Credit to Rawsome, Stable Infery for the base of this method.
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("MonsterTrain.exe")]
    [BepInProcess("MtLinkHandler.exe")]
    [BepInDependency("tools.modding.trainworks", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("ca.chronometry.disciple", BepInDependency.DependencyFlags.SoftDependency)]

    public class Beyonder : BaseUnityPlugin, IInitializable
    {
        public static Beyonder Instance { get; private set; }
        public static bool IsInit = false;
        public static ClassData BeyonderClanData;
        public static string BasePath;
        public static ScalingByAnxiety ScalingByAnxiety { get; private set; }
        public static ScalingByHysteria ScalingByHysteria { get; private set; }
        public static TrackManicCardsPlayed TrackManicCardsPlayed { get; private set; }
        public static TrackCardsByEntropic TrackCardsByEntropic { get; private set; }
        public static TrackAlliesSacrificed trackAlliesSacrificed { get; private set; }
        public static TrackSacrificedHP trackSacrificedHP { get; private set; }
        public static TrackSacrificedSize trackSacrificedSize { get; private set; }

        public const string GUID = "mod.beyonder.clan.monstertrain";
        public const string NAME = "Beyonder Clan";
        public const string VERSION = "0.9.0";

        public void Initialize()
        {
            //Unit subtypes
            SubtypeVeilrich.BuildAndRegister();
            Beyonder.Log("Subtype Veilritch");
            SubtypeUndretch.BuildAndRegister();
            Beyonder.Log("Subtype Undretch");

            //Clan
            BeyonderClanData = BeyonderClan.Buildclan();
            Beyonder.Log("Beyonder Clan");

            //Status Effects
            StatusEffectChronic.Build();
            Beyonder.Log("Chronic Status");
            StatusEffectFormless.Build();
            Beyonder.Log("Formless Status");
            StatusEffectJitters.Build();
            Beyonder.Log("Jitters Status");
            StatusEffectMutated.Build();
            Beyonder.Log("Mutated Status");
            StatusEffectPanic.Build();
            Beyonder.Log("Panic Status");
            StatusEffectShock.Build();
            Beyonder.Log("Shock Status");
            StatusEffectSoundless.Build();
            Beyonder.Log("Soundless Status");

            //Enum Extensions
            ScalingByAnxiety = new ScalingByAnxiety("ScalingByAnxietyKey");
            Beyonder.Log("ScalingByAnxiety Enum");
            ScalingByHysteria = new ScalingByHysteria("ScalingByHysteriaKey");
            Beyonder.Log("ScalingByHysteria Enum");
            TrackManicCardsPlayed = new TrackManicCardsPlayed("TrackManicCardsPlayedKey");
            Beyonder.Log("TrackManicCardsPlayed Enum");
            TrackCardsByEntropic = new TrackCardsByEntropic("TrackCardsByEntropicKey");
            Beyonder.Log("TrackCardsByEntropic Enum");
            trackAlliesSacrificed = new TrackAlliesSacrificed("TrackAlliesSacrificedKey");
            Beyonder.Log("TrackAlliesSacrificed Enum");
            trackSacrificedHP = new TrackSacrificedHP("TrackSacrificedHPKey");
            Beyonder.Log("TrackSacrificedHP Enum");
            trackSacrificedSize = new TrackSacrificedSize("TrackSacrificedSizeKey");
            Beyonder.Log("TrackSacrificedSize Enum");

            //Add some dynamic localization keys.
            CustomLocalizationManager.ImportSingleLocalization(typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName + "_TooltipText", "Text", "", "", "", "", "Each turn, this card will be drawn to your hand.", "Each turn, this card will be drawn to your hand.", "Each turn, this card will be drawn to your hand.", "Each turn, this card will be drawn to your hand.", "Each turn, this card will be drawn to your hand.", "Each turn, this card will be drawn to your hand.");
            CustomLocalizationManager.ImportSingleLocalization(typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName + "_CardText", "Text", "", "", "", "", "Stalker", "Stalker", "Stalker", "Stalker", "Stalker", "Stalker");
            CustomLocalizationManager.ImportSingleLocalization("Trigger_" + (int)Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum() + "_CardText", "Text", "", "", "", "", "Hysteria", "Hysteria", "Hysteria", "Hysteria", "Hysteria", "Hysteria");
            CustomLocalizationManager.ImportSingleLocalization("Trigger_" + (int)Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum() + "_TooltipText", "Text", "", "", "", "", "Triggers when <b>Mania</b> rises above 0.", "Triggers when <b>Mania</b> rises above 0.", "Triggers when <b>Mania</b> rises above 0.", "Triggers when <b>Mania</b> rises above 0.", "Triggers when <b>Mania</b> rises above 0.", "Triggers when <b>Mania</b> rises above 0.");
            CustomLocalizationManager.ImportSingleLocalization("Trigger_" + (int)Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum() + "_CardText", "Text", "", "", "", "", "Anxiety", "Anxiety", "Anxiety", "Anxiety", "Anxiety", "Anxiety");
            CustomLocalizationManager.ImportSingleLocalization("Trigger_" + (int)Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum() + "_TooltipText", "Text", "", "", "", "", "Triggers when <b>Mania</b> drops below 0.", "Triggers when <b>Mania</b> drops below 0.", "Triggers when <b>Mania</b> drops below 0.", "Triggers when <b>Mania</b> drops below 0.", "Triggers when <b>Mania</b> drops below 0.", "Triggers when <b>Mania</b> drops below 0.");

            //TestSpellCards.GiveIncantArmor.BuildAndRegister();
            //TestSpellCards.AddOnReserve.BuildAndRegister();

            //Starter Spell (base)
            MindScar.BuildAndRegister();
            Beyonder.Log("Mind Sear");

            //Starter Spell (exile)
            OcularInfection.BuildAndRegister();
            Beyonder.Log("Ocular Infection");

            //Common Cards (8 total)
            ThreeEyedFish.BuildAndRegister();
            Beyonder.Log("Three-Eyed Fish");
            Microaggression.BuildAndRegister();
            Beyonder.Log("Microaggression");
            SurvivalInstinct.BuildAndRegister();
            Beyonder.Log("Survival Instinct");
            MouthInMouth.BuildAndRegister();
            Beyonder.Log("Mouth In Mouth");
            SuctionCups.BuildAndRegister();
            Beyonder.Log("Suction Cups");
            IntoTheBeyond.BuildAndRegister();
            Beyonder.Log("Into the Beyond");
            EyeballsForDays.BuildAndRegister();
            Beyonder.Log("Eyeballs for Days");
            MassHysteria.BuildAndRegister();
            Beyonder.Log("Mass Hysteria");

            //Uncommon Cards (12 total)
            Phleghmbuyoancy.BuildAndRegister();
            Beyonder.Log("Phleghmbuyoancy");
            DisembodiedMaw.BuildAndRegister();
            Beyonder.Log("Disembodied Maw");
            PostItNoteOfForbiddenKnowledge.BuildAndRegister();
            Beyonder.Log("Forbidden Sticky Note");
            SupplementalDeadBrain.BuildAndRegister();
            Beyonder.Log("Supplemental Dead Brain");
            MentalDisorder.BuildAndRegister();
            Beyonder.Log("Mental Disorder");
            LookingStars.BuildAndRegister();
            Beyonder.Log("Looking Stars");
            DarkRecipe.BuildAndRegister();
            Beyonder.Log("Dark Recipe");
            Seizure.BuildAndRegister();
            Beyonder.Log("Seizure");
            Paranoia.BuildAndRegister();
            Beyonder.Log("Paranoia");
            PacingRut.BuildAndRegister();
            Beyonder.Log("Pacing Rut");
            HeebieJeebies.BuildAndRegister();
            Beyonder.Log("Heebie Jeebies");
            DesperateSearch.BuildAndRegister();
            Beyonder.Log("Desperate Search");

            //Rare Cards (8 total)
            SpikeFromBeyond.BuildAndRegister();
            Beyonder.Log("Spike From Beyond");
            CopingMechanism.BuildAndRegister();
            Beyonder.Log("Coping Mechanism");
            CerebralDetonation.BuildAndRegister();
            Beyonder.Log("Cerebral Detonation");
            BasketCase.BuildAndRegister();
            Beyonder.Log("Basket Case");
            EmbraceTheMadness.BuildAndRegister();
            Beyonder.Log("Embrace The Madness");
            EntropicStorm.BuildAndRegister();
            Beyonder.Log("Entropic Storm");
            ExistentialDread.BuildAndRegister();
            Beyonder.Log("Existential Dread");
            PrimordialSoup.BuildAndRegister();
            Beyonder.Log("Primordial Soup");
            Sociopathy.BuildAndRegister();
            Beyonder.Log("Sociopathy");

            //Boons and Banes
            ChaosManager.Init();
            Beyonder.Log("ChaosManager Init");

            //Load Save Data
            BeyonderSaveManager.LoadDataFromFile();
            Beyonder.Log("Loading Saved Data");

            //Uncommon Banner Units (6 total)
            FormlessHorror.BuildAndRegister();
            Beyonder.Log($"Formless Horror Version {ChaosManager.Vboons[FormlessHorror.VboonIndex]}.{ChaosManager.Vbanes[FormlessHorror.VbaneIndex]}");
            SoundlessSwarm.BuildAndRegister();
            Beyonder.Log($"Soundless Swarm Version {ChaosManager.Uboons[SoundlessSwarm.UboonIndex]}.{ChaosManager.Ubanes[SoundlessSwarm.UbaneIndex]}");
            Malevolence.BuildAndRegister();
            Beyonder.Log($"Malevolence Version x.{ChaosManager.Vbanes[Malevolence.VbaneIndex]}");
            HairyPotty.BuildAndRegister();
            Beyonder.Log($"Hairy Potty Version {ChaosManager.Uboons[HairyPotty.UboonIndex]}.x");
            FurryBeholder.BuildAndRegister();
            Beyonder.Log($"Furry Beholder Version x.{ChaosManager.Ubanes[FurryBeholder.UbaneIndex]}");
            Vexation.BuildAndRegister();
            Beyonder.Log($"Vexation Version {ChaosManager.Vboons[Vexation.VboonIndex]}.x");

            //Rare Banner Units (3 total)
            Deathwood.BuildAndRegister();
            Beyonder.Log("Deathwood");
            Chutzpah.BuildAndRegister();
            Beyonder.Log("Chutzpah");
            ApostleoftheVoid.BuildAndRegister();
            Beyonder.Log("Apostle of the Void");

            //Card Pools
            BeyonderCardPools.BuildCardPools();
            Beyonder.Log("Beyonder Card Pool Setup");

            //Enhancers
            Riftstone.BuildAndRegister();
            Beyonder.Log("Riftstone Unit Enhancer");
            Veilstone.BuildAndRegister();
            Beyonder.Log("Veilstone Spell Enhancer");
            Voidstone.BuildAndRegister();
            Beyonder.Log("Voidstone Spell Enhancer");
            Sanitystone.BuildAndRegister();
            Beyonder.Log("Bafflestone Spell Enhancer");

            //Artifacts (11 total) +1 starter
            UnSeeingEye.BuildAndRegister(); //starter
            Beyonder.Log("The Unseeing Eye");
            TearInReality.BuildAndRegister();
            Beyonder.Log("Warped Reality");
            BrainBleach.BuildAndRegister();
            Beyonder.Log("Brain Bleach");
            BrainClamps.BuildAndRegister();
            Beyonder.Log("Brain Clamps");
            BedMonster.BuildAndRegister();
            Beyonder.Log("Bed Monster");
            VialOfBlackEyedBlood.BuildAndRegister();
            Beyonder.Log("Vial of Black-Eyed Blood");
            LivingEntropy.BuildAndRegister();
            Beyonder.Log("Living Entropy");
            LingeringChaos.BuildAndRegister();
            Beyonder.Log("Collection of Tentacles");
            RadioactiveWaste.BuildAndRegister();
            Beyonder.Log("Radioactive Waste");
            EyevoryEyedol.BuildAndRegister();
            Beyonder.Log("Eyevory Eyedol");
            UnstableEnergy.BuildAndRegister();
            Beyonder.Log("Unstable Energy");
            BloodyTentacles.BuildAndRegister();
            Beyonder.Log("Bloody Tentacles");

            //Malica Other Clan Relics
            BlackLight.BuildAndRegister(); //Beyonder
            Beyonder.Log("Black Light");
            GenieImp.BuildAndRegister(); //Hellhorned
            Beyonder.Log("Impspector Gadget");
            FasciatedKernels.BuildAndRegister(); //Awoken
            Beyonder.Log("FasciatedKernels");
            MalickasGift.BuildAndRegister(); //Stygian
            Beyonder.Log("Malicka's Gift");
            ShallowGraves.BuildAndRegister(); //Melting Remnant
            Beyonder.Log("Shallow Graves");
            ShadowPuppeteer.BuildAndRegister(); //Umbra
            Beyonder.Log("Shadow Puppeteer");
            BadEggs.BuildAndRegister(); //Wurmkin
            Beyonder.Log("Bad Eggs");
            PurloinedHeavensSeal.BuildAndRegister(); //Arcadian
            Beyonder.Log("[Arcadian] Purloined Heaven's Seal");
            Preservatives.BuildAndRegister(); //Sweetkin
            Beyonder.Log("[Sweetkin] Preservatives");
            //MemoryJewel.BuildAndRegister(); //Equestrian (Must be loaded later due to Equestrian's late initialization.)
            //Beyonder.Log("[Equestrian] Memory Jewel");
            ScourgeMagnet.BuildAndRegister(); //Succubus
            Beyonder.Log("[Succubus] Scourge Magnet");

            //Champions
            LocoMotive.BuildAndRegister();
            Beyonder.Log("Base Champion Loco Motive");
            Epidemial.BuildAndRegister();
            Beyonder.Log("Exile Chapion Epidemial");

            //Banner
            BeyonderBanner.buildbanner();
            Beyonder.Log("Clan banner");

            //ManiaUI
            //This inits as needed.

            //Default Unit Synthesis (Units with dynamic synthesis are handled differently.)
            AccessTools.Field(typeof(UnitSynthesisMapping), "_dictionaryMapping").SetValue(ProviderManager.SaveManager.GetBalanceData().SynthesisMapping, null);
            Trainworks.Patches.AccessUnitSynthesisMapping.FindUnitSynthesisMappingInstanceToStub();
            Beyonder.Log("Trainworks Unit Synthesis Patch");

            //Mutators
            FirstLaugh.BuildAndRegister();
            Beyonder.Log("First Laugh Mutator");

            //Misc stuff
            MakeChildFormless.DoIt();
            Beyonder.Log("Formless Child is Formless now.");
            DoNotDoublestackMutations.JustDont();
            Beyonder.Log("Exclude Mutations from Doublestack.");

            TutorialManager.LoadProgress();
            Beyonder.Log("Loading tutorial progress.");

            //Signifies that the clan's data is loaded and ready to use.
            IsInit = true;
            Beyonder.Log("The Beyonder Clan has initialized successfully.");
        }

        private void Awake()
        {
            Beyonder.Instance = this;
            BasePath = Path.GetDirectoryName(Instance.Info.Location);

            var harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        public static void Log(string message, BepInEx.Logging.LogLevel level = BepInEx.Logging.LogLevel.Info)
        {
            Beyonder.Instance.Logger.Log(level, message);
        }

        public static void LogError(string message)
        {
            Beyonder.Instance.Logger.LogError(message);
        }
    }
}

//Bug list: Consume spells were not properly triggering artifact that deal 30 damage to front unit. Likely related to floor shift on triggered (Hysteria/Anxiety) effects.
//Found in run: {"RunID":"213bce2c-c1ec-4460-ae00-6489eea4fc7e","ForceSeed":false,"BeyonderVersion":"0.8.1","LocoMotiveConductorPath":2,"LocoMotiveHorrorPath":2,"LocoMotiveFormlessPath":0,"EpidemialInnumerablePath":2,"EpidemialContagiousPath":2,"EpidemialSoundlessPath":0,"VboonIndexes":[2,1,0,3,7,9,6,4,8,5],"VbaneIndexes":[8,2,1,7,0,6,5,9,3,4],"UboonIndexes":[3,8,6,2,9,0,7,1,4,5],"UbaneIndexes":[9,0,5,7,3,2,8,1,6,4],"StartingConditions":{"seed":113526943,"isBattleMode":false,"isFtueRun":false,"battleModeStartTime":"","battleModeTimerScalar":0.0,"battleWarmup":0,"version":"12923","mainClassInfo":{"className":"55bcaaa0-6e62-4b91-81c9-c44e93f9b0e6","classLevel":10,"championIndex":1,"random":false},"subclassInfo":{"className":"fda62ada-520e-42f3-aa88-e4a78549c4a2","classLevel":10,"championIndex":1,"random":true},"ascensionLevel":25,"spChallengeId":"","covenants":["a498b6c7-a094-48c4-8f99-8e3a2d2cc35b","0dd4f35c-010e-4455-bd7a-7d801d128027","cd0cccc6-70d7-4012-8bd4-72f32ee0a444","ac6a3426-b233-44b7-a221-7eedd7a5b046","e13fa3da-6dc0-40a3-942a-1f4c1863a9e4","2d48cc06-c584-4f1a-be83-06831591fe29","10247c18-57e3-40f2-89ac-2048c7b2dfc9","5d03474a-10e7-46db-abc1-90b5e53f400a","19305ab8-a44b-45a9-926f-1ee70912d7d1","bd5841ce-ead5-4d95-ba0c-a4a220b6d245","d639494a-9557-481a-91b7-d5f3822628da","21c51e96-3281-4949-a6f6-72d7d189c232","8c52be61-d238-45cc-b186-291e396f31e5","a0b06fe6-b736-48cd-b5c8-b7f6c892f4e8","f4ca28b4-9321-49fc-b15e-092d17996f29","cfa8ef9e-2a65-4b8d-ad38-41ad90154563","fd1071c8-63d7-4bb4-9b6a-ac4a8090dfbe","15ca2a42-1102-43d9-b438-4281f0ff2c2e","7f35fbac-8dfb-470c-b445-661b0eae3e74","2f17e948-0197-4416-8b42-5c32297f1bb6","bcfedf6f-fe28-4950-a8a2-787bf56df238","f1be8eda-7b00-4322-ac8c-f1ece7601c9e","29019224-00ef-4e32-9136-655375eda84a","54f2540e-1351-41c1-8c44-e26a07199cd7","6ec123f0-9ca5-4bf8-b68c-77f208f3f075"],"mutators":[],"enabledDlcs":[1]}}