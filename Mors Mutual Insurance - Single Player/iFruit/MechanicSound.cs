using MMI_SP.Config;
using MMI_SP.Dialogue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace MMI_SP.iFruit
{
    internal static class MechanicSound
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal enum SoundFamily { Hello, Confirm, Deny }

        private static readonly Random _rnd = new Random();

        private static readonly List<UnmanagedMemoryStream> _helloList = new List<UnmanagedMemoryStream>
        {
            Properties.Resources.mechanic_1,
            Properties.Resources.mechanic_2,
            Properties.Resources.mechanic_3,
            Properties.Resources.mechanic_4,
            Properties.Resources.mechanic_5
        };

        private static readonly List<UnmanagedMemoryStream> _confirmList = new List<UnmanagedMemoryStream>
        {
            Properties.Resources.mechanic_affirmative_1,
            Properties.Resources.mechanic_affirmative_2,
            Properties.Resources.mechanic_affirmative_3,
            Properties.Resources.mechanic_affirmative_4,
            Properties.Resources.mechanic_affirmative_5,
            Properties.Resources.mechanic_affirmative_6,
            Properties.Resources.mechanic_affirmative_7,
            Properties.Resources.mechanic_affirmative_8,
            Properties.Resources.mechanic_affirmative_9,
            Properties.Resources.mechanic_affirmative_10,
            Properties.Resources.mechanic_affirmative_11,
            Properties.Resources.mechanic_affirmative_12
        };

        private static readonly List<UnmanagedMemoryStream> _denyList = new List<UnmanagedMemoryStream>
        {
            Properties.Resources.mechanic_dont_1,
            Properties.Resources.mechanic_dont_2,
            Properties.Resources.mechanic_dont_3,
            Properties.Resources.mechanic_dont_4,
            Properties.Resources.mechanic_dont_5
        };

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Play(SoundFamily family)
        {
            List<UnmanagedMemoryStream> list = GetList(family);
            if (list.Count == 0) return;

            int index = _rnd.Next(0, list.Count);
            UnmanagedMemoryStream resourceStream = list[index];

            byte[] audioBytes = new byte[resourceStream.Length];
            resourceStream.Position = 0;
            resourceStream.Read(audioBytes, 0, audioBytes.Length);

            using (MemoryStream memStream = new MemoryStream(audioBytes))
            using (var audioPlayer = new AudioPlayer(memStream))
            using (var player = new SoundPlayer(audioPlayer))
            {
                audioPlayer.Volume = ModSettings.iFruitVolume;
                player.Play();
            }
        }

        private static List<UnmanagedMemoryStream> GetList(SoundFamily family)
        {
            switch (family)
            {
                case SoundFamily.Hello: return _helloList;
                case SoundFamily.Confirm: return _confirmList;
                case SoundFamily.Deny: return _denyList;
                default: return new List<UnmanagedMemoryStream>();
            }
        }
    }
}