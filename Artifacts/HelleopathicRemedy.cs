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
using TMPro;
using System;
using System.Collections;
using System.Threading;

namespace Void.Artifacts
{
    public static class HelleopathicRemedy
    {
        public static CollectableRelicData Artifact;
        public static string ID = "HelleopathicRemedy_" + Beyonder.GUID;
        //public static CharacterTriggerData.Trigger culture;

        public static bool TryGetClassData(out ClassData clan)
        {
            clan = null;

            if (!Trainworks.Managers.PluginManager.GetAllPluginGUIDs().Contains("com.Tang.Rats.generic"))
            {
                return false;
            }

            clan = CustomClassManager.GetClassDataByID("com.Tang.Rats.genericHellPathogens_Clan");

            if (clan == null)
            {
                Beyonder.LogError("Hellborne Pathogens detected but failed to find class data.");
                return false;
            }

            //try
            //{
            //    culture = clan.GetChampionData(0).upgradeTree.GetUpgradeTrees()[0].GetFirstUpgrade().GetTriggerUpgrades()[0].GetTrigger();
            //}
            //catch (Exception eee)
            //{
            //    Beyonder.LogError("Hellborne Pathogens detected but failed to locate Culture trigger.");
            //    Beyonder.LogError(eee.Message);
            //    return false;
            //}

            return true;
        }

        public static void UpdateMagicPowerFromAllUnits(bool ignorePreviewStatus = false)
        {
            if (!ProviderManager.TryGetProvider<RoomManager>(out RoomManager roomManager))
            {
                return;
            }

            int currentRoom = roomManager.GetSelectedRoom();

            RoomState here = roomManager.GetRoom(currentRoom);

            List<CharacterState> monsters = new List<CharacterState>();

            here.AddCharactersToList(monsters, Team.Type.Monsters, false);

            if (monsters.IsNullOrEmpty<CharacterState>())
            {
                return;
            }

            foreach (CharacterState monster in monsters)
            {
                if (!monster.IsPyreHeart())
                {
                    if (monster.GetRoomStateModifiers().Count > 0)
                    {
                        foreach (IRoomStateModifier roomStateModifier in monster.GetRoomStateModifiers())
                        {
                            CustomRoomStateDynamicMagicalPowerModifier dynamicMagicalPowerModifier = roomStateModifier as CustomRoomStateDynamicMagicalPowerModifier;

                            dynamicMagicalPowerModifier?.Update(ignorePreviewStatus);
                        }
                    }
                }
            }
            return;
        }

