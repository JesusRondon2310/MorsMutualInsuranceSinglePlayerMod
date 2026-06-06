using System;
using System.Collections.Generic;
using GTA;

namespace MMI_SP.Dialogue
{
    public static class Core
    {
        public enum SpeechType
        {
            OfficeHi,
            OfficeBye,
            OfficeNiceCar,
            OfficeSomething
        }

        // ==========================================
        // BLOQUE 1: Listas de frases
        // ==========================================
        private static readonly List<Speech> OfficeHiCollection = new List<Speech>
        {
            new Speech("GENERIC_HI", "A_F_M_BUSINESS_02_WHITE_MINI_01", SpeechModifier.Force),
            new Speech("GENERIC_HOWS_IT_GOING", "A_F_M_BUSINESS_02_WHITE_MINI_01", SpeechModifier.Force),
            new Speech("GENERIC_HI", "A_F_M_BEVHILLS_01_WHITE_MINI_01", SpeechModifier.Force),
            new Speech("GENERIC_HOWS_IT_GOING", "A_F_M_BEVHILLS_01_WHITE_MINI_01", SpeechModifier.Force),
            new Speech("GENERIC_HI", "A_F_M_BEACH_01_WHITE_FULL_01", SpeechModifier.Force),
            new Speech("GENERIC_HOWS_IT_GOING", "A_F_M_BEACH_01_WHITE_FULL_01", SpeechModifier.Force)
        };

        private static readonly List<Speech> OfficeByeCollection = new List<Speech>
        {
            new Speech("GENERIC_BYE", "A_F_M_BUSINESS_02_WHITE_MINI_01", SpeechModifier.Force),
            new Speech("GOODBYE_ACROSS_STREET", "A_F_M_BUSINESS_02_WHITE_MINI_01", SpeechModifier.Force)
        };

        private static readonly List<Speech> OfficeNiceCarCollection = new List<Speech>
        {
            new Speech("NICE_CAR", "A_F_M_BEACH_01_WHITE_FULL_01", SpeechModifier.Standard),
            new Speech("NICE_CAR", "A_F_M_BEVHILLS_02_WHITE_FULL_01", SpeechModifier.Standard),
            new Speech("NICE_CAR", "A_F_M_BEVHILLS_02_WHITE_FULL_02", SpeechModifier.Standard)
        };

        private static readonly List<Speech> OfficeSomethingCollection = new List<Speech>
        {
            new Speech("GENERIC_HOWS_IT_GOING", "A_F_M_BEVHILLS_01_WHITE_MINI_02", SpeechModifier.Standard),
            new Speech("GENERIC_HOWS_IT_GOING", "A_F_M_BEVHILLS_02_WHITE_MINI_01", SpeechModifier.Standard),
            new Speech("CHALLENGE_ACCEPTED_GENERIC", "A_F_M_BEACH_01_WHITE_FULL_01", SpeechModifier.Standard),
            new Speech("CHAT_RESP", "A_F_M_BEACH_01_WHITE_FULL_01", SpeechModifier.Standard),
            new Speech("GENERIC_WHATEVER", "A_F_M_BEACH_01_WHITE_FULL_01", SpeechModifier.Standard),
            new Speech("NICE_CAR", "A_F_M_BEACH_01_WHITE_FULL_01", SpeechModifier.Standard),
            new Speech("NICE_CAR", "A_F_M_BEVHILLS_02_WHITE_FULL_01", SpeechModifier.Standard),
            new Speech("NICE_CAR", "A_F_M_BEVHILLS_02_WHITE_FULL_02", SpeechModifier.Standard)
        };

        // ==========================================
        // BLOQUE 2: Métodos públicos
        // ==========================================
        public static void PlayRandom(SpeechType type, Ped npc)
        {
            if (npc == null || !npc.Exists()) return;

            List<Speech> pool = GetSpeechList(type);
            if (pool.Count == 0) return;

            Speech chosen = pool[new Random().Next(pool.Count)];
            npc.PlayAmbientSpeech(chosen.Name, chosen.Voice, chosen.Modifier);
        }

        // ==========================================
        // BLOQUE 3: Métodos privados
        // ==========================================
        private static List<Speech> GetSpeechList(SpeechType type)
        {
            switch (type)
            {
                case SpeechType.OfficeHi: return OfficeHiCollection;
                case SpeechType.OfficeBye: return OfficeByeCollection;
                case SpeechType.OfficeNiceCar: return OfficeNiceCarCollection;
                case SpeechType.OfficeSomething: return OfficeSomethingCollection;
                default: return new List<Speech>();
            }
        }

        private class Speech
        {
            internal string Name;
            internal string Voice;
            internal SpeechModifier Modifier;

            internal Speech(string name, string voice, SpeechModifier modifier)
            {
                Name = name;
                Voice = voice;
                Modifier = modifier;
            }
        }
    }
}