using System;
using System.Collections;
using Trainworks.Managers;
using Void.Mania;

// Token: 0x020000D0 RID: 208
public sealed class BeyonderCardTraitCompulsive : CardTraitState
{
	// Token: 0x0600081C RID: 2076 RVA: 0x000239E2 File Offset: 0x00021BE2
	public override IEnumerator OnPreCardPlayed(CardState cardState, CardManager cardManager, RoomManager roomManager, CombatManager combatManager, RelicManager relicManager)
	{
		//int maniaPerTrait = base.GetParamInt();

        if (!ProviderManager.SaveManager.PreviewMode)
        {
            AddTrackManicCardsPlayed.cardCount++;
        }

        yield return CustomCardEffectManic.AssertCardManic(cardState);
        //yield return ManiaManager.Compulsion(maniaPerTrait);
		yield break;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00023A01 File Offset: 0x00021C01
	public override string GetCardText()
	{
		return base.LocalizeTraitKey("BeyonderCardTraitCompulsive_CardText");
	}

    public override string GetCardTooltipTitle()
    {
        return base.LocalizeTraitKey("BeyonderCardTraitCompulsive_TooltipTitle");
    }

    public override string GetCardTooltipText()
    {
        return string.Format(base.LocalizeTraitKey("BeyonderCardTraitCompulsive_TooltipText2"), base.GetParamInt());
    }
}
