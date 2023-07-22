using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Enums;
using Void.Init;

namespace Void.Patches
{

    public class EntropicCardSelectionError : ExtendedEnum<EntropicCardSelectionError, CardSelectionBehaviour.SelectionError>
    {
        // Token: 0x06000106 RID: 262 RVA: 0x00005CDC File Offset: 0x00003EDC
        public EntropicCardSelectionError(string localizationKey, int? ID = null) : base(localizationKey, ID ?? EntropicCardSelectionError.GetNewTooltipTypeGUID())
        {
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00005D20 File Offset: 0x00003F20
        public static int GetNewTooltipTypeGUID()
        {
            EntropicCardSelectionError.NumTypes++;
            return EntropicCardSelectionError.NumTypes;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005D44 File Offset: 0x00003F44
        public static implicit operator EntropicCardSelectionError(CardSelectionBehaviour.SelectionError herdSelectionError)
        {
            return ExtendedEnum<EntropicCardSelectionError, CardSelectionBehaviour.SelectionError>.Convert(herdSelectionError);
        }

        //A number bigger than the types currently available.
        public static int NumTypes = 15;

        public static void Initialize()
        {
            CardSelectionBehaviour selectionBehaviour = new CardSelectionBehaviour();

            Dictionary<CardSelectionBehaviour.SelectionError, string> errors = AccessTools.Field(typeof(CardSelectionBehaviour), "SelectionErrorToMessageKey").GetValue(selectionBehaviour) as Dictionary<CardSelectionBehaviour.SelectionError, string>;

            errors.Add(Beyonder.EntropicCardSelectionError.GetEnum(), "SelectionError_CardEntropic");

            AccessTools.Field(typeof(CardSelectionBehaviour), "SelectionErrorToMessageKey").SetValue(selectionBehaviour, errors);
        }
    }

    public class HoldoverCardSelectionError : ExtendedEnum<HoldoverCardSelectionError, CardSelectionBehaviour.SelectionError>
    {
        // Token: 0x06000106 RID: 262 RVA: 0x00005CDC File Offset: 0x00003EDC
        public HoldoverCardSelectionError(string localizationKey, int? ID = null) : base(localizationKey, ID ?? HoldoverCardSelectionError.GetNewTooltipTypeGUID())
        {
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00005D20 File Offset: 0x00003F20
        public static int GetNewTooltipTypeGUID()
        {
            HoldoverCardSelectionError.NumTypes++;
            return HoldoverCardSelectionError.NumTypes;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005D44 File Offset: 0x00003F44
        public static implicit operator HoldoverCardSelectionError(CardSelectionBehaviour.SelectionError herdSelectionError)
        {
            return ExtendedEnum<HoldoverCardSelectionError, CardSelectionBehaviour.SelectionError>.Convert(herdSelectionError);
        }

        //A number bigger than the types currently available.
        public static int NumTypes = 16;

        public static void Initialize()
        {
            CardSelectionBehaviour selectionBehaviour = new CardSelectionBehaviour();

            Dictionary<CardSelectionBehaviour.SelectionError, string> errors = AccessTools.Field(typeof(CardSelectionBehaviour), "SelectionErrorToMessageKey").GetValue(selectionBehaviour) as Dictionary<CardSelectionBehaviour.SelectionError, string>;

            errors.Add(Beyonder.HoldoverCardSelectionError.GetEnum(), "SelectionError_CardHoldover");

            AccessTools.Field(typeof(CardSelectionBehaviour), "SelectionErrorToMessageKey").SetValue(selectionBehaviour, errors);
        }
    }

    public class CardNotSpawnerSelectionError : ExtendedEnum<CardNotSpawnerSelectionError, CardSelectionBehaviour.SelectionError>
    {
        // Token: 0x06000106 RID: 262 RVA: 0x00005CDC File Offset: 0x00003EDC
        public CardNotSpawnerSelectionError(string localizationKey, int? ID = null) : base(localizationKey, ID ?? CardNotSpawnerSelectionError.GetNewTooltipTypeGUID())
        {
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00005D20 File Offset: 0x00003F20
        public static int GetNewTooltipTypeGUID()
        {
            CardNotSpawnerSelectionError.NumTypes++;
            return CardNotSpawnerSelectionError.NumTypes;
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00005D44 File Offset: 0x00003F44
        public static implicit operator CardNotSpawnerSelectionError(CardSelectionBehaviour.SelectionError herdSelectionError)
        {
            return ExtendedEnum<CardNotSpawnerSelectionError, CardSelectionBehaviour.SelectionError>.Convert(herdSelectionError);
        }

        //A number bigger than the types currently available.
        public static int NumTypes = 17;

        public static void Initialize()
        {
            CardSelectionBehaviour selectionBehaviour = new CardSelectionBehaviour();

            Dictionary<CardSelectionBehaviour.SelectionError, string> errors = AccessTools.Field(typeof(CardSelectionBehaviour), "SelectionErrorToMessageKey").GetValue(selectionBehaviour) as Dictionary<CardSelectionBehaviour.SelectionError, string>;

            errors.Add(Beyonder.CardNotSpawnerSelectionError.GetEnum(), "SelectionError_CardNotSpawner");

            AccessTools.Field(typeof(CardSelectionBehaviour), "SelectionErrorToMessageKey").SetValue(selectionBehaviour, errors);
        }
    }
}