        public static void HandleCombatPlayerControlSignal(bool playerHasControl)
        {
            if (playerHasControl)
            {
                UpdateMagicPowerFromAllUnits();
            }
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
                NameKey = "Malicka_Artifact_HelleopathicRemedy_Name_Key",
                DescriptionKey = "Malicka_Artifact_HelleopathicRemedy_Description_Key",
                RelicLoreTooltipKeys = new List<string>
                {
                    "Malicka_Artifact_HelleopathicRemedy_Lore_Key"
                },
                IconPath = "ArtifactAssets/HelleopathicRemedy.png",
                FromStoryEvent = false,
                IsBossGivenRelic = false,
                UnlockLevel = 1,
                Rarity = CollectableRarity.Common,
                RelicActivatedKey = "",
                RelicLoreTooltipStyle = RelicData.RelicLoreTooltipStyle.Malicka,
                RequiredDLC = ShinyShoe.DLC.Hellforged,

                EffectBuilders = new List<RelicEffectDataBuilder>
                {
                    new RelicEffectDataBuilder
                    {
                        //RelicEffectClassType = typeof(RelicEffectAddTempUpgrade),
                        RelicEffectClassType = typeof(CustomRelicEffectRoomStateDynamicMagicalPowerModifierMultistrikeMonitor),
                        ParamCardType = CardType.Monster,
                        ParamSourceTeam = Team.Type.Monsters,

                        ParamCardUpgradeDataBuilder = new CardUpgradeDataBuilder
                        {
                            UpgradeID = "HelleopathicRemedyEffectUpgrade",
                            IsUnique = true,
                            /*
                            TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>
                            {
                                new CharacterTriggerDataBuilder
                                {
                                    TriggerID = "HelleopathicRemedyEffectUpgrade_TriggerUpdate_Attacked",
                                    HideTriggerTooltip = true,
                                    Trigger = CharacterTriggerData.Trigger.OnHit,
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        {
                                            EffectStateType = typeof(CustomCardEffectUpdateDynamicMagicPowerOnHit),
                                        }
                                    }
                                },
                                new CharacterTriggerDataBuilder
                                {
                                    TriggerID = "HelleopathicRemedyEffectUpgrade_TriggerUpdate_Spell",
                                    HideTriggerTooltip = true,
                                    Trigger = CharacterTriggerData.Trigger.CardSpellPlayed,
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        {
                                            EffectStateType = typeof(CustomCardEffectUpdateDynamicMagicPowerOnHit),
                                        }
                                    }
                                },
                                new CharacterTriggerDataBuilder
                                {
                                    TriggerID = "HelleopathicRemedyEffectUpgrade_TriggerUpdate_Culture",
                                    HideTriggerTooltip = true,
                                    Trigger = culture,
                                    EffectBuilders = new List<CardEffectDataBuilder>
                                    {
                                        new CardEffectDataBuilder
                                        {
                                            EffectStateType = typeof(CustomCardEffectUpdateDynamicMagicPowerOnHit),
                                        }
                                    }
                                }
                            },
                            */
                            RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>
                            { 
                                new RoomModifierDataBuilder
                                { 
                                    RoomModifierID = "HelleopathicRemedyEffectUpgrade_RoomModifier",
                                    RoomModifierClassType = typeof(CustomRoomStateDynamicMagicalPowerModifier),
                                    ParamCardUpgradeDataBuilder = new CardUpgradeDataBuilder
                                    {
                                        UpgradeID = "HelleopathicRemedyEffectUpgrade_RoomModifier_DummyUpgrade"
                                    },
                                    ParamInt = 10,
                                    DescriptionKey = "Malicka_Artifact_HelleopathicRemedy_Card_Description_Key",
                                    DescriptionKeyInPlay = "Malicka_Artifact_HelleopathicRemedy_In_Play_Key",
                                    ParamStatusEffects = new List<StatusEffectStackData>{ },
                                }
                            },
                            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>
                            { 
                                new CardUpgradeMaskDataBuilder
                                { 
                                    CardUpgradeMaskID = "HelleopathicRemedyEffectUpgrade_Filter",
                                    CardType = CardType.Monster,
                                }
                            }
                        }
                    }
                }
            }.BuildAndRegister();

            CustomCharacterManager.AddCustomRoomModifierIcon(typeof(CustomRoomStateDynamicMagicalPowerModifier), "ArtifactAssets/HelleopathicRemedyIcon.png", "ArtifactAssets/HelleopathicRemedyTooltipIcon.png");

            return Artifact;
        }
    }
}


namespace Void.Patches
{
    [HarmonyPatch(typeof(CharacterState), "GetCanAttack")]
    public static class HelleopathicRemedyNoAttackPatch1
    {
        public static void Postfix(ref CharacterState __instance, ref bool __result)
        {
            if (HelleopathicRemedy.HasIt())
            {
                if (__instance != null && __instance.GetTeamType() == Team.Type.Monsters && !__instance.IsPyreHeart())
                {
                    __result = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PrimaryAbilityDisplay), "Set", new System.Type[] { typeof(CharacterState) })]
    public static class HelleopathicRemedyNoAttackPatch2
    {
        public static void Postfix(ref PrimaryAbilityDisplay __instance, ref CharacterState characterState)
        {
            if (HelleopathicRemedy.HasIt())
            {
                if (characterState != null && characterState.GetTeamType() == Team.Type.Monsters && !characterState.IsPyreHeart())
                {
                    __instance.Set(false, characterState.GetAttackDamage(), characterState.GetNumAttacks(), characterState.GetAttackDamageWithoutStatusEffectBuffs(), PrimaryAbilityDisplay.AttackColor.Normal);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PrimaryAbilityDisplay), "Set", new System.Type[] { typeof(bool), typeof(int), typeof(int), typeof(int), typeof(PrimaryAbilityDisplay.AttackColor) })]
    public static class HelleopathicRemedyNoAttackPatch3
    {
        public static void Postfix(ref PrimaryAbilityDisplay __instance, ref TMP_Text ___abilityLabel, bool canAttack, int attack, int numAttacks, PrimaryAbilityDisplay.AttackColor attackColor, ref string ___attackMultistrikeKey, ref string ___attackStandardKey)
        {
            if (HelleopathicRemedy.HasIt() && !isHero(__instance))
            {
                string key = (numAttacks > 1) ? ___attackMultistrikeKey : ___attackStandardKey;
                string text = string.Format(key.Localize(null), attack, numAttacks);

                Color setColor = (Color)AccessTools.Method(typeof(PrimaryAbilityDisplay),"GetColor", new System.Type[] { typeof(PrimaryAbilityDisplay.AttackColor), }).Invoke(__instance, new object[] { attackColor });

                ___abilityLabel.SetTextSafe(text, false);
                ___abilityLabel.color = setColor;
                ___abilityLabel.gameObject.SetActive(true);
            }
        }

