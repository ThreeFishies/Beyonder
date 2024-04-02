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
using Void.Arcadian;
using Void.BeyonderStory;
using I2.Loc;
using Equestrian.Metagame;

namespace Void.Init
{ 
    // Credit to Rawsome, Stable Infery for the base of this method.
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("MonsterTrain.exe")]
    [BepInProcess("MtLinkHandler.exe")]
    [BepInDependency("tools.modding.trainworks", BepInDependency.DependencyFlags.HardDependency)]
    //Dependencies are unlikely to be needed at this point.
    //[BepInDependency("ca.chronometry.disciple", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("Exas4000", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("com.name.package.succclan-mod", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("mod.equestrian.clan.monstertrain", BepInDependency.DependencyFlags.SoftDependency)]

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
        public static EntropicCardSelectionError EntropicCardSelectionError { get; private set; }
        public static HoldoverCardSelectionError HoldoverCardSelectionError { get; private set; }
        public static CardNotSpawnerSelectionError CardNotSpawnerSelectionError { get; private set; }

        public const string GUID = "mod.beyonder.clan.monstertrain";
        public const string NAME = "Beyonder Clan";
        public const string VERSION = "0.9.9";

        public void Initialize()
        {
            //Import localization
            CustomLocalizationManager.ImportCSV("Localization/InfiniteVoid.csv", ',');

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
            EntropicCardSelectionError = new EntropicCardSelectionError("EntropicCardSelectionErrorKey");
            EntropicCardSelectionError.Initialize();
            Beyonder.Log("Entropic Card Selection Error Enum");
            HoldoverCardSelectionError = new HoldoverCardSelectionError("HoldoverCardSelectionErrorKey");
            HoldoverCardSelectionError.Initialize();
            Beyonder.Log("Holdover Card Selection Error Enum");
            CardNotSpawnerSelectionError = new CardNotSpawnerSelectionError("CardNotSpawnerSelectionErrorKey");
            CardNotSpawnerSelectionError.Initialize();
            Beyonder.Log("Card Not Spawner Selection Error Enum");

            //Add some dynamic localization keys.
            ChaosLocalizationManager.Queue.Add(typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName + "_TooltipText", "Each turn, this card will be drawn to your hand.");
            ChaosLocalizationManager.Queue.Add(typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName + "_CardText", "Stalker");
            ChaosLocalizationManager.Queue.Add("Trigger_" + (int)Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum() + "_CardText", "Hysteria");
            ChaosLocalizationManager.Queue.Add("Trigger_" + (int)Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum() + "_TooltipText", "Triggers when <b>Mania</b> rises above 0.");
            ChaosLocalizationManager.Queue.Add("Trigger_" + (int)Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum() + "_CardText", "Anxiety");
            ChaosLocalizationManager.Queue.Add("Trigger_" + (int)Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum() + "_TooltipText", "Triggers when <b>Mania</b> drops below 0.");

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
            CaveofaThousandEyes.BuildAndRegister(); //for cavern event reward
            Beyonder.Log("Cave of a Thousand Eyes");

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
            //UnstableEnergy.BuildAndRegister();
            //Beyonder.Log("Unstable Energy");
            SeedOfDoubt.BuildAndRegister();
            Beyonder.Log("Seed of Doubt");
            BloodyTentacles.BuildAndRegister();
            Beyonder.Log("Bloody Tentacles");

            //Malica Other Clan Relics
            BlackLight.BuildAndRegister(); //Beyonder
            Beyonder.Log("Black Light");
            ImpspectorGadget.BuildAndRegister(); //Hellhorned
            Beyonder.Log("Impspector Gadget");
            FasciatedKernels.BuildAndRegister(); //Awoken
            Beyonder.Log("Fasciated Kernels");
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
            //Trainworks.Patches.AccessUnitSynthesisMapping.FindUnitSynthesisMappingInstanceToStub();
            //Beyonder.Log("Trainworks Unit Synthesis Patch");

            //Mutators
            FirstLaugh.BuildAndRegister();
            Beyonder.Log("First Laugh Mutator");
            PettyVengeance.BuildAndRegister();
            Beyonder.Log("Petty Vengeance Mutator");
            MadnessWithin.BuildAndRegister();
            Beyonder.Log("Madness Within Mutator");
            RestlessBeast.BuildAndRegister();
            Beyonder.Log("Restless Beast Mutator");

            //Challenges
            TruestChampion.BuildAndRegister();
            Beyonder.Log("Truest Champion spChallenge");

            //Cave Story
            CaveStory.EditMasterStoryFile();
            Beyonder.Log("Modifying Master Story File");
            CaveStory.BuildEventData();
            Beyonder.Log("Registering Event Reward Data");

            //Misc stuff
            MakeChildFormless.DoIt();
            Beyonder.Log("Formless Child is Formless now.");
            DoNotDoublestackMutations.JustDont();
            Beyonder.Log("Exclude Mutations from Doublestack.");
            Beyonder.Log("Checking Arcadian Compatibility.");
            ArcadianCompatibility.Initialize();
            RegisterTMPSprites.SetupSprites();

            PonyMetagame.LoadPonyMetaFile();
            TutorialManager.LoadProgress();
            Beyonder.Log("Loading tutorial progress.");

            //UpdateRewardCountTest.AwokenHerzalTestReward();
            //Beyonder.Log("*** Testing more rewards for Awoken banner and Herzal's Hoards. ***");

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