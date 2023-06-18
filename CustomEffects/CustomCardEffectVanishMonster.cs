using System;
using System.Collections;
using ShinyShoe.Logging;
using Trainworks.Constants;

// Token: 0x020000A1 RID: 161
namespace CustomEffects
{
    public sealed class CustomCardEffectVanishMonster : CardEffectBase
    {
        // Token: 0x06000736 RID: 1846 RVA: 0x00021E47 File Offset: 0x00020047
        public static void GetCardText(CardEffectData cardEffectData, out string outText)
        {
            outText = string.Format("CustomCardEffectVanishMonster_CardText".Localize(null), (CardPile)cardEffectData.GetParamInt());
        }

        public static string GetPileText(CardPile cardPile) 
        { 
            switch (cardPile) 
            { 
                case CardPile.HandPile:
                    return "and return to your hand";
                case CardPile.DeckPile:
                    return "and return to your deck";
                case CardPile.DeckPileRandom:
                    return "and return to your deck";
                case CardPile.DeckPileTop:
                    return "and return to the top of your deck";
                case CardPile.DiscardPile:
                    return "and go to your discard pile";
                case CardPile.EatenPile:
                    return "and go to your eaten pile";
                case CardPile.ExhaustedPile:
                    return "and go to your consume pile";
                default: return "forever";
            }
        }

        // Token: 0x06000737 RID: 1847 RVA: 0x00021E66 File Offset: 0x00020066
        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            this.despawnCounter--;
            if (this.despawnCounter <= 0 && cardEffectParams != null)
            {
                CharacterState selfTarget = cardEffectParams.selfTarget;
                if (selfTarget != null)
                {
                    if (selfTarget.HasStatusEffect(VanillaStatusEffectIDs.Rooted)) 
                    {
                        selfTarget.ShowNotification("CardEffectCantDespawnCharacter_Rooted".Localize(null), PopupNotificationUI.Source.General, null);
                        selfTarget.RemoveStatusEffect(VanillaStatusEffectIDs.Rooted, false, 1, true, null, typeof(CustomCardEffectVanishMonster));
                        yield break;
                    }
                    selfTarget.GetCharacterUI().ShowEffectVFX(selfTarget, cardEffectState.GetAppliedVFX());
                    selfTarget.SetDespawned();
                    selfTarget.PlayCharacterSound("Multiplayer_Emote_Lol");
                    yield return OnTriggered(selfTarget, cardPile);
                    yield return selfTarget.GetCharacterManager().RemoveCharacter(selfTarget, false, true, true, false, false, false);
                }
                else
                {
                    Log.Error(LogGroups.Gameplay, "Self target could not be found for CardEffectDespawnCharacter! Make sure this effect is only attached to characterData.");
                }
            }
            yield break;
        }

        // Token: 0x06000738 RID: 1848 RVA: 0x00021E83 File Offset: 0x00020083
        public override void Setup(CardEffectState cardEffectState)
        {
            cardEffectState.SetShouldOverrideTriggerUI(true);
            this.despawnCounter = 1; //cardEffectState.GetIntInRange();
            this.cardPile = (CardPile)cardEffectState.GetParamInt();
            if (this.cardPile == CardPile.None) { this.cardPile = CardPile.KeepInStandBy; }
            if (this.despawnCounter <= 0)
            {
                Log.Warning(LogGroups.Gameplay, "CardEffectDespawnCharacter should have paramInt set to a value greater than 0! Setting to 1...");
                this.despawnCounter = 1;
            }
        }

        // Token: 0x06000739 RID: 1849 RVA: 0x00021EB4 File Offset: 0x000200B4
        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            CharacterState characterState = (cardEffectParams != null) ? cardEffectParams.selfTarget : null;
            return !(characterState == null) && characterState.IsAlive;
        }

        // Token: 0x0400044A RID: 1098
        private int despawnCounter = -1;
        private CardPile cardPile = CardPile.DiscardPile;

        // StatusEffectEndlessState
        // Token: 0x06002BC6 RID: 11206 RVA: 0x000AB4F7 File Offset: 0x000A96F7
        private IEnumerator OnTriggered(CharacterState character, CardPile cardPile)
        {
            //CharacterState characterState = inputTriggerParams.fromEaten ? inputTriggerParams.associatedCharacter : inputTriggerParams.attacked;
            if (character == null)
            {
                yield break;
            }
            CardState spawnerCard = character.GetSpawnerCard();
            if (spawnerCard != null)
            {
                spawnerCard.SetRemoveFromStandByPileOverride(cardPile);
                character.ShowNotification("CardEffectDespawnCharacter_Activated".Localize(null), PopupNotificationUI.Source.General, null);
            }
            yield break;
        }
    }
}


