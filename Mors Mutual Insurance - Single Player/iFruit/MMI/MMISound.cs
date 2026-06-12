using MMI_SP.Config;
using MMI_SP.Dialogue;
using MMI_SP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace MMI_SP.iFruit.MMI
{
    internal static class MMISound
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal enum SoundFamily { Hello, Okay, Bye, NoMoney }

        private static readonly Random _rnd = new Random();

        private static readonly List<UnmanagedMemoryStream> _helloList = new List<UnmanagedMemoryStream>
        {
            Properties.Resources.Start_HelloThisIsMMI,
            Properties.Resources.Start_MMIExpectUnexpected,
            Properties.Resources.Start_MMIHereToHelp,
            Properties.Resources.Start_MMIHowCanHelp,
            Properties.Resources.Start_MMIHowCanIBeService,
            Properties.Resources.Start_MMIPeaceOfMind,
            Properties.Resources.Start_MMITrust,
            Properties.Resources.Start_WhatCanIDo,
            Properties.Resources.Start_WhatCanIHelpYouWith
        };

        private static readonly List<UnmanagedMemoryStream> _okayList = new List<UnmanagedMemoryStream>
        {
            Properties.Resources.Mid_ICanDoThat,
            Properties.Resources.Mid_ILookIntoIt,
            Properties.Resources.Mid_IWillDoMyBest,
            Properties.Resources.Mid_Okay,
            Properties.Resources.Mid_Sure,
            Properties.Resources.Mid_WeCanDoThat,
            Properties.Resources.Mid_WeCanHandleThat
        };

        private static readonly List<UnmanagedMemoryStream> _byeList = new List<UnmanagedMemoryStream>
        {
            Properties.Resources.End_ByeNow,
            Properties.Resources.End_DriveSafe,
            Properties.Resources.End_NiceDay,
            Properties.Resources.End_NiveDay2,
            Properties.Resources.End_SoLong,
            Properties.Resources.End_StaySafe
        };

        private static readonly List<UnmanagedMemoryStream> _noMoneyList = new List<UnmanagedMemoryStream>
        {
            Properties.Resources.NoMoney
        };

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Play(SoundFamily family)
        {
            List<UnmanagedMemoryStream> list = GetList(family);
            if (list.Count == Constants.ZERO) return;

            int index = _rnd.Next(Constants.FIRST_INDEX, list.Count);
            UnmanagedMemoryStream resourceStream = list[index];

            byte[] audioBytes = new byte[resourceStream.Length];
            resourceStream.Position = Constants.ZERO;
            resourceStream.Read(audioBytes, Constants.ZERO, audioBytes.Length);

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
                case SoundFamily.Okay: return _okayList;
                case SoundFamily.Bye: return _byeList;
                case SoundFamily.NoMoney: return _noMoneyList;
                default: return new List<UnmanagedMemoryStream>();
            }
        }
    }
}