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
using System;

namespace Void.Champions
{
    [Serializable]
    public struct LocoMotivePathData
    {
        [SerializeField]
        public int ConductorTreePathID;
        [SerializeField]
        public int HorrorTreePathID;
        [SerializeField]
        public int FormlessTreePathID;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public static class LocoMotive
    {
        public static CharacterDataBuilder character;
        public static CardData card;
        public static string ID = Beyonder.GUID + "_LocoMotive_Card";
        public static string CharID = Beyonder.GUID + "_LocoMotive_Character";
        public const string SavePath = "SaveData/CurrentRun/LocoMotiveUpgradeTree.json";
        //public static int MutatorTreeRngPath = 0; //wasn't working right.
        public static int ConductorTreeRngPath = 0;
        public static int FormlessTreeRngPath = 0;
        public static int HorrorTreeRngPath = 0;

        public static void BuildAndRegister()
        {
            character = new CharacterDataBuilder
            {
                Name = "Loco Motive",
                NameKey = "Beyonder_Champ_LocoMotive_Name_Key",
                AttackDamage = 10,
                Health = 20,
                Size = 3,
                CharacterID = CharID,

                //CharacterChatterData = null,
                AssetPath = "Monsters/Assets/Loco_Motive_Monster.png",
            };

            card = new ChampionCardDataBuilder
            {
                ClanID = Beyonder.BeyonderClanData.GetID(),
                CardID = ID,
                StarterCardData = MindScar.Card,
                ChampionSelectedCue = "Multiplayer_Emote_Lol",
                //OverrideDescriptionKey = "Beyonder_Champ_LocoMotive_Description_Key",
                Champion = character,
                AssetPath = "Monsters/Assets/Loco_Motive_Card.png",
                ChampionIconPath = "Monsters/Assets/Loco_Motive_Card.png",
                CardLoreTooltipKeys = new List<string> 
                { 
                    "Beyonder_Champ_LocoMotive_Lore_Key" 
                },
                CardPoolIDs = new List<string> 
                { 
                    VanillaCardPoolIDs.ChampionPool
                },

                UpgradeTree = new CardUpgradeTreeDataBuilder
                {
                    UpgradeTrees = new List<List<CardUpgradeDataBuilder>>
                    {
                        new List<CardUpgradeDataBuilder>
                        {
                            Conductor(0,false),
                            Conductor(1,false),
                            Conductor(2,false)
                        },
                        new List<CardUpgradeDataBuilder>
                        {
                            Horror(0,false),
                            Horror(1,false),
                            Horror(2,false)
                        },
                        new List<CardUpgradeDataBuilder>
                        {
                            Formless(0,false),
                            Formless(1,false),
                            Formless(2,false)
                        }
                    }
                }
            }.BuildAndRegister(0);
        }

        public static void BuildTreeForNewRun(RngId rngId = RngId.SetupRun, bool ShouldRandomize = true)
        {
            ChampionData championData = Beyonder.BeyonderClanData.GetChampionData(0);
            championData.upgradeTree = new CardUpgradeTreeDataBuilder
            { 
                Champion = championData.championCardData.GetSpawnCharacterData(),
                UpgradeTrees = new List<List<CardUpgradeDataBuilder>> 
                {
                    new List<CardUpgradeDataBuilder>
                    { 
                        Conductor(0,ShouldRandomize,rngId),
                        Conductor(1,false),
                        Conductor(2,false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Horror(0,ShouldRandomize,rngId),
                        Horror(1,false),
                        Horror(2,false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Formless(0,ShouldRandomize,rngId),
                        Formless(1,false),
                        Formless(2,false)
                    },
                }
            }.Build();

            //SaveTreeData(SavePath);
        }

        public static void BuildTreeFromData(LocoMotivePathData pathData)
        {
            SetupTrees(pathData.ConductorTreePathID, pathData.HorrorTreePathID, pathData.FormlessTreePathID);

            if (!Beyonder.IsInit) { return; }

            ChampionData championData = Beyonder.BeyonderClanData.GetChampionData(0);
            championData.upgradeTree = new CardUpgradeTreeDataBuilder
            {
                Champion = championData.championCardData.GetSpawnCharacterData(),
                UpgradeTrees = new List<List<CardUpgradeDataBuilder>>
                {
                    new List<CardUpgradeDataBuilder>
                    {
                        Conductor(0,false),
                        Conductor(1,false),
                        Conductor(2,false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Horror(0,false),
                        Horror(1,false),
                        Horror(2,false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Formless(0,false),
                        Formless(1,false),
                        Formless(2,false)
                    },
                }
            }.Build();

            //SaveTreeData(SavePath);
        }

        public static LocoMotivePathData GetPathTreeData() 
        {
            return new LocoMotivePathData
            {
                ConductorTreePathID = ConductorTreeRngPath,
                HorrorTreePathID = HorrorTreeRngPath,
                FormlessTreePathID = FormlessTreeRngPath,
            };
        }

        /*
        public static void SaveTreeData(string SaveFilePath)
        {
            string path = Path.Combine(Beyonder.BasePath,SaveFilePath);

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch 
                {
                    Beyonder.LogError("Removing previous Loco Motive Upgrade Tree Data Failed.");
                    return;
                }
            }

            LocoMotivePathData pathData = new LocoMotivePathData() 
            {
                ConductorTreePathID = ConductorTreeRngPath,
                HorrorTreePathID = HorrorTreeRngPath,
                FormlessTreePathID = FormlessTreeRngPath,
            };

            try
            {
                File.WriteAllText(path, JsonUtility.ToJson(pathData));
            }
            catch
            {
                Beyonder.LogError("Saving Loco Motive Upgrade Tree Data Failed.");
                return;
            }
        }

        public static void LoadTreeData(string SaveFilePath)
        {
            string path = Path.Combine(Beyonder.BasePath, SaveFilePath);

            if (File.Exists(path)) 
            {
                try
                {
                    string data = File.ReadAllText(path);
                    LocoMotivePathData pathData = JsonUtility.FromJson<LocoMotivePathData>(data);
                    SetupTrees(pathData.ConductorTreePathID, pathData.HorrorTreePathID, pathData.FormlessTreePathID);
                }
                catch
                {
                    Beyonder.Log("Failed to load Loco Motive Upgrade Tree Pata Data from file. Using default data.", BepInEx.Logging.LogLevel.Warning);
                }
            }
        }
        */

        public static void SetupTrees(int ConductorTreePath = 0, int HorrorTreePath = 0, int FormlessTreePath = 0)
        {
            ConductorTreeRngPath = ConductorTreePath;
            HorrorTreeRngPath = HorrorTreePath;
            FormlessTreeRngPath = FormlessTreePath;
        }

        /*
        public static CardUpgradeData GetMutatorATriggeredEffect(int upgradeLevel)
        {
            int bonusDamage = 5 + 5 * upgradeLevel + upgradeLevel > 1 ? 5 : 0;

            CardUpgradeData upgradeData = new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = $"MutatorA{upgradeLevel}TriggeredEffect",
                BonusDamage = bonusDamage,
                SourceSynthesisUnit = new CharacterDataBuilder 
                {
                    CharacterID = $"MutatorA{upgradeLevel}FakeCharacterUnitToFoolSynthesis"
                }.Build(),
            }.Build();

            return upgradeData;
        }

        public static CardUpgradeData GetMutatorCTriggeredEffect(int upgradeLevel)
        {
            int bonusDamage = 5 + 5 * upgradeLevel + upgradeLevel > 1 ? 5 : 0;

            CardUpgradeData upgradeData = new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = $"MutatorC{upgradeLevel}TriggeredEffect",
                BonusDamage = bonusDamage * -1,
                SourceSynthesisUnit = new CharacterDataBuilder
                {
                    CharacterID = $"MutatorC{upgradeLevel}FakeCharacterUnitToFoolSynthesis"
                }.Build(),
            }.Build();

            return upgradeData;
        }

        public static CardUpgradeDataBuilder Mutator(int upgradeLevel, bool randomize = false, RngId rngId = RngId.NonDeterministic)
        {
            if (randomize)
            {
                List<int> list = new List<int>(3) { 0, 1, 2 };
                list.Shuffle(rngId);
                MutatorTreeRngPath = list[0];
            }

            Beyonder.Log($"Building Mutator Variant Path: {MutatorTreeRngPath} at upgrade level: {upgradeLevel}.");

            //variant A {OnHysteria: +Jitters, +Attack, Mutated}
            if (MutatorTreeRngPath == 0)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 0 + upgradeLevel * 20 + (upgradeLevel > 1 ? 20 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Mutator_{upgradeLevel}_TitleKey",

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.AfterSpawnEnchant,
                            DescriptionKey = $"Beyonder_Champ_LocoMotive_Mutator_A_{upgradeLevel}_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectAddTempCardUpgradeToUnits).AssemblyQualifiedName,
                                    ParamBool = false,
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetMode = TargetMode.Room,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                                        {
                                            new CharacterTriggerDataBuilder
                                            {
                                                Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                                                DescriptionKey = "Beyonder_Champ_LocoMotive_Mutator_A_AppliedDescriptionKey",
                                                EffectBuilders = new List<CardEffectDataBuilder>
                                                {
                                                    new CardEffectDataBuilder
                                                    {
                                                        EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                                        TargetCardType = CardType.Monster,
                                                        TargetMode = TargetMode.Self,
                                                        TargetTeamType = Team.Type.Monsters,
                                                        ParamCardUpgradeData = GetMutatorATriggeredEffect(upgradeLevel),
                                                    },
                                                    new CardEffectDataBuilder
                                                    {
                                                        EffectStateName = "CardEffectAddStatusEffect",
                                                        TargetCardType = CardType.Monster,
                                                        TargetMode = TargetMode.Self,
                                                        TargetTeamType = Team.Type.Monsters,
                                                        ParamStatusEffects = new StatusEffectStackData[]
                                                        {
                                                            new StatusEffectStackData
                                                            {
                                                                statusId = StatusEffectJitters.statusId,
                                                                count = 1,
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        StatusEffectUpgrades = new List<StatusEffectStackData>
                                        {
                                            new StatusEffectStackData
                                            {
                                                statusId = StatusEffectMutated.statusId,
                                                count = 1,
                                            }
                                        }
                                    }.Build(),
                                },
                            }
                        }
                    },
                };
            }

            //variant B {OnStrike: +Jitters, Mutated}
            if (MutatorTreeRngPath == 1)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 0 + upgradeLevel * 20 + upgradeLevel > 1 ? 20 : 0,
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Mutator_{upgradeLevel}_TitleKey",

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.AfterSpawnEnchant,
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Mutator_B_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectAddTempCardUpgradeToUnits).AssemblyQualifiedName,
                                    ParamBool = false,
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetMode = TargetMode.Room,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                                        {
                                            new CharacterTriggerDataBuilder
                                            {
                                                Trigger = CharacterTriggerData.Trigger.OnAttacking,
                                                DescriptionKey = "Beyonder_Champ_LocoMotive_Mutator_B_AppliedDescriptionKey",
                                                EffectBuilders = new List<CardEffectDataBuilder>
                                                {
                                                    new CardEffectDataBuilder
                                                    {
                                                        EffectStateName = "CardEffectAddStatusEffect",
                                                        TargetMode = TargetMode.LastAttackedCharacter,
                                                        TargetTeamType = Team.Type.Heroes,
                                                        ParamStatusEffects = new StatusEffectStackData[]
                                                        {
                                                            new StatusEffectStackData
                                                            {
                                                                statusId = StatusEffectJitters.statusId,
                                                                count = 3 + 3 * upgradeLevel + upgradeLevel > 1 ? 3 : 0,
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }.Build(),
                                }
                            }
                        }
                    }
                };
            }

            //variant C {+Attack, OnHysteria: -Attack, Mutated}
            if (MutatorTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 0 + upgradeLevel * 20 + upgradeLevel > 1 ? 20 : 0,
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Mutator_{upgradeLevel}_TitleKey",

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.AfterSpawnEnchant,
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Mutator_C_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectAddTempCardUpgradeToUnits).AssemblyQualifiedName,
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetMode = TargetMode.Room,
                                    ParamBool = false,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        BonusDamage = 30 + upgradeLevel * 30 + upgradeLevel > 1 ? 30 : 0,
                                        TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                                        {
                                            new CharacterTriggerDataBuilder
                                            {
                                                Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                                                DescriptionKey = "Beyonder_Champ_LocoMotive_Mutator_C_AppliedDescriptionKey",
                                                EffectBuilders = new List<CardEffectDataBuilder>
                                                {
                                                    new CardEffectDataBuilder
                                                    {
                                                        EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                                        TargetCardType = CardType.Monster,
                                                        TargetMode = TargetMode.Self,
                                                        TargetTeamType = Team.Type.Monsters,
                                                        ParamCardUpgradeData = GetMutatorCTriggeredEffect(upgradeLevel),
                                                    }
                                                }
                                            }
                                        },
                                        StatusEffectUpgrades = new List<StatusEffectStackData>
                                        {
                                            new StatusEffectStackData
                                            {
                                                statusId = StatusEffectMutated.statusId,
                                                count = 1,
                                            }
                                        }
                                    }.Build(),
                                },
                            }
                        }
                    },
                };
            }

            return null;
        }
        */

        public static CardUpgradeDataBuilder Conductor(int upgradeLevel, bool randomize = false, RngId rngId = RngId.NonDeterministic)
        {
            if (randomize)
            {
                List<int> list = new List<int>(3) { 0, 1, 2 };
                list.Shuffle(rngId);
                ConductorTreeRngPath = list[0];
                //ConductorTreeRngPath = 2;
            }

            Beyonder.Log($"Building Conductor Variant Path: {ConductorTreeRngPath} at upgrade level: {upgradeLevel}.");

            //variant A {Mutated and OnHysteria: +Attack, +Jitters}
            if (ConductorTreeRngPath == 0)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 15 + (upgradeLevel * 25) + (upgradeLevel > 1 ? 25 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Conductor_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectMutated.statusId,
                            count = 1,
                        }
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Conductor_A_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                { 
                                    EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                                    TargetModeStatusEffectsFilter = new string[]
                                    {
                                        StatusEffectMutated.statusId,
                                    },
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    { 
                                        UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Conductor_A_Triggered_effect_{upgradeLevel}",
                                        BonusDamage = 3 + (upgradeLevel * 3) + (upgradeLevel > 1 ? 3 : 0),
                                        BonusHP = 0,
                                        //FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                                        //{
                                        //    new CardUpgradeMaskDataBuilder
                                        //    { 
                                        //        RequiredStatusEffects = new List<StatusEffectStackData>
                                        //        {
                                        //            new StatusEffectStackData
                                        //            {
                                        //                statusId = StatusEffectMutated.statusId,
                                        //                count = 1,
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                    }.Build(),
                                },
                                new CardEffectDataBuilder
                                { 
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                                    TargetModeStatusEffectsFilter = new string[] 
                                    {
                                        StatusEffectMutated.statusId,
                                    },
                                    ParamStatusEffects = new StatusEffectStackData[] 
                                    {
                                        new StatusEffectStackData
                                        { 
                                            statusId = StatusEffectJitters.statusId,
                                            count = 1,
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }

            //variant B {Mutated and OnResolve: +Attack, -Health}
            if (ConductorTreeRngPath == 1)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 15 + (upgradeLevel * 25) + (upgradeLevel > 1 ? 25 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Conductor_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectMutated.statusId,
                            count = 1,
                        }
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.PostCombat,
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Conductor_B_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                                    TargetModeStatusEffectsFilter = new string[] 
                                    {
                                        StatusEffectMutated.statusId
                                    },
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Conductor_B_Triggered_effect_{upgradeLevel}",
                                        BonusDamage = 6 + (upgradeLevel * 6) + (upgradeLevel > 1 ? 6 : 0),
                                        BonusHP = -1 + -(upgradeLevel * 1) + -(upgradeLevel > 1 ? 1 : 0),
                                        FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                                        {
                                            new CardUpgradeMaskDataBuilder
                                            {
                                                RequiredStatusEffects = new List<StatusEffectStackData>
                                                {
                                                    new StatusEffectStackData
                                                    {
                                                        statusId = StatusEffectMutated.statusId,
                                                        count = 1,
                                                    }
                                                }
                                            }
                                        }
                                    }.Build(),
                                },
                            }
                        }
                    }
                };
            }

            //variant C {OnHysteria: Mutate, OnAnxiety: +Jitters}
            if (ConductorTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 15 + (upgradeLevel * 25) + (upgradeLevel > 1 ? 25 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Conductor_{upgradeLevel}_TitleKey",

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {

                        /*
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.AfterSpawnEnchant,
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Conductor_C0_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                { 
                                    EffectStateName = "CardEffectEnchant",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamStatusEffects = new StatusEffectStackData[] 
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectMutated.statusId,
                                            count = 1,
                                        }
                                    }
                                }
                            }
                        },
                        */
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Conductor_Cb0_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Heroes,

                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectMutated.statusId,
                                            count = 1,
                                        }
                                    }
                                },
                            }
                        },
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Conductor_Cb1_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder> 
                            { 
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Heroes,
                                    TargetModeStatusEffectsFilter = new string[]
                                    {
                                        StatusEffectMutated.statusId
                                    },

                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectJitters.statusId,
                                            count = 8 + (upgradeLevel * 8) + (upgradeLevel > 1 ? 8 : 0),
                                        }
                                    }
                                },
                                /*
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetModeStatusEffectsFilter = new string[]
                                    {
                                        StatusEffectMutated.statusId
                                    },

                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectChronic.statusId,
                                            count = 2 + (upgradeLevel * 2) + (upgradeLevel > 1 ? 2 : 0),
                                        }
                                    }
                                },
                                */
                            }
                        }
                    }
                };
            }

            Beyonder.LogError($"Missing upgrade data for Loco Motive Conductor path# {ConductorTreeRngPath} and upgrade level {upgradeLevel}. This will cause crashes.");

            return null;
        }

        public static CardUpgradeDataBuilder Horror(int upgradeLevel, bool randomize = false, RngId rngId = RngId.NonDeterministic)
        {
            if (randomize)
            {
                List<int> list = new List<int>(3) { 0, 1, 2 };
                list.Shuffle(rngId);
                HorrorTreeRngPath = list[0];
                //HorrorTreeRngPath = 2;
            }

            Beyonder.Log($"Building Horror Variant Path: {HorrorTreeRngPath} at upgrade level: {upgradeLevel}.");

            //variant A {Sweep, OnHysteria: +Jitters}
            if (HorrorTreeRngPath == 0)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 10 + 10 * upgradeLevel,
                    BonusHP = 10 + 10 * upgradeLevel,
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Horror_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData> 
                    { 
                        new StatusEffectStackData
                        {
                            statusId = VanillaStatusEffectIDs.Sweep,
                            count = 1,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                    { 
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Horror_A_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Heroes,
                                    ParamStatusEffects = new StatusEffectStackData[] 
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectJitters.statusId,
                                            count = 3 + (3 * upgradeLevel) + (upgradeLevel > 1 ? 3 : 0),
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }

            //variant B {Sweep, Action: +Jitters}
            if (HorrorTreeRngPath == 1)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 10 + 10 * upgradeLevel,
                    BonusHP = 10 + 10 * upgradeLevel,
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Horror_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = VanillaStatusEffectIDs.Sweep,
                            count = 1,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.OnTurnBegin,
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Horror_B_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Heroes,
                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectJitters.statusId,
                                            count = 6 + (6 * upgradeLevel) + (upgradeLevel > 1 ? 6 : 0),
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }

            /*
            //variant C {Sweep, Chronic, OnAnxiety: +Jitters}
            if (HorrorTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 10 * upgradeLevel,
                    BonusHP = 10 * upgradeLevel,
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Horror_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = VanillaStatusEffectIDs.Sweep,
                            count = 1,
                        },
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectChronic.statusId,
                            count = 5 + (upgradeLevel * 5) + (upgradeLevel > 1 ? 5 : 0),
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Horror_C_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Self,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectJitters.statusId,
                                            count = 5,
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
            */

            //variant C {Sweep, Stalker, Anxiety: Escape}
            if (HorrorTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 10 + 10 * upgradeLevel,
                    BonusHP = 10 + 10 * upgradeLevel,
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Horror_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = VanillaStatusEffectIDs.Sweep,
                            count = 1,
                        },
                        new StatusEffectStackData
                        {
                            statusId = VanillaStatusEffectIDs.Quick,
                            count = upgradeLevel > 1 ? 1 : 0,
                        }
                    },
                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                    { 
                        new CardTraitDataBuilder
                        { 
                            TraitStateName = typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                    { 
                        new CharacterTriggerDataBuilder
                        { 
                            Trigger = CharacterTriggerData.Trigger.PostCombat,
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Horror_Cb_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            { 
                                new CardEffectDataBuilder
                                { 
                                    EffectStateName = typeof(CustomCardEffectVanishMonster).AssemblyQualifiedName,
                                    TargetMode = TargetMode.Self,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamInt = (int)CardPile.DiscardPile,
                                }
                            }
                        }
                    }
                };
            }

            return null;
        }

        public static CardUpgradeDataBuilder Formless(int upgradeLevel, bool randomize = false, RngId rngId = RngId.NonDeterministic)
        {
            if (randomize)
            {
                List<int> list = new List<int>(3) { 0, 1, 2 };
                list.Shuffle(rngId);
                FormlessTreeRngPath = list[0];
            }

            Beyonder.Log($"Building Formless Variant Path: {FormlessTreeRngPath} at upgrade level: {upgradeLevel}.");

            //variant A {+/- Attack}
            if (FormlessTreeRngPath == 0)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 30 + (upgradeLevel * 40) + (upgradeLevel > 1 ? 40 : 0),
                    BonusHP = 20 + (upgradeLevel * 40) + (upgradeLevel > 1 ? 40 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Formless_{upgradeLevel}_TitleKey",
                    StatusEffectUpgrades = new List<StatusEffectStackData> 
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectFormless.statusId,
                            count = 1,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Formless_A1_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetMode = TargetMode.Self,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Formless_A1_{upgradeLevel}_Effect",
                                        BonusDamage = upgradeLevel + 1,
                                        BonusHP = 0,
                                    }.Build(),
                                }
                            }
                        },
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Formless_A2_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetMode = TargetMode.Self,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Formless_A2_{upgradeLevel}_Effect",
                                        BonusDamage = -1 * (upgradeLevel + 1),
                                        BonusHP = 0,
                                    }.Build(),
                                }
                            }
                        }
                    }
                };
            }

            //variant B {OnResolve: Trigger Hysteria}
            if (FormlessTreeRngPath == 1)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 30 + (upgradeLevel * 40) + (upgradeLevel > 1 ? 40 : 0),
                    BonusHP = 20 + (upgradeLevel * 40) + (upgradeLevel > 1 ? 40 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Formless_{upgradeLevel}_TitleKey",
                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectFormless.statusId,
                            count = 1,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.PostCombat,
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Formless_B_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectPlayUnitTrigger",
                                    TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                                    TargetMode = TargetMode.Room,
                                    ParamTrigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                                    ParamInt = 1,
                                    ShouldTest = true,
                                }
                            }
                        }
                    }
                };
            }

            //variant C {OnAnxiety: Jitters}
            if (FormlessTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 30 + (upgradeLevel * 40) + (upgradeLevel > 1 ? 40 : 0),
                    BonusHP = 20 + (upgradeLevel * 40) + (upgradeLevel > 1 ? 40 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_LocoMotive_Formless_{upgradeLevel}_TitleKey",
                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectFormless.statusId,
                            count = 1,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_LocoMotive_Formless_C_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters | Team.Type.Heroes,
                                    ParamStatusEffects = new StatusEffectStackData[] 
                                    {
                                        new StatusEffectStackData
                                        { 
                                            statusId = StatusEffectJitters.statusId, 
                                            count = 1 + upgradeLevel,
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }

            return null;
        }
    }
}