        private static bool isHero(PrimaryAbilityDisplay @this) 
        {
            if (ProviderManager.CombatManager == null) 
            {
                return true;
            }

            List<CharacterState> monsters = new List<CharacterState>();

            ProviderManager.CombatManager.GetMonsterManager().AddCharactersInTowerToList(monsters);

            if (monsters.IsNullOrEmpty()) 
            {
                return true;
            }

            foreach (CharacterState monster in monsters) 
            {
                if (monster.GetCharacterUI().GetPrimaryAbilityDisplay() == @this) 
                {
                    return false;
                }
            }

            return true;
        }
    }

    /*
    [HarmonyPatch(typeof(PrimaryAbilityDisplay), "Set", new System.Type[] { typeof(CharacterData), typeof(string), typeof(Color) })]
    public static class HelleopathicRemedyNoAttackPatch4 
    {        
        public static void Postfix(ref PrimaryAbilityDisplay __instance)
        {
            if (HelleopathicRemedy.HasIt())
            {

            }
        }
    }
    */

    [HarmonyPatch(typeof(CombatManager), "ApplyEffects", new Type[] { typeof(CombatManager.EffectQueueData) })]
    public static class UpdateVaccinationStatusOnAnyCardPlayed1
    {
        public static IEnumerator Postfix(IEnumerator __result, Queue<CombatManager.EffectQueueData> ___effectsQueue)
        {
            if (HelleopathicRemedy.HasIt() && ___effectsQueue.IsNullOrEmpty<CombatManager.EffectQueueData>())
            {
                if (ProviderManager.SaveManager != null && !ProviderManager.SaveManager.PreviewMode)
                {
                    HelleopathicRemedy.UpdateMagicPowerFromAllUnits();
                }
            }

            yield return __result;
            yield break;
        }
    }

    [HarmonyPatch(typeof(CombatManager), "RunTriggerQueue")]
    public static class UpdateVaccinationStatusOnAnyCardPlayed2
    {
        public static IEnumerator Postfix(IEnumerator __result, CombatManager __instance, bool ___temporaryTriggerQueueEnabled)
        {
            if (HelleopathicRemedy.HasIt() && !___temporaryTriggerQueueEnabled && !__instance.IsRunningTriggerQueue)
            {
                //if (ProviderManager.SaveManager != null && !ProviderManager.SaveManager.PreviewMode)
                //{
                    HelleopathicRemedy.UpdateMagicPowerFromAllUnits(true);
                //}
            }

            yield return __result;
            yield break;
        }
    }

    [HarmonyPatch(typeof(BattleHud), "NewProviderAvailable")]
    public static class AddHelleopathicRemedyListener 
    {
        public static void Postfix(ref IProvider newProvider, ref CombatManager ___combatManager)
        {
            if (!HelleopathicRemedy.HasIt()) 
            {
                return;
            }

            if (DepInjector.MapProvider<CombatManager>(newProvider, ref ___combatManager))
            {
                ___combatManager.monsterTurnPlayerHasControlSignal.AddListener(new Action<bool>(HelleopathicRemedy.HandleCombatPlayerControlSignal));
            }
        }
    }

    [HarmonyPatch(typeof(BattleHud), "OnDestroy")]
    public static class RemoveHelleopathicRemedyListener
    {
        public static void Postfix(ref CombatManager ___combatManager) 
        {
            if (!HelleopathicRemedy.HasIt())
            {
                return;
            }

            if (___combatManager != null)
            {
                ___combatManager.monsterTurnPlayerHasControlSignal.RemoveListener(new Action<bool>(HelleopathicRemedy.HandleCombatPlayerControlSignal));
            }
        }
    }
}