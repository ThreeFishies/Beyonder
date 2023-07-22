//CustomCardEffectSacrificeAssertTarget
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using HarmonyLib;
using Void.Init;
using Void.Spells;

// Token: 0x020000C3 RID: 195
namespace CustomEffects
{
    public sealed class CustomCardEffectSacrificeAssertTarget : CardEffectBase, ICardEffectStatuslessTooltip
    {
        public static int HPofSacrifice = 0;
        public static int SizeofSacrifice = 0;
        public static bool FocusOverride = false;
        // Token: 0x17000084 RID: 132
        // (get) Token: 0x060007E6 RID: 2022 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool CanRandomizeTargetMode
        {
            get
            {
                return false;
            }
        }

        // Token: 0x060007E7 RID: 2023 RVA: 0x00023553 File Offset: 0x00021753
        public override void Setup(CardEffectState cardEffectState)
        {
            this.cachedState = cardEffectState;
            base.Setup(cardEffectState);
        }

        // Token: 0x060007E8 RID: 2024 RVA: 0x00023564 File Offset: 0x00021764
        private void RecollectTargets(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (cardEffectState.GetTargetMode() != TargetMode.RandomInRoom)
            {
                return;
            }
            TargetHelper.CollectTargets(cardEffectState, cardEffectParams, cardEffectParams.saveManager.PreviewMode);
            for (int i = cardEffectParams.targets.Count - 1; i >= 0; i--)
            {
                CharacterState characterState = cardEffectParams.targets[i];
                if (!characterState.GetCharacterManager().DoesCharacterPassSubtypeCheck(characterState, cardEffectState.GetParamSubtype()))
                {
                    cardEffectParams.targets.RemoveAt(i);
                }
            }
            if (cardEffectParams.targets.Count > 0)
            {
                RngId rngId = cardEffectParams.saveManager.PreviewMode ? RngId.BattleTest : RngId.Battle;
                CharacterState item = cardEffectParams.targets[RandomManager.Range(0, cardEffectParams.targets.Count, rngId)];
                cardEffectParams.targets.Clear();
                cardEffectParams.targets.Add(item);
            }
        }

        // Token: 0x060007E9 RID: 2025 RVA: 0x00023628 File Offset: 0x00021828
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (FocusOverride) 
            { 
                return true; 
            }

            this.RecollectTargets(cardEffectState, cardEffectParams);

            if (cardEffectParams.targets.Count > 0) 
            {
                if (cardEffectParams.dropLocation != null) 
                { 
                    CharacterState dropTarget = cardEffectParams.dropLocation.GetCharacterState();
                    if (dropTarget != null) 
                    {
                        if (dropTarget.GetTeamType() == Team.Type.Monsters)
                        {
                            HPofSacrifice = dropTarget.GetHP();
                            SizeofSacrifice = dropTarget.GetSize();
                            return true;
                        }
                    }
                }
                if (cardEffectState.GetTargetMode() != TargetMode.RandomInRoom)
                {
                    return false;
                }
            }

            HPofSacrifice = 0;
            SizeofSacrifice = 0;

            foreach (CharacterState sacrifice in cardEffectParams.targets) 
            { 
                HPofSacrifice += sacrifice.GetHP();
                SizeofSacrifice += sacrifice.GetSize();
            }

            return cardEffectParams.targets.Count > 0;
        }

