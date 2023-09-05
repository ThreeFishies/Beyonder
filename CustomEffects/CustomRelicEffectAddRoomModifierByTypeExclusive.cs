using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using ShinyShoe.Logging;
using Trainworks.Managers;
using Void.Init;

// Token: 0x0200027C RID: 636
public sealed class CustomRelicEffectAddRoomModifierByTypeExclusive : RelicEffectBase
{
    // Token: 0x170001B8 RID: 440
    // (get) Token: 0x06001627 RID: 5671 RVA: 0x0000C623 File Offset: 0x0000A823
    public override bool CanShowNotifications
    {
        get
        {
            return false;
        }
    }

    // Token: 0x06001628 RID: 5672 RVA: 0x00058298 File Offset: 0x00056498
    public override void Initialize(RelicState relicState, RelicData relicData, RelicEffectData relicEffectData)
    {
        base.Initialize(relicState, relicData, relicEffectData);
        this.targetTeam = relicEffectData.GetParamSourceTeam();
        this.characterSubtype = relicEffectData.GetParamCharacterSubtype();
        this.dummyUpgrade = relicEffectData.GetParamCardUpgradeData();
        if (dummyUpgrade.GetRoomModifierUpgrades().Count > 0)
        {
            this.roomModifier = dummyUpgrade.GetRoomModifierUpgrades()[0];
        }
        this.statusEffects = relicEffectData.GetParamStatusEffects();
    }

    private bool IsOfTypeExclusive(CharacterState target) 
    {
        if (target == null) { return false; }

        if (ProviderManager.SaveManager != null && ProviderManager.SaveManager.GetMutatorCount() > 0)
        {
            foreach (MutatorState mutator in ProviderManager.SaveManager.GetMutators())
            {
                if (mutator.GetRelicDataID() == "3894bbe3-6ad6-42ed-9f63-6b47371b4583") // Hive mind
                {
                    return true;
                }
            }
        }

        if (target.GetSubtypes().Count == 0) { return false; }

        foreach (SubtypeData subtype in target.GetSubtypes()) 
        {
            if ((subtype.Key != this.characterSubtype.Key) && (subtype.Key != "SubtypesData_Chosen")) 
            {
                return false;
            }
        }

        return true;
    }

    // Token: 0x06001629 RID: 5673 RVA: 0x00058322 File Offset: 0x00056522
    public override IEnumerator OnCharacterAdded(CharacterState character, CardState fromCard, RelicManager relicManager, SaveManager saveManager, PlayerManager playerManager, RoomManager roomManager, CombatManager combatManager, CardManager cardManager)
    {
        if (this.roomModifier == null || this.targetTeam != Team.Type.Monsters || this.characterSubtype == null) 
        {
            Beyonder.Log("Failed to add room modifier data. Please check relic effect data configuration.", BepInEx.Logging.LogLevel.Warning);
            yield break;
        }

        if (IsOfTypeExclusive(character)) 
        {
            List<IRoomStateModifier> roomModifiers = character.GetRoomStateModifiers();

            foreach (IRoomStateModifier modifier in roomModifiers) 
            {
                if (Type.GetType(this.roomModifier.GetRoomStateModifierClassName()) == modifier.GetType()) 
                {
                    //Beyonder.Log($"Skipping duplicate state modifier on character {character.GetName()}.");
                    yield break;
                }
            }

            //Beyonder.Log($"Adding room state modifier to character {character.GetName()}.");
            AccessTools.Method(typeof(CharacterState), "AddNewCharacterRoomModifierState", new Type[] { typeof(RoomModifierData) }).Invoke(character, new object[] { this.roomModifier });

            if (statusEffects.Length > 0) 
            {
                character.AddStatusEffect(statusEffects[0].statusId, 0, new CharacterState.AddStatusEffectParams
                {
                    cardManager = cardManager,
                    fromEffectType = null,
                    overrideImmunity = false,
                    relicEffects = null,
                    sourceCardState = null,
                    sourceIsHero = false,
                    sourceRelicState = base._srcRelicState,
                    spawnEffect = true,
                });
            }
        }

        yield break;
    }

    // Token: 0x0600162A RID: 5674 RVA: 0x0005834E File Offset: 0x0005654E
    public bool ImpactsPyre()
    {
        return false;
    }

    // Token: 0x04000BAC RID: 2988
    private Team.Type targetTeam;

    // Token: 0x04000BAE RID: 2990
    private SubtypeData characterSubtype;

    private CardUpgradeData dummyUpgrade;

    private RoomModifierData roomModifier;

    private StatusEffectStackData[] statusEffects;
}
