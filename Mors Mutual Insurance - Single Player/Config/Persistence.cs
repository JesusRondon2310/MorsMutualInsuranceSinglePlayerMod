using System;
using System.Collections.Generic;
using System.IO;

namespace MMI_SP.Config
{
    internal static class Persistence
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static readonly Dictionary<string, string> _settings = new Dictionary<string, string>();
        private static readonly string IniPath = ModSettings.BaseDir + "\\config.ini";

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Initialize()
        {
            if (!File.Exists(IniPath)) return;

            foreach (string line in File.ReadAllLines(IniPath))
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#")) continue;

                string[] parts = trimmed.Split('=');
                if (parts.Length != 2) continue;

                string[] sectionKey = parts[0].Trim().Split('.');
                if (sectionKey.Length != 2) continue;

                string section = sectionKey[0].Trim();
                string key = sectionKey[1].Trim();
                string value = parts[1].Trim();

                string composite = $"{section}.{key}";
                _settings[composite] = value;
            }
        }

        internal static void LoadFromSettings()
        {
            foreach (KeyValuePair<string, string> entry in _settings)
            {
                string[] sectionKey = entry.Key.Split('.');
                if (sectionKey.Length != 2) continue;

                string key = sectionKey[1];
                string value = entry.Value;

                switch (key)
                {
                    case "InsuranceCostMultiplier": ModSettings.InsuranceMult = Convert.ToSingle(value); break;
                    case "RecoverCostMultiplier": ModSettings.RecoverMult = Convert.ToSingle(value); break;
                    case "PersistentInsuredVehicles": ModSettings.PersistentVehicles = Convert.ToBoolean(value); break;
                    case "PhoneVolume": ModSettings.iFruitVolume = Convert.ToInt32(value); break;
                    case "CaniFruitInsure": ModSettings.CaniFruitInsure = Convert.ToBoolean(value); break;
                    case "CaniFruitCancel": ModSettings.CaniFruitCancel = Convert.ToBoolean(value); break;
                    case "CaniFruitRecover": ModSettings.CaniFruitRecover = Convert.ToBoolean(value); break;
                    case "BringVehicleBasePrice": ModSettings.BringVehicleBasePrice = Convert.ToInt32(value); break;
                    case "BringVehicleInstant": ModSettings.BringVehicleInstant = Convert.ToBoolean(value); break;
                    case "BringVehicleRadius": ModSettings.BringVehicleRadius = Convert.ToInt32(value); break;
                    case "BringVehicleTimeout": ModSettings.BringVehicleTimeout = Convert.ToInt32(value); break;
                }
            }
        }

        internal static void SetSetting(string section, string key, string value)
        {
            string composite = $"{section}.{key}";
            _settings[composite] = value;
            SaveSettings();
        }

        internal static string GetSetting(string section, string key, string defaultValue = "")
        {
            string composite = $"{section}.{key}";
            return _settings.TryGetValue(composite, out string value) ? value : defaultValue;
        }

        internal static void SaveSettings()
        {
            using (StreamWriter writer = new StreamWriter(IniPath, false))
            {
                writer.WriteLine("; MMI-SP Configuration");
                writer.WriteLine();
                foreach (KeyValuePair<string, string> entry in _settings)
                {
                    string[] sectionKey = entry.Key.Split('.');
                    if (sectionKey.Length == 2)
                        writer.WriteLine($"{sectionKey[0]}.{sectionKey[1]}={entry.Value}");
                }
            }
        }
    }
}