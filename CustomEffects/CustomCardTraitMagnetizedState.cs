using System;
using System.Collections;

// Token: 0x020000D3 RID: 211
namespace CustomEffects
{
    //Basically Freeze light without ant text or visual effects.
    public sealed class CustomCardTraitMagnetizedState : CardTraitState
    { 
        // Token: 0x0600082C RID: 2092 RVA: 0x0000C623 File Offset: 0x0000A823
        public override bool GetIsDiscardable()
        {
            return false;
        }
    }
}