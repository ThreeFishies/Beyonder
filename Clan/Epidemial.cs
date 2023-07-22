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
using Void.Builders;
using CustomEffects;
using RunHistory;
using Malee;
using Void.Spells;
using System;

namespace Void.Champions
{
    [Serializable]
    public struct EpidemialPathData
    {
        [SerializeField]
        public int InnumerableTreePathID;
        [SerializeField]
        public int ContagiousTreePathID;
        [SerializeField]
        public int SoundlessTreePathID;
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public static class Epidemial
    {
        public static CharacterDataBuilder character;
        public static CardData card;
        public static string ID = Beyonder.GUID + "_Epidemial_Card";
        public static string CharID = Beyonder.GUID + "_Epidemial_Character";
        public const string SavePath = "SaveData/CurrentRun/EpidemialUpgradeTree.json";
        public static int InnumerableTreeRngPath = 0;
        public static int SoundlessTreeRngPath = 0;
        public static int ContagiousTreeRngPath = 0;
        public static CardPool SelfPool = null;

        public static void BuildAndRegister()
        {
            SelfPool = new CardPoolBuilder
            {
                CardPoolID = "Epidemial_Self_Pool",
                Cards = new List<CardData> { },
            }.Build();

            character = new CharacterDataBuilder
            {
                Name = "Epidemial",
                NameKey = "Beyonder_Champ_Epidemial_Name_Key",
                AttackDamage = 1,
                Health = 1,
                Size = 1,
                CharacterID = CharID,
                AssetPath = "Monsters/Assets/Epidemial_Monster.png",
                CharacterChatterData = new CharacterChatterDataBuilder 
                { 
                    name = "EpidemialChatterData",
                    gender = CharacterChatterData.Gender.Neutral,

                    characterAddedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Champ_Epidemial_Chatter_Key_Added_0",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Added_1"
                    },
                    characterIdleExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_0",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_1",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_2",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_3",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_4",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_5",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_6",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_7",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Idle_8"
                    },
                    characterSlayedExpressionKeys = new List<string>() 
                    {
                        "Beyonder_Champ_Epidemial_Chatter_Key_Slay_0",
                        "Beyonder_Champ_Epidemial_Chatter_Key_Slay_1"
                    },
                    characterTriggerExpressionKeys = new List<CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys> 
                    { 
                        new CharacterChatterDataBuilder.CharacterTriggerDataChatterExpressionKeys
                        { 
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            Key = "Beyonder_Champ_Epidemial_Chatter_Key_Anxiety_0"
                        }
                    }
                    
                }.Build(),

                //ProjectilePrefab = CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.Morselmaster).GetProjectilePrefab()
            };

