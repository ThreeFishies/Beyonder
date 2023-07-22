using System;
using UnityEngine;
using Void.Mania;
using Void.Init;

// Token: 0x020000D6 RID: 214
public sealed class BeyonderCardTraitEntropic : CardTraitState
{
    // Token: 0x06000835 RID: 2101 RVA: 0x00023BAA File Offset: 0x00021DAA
    public override int OnStatusEffectApplied(CharacterState affectedCharacter, CardState thisCard, CardManager cardManager, RelicManager relicManager, string statusId, int sourceStacks = 0)
    {
        int currentMania = ManiaManager.GetCurrentMania(base.GetCard());

        return Mathf.Max(sourceStacks * (ManiaManager.GetEntopicScalingValue(false, currentMania) - 1), 0);
    }

    // Token: 0x06000836 RID: 2102 RVA: 0x00023BB4 File Offset: 0x00021DB4
    public override int GetModifiedStatusEffectStacks(StatusEffectStackData statusEffectStackData)
    {
        int currentMania = ManiaManager.GetCurrentMania(base.GetCard());

        return Mathf.Max((int)(statusEffectStackData.count * ManiaManager.GetEntopicScalingValue(false, currentMania)), 0);
    }

    // Token: 0x06000837 RID: 2103 RVA: 0x00023BC4 File Offset: 0x00021DC4
    public override string GetCardText()
    {
        return base.LocalizeTraitKey("BeyonderCardTraitEntropic_CardText");
    }

    public override string GetCardTooltipTitle()
    {
        return base.LocalizeTraitKey("BeyonderCardTraitEntropic_TooltipTitle");
    }

    public override string GetCardTooltipText()
    {
        return string.Format(base.LocalizeTraitKey("BeyonderCardTraitEntropic_TooltipText"), ManiaManager.GetInsanityMultiplier(), ManiaManager.GetInsanityThreshold());
    }

    // Token: 0x06000838 RID: 2104 RVA: 0x00023BD4 File Offset: 0x00021DD4
    public override void CreateAdditionalTooltips(TooltipContainer tooltipContainer)
    {
        if (PreferencesManager.Instance.TipTooltipsEnabled)
        {
            TooltipUI tooltipUI = tooltipContainer.InstantiateTooltip("StatusEffect_TooltipTitle", TooltipDesigner.TooltipDesignType.Default, false);
            string body = "StatusEffect_TooltipText".Localize(null);
            if (tooltipUI == null)
            {
                return;
            }
            tooltipUI.Set(null, body);
        }
    }

    public override int OnApplyingDamage(ApplyingDamageParameters damageParams)
    {
        int currentMania = ManiaManager.GetCurrentMania(base.GetCard());

        return damageParams.damage * ManiaManager.GetEntopicScalingValue(false, currentMania);
    }

    public override string GetCurrentEffectText(CardStatistics cardStatistics, SaveManager saveManager, RelicManager relicManager)
    {
        CardStatistics.StatValueData statValueData1 = base.StatValueData;
        statValueData1.forPreviewText = true;
        statValueData1.cardState = this.GetCard();
        statValueData1.trackedValue = Beyonder.ScalingByAnxiety.GetEnum();

        if (cardStatistics == null || !cardStatistics.GetStatValueShouldDisplayOnCardNow(statValueData1)) 
        {
            return string.Empty;
        }

        int multiplier = cardStatistics.GetStatValue(statValueData1);

        if (multiplier == 0)
        {
            CardStatistics.StatValueData statValueData2 = base.StatValueData;
            statValueData2.forPreviewText = true;
            statValueData2.cardState = this.GetCard();
            statValueData2.trackedValue = Beyonder.ScalingByHysteria.GetEnum();

            multiplier = cardStatistics.GetStatValue(statValueData2);
        }

        multiplier = ManiaManager.GetEntopicScalingValue(false, multiplier);

        if (multiplier == 1)
        {
            return string.Empty;
        }

        return string.Format("<i>(Insanity! x{0})</i>", multiplier);
    }
}