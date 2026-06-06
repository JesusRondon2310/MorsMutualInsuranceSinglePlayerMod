using GTA.Math;
using MMI_SP.PatternMatching;
using System;
using System.IO;

namespace MMI_SP.Config
{
    internal static class ModSettings
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory + "\\MMI";
        internal static readonly string BannerImage = BaseDir + "\\banner.png";
        internal static readonly string InsuranceImage = BaseDir + "\\insurance.png";

        public static float InsuranceMult { get; set; } = 1.0f;
        public static float RecoverMult { get; set; } = 1.0f;
        public static bool PersistentVehicles { get; set; } = true;
        public static Vector3 PlayerPos => new Vector3(-822.528f, -260.00f, 35.79341f);

        public static int iFruitVolume { get; set; } = 25;
        public static bool CaniFruitInsure { get; set; } = true;
        public static bool CaniFruitCancel { get; set; } = true;
        public static bool CaniFruitRecover { get; set; } = true;

        public static int BringVehicleBasePrice { get; set; } = 1500;
        public static bool BringVehicleInstant { get; set; } = false;
        public static int BringVehicleRadius { get; set; } = 100;
        public static int BringVehicleTimeout { get; set; } = 5;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<bool> Initialize()
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            if (!File.Exists(InsuranceImage))
                File.Copy("Resources/insurance.png", InsuranceImage);

            Persistence.Initialize();
            Persistence.LoadFromSettings();

            return new Ok<bool>(true);
        }

        internal static void UpdateValue(string key, object value)
        {
            switch (key)
            {
                case "InsuranceCostMultiplier": InsuranceMult = Convert.ToSingle(value); break;
                case "RecoverCostMultiplier": RecoverMult = Convert.ToSingle(value); break;
                case "PersistentInsuredVehicles": PersistentVehicles = Convert.ToBoolean(value); break;
                case "PhoneVolume": iFruitVolume = Convert.ToInt32(value); break;
                case "CaniFruitInsure": CaniFruitInsure = Convert.ToBoolean(value); break;
                case "CaniFruitCancel": CaniFruitCancel = Convert.ToBoolean(value); break;
                case "CaniFruitRecover": CaniFruitRecover = Convert.ToBoolean(value); break;
                case "BringVehicleBasePrice": BringVehicleBasePrice = Convert.ToInt32(value); break;
                case "BringVehicleInstant": BringVehicleInstant = Convert.ToBoolean(value); break;
                case "BringVehicleRadius": BringVehicleRadius = Convert.ToInt32(value); break;
                case "BringVehicleTimeout": BringVehicleTimeout = Convert.ToInt32(value); break;
            }
        }
    }
}