        // Token: 0x060007EA RID: 2026 RVA: 0x00023640 File Offset: 0x00021840
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            this.RecollectTargets(cardEffectState, cardEffectParams);
            foreach (CharacterState characterState in cardEffectParams.targets)
            {
                bool paramBool = cardEffectState.GetParamBool();
                yield return characterState.Sacrifice(cardEffectParams.playedCard, false, paramBool);
            }
            //List<CharacterState>.Enumerator enumerator = default(List<CharacterState>.Enumerator);
            yield break;
            //yield break;
        }

        // Token: 0x060007EB RID: 2027 RVA: 0x00023660 File Offset: 0x00021860
        public string GetTooltipBaseKey(CardEffectState cardEffectState)
        {
            string str = string.Empty;
            if (this.cachedState.GetParamSubtype().IsNone)
            {
                str = "_NoSubtype";
            }
            return "CardEffectSacrifice" + str;
        }

        // Token: 0x060007EC RID: 2028 RVA: 0x00023696 File Offset: 0x00021896
        public override string GetDescriptionAsTrait(CardEffectState cardEffectState)
        {
            if (cardEffectState.GetParamSubtype() != null && !cardEffectState.GetParamSubtype().IsNone)
            {
                return string.Format("CardEffectSacrifice_AsTrait_WithSubtype".Localize(null), cardEffectState.GetParamSubtype().LocalizedName);
            }
            return "CardEffectSacrifice_AsTrait".Localize(null);
        }

        // Token: 0x04000468 RID: 1128
        private CardEffectState cachedState;
    }

    [HarmonyPatch(typeof(CardSelectionBehaviour), "HighlightDropTarget")]
    public static class UpdateCerebralDetonationNumbersByTarget 
    {
        public static void Postfix(ref CardSelectionBehaviour __instance, ref SpawnPointUI hoveredSpawnPointUI, ref CardManager ___cardManager, ref HandUI ___handUI) 
        {
            if (CustomCardEffectSacrificeAssertTarget.FocusOverride) 
            { 
                return; 
            }

            if (hoveredSpawnPointUI == null || hoveredSpawnPointUI.GetParentSpawnPoint() == null) 
            {
                return;
            }

            CharacterState target = hoveredSpawnPointUI.GetParentSpawnPoint().GetCharacterState();

            if (target != null && target.IsAlive)
            {
                //Beyonder.Log($"Testing on: {target.GetName()} HP: {target.GetHP()} Size: {target.GetSize()} || SacrificeTargetHP: {CustomCardEffectSacrificeAssertTarget.HPofSacrifice} SacrificeTargetSize: {CustomCardEffectSacrificeAssertTarget.SizeofSacrifice}");
            }
            else 
            {
                return;
            }

            CardState ___focusedCardState = __instance.GetFocusedOrSelectedCardState();

            if (___focusedCardState == null || ___cardManager == null || ___focusedCardState.GetCardDataID() != CerebralDetonation.Card.GetID())
            {
                //Beyonder.Log("Abort: null detected or focused card is not Cereberal Detonation.");
                return;
            }

            //Beyonder.Log($"Testing card {___focusedCardState.GetTitle()} on: {target.GetName()} HP: {target.GetHP()} Size: {target.GetSize()}");

            CardUI cardUI = ___handUI.GetFocusedCardUI(); //___cardManager.GetCardInHand(___focusedCardState);
            if (cardUI == null)
            {
                //Beyonder.Log("Fail_0");
                return;
            }

            if (___cardManager.GetCardStatistics() == null) 
            {
                //Beyonder.Log("Failed to find Card Statistics.");
                return;
            }

            CustomCardEffectSacrificeAssertTarget.FocusOverride = true;

            //Beyonder.Log("Attempting to update card text.");

            //if (!cardUI.IsCardVisible()) 
            //{
                //Beyonder.Log("Fail_1");
            //}
            //if (!___cardManager.CanSelectCard(__instance.GetFocusedOrSelectedCardIndex()))
            //{
                //Beyonder.Log("Fail_2");
            //}
            //if (__instance.GetFocusedOrSelectedCardState() == null)
            //{
                //Beyonder.Log("Fail_3");
            //}

            if (cardUI.IsCardVisible() && ___cardManager.CanSelectCard(__instance.GetFocusedOrSelectedCardIndex()) && __instance.GetFocusedOrSelectedCardState() != null)
            {
                ___handUI.UpdateCardUI(cardUI, __instance.GetFocusedOrSelectedCardState(), true, false);
                cardUI.UpdateTextContent(__instance.GetFocusedOrSelectedCardState(), ___cardManager.GetCardStatistics(), false);
            }
            else 
            {
                //Beyonder.Log("Failed to update card text.");
            }

            CustomCardEffectSacrificeAssertTarget.FocusOverride = false;
        }
    }

    /*
    [HarmonyPatch(typeof(CardTraitState), "GetCurrentEffectText")]
    public static class DebugTextPreview 
    {
        public static void Postfix(ref CardTraitState __instance, ref string __result) 
        {
            if (__instance is CardTraitScalingAddStatusEffect && CustomCardEffectSacrificeAssertTarget.FocusOverride)
            {
                Beyonder.Log("CardTraitScalingAddStatusEffect returned value: " + __result);
            }
        }
    }
    */
}