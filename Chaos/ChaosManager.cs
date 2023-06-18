using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Builders;
using Trainworks.Managers;
using Trainworks.Constants;
using System.Linq;
using UnityEngine;
using Trainworks.Utilities;
using Void.Init;
using CustomEffects;
using Void.Clan;
using Void.Unit;
using Void.Triggers;
using Void.Status;
using Steamworks;
using static ChampionUpgradeScreen;
using TMPro;
using Void.Monsters;
using System.Dynamic;

namespace Void.Chaos
{
    [Serializable]
    public struct BoonsBanesData
    {
        [SerializeField]
        public List<int> VBoons;
        [SerializeField]
        public List<int> VBanes;
        [SerializeField]
        public List<int> UBoons;
        [SerializeField]
        public List<int> UBanes;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public static class ChaosManager
    {
        public static List<CardUpgradeData> VBoonsData = new List<CardUpgradeData>();
        public static List<CardUpgradeData> VBanesData = new List<CardUpgradeData>();
        public static List<CardUpgradeData> UBoonsData = new List<CardUpgradeData>();
        public static List<CardUpgradeData> UBanesData = new List<CardUpgradeData>();
        public static List<int> Vboons = new List<int>();
        public static List<int> Vbanes = new List<int>();
        public static List<int> Uboons = new List<int>();
        public static List<int> Ubanes = new List<int>();
        public static bool IsInit = false;

        public static void SetIndex(string list = "Vboons", int indexNumber = 0, int indexValue = 0)
        {
            if (list == "Vboons")
            {
                int temp = Vboons[indexNumber];

                for (int ii = 0; ii < Vboons.Count; ii++) 
                { 
                    if (Vboons[ii] == indexValue)
                    {
                        Vboons[ii] = temp;
                    }
                }

                Vboons[indexNumber] = indexValue;
            }

            if (list == "Vbanes")
            {
                int temp = Vbanes[indexNumber];

                for (int ii = 0; ii < Vbanes.Count; ii++)
                {
                    if (Vbanes[ii] == indexValue)
                    {
                        Vbanes[ii] = temp;
                    }
                }

                Vbanes[indexNumber] = indexValue;
            }

            if (list == "Ubanes")
            {
                int temp = Ubanes[indexNumber];

                for (int ii = 0; ii < Ubanes.Count; ii++)
                {
                    if (Ubanes[ii] == indexValue)
                    {
                        Ubanes[ii] = temp;
                    }
                }

                Ubanes[indexNumber] = indexValue;
            }

            if (list == "Uboons")
            {
                int temp = Uboons[indexNumber];

                for (int ii = 0; ii < Uboons.Count; ii++)
                {
                    if (Uboons[ii] == indexValue)
                    {
                        Uboons[ii] = temp;
                    }
                }

                Uboons[indexNumber] = indexValue;
            }
        }

        public static int FindValue(ref List<CardUpgradeData> list, string UpgradeTitleKey) 
        {
            int index = 0;

            foreach (CardUpgradeData ii in list) 
            {
                if (ii.GetUpgradeTitleKey() == UpgradeTitleKey) 
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static bool LoadFromData(BoonsBanesData boonsBanesData) 
        {
            bool successful = true;

            if (!boonsBanesData.VBoons.IsNullOrEmpty() && (VBoonsData.Count == boonsBanesData.VBoons.Count))
            {
                Vboons = boonsBanesData.VBoons;
            }
            else 
            {
                successful = false;
            }
            if (!boonsBanesData.VBanes.IsNullOrEmpty() && (VBanesData.Count == boonsBanesData.VBanes.Count))
            {
                Vbanes = boonsBanesData.VBanes;
            }
            else
            {
                successful = false;
            }
            if (!boonsBanesData.UBoons.IsNullOrEmpty() && (UBoonsData.Count == boonsBanesData.UBoons.Count))
            {
                Uboons = boonsBanesData.UBoons;
            }
            else
            {
                successful = false;
            }
            if (!boonsBanesData.UBanes.IsNullOrEmpty() && (UBanesData.Count == boonsBanesData.UBanes.Count))
            {
                Ubanes = boonsBanesData.UBanes;
            }
            else
            {
                successful = false;
            }

            return successful;
        }

        public static BoonsBanesData GetData() 
        {
            return new BoonsBanesData
            {
                VBoons = Vboons,
                VBanes = Vbanes,
                UBoons = Uboons,
                UBanes = Ubanes
            };
        }

        public static void Init() 
        {
            if (IsInit) { return; }

            VBoonsData = VeilritchBoons.Build();
            for (int ii = 0; ii < VBoonsData.Count; ii++) 
            {
                Vboons.Add(ii);
            }

            VBanesData = VeilritchBanes.Build();
            for (int ii = 0; ii < VBanesData.Count; ii++)
            {
                Vbanes.Add(ii);
            }

            UBoonsData = UndretchBoons.Build();
            for (int ii = 0; ii < UBoonsData.Count; ii++)
            {
                Uboons.Add(ii);
            }

            UBanesData = UndretchBanes.Build();
            for (int ii = 0; ii < UBanesData.Count; ii++)
            {
                Ubanes.Add(ii);
            }

            foreach (CardUpgradeData data in VBoonsData) 
            {
                AccessTools.Field(typeof(CardUpgradeData), "isUnique").SetValue(data, true);
            }
            foreach (CardUpgradeData data in VBanesData)
            {
                AccessTools.Field(typeof(CardUpgradeData), "isUnique").SetValue(data, true);
            }
            foreach (CardUpgradeData data in UBoonsData)
            {
                AccessTools.Field(typeof(CardUpgradeData), "isUnique").SetValue(data, true);
            }
            foreach (CardUpgradeData data in UBanesData)
            {
                AccessTools.Field(typeof(CardUpgradeData), "isUnique").SetValue(data, true);
            }

            IsInit = true;
        }

        public static void Shuffle(RngId rngId) 
        {
            Vboons.Shuffle(rngId);
            Vbanes.Shuffle(rngId);
            Uboons.Shuffle(rngId);
            Ubanes.Shuffle(rngId);

            FormlessHorror.UpdateStartingUpgrades();
            Malevolence.UpdateStartingUpgrades();
            Vexation.UpdateStartingUpgrades();

            SoundlessSwarm.UpdateStartingUpgrades();
            HairyPotty.UpdateStartingUpgrades();
            FurryBeholder.UpdateStartingUpgrades();

            //cache this synthesis data too
            ApostleoftheVoid.GetSynthesis();
        }

        public static void UpdateStartingUpgrades(BoonsBanesData data) 
        {
            Vboons = data.VBoons;
            Vbanes = data.VBanes;
            Uboons = data.UBoons;
            Ubanes = data.UBanes;

            //Beyonder.Log("line 179 (ChaosManager)");
            FormlessHorror.UpdateStartingUpgrades();
            //Beyonder.Log("line 181 (ChaosManager)");
            Malevolence.UpdateStartingUpgrades();
            //Beyonder.Log("line 183 (ChaosManager)");
            Vexation.UpdateStartingUpgrades();

            //Beyonder.Log("line 186 (ChaosManager)");
            SoundlessSwarm.UpdateStartingUpgrades();
            //Beyonder.Log("line 188 (ChaosManager)");
            HairyPotty.UpdateStartingUpgrades();
            //Beyonder.Log("line 190 (ChaosManager)");
            FurryBeholder.UpdateStartingUpgrades();

            //cache this synthesis data too
            //Beyonder.Log("line 194 (ChaosManager)");
            ApostleoftheVoid.GetSynthesis();
            //Beyonder.Log("line 196 (ChaosManager)");
        }

        //Not comprehensive. Limited to expected upgrade data.
        public static string GenerateDescription(string DescriptionKey, CardUpgradeData upgradeData, bool register = true)
        {
            List<string> description = new List<string> { };
            string temp = string.Empty;

            temp += (upgradeData.GetBonusDamage() > 0) ? ("+" + upgradeData.GetBonusDamage() + "[attack]" ) : "";
            temp += (upgradeData.GetBonusDamage() < 0) ? (upgradeData.GetBonusDamage() + "[attack]") : "";
            if (temp.Length > 0) 
            {
                description.Add(temp);
                temp = string.Empty;
            }

            temp += (upgradeData.GetBonusHP() > 0) ? ("+" + upgradeData.GetBonusHP() + "[health]") : "";
            temp += (upgradeData.GetBonusHP() < 0) ? (upgradeData.GetBonusHP() + "[health]") : "";
            if (temp.Length > 0)
            {
                description.Add(temp);
                temp = string.Empty;
            }

            //skip bonusheal

            temp += (upgradeData.GetBonusSize() > 0) ? ("+" + upgradeData.GetBonusSize() + "[size]") : "";
            temp += (upgradeData.GetBonusSize() < 0) ? (upgradeData.GetBonusSize() + "[size]") : "";
            if (temp.Length > 0)
            {
                description.Add(temp);
                temp = string.Empty;
            }

            temp += (upgradeData.GetCostReduction() < 0) ? ("+" + -upgradeData.GetCostReduction() + "[ember]") : "";
            temp += (upgradeData.GetCostReduction() > 0) ? (-upgradeData.GetCostReduction() + "[ember]") : "";
            if (temp.Length > 0)
            {
                description.Add(temp);
                temp = string.Empty;
            }

            //skip xCostReduction

            //skip CardTriggers

            if (upgradeData.GetRoomModifierUpgrades() != null && upgradeData.GetRoomModifierUpgrades().Count > 0)
            {
                foreach (RoomModifierData data in upgradeData.GetRoomModifierUpgrades()) 
                {
                    temp = data.GetDescriptionKey() + "_Essence";
                    temp = temp.Localize();
                    temp = "'" + temp + "'";
                    description.Add(temp);
                }
                temp = string.Empty;
            }

            if (upgradeData.GetStatusEffectUpgrades() != null && upgradeData.GetStatusEffectUpgrades().Count > 0)
            {
                foreach (StatusEffectStackData status in upgradeData.GetStatusEffectUpgrades())
                {
                    temp = StatusEffectManager.GetLocalizedName(status.statusId, status.count, true, StatusEffectManager.Instance.GetStatusEffectDataById(status.statusId).ShowStackCount(), true);
                    description.Add(temp);
                }
                temp = string.Empty;
            }

            if (upgradeData.GetTraitDataUpgrades() != null && upgradeData.GetTraitDataUpgrades().Count > 0)
            {
                foreach (CardTraitData trait in upgradeData.GetTraitDataUpgrades())
                {
                    //Beyonder.Log("***********************************************************************");
                    //Beyonder.Log(typeof(BeyonderCardTraitStalkerState).AssemblyQualifiedName + "_CardText");
                    //Beyonder.Log(CardTraitData.GetTraitCardTextLocalizationKey(trait.GetTraitStateName()));
                    //Beyonder.Log("***********************************************************************");

                    temp = CardTraitData.GetTraitCardTextLocalizationKey(trait.GetTraitStateName());
                    temp = temp.Localize();
                    description.Add(temp);
                }
                temp = string.Empty;
            }

            if (upgradeData.GetTriggerUpgrades() != null && upgradeData.GetTriggerUpgrades().Count > 0)
            {
                foreach (CharacterTriggerData data in upgradeData.GetTriggerUpgrades())
                {
                    temp = data.GetDescriptionKey() + "_Essence";
                    temp = temp.Localize();
                    temp = CharacterTriggerData.GetKeywordText(data.GetTrigger(), true) + ": " + temp;
                    temp = "'" + temp + "'";
                    description.Add(temp);
                }
                temp = string.Empty;
            }


            if (description.Count == 0)
            {
                temp = string.Empty;
            }
            else if (description.Count == 1)
            {
               temp = description[0];
            }
            else 
            {
                for (int ii = 0; ii < description.Count; ii++) 
                {
                    if (ii == 0)
                    {
                        temp = description[ii];
                    }
                    else if (ii + 1 < description.Count)
                    {
                        temp += ", " + description[ii];
                    }
                    else 
                    {
                        temp += " and " + description[ii];
                    }
                }

                if (!(temp.EndsWith(".") || temp.EndsWith(".'")))
                {
                    if (!temp.EndsWith("'")) 
                    { 
                        temp += ".";
                    }
                    else 
                    {
                        temp = temp.Insert(temp.Length-1,".");
                    }
                }
            }

            if (register)
            {
                CustomLocalizationManager.ImportSingleLocalization(DescriptionKey, "Text", "", "", "", "", temp, temp, temp, temp, temp, temp);
            }

            return temp;
        }

        public static CardUpgradeData MergeUpgrades(CardUpgradeData upgradeData1, CardUpgradeData upgradeData2, CharacterData SourceSynthesisUnit = null, string UpgradeDescKey = "")
        {
            if (upgradeData1 == null) 
            {
                return upgradeData2;
            }
            if (upgradeData2 == null)
            {
                return upgradeData1;
            }

            List<CardUpgradeMaskData> mFilters = new List<CardUpgradeMaskData> { };
            mFilters.AddRange((upgradeData1.GetFilters() != null) ? upgradeData1.GetFilters() : new List<CardUpgradeMaskData> { });
            mFilters.AddRange((upgradeData2.GetFilters() != null) ? upgradeData2.GetFilters() : new List<CardUpgradeMaskData> { });

            List<CardTriggerEffectData> mCardTriggers = new List<CardTriggerEffectData> { };
            mCardTriggers.AddRange((upgradeData1.GetCardTriggerUpgrades() != null) ? upgradeData1.GetCardTriggerUpgrades() : new List<CardTriggerEffectData> { });
            mCardTriggers.AddRange((upgradeData2.GetCardTriggerUpgrades() != null) ? upgradeData2.GetCardTriggerUpgrades() : new List<CardTriggerEffectData> { });

            List<string> mRemoveTraits = new List<string> { };
            mRemoveTraits.AddRange((upgradeData1.GetRemoveTraitUpgrades() != null) ? upgradeData1.GetRemoveTraitUpgrades() : new List<string> { });
            mRemoveTraits.AddRange((upgradeData2.GetRemoveTraitUpgrades() != null) ? upgradeData2.GetRemoveTraitUpgrades() : new List<string> { });

            List<RoomModifierData> mRoomModifiers = new List<RoomModifierData> { };
            mRoomModifiers.AddRange((upgradeData1.GetRoomModifierUpgrades() != null) ? upgradeData1.GetRoomModifierUpgrades() : new List<RoomModifierData> { });
            mRoomModifiers.AddRange((upgradeData2.GetRoomModifierUpgrades() != null) ? upgradeData2.GetRoomModifierUpgrades() : new List<RoomModifierData> { });
            
            List<StatusEffectStackData> mStatus = new List<StatusEffectStackData> { };
            mStatus.AddRange((upgradeData1.GetStatusEffectUpgrades() != null) ? upgradeData1.GetStatusEffectUpgrades() : new List<StatusEffectStackData> { });
            mStatus.AddRange((upgradeData2.GetStatusEffectUpgrades() != null) ? upgradeData2.GetStatusEffectUpgrades() : new List<StatusEffectStackData> { });

            List<CardTraitData> mTraits = new List<CardTraitData> { };
            mTraits.AddRange((upgradeData1.GetTraitDataUpgrades() != null) ? upgradeData1.GetTraitDataUpgrades() : new List<CardTraitData> { });
            mTraits.AddRange((upgradeData2.GetTraitDataUpgrades() != null) ? upgradeData2.GetTraitDataUpgrades() : new List<CardTraitData> { });

            List<CharacterTriggerData> mUnitTriggers = new List<CharacterTriggerData> { };
            mUnitTriggers.AddRange((upgradeData1.GetTriggerUpgrades() != null) ? upgradeData1.GetTriggerUpgrades() : new List<CharacterTriggerData> { });
            mUnitTriggers.AddRange((upgradeData2.GetTriggerUpgrades() != null) ? upgradeData2.GetTriggerUpgrades() : new List<CharacterTriggerData> { });

            List<CardUpgradeData> mRemoves = new List<CardUpgradeData> { };
            mRemoves.AddRange((upgradeData1.GetUpgradesToRemove() != null) ? upgradeData1.GetUpgradesToRemove() : new List<CardUpgradeData> { });
            mRemoves.AddRange((upgradeData2.GetUpgradesToRemove() != null) ? upgradeData2.GetUpgradesToRemove() : new List<CardUpgradeData> { });

            CardUpgradeData upgradeDataMerged = new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = upgradeData1.GetUpgradeTitleKey() + "_merge_" + upgradeData2.GetUpgradeTitleKey(),
                BonusDamage = upgradeData1.GetBonusDamage() + upgradeData2.GetBonusDamage(),
                BonusHeal = upgradeData1.GetBonusHeal() + upgradeData2.GetBonusHeal(),
                BonusHP = upgradeData1.GetBonusHP() + upgradeData2.GetBonusHP(),
                BonusSize = upgradeData1.GetBonusSize() + upgradeData2.GetBonusSize(),
                CostReduction = upgradeData1.GetCostReduction() + upgradeData2.GetCostReduction(),
                XCostReduction = upgradeData1.GetXCostReduction() + upgradeData2.GetXCostReduction(),
                HideUpgradeIconOnCard = upgradeData1.GetHideUpgradeIconOnCard() && upgradeData2.GetHideUpgradeIconOnCard(),
                RemoveTraitUpgrades = mRemoveTraits,
                Filters = mFilters,
                CardTriggerUpgrades = mCardTriggers,
                SourceSynthesisUnit = SourceSynthesisUnit,
                RoomModifierUpgrades = mRoomModifiers,
                StatusEffectUpgrades = mStatus,
                TraitDataUpgrades = mTraits,
                TriggerUpgrades = mUnitTriggers,
                UpgradesToRemove = mRemoves,
                UpgradeDescriptionKey = UpgradeDescKey,
                
            }.Build();

            return upgradeDataMerged;
        }
    }
}