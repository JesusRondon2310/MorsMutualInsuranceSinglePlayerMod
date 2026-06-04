using GTA;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Insurance.Policies;
using System;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    public class Manager : Script
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Manager _instance;
        private static List<Vehicle> _recoveredVehList = new List<Vehicle>();
        private static Dictionary<string, Blip> _blipsToRemove = new Dictionary<string, Blip>();
        private static bool _initialized = false;
        private static List<Vehicle> _insuredVehList = new List<Vehicle>();
        internal static HashSet<string> RecoveredVehicleKeys = new HashSet<string>();
        internal static Manager Instance => _instance;
        internal static bool Initialized => _initialized;
        internal static List<Vehicle> InsuredVehList => _insuredVehList;
        internal static List<Vehicle> RecoveredVehList => _recoveredVehList;
        internal static Dictionary<string, Blip> BlipsToRemove => _blipsToRemove;

        // ==========================================
        // BLOQUE 2: Constructor y eventos del Script
        // ==========================================
        public Manager()
        {
            _instance = this;
            Tick += Initialize;
        }

        private void Initialize(object sender, EventArgs e)
        {
            while (!MMI.IsInitialized) Yield();

            _initialized = true;

            Timers.Initialize();

            Initializer.RestoreVehiclesFromDatabase(_insuredVehList, _blipsToRemove);

            Tick -= Initialize;
            Aborted += OnAborted;
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Insurer.Instance == null) return;

            // Timers centralizados
            Timers.CheckAndExecute("Insurance", () =>
                VehicleMonitor.UpdateInsurance(Insurer.Instance, _insuredVehList, _blipsToRemove));

            Timers.CheckAndExecute("DetectInsuredVehicles", () =>
                VehicleMonitor.CheckForInsuredVehicles(Insurer.Instance, _insuredVehList, _blipsToRemove));

            Timers.CheckAndExecute("RecoveredVehicle", () => Recovery.Handler.UpdateRecoveredVehicles(_recoveredVehList));

            Timers.CheckAndExecute("IncomingVehicle", () => Delivery.Manager.UpdateIncomingVehicles());

            Timers.CheckAndExecute("CleanupRecoveredKeys", () => Recovery.KeyManager.CleanupOrphanedKeys());

            Timers.CheckAndExecute("AliveVehicleSave", () =>
            {
                Vehicle playerVeh = Game.Player.Character.CurrentVehicle;
                if (playerVeh != null && playerVeh.Exists() && Insurer.Instance.IsInsured(playerVeh))
                {
                    Insurer.Instance.UpdateVehicleData(playerVeh).match<bool>(
                        onOk: _ => true,
                        onErr: error =>
                        {
                            Logger.Error($"Error al actualizar datos del vehículo: {error}");
                            return false;
                        }
                    );
                }
            });

            // Eliminar protección de dormancia cuando el jugador sube al vehículo recuperado
            Vehicle playerVehForRecovery = Game.Player.Character.CurrentVehicle;
            if (playerVehForRecovery != null && playerVehForRecovery.Exists()) Recovery.Handler.RemoveRecoveryKey(playerVehForRecovery);

            VehicleChangeHandler.Handle(_insuredVehList, _recoveredVehList, _blipsToRemove);

            LockVehicle.Handler.ShowHint(_insuredVehList);
            LockVehicle.Handler.Update(_insuredVehList, Insurer.Instance);
        }

        private void OnAborted(object sender, EventArgs e)
        {
            BlipCleanupHandler.ClearAll(_blipsToRemove);
            VehiclePersistence.RemovePersistence(_recoveredVehList);
        }

        public static void RemoveVehicleFromObservation(string vehicleId)
        {
            Recovery.RemoveVehicleObservation.Execute(vehicleId, InsuredVehList, RecoveredVehList, BlipsToRemove);
        }

        public static bool IsRecoveredAndNotDriven(Vehicle veh)
        {
            return Recovery.Handler.IsRecoveredAndNotDriven(veh);
        }
    }
}