            card = new ChampionCardDataBuilder
            {
                ClanID = Beyonder.BeyonderClanData.GetID(),
                CardID = ID,
                StarterCardData = OcularInfection.Card,
                ChampionSelectedCue = "Multiplayer_Emote_Hmm",
                Champion = character,
                //OverrideDescriptionKey = "Beyonder_Champ_Epidemial_Description_Key",
                AssetPath = "Monsters/Assets/Epidemial_Card.png",
                ChampionIconPath = "Monsters/Assets/Epidemial_Card.png",
                CardLoreTooltipKeys = new List<string> 
                { 
                    "Beyonder_Champ_Epidemial_Lore_Key" 
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
                            Innumerable(0,false),
                            Innumerable(1,false),
                            Innumerable(2,false)
                        },
                        new List<CardUpgradeDataBuilder>
                        {
                            Contagious(0,false),
                            Contagious(1,false),
                            Contagious(2,false)
                        },
                        new List<CardUpgradeDataBuilder>
                        {
                            Soundless(0,false),
                            Soundless(1,false),
                            Soundless(2,false)
                        }
                    }
                }
            }.BuildAndRegister(1);

            CardEffectData Noise = new CardEffectDataBuilder
            {
                EffectStateName = typeof(CustomCardEffectPlaySoundCue).AssemblyQualifiedName,
                ParamStr = "Multiplayer_Emote_Hmm",
            }.Build();

            List<CardEffectData> effectDatas = card.GetEffects();
            effectDatas.Add(Noise);
            AccessTools.Field(typeof(CardData), "effects").SetValue(card, effectDatas);

            ReorderableArray<CardData> pool = AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(SelfPool) as ReorderableArray<CardData>;
            pool.Add(card);
        }

        public static void BuildTreeForNewRun(RngId rngId = RngId.SetupRun, bool ShouldRandomize = true)
        {
            ChampionData championData = Beyonder.BeyonderClanData.GetChampionData(1);
            championData.upgradeTree = new CardUpgradeTreeDataBuilder
            { 
                Champion = championData.championCardData.GetSpawnCharacterData(),
                UpgradeTrees = new List<List<CardUpgradeDataBuilder>> 
                { 
                    new List<CardUpgradeDataBuilder>
                    { 
                        Innumerable(0, ShouldRandomize, rngId),
                        Innumerable(1, false),
                        Innumerable(2, false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Contagious(0, ShouldRandomize, rngId),
                        Contagious(1, false),
                        Contagious(2, false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Soundless(0, ShouldRandomize, rngId),
                        Soundless(1, false),
                        Soundless(2, false)
                    },
                }
            }.Build();

            //SaveTreeData(SavePath);
        }

        public static void BuildTreeFromData(EpidemialPathData data)
        {
            SetupTrees(data.InnumerableTreePathID, data.ContagiousTreePathID, data.SoundlessTreePathID);

            if (!Beyonder.IsInit) { return; }

            ChampionData championData = Beyonder.BeyonderClanData.GetChampionData(1);
            championData.upgradeTree = new CardUpgradeTreeDataBuilder
            {
                Champion = championData.championCardData.GetSpawnCharacterData(),
                UpgradeTrees = new List<List<CardUpgradeDataBuilder>>
                {
                    new List<CardUpgradeDataBuilder>
                    {
                        Innumerable(0,false),
                        Innumerable(1,false),
                        Innumerable(2,false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Contagious(0,false),
                        Contagious(1,false),
                        Contagious(2,false)
                    },
                    new List<CardUpgradeDataBuilder>
                    {
                        Soundless(0, false),
                        Soundless(1, false),
                        Soundless(2, false)
                    },
                }
            }.Build();

            //SaveTreeData(SavePath);
        }

        public static EpidemialPathData GetPathTreeData() 
        {
            return new EpidemialPathData 
            {
                InnumerableTreePathID = InnumerableTreeRngPath,
                ContagiousTreePathID = ContagiousTreeRngPath,
                SoundlessTreePathID = SoundlessTreeRngPath,
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
                    Beyonder.LogError("Removing previous Epidemial Upgrade Tree Data Failed.");
                    return;
                }
            }

            EpidemialPathData pathData = new EpidemialPathData() 
            {
                InnumerableTreePathID = InnumerableTreeRngPath,
                ContagiousTreePathID = ContagiousTreeRngPath,
                SoundlessTreePathID = SoundlessTreeRngPath,
            };

            try
            {
                File.WriteAllText(path, JsonUtility.ToJson(pathData));
            }
            catch
            {
                Beyonder.LogError("Saving Epidemial Upgrade Tree Data Failed.");
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
                    EpidemialPathData pathData = JsonUtility.FromJson<EpidemialPathData>(data);
                    SetupTrees(pathData.InnumerableTreePathID, pathData.ContagiousTreePathID, pathData.SoundlessTreePathID);
                }
                catch
                {
                    Beyonder.Log("Failed to load Epidemial Upgrade Tree Pata Data from file. Using default data.", BepInEx.Logging.LogLevel.Warning);
                }
            }
        }
        */

        public static void SetupTrees(int InnumerableTreePath = 0, int ContagiousTreePath = 0, int SoundlessTreePath = 0)
        {
            InnumerableTreeRngPath = InnumerableTreePath;
            ContagiousTreeRngPath = ContagiousTreePath;
            SoundlessTreeRngPath = SoundlessTreePath;
        }

        public static CardUpgradeDataBuilder Innumerable(int upgradeLevel, bool randomize = false, RngId rngId = RngId.NonDeterministic)
        {
            if (randomize)
            {
                List<int> list = new List<int>(3) { 0, 1, 2 };
                list.Shuffle(rngId);
                InnumerableTreeRngPath = list[0];
                //InnumerableTreeRngPath = 2;
            }

            //variant A {Draw}
            if (InnumerableTreeRngPath == 0)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 14 + (upgradeLevel * 15) + (upgradeLevel > 1 ? 15 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Innumerable_{upgradeLevel}_TitleKey",

                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder> 
                    { 
                        new CardTraitDataBuilder
                        { 
                            TraitStateName = (upgradeLevel < 2) ? "CardTraitDummy" : typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName,
                        },
                        //new CardTraitDataBuilder  //This trait is not working properly for this usage.
                        //{
                        //    TraitStateName = "CardTraitShowCardTargets",
                        //}
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.OnUnscaledSpawn,
                            DescriptionKey = "Beyonder_Champ_Epidemial_Innumerable_SpawnCopy_Key",
                            HideTriggerTooltip = true,
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectAddSpawnerBattleCard).AssemblyQualifiedName,
                                    TargetMode = TargetMode.DrawPile,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamInt = (int)CardPile.DiscardPile,
                                    AdditionalParamInt = 1, //one copy
                                    ParamCardPool = SelfPool,
                                    CopyModifiersFromSource = true,
                                    //IgnoreTemporaryModifiersFromSource = true,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder { UpgradeTitleKey = "Fail_0" }.Build(),
                                }
                            }
                        },
                        new CharacterTriggerDataBuilder 
                        {
                            Trigger = CharacterTriggerData.Trigger.OnSpawn,
                            DescriptionKey = $"Beyonder_Champ_Epidemial_Innumerable_A{upgradeLevel}_Key",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectDrawType",
                                    ParamInt = 1 + (upgradeLevel > 0 ? 1 : 0),
                                    TargetCardType = CardType.Spell,
                                    TargetMode = TargetMode.DrawPile,
                                }
                            }
                        },
                    },
                    /*
                    RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder> 
                    { 
                        new RoomModifierDataBuilder
                        { 
                            DescriptionKey = "CardUpgradeData_descriptionKey-d9caf7b2d16bd373-95ccdbde6ea9534419b4c4023dbb4fd1-v2",
                            ParamStatusEffects = new StatusEffectStackData[]{},
                            RoomStateModifierClassName = "RoomStateHandSizeModifier",
                            ExtraTooltipTitleKey = $"Beyonder_Champ_Epidemial_Innumerable_{upgradeLevel}_TitleKey",
                            ExtraTooltipBodyKey = "Beyonder_Champ_Epidemial_Innumerable_A_DescriptionKey",
                            ParamInt = 1,
                        }
                    }*/

                };
            }

            //variant B {Ember}
            if (InnumerableTreeRngPath == 1)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 14 + (upgradeLevel * 15) + (upgradeLevel > 1 ? 15 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Innumerable_{upgradeLevel}_TitleKey",

                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                    {
                        new CardTraitDataBuilder
                        {
                            TraitStateName = (upgradeLevel < 2) ? "CardTraitDummy" : typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName,
                        }
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.OnUnscaledSpawn,
                            DescriptionKey = "Beyonder_Champ_Epidemial_Innumerable_SpawnCopy_Key",
                            HideTriggerTooltip = true,
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectAddSpawnerBattleCard).AssemblyQualifiedName,
                                    TargetMode = TargetMode.DrawPile,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamInt = (int)CardPile.DiscardPile,
                                    AdditionalParamInt = 1, //one copy
                                    ParamCardPool = SelfPool,
                                    CopyModifiersFromSource = true,
                                    //IgnoreTemporaryModifiersFromSource = true,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder { UpgradeTitleKey = "Fail_0" }.Build(),
                                }
                            }
                        },
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.OnSpawn,
                            DescriptionKey = $"Beyonder_Champ_Epidemial_Innumerable_B_Key",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectGainEnergy",
                                    ParamInt = 2 + (upgradeLevel > 0 ? 1 : 0),
                                    TargetMode = TargetMode.Self,
                                    TargetTeamType = Team.Type.Monsters,
                                }
                            }
                        }
                    },
                    /*
                    RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>
                    {
                        new RoomModifierDataBuilder
                        {
                            DescriptionKey = "CardUpgradeData_descriptionKey-32c0fbfbaabd6118-54a1f5a8de28db44f8049e97094e306d-v2",
                            ParamStatusEffects = new StatusEffectStackData[]{},
                            RoomStateModifierClassName = "RoomStateEnergyModifier",
                            ExtraTooltipTitleKey = $"Beyonder_Champ_Epidemial_Innumerable_{upgradeLevel}_TitleKey",
                            ExtraTooltipBodyKey = "Beyonder_Champ_Epidemial_Innumerable_B_DescriptionKey",
                            ParamInt = 1,
                        }
                    }
                    */
                };
            }

            //variant C {Duplicator}
            if (InnumerableTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 14 + (upgradeLevel * 15) + (upgradeLevel > 1 ? 15 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Innumerable_{upgradeLevel}_TitleKey",

                    TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                    {
                        new CardTraitDataBuilder
                        {
                            TraitStateName = (upgradeLevel < 2) ? "CardTraitDummy" : typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName,
                        }
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.OnUnscaledSpawn,
                            DescriptionKey = "Beyonder_Champ_Epidemial_Innumerable_SpawnCopy_Key",
                            HideTriggerTooltip = true,
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectAddSpawnerBattleCard).AssemblyQualifiedName,
                                    TargetMode = TargetMode.DrawPile,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamInt = (int)CardPile.DiscardPile,
                                    AdditionalParamInt = 1, //one copy
                                    ParamCardPool = SelfPool,
                                    CopyModifiersFromSource = true,
                                    //IgnoreTemporaryModifiersFromSource = true,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder { UpgradeTitleKey = "Fail_0" }.Build(),
                                }
                            }
                        },
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.OnSpawn,
                            DescriptionKey = $"Beyonder_Champ_Epidemial_Innumerable_C{upgradeLevel}_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectCopyUnitsAndApplyUpgrade).AssemblyQualifiedName,
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamInt = 1, //+ (upgradeLevel == 2 ? 1 : 0),
                                }
                            }
                        }
                    }
                };
            }

            Beyonder.LogError($"Invalid rng variant: {InnumerableTreeRngPath} for Epidemial Innumerabe path. This will cause crashes.");
            return null;
        }

        public static CardUpgradeDataBuilder Contagious(int upgradeLevel, bool randomize = false, RngId rngId = RngId.NonDeterministic)
        {
            if (randomize)
            {
                List<int> list = new List<int>(3) { 0, 1, 2 };
                list.Shuffle(rngId);
                ContagiousTreeRngPath = list[0];
                //ContagiousTreeRngPath = 2;
            }

            //variant A {OnSummon: +Jitters, +Chronic}
            if (ContagiousTreeRngPath == 0)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 0,
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Contagious_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData> 
                    { 
                        new StatusEffectStackData
                        { 
                            statusId = VanillaStatusEffectIDs.Endless,
                            count = 1
                        }
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder> 
                    { 
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.OnSpawn,
                            DescriptionKey = "Beyonder_Champ_Epidemial_Contagious_AB_DescriptionKey",
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
                                            count = 6 + (3 * upgradeLevel),
                                        }
                                    }
                                },
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectChronic.statusId,
                                            count = 3 + (3 * upgradeLevel) + (upgradeLevel > 1 ? 3 : 0),
                                        }
                                    }
                                },
                            }
                        }
                    }
                };
            }

            //variant B {OnAnxiety: +Jitters, +Chronic}
            if (ContagiousTreeRngPath == 1)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 0,
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Contagious_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = VanillaStatusEffectIDs.Endless,
                            count = 1
                        }
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_Epidemial_Contagious_AB_DescriptionKey",
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
                                            count = 2 + (1 * upgradeLevel),
                                        }
                                    }
                                },
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddStatusEffect",
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamStatusEffects = new StatusEffectStackData[]
                                    {
                                        new StatusEffectStackData
                                        {
                                            statusId = StatusEffectChronic.statusId,
                                            count = 1 + (1 * upgradeLevel) + (upgradeLevel > 1 ? 1 : 0),
                                        }
                                    }
                                },
                            }
                        }
                    }
                };
            }

            //variant C {Chronic, Hysteria: Jitters and OnExtinguish: copy buffs to allies and debuffs to enemies.}
            if (ContagiousTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 0,
                    BonusHP = 0,
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Contagious_{upgradeLevel}_TitleKey",

                    StatusEffectUpgrades = new List<StatusEffectStackData> 
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectChronic.statusId,
                            count = 3 + (3 * upgradeLevel) + (upgradeLevel > 1 ? 3 : 0),
                        },
                        new StatusEffectStackData
                        {
                            statusId = VanillaStatusEffectIDs.Endless,
                            count = 1
                        }
                    },

                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = CharacterTriggerData.Trigger.CardSpellPlayed,
                            DescriptionKey = "Beyonder_Champ_Epidemial_Contagious_C0_DescriptionKey",
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
                                            count = 1 + (1 * upgradeLevel) + (upgradeLevel > 1 ? 1 : 0),
                                        }
                                    }
                                },
                            }
                        },
                        new CharacterTriggerDataBuilder
                        { 
                            Trigger = CharacterTriggerData.Trigger.OnDeath,
                            DescriptionKey = "Beyonder_Champ_Epidemial_Contagious_C1_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                { 
                                    EffectStateName = typeof(CustomCardEffectAddStatusEffectsFromSelf).AssemblyQualifiedName,
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Heroes,
                                    ParamInt = (int)StatusEffectData.DisplayCategory.Negative
                                },
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = typeof(CustomCardEffectAddStatusEffectsFromSelf).AssemblyQualifiedName,
                                    TargetMode = TargetMode.Room,
                                    TargetTeamType = Team.Type.Monsters,
                                    ParamInt = (int)StatusEffectData.DisplayCategory.Positive
                                }
                            }
                        }
                    }
                };
            }

            Beyonder.LogError($"Invalid rng variant: {ContagiousTreeRngPath} for Epidemial Contagious path. This will cause crashes.");
            return null;
        }

        public static CardUpgradeDataBuilder Soundless(int upgradeLevel, bool randomize = false, RngId rngId = RngId.NonDeterministic)
        {
            if (randomize)
            {
                List<int> list = new List<int>(3) { 0, 1, 2 };
                list.Shuffle(rngId);
                SoundlessTreeRngPath = list[0];
            }

            //variant A {OnAnxiety: -Attack, +Health}
            if (SoundlessTreeRngPath == 0)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 19 + (upgradeLevel * 20) + (upgradeLevel > 1 ? 20 : 0),
                    BonusHP = 0,
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Soundless_{upgradeLevel}_TitleKey",
                    StatusEffectUpgrades = new List<StatusEffectStackData> 
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectSoundless.statusId,
                            count = 1,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnAnxiety.OnAnxietyCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_Epidemial_Soundless_A_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetMode = TargetMode.Self,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Soundless_A_{upgradeLevel}_Effect",
                                        BonusDamage = -1 - (upgradeLevel * 1) - (upgradeLevel > 1 ? 1 : 0),
                                        BonusHP = 5 + (upgradeLevel * 5) + (upgradeLevel > 1 ? 5 : 0),
                                    }.Build(),
                                }
                            }
                        }
                    }
                };
            }

            //variant B {Chronic}
            if (SoundlessTreeRngPath == 1)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 19 + (upgradeLevel * 20) + (upgradeLevel > 1 ? 20 : 0),
                    BonusHP = 0,
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Soundless_{upgradeLevel}_TitleKey",
                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectSoundless.statusId,
                            count = 1,
                        },
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectChronic.statusId,
                            count = 8 + (8 * upgradeLevel) + (upgradeLevel > 1 ? 8 : 0),
                        }
                    }
                };
            }

            //variant C {MoreHP, OnHysteria: -HP}
            if (SoundlessTreeRngPath == 2)
            {
                return new CardUpgradeDataBuilder
                {
                    BonusDamage = 19 + (upgradeLevel * 20) + (upgradeLevel > 1 ? 20 : 0),
                    BonusHP = 39 + (upgradeLevel * 40) + (upgradeLevel > 1 ? 40 : 0),
                    UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Soundless_{upgradeLevel}_TitleKey",
                    StatusEffectUpgrades = new List<StatusEffectStackData>
                    {
                        new StatusEffectStackData
                        {
                            statusId = StatusEffectSoundless.statusId,
                            count = 1,
                        }
                    },
                    TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                    {
                        new CharacterTriggerDataBuilder
                        {
                            Trigger = Trigger_Beyonder_OnHysteria.OnHysteriaCharTrigger.GetEnum(),
                            DescriptionKey = "Beyonder_Champ_Epidemial_Soundless_C_DescriptionKey",
                            EffectBuilders = new List<CardEffectDataBuilder>
                            {
                                new CardEffectDataBuilder
                                {
                                    EffectStateName = "CardEffectAddTempCardUpgradeToUnits",
                                    TargetTeamType = Team.Type.Monsters,
                                    TargetMode = TargetMode.Self,
                                    ParamCardUpgradeData = new CardUpgradeDataBuilder
                                    {
                                        UpgradeTitleKey = $"Beyonder_Champ_Epidemial_Soundless_C_{upgradeLevel}_Effect",
                                        BonusDamage = 0,
                                        BonusHP = -5 + (upgradeLevel * -5) + (upgradeLevel > 1 ? -5 : 0),
                                    }.Build(),
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