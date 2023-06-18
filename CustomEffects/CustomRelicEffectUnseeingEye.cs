using System.Collections.Generic;
using Trainworks.Builders;
using Trainworks.Managers;
using Void.Init;

namespace Void.Artifacts
{
    public class CustomRelicEffectUnseeingEye : RelicEffectBase, ICardModifierRelicEffect, IRelicEffect
    {
        public static int BeyonderChampionIndex = -1;
        public static CardData AlliedStarterCard;
        public static CardUpgradeData[] ManicUpgrades = new CardUpgradeData[2]
        {
            new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Unseeing_Eye_Manic_Compulsive_Upgrade",
                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitCompulsive).AssemblyQualifiedName,
                        ParamInt = 1
                    }
                }
            }.Build(),
            new CardUpgradeDataBuilder
            {
                UpgradeTitleKey = "Unseeing_Eye_Manic_Afflictive_Upgrade",
                TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>
                {
                    new CardTraitDataBuilder
                    {
                        TraitStateName = typeof(BeyonderCardTraitAfflictive).AssemblyQualifiedName,
                        ParamInt = 1
                    }
                }
            }.Build()
        };
        public const string TitleKey = "Beyonder_Artifact_UnSeeingEye_Name_Key";
        public const string DescriptionKey = "Beyonder_Artifact_UnSeeingEye_Description_Key";

        public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData) 
        { 
            BeyonderChampionIndex = -1;

            if (ProviderManager.SaveManager.HasMainClass())
            {
                if (ProviderManager.SaveManager.GetMainClass() == Beyonder.BeyonderClanData)
                {
                    BeyonderChampionIndex = ProviderManager.SaveManager.GetMainChampionIndex();
                    AlliedStarterCard = ProviderManager.SaveManager.GetSubChampionData().starterCardData;

                    //Beyonder.Log($"Beyonder champ index: {0}. Allied Started Card: {AlliedStarterCard.GetNameEnglish()}.");
                }
                else if (ProviderManager.SaveManager.GetSubClass() == Beyonder.BeyonderClanData)
                {
                    BeyonderChampionIndex = ProviderManager.SaveManager.GetSubChampionIndex();
                    AlliedStarterCard = ProviderManager.SaveManager.GetMainChampionData().starterCardData;

                    //Beyonder.Log($"Beyonder champ index: {0}. Allied Started Card: {AlliedStarterCard.GetNameEnglish()}.");
                }
                else
                {
                    //Beyonder.Log("Beyonder clan not found. Can't apply Manic traits to starting cards.");
                }
            }
            else
            {
                Beyonder.Log("Can't setup Unseeing Eye without a main class.");
            }

            base.Initialize(relicState, relicData, relicEffectData);
        }

        public bool ApplyCardStateModifiers(CardState cardState, SaveManager saveManager, CardManager cardManager, RelicManager relicManager)
        {
            if (BeyonderChampionIndex == -1)
            {
                return false;
            }

            if (cardState == null)
            {
                return false;
            }

            if (cardState.GetCardDataID() != AlliedStarterCard.GetID())
            {
                return false;
            }

            if (cardState.HasTrait(typeof(BeyonderCardTraitAfflictive)) || cardState.HasTrait(typeof(BeyonderCardTraitCompulsive)))
            {
                return false;
            }

            if (cardState.HasTemporaryTrait(typeof(BeyonderCardTraitAfflictive)) || cardState.HasTemporaryTrait(typeof(BeyonderCardTraitCompulsive)))
            {
                return false;
            }

            CardUpgradeState cardUpgradeState = new CardUpgradeState();
            cardUpgradeState.Setup(ManicUpgrades[BeyonderChampionIndex], false);
            cardState.Upgrade(cardUpgradeState, saveManager, true);

            return true;
        }

        public bool GetTooltip(out string title, out string body)
        {
            title = string.Empty;
            body = string.Empty;
            return false;
        }
    }
}