using System;
using GTA;
using MMI_SP.Insurance.Policies;
using MMI_SP.Helpers;
using MMI_SP.Debug;

namespace MMI_SP
{
    internal class MMI : Script
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        public static bool IsDebug = false;
        private static bool _initialized = false;
        public static bool IsInitialized => _initialized;

        // ==========================================
        // BLOQUE 2: Constructor
        // ==========================================
        public MMI()
        {
        #if DEBUG
            IsDebug = true;
        #endif
            Tick += Initialize;
        }

        // ==========================================
        // BLOQUE 3: Funciones
        // ==========================================
        private void Initialize(object sender, EventArgs e)
        {
            Logger.ResetLogFile();

            while (GTA.UI.Screen.IsFadingIn) Yield();

            Config.ModSettings.Initialize();

            var result = Manager.Initialize();

            result.match<bool>(
                onOk: _ => {
                    _initialized = true;
                    return true;
                },
                onErr: error => {
                    Logger.Error($"InsuranceManager initialization failed: {error}");
                    Notification.ShowMMI("~r~Error~w~ al iniciar MMI-SP", error);
                    return false;
                }
            );

            Tick -= Initialize;
        }
    }
}