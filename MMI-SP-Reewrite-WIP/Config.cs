using System;
using System.IO;
using MMI_SP.PatternMatching;

namespace MMI_SP
{
    internal static class Config
    {
        // ==========================================
        // BLOQUE 1: Variables de la clase y creación del objeto
        // ==========================================
        internal static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory + "\\MMI";
        internal static readonly string BannerImage = BaseDir + "\\banner.png";
        public static float InsuranceMult { get; } = 1.0f;
        public static bool PersistentVehicles { get; } = true;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<bool> Initialize()
        {
            try
            {
                if (!Directory.Exists(BaseDir))
                {
                    Logger.Debug("Creando carpeta MMI...");
                    Directory.CreateDirectory(BaseDir);
                }
                return new Ok<bool>(true);
            }
            catch (Exception ex)
            {
                return new Err<bool>(ex.Message);
            }
        }
    }
}