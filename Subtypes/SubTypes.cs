using Void.Init;
using Trainworks.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Void.Unit
{
    class SubtypeVeilrich
    {
        public static readonly string Key = Beyonder.GUID + "_Subtype_Veilrich";

        public static void BuildAndRegister()
        {
            CustomCharacterManager.RegisterSubtype(Key, "Veilrich");
        }
    }
    class SubtypeUndretch
    {
        public static readonly string Key = Beyonder.GUID + "_Subtype_Undretch";

        public static void BuildAndRegister()
        {
            CustomCharacterManager.RegisterSubtype(Key, "Undretch");
        }
    }
}