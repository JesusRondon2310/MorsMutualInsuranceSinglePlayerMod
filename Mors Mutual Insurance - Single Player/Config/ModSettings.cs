using GTA.Math;
using MMI_SP.PatternMatching;
using MMI_SP.Helpers;
using System;
using System.IO;

namespace MMI_SP.Config
{
    internal static class ModSettings
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory + Constants.DEFAULT_BASE_DIR_EXPR;
        internal static readonly string BannerImage = BaseDir + Constants.DEFAULT_MMI_BANNER_IMAGE_EXPR;
        internal static readonly string InsuranceImage = BaseDir + Constants.DEFAULT_INSURANCE_IMAGE_EXPR;

        public static float InsuranceMult { get; set; } = Constants.DEFAULT_INSURANCE_MULT;
        public static float RecoverMult { get; set; } = Constants.DEFAULT_RECOVER_MULT;
        public static bool PersistentVehicles { get; set; } = true;
        public static Vector3 PlayerPos => new Vector3(Constants.DEFAULT_PLAYER_POS_X, Constants.DEFAULT_PLAYER_POS_Y, Constants.DEFAULT_PLAYER_POS_Z);
        public static int iFruitVolume { get; set; } = Constants.DEFAULT_IFRUIT_VOLUME;
        public static bool CaniFruitInsure { get; set; } = true;
        public static bool CaniFruitCancel { get; set; } = true;
        public static bool CaniFruitRecover { get; set; } = true;

        public static int BringVehicleBasePrice { get; set; } = Constants.DEFAULT_BRING_BASE_PRICE;
        public static bool BringVehicleInstant { get; set; } = false;
        public static int BringVehicleRadius { get; set; } = Constants.DEFAULT_BRING_RADIUS;
        public static int BringVehicleTimeout { get; set; } = Constants.DEFAULT_BRING_TIMEOUT;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<bool> Initialize()
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            if (!File.Exists(InsuranceImage))
                File.Copy(Constants.INSURANCE_IMAGE_SOURCE, InsuranceImage);

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