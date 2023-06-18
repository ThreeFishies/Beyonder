using System;
using System.Collections;

// Token: 0x020000DC RID: 220
public sealed class BeyonderCardTraitStalkerState : CardTraitState
{

    // Token: 0x0600084E RID: 2126 RVA: 0x00023E40 File Offset: 0x00022040
    public override void OnPreDrawingHand(CardState thisCard, CardManager cardManager, PlayerManager playerManager, MonsterManager monsterManager)
    {
        if (!cardManager.GetHand(false).Contains(thisCard))
        {
            cardManager.DrawSpecificCard(thisCard, 0f, HandUI.DrawSource.Deck, null, 1, 1);
            cardManager.AdjustDrawCountModifier(-1);
        }
    }

    // Token: 0x06000850 RID: 2128 RVA: 0x00023E81 File Offset: 0x00022081
    public override string GetCardText()
    {
        return base.LocalizeTraitKey("BeyonderCardTraitStalkerState_CardText");
    }
}
