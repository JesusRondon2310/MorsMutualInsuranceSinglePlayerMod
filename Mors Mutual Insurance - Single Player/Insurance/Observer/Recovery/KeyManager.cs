using GTA;
using MMI_SP.DB;
using MMI_SP.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer.Recovery
{
    internal static class KeyManager
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static VehicleData FindVehicleDataByPlate(string plate)
        {
            var allVehicles = DB.Core.GetAll();
            foreach (var vd in allVehicles)
            {
                if (vd.Plate == plate) return vd;
            }
            return null;
        }

        internal static void RemoveRecoveryKey(Vehicle playerVeh)
        {
            if (playerVeh == null || !playerVeh.Exists()) return;
            var vd = FindVehicleDataByPlate(playerVeh.Mods.LicensePlate);
            if (vd != null) {
                string recoveryKey = VehicleKey.FullKeyFrom(vd);
                Manager.RecoveredVehicleKeys.Remove(recoveryKey);
            }
        }

        internal static bool IsRecoveredAndNotDriven(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;
            var vd = FindVehicleDataByPlate(veh.Mods.LicensePlate);
            if (vd != null)
            {
                string key = VehicleKey.FullKeyFrom(vd);
                return Manager.RecoveredVehicleKeys.Contains(key) && !Game.Player.Character.IsInVehicle(veh);
            }
            return false;
        }

        internal static void CleanupOrphanedKeys()
        {
            var allPlates = DB.Core.GetAll().Select(d => d.Plate).ToHashSet();
            var keysToRemove = new List<string>();

            foreach (string key in Manager.RecoveredVehicleKeys)
            {
                // El formato esperado es: ModelName_Plate_Id
                var parts = key.Split('_');
                if (parts.Length < 2) {
                    keysToRemove.Add(key);
                    continue;
                }
                string plate = parts[1]; // segundo componente es la placa
                if (!allPlates.Contains(plate)) keysToRemove.Add(key);
            }

            foreach (string key in keysToRemove) Manager.RecoveredVehicleKeys.Remove(key);
        }
    }
}