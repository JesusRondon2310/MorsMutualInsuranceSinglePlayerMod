using GTA;
using GTA.Native;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer.Recovery
{
    internal static class KeyManager
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Dictionary<string, VehicleData> GetModelPlateDictionary()
        {
            return DB.Core.GetAll().ToDictionary(vd => $"{vd.ModelName}_{vd.Plate}", vd => vd);
        }

        private static string GetModelNameFromVehicle(Vehicle veh)
        {
            return Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
        }

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static VehicleData FindVehicleDataByModelPlate(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return null;
            string modelName = GetModelNameFromVehicle(veh);
            string plate = veh.Mods.LicensePlate;
            string key = $"{modelName}_{plate}";
            var dict = GetModelPlateDictionary();
            return dict.TryGetValue(key, out var vd) ? vd : null;
        }

        internal static VehicleData FindVehicleDataByPlate(string plate)
        {
            if (string.IsNullOrEmpty(plate)) return null;
            return DB.Core.GetAll().FirstOrDefault(vd => vd.Plate == plate);
        }

        internal static void RemoveRecoveryKey(Vehicle playerVeh)
        {
            if (playerVeh == null || !playerVeh.Exists()) return;
            var vd = FindVehicleDataByModelPlate(playerVeh);
            if (vd != null)
            {
                string recoveryKey = VehicleKey.FullKeyFrom(vd);
                if (Manager.RecoveredVehicleKeys.Remove(recoveryKey))
                    Logger.Debug($"[KeyManager] Clave de recuperación removida: {recoveryKey}");
                else
                    Logger.Debug($"[KeyManager] Clave no encontrada: {recoveryKey}");
            }
        }

        internal static bool IsRecoveredAndNotDriven(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;
            var vd = FindVehicleDataByModelPlate(veh);
            if (vd != null)
            {
                string key = VehicleKey.FullKeyFrom(vd);
                return Manager.RecoveredVehicleKeys.Contains(key) && !Game.Player.Character.IsInVehicle(veh);
            }
            return false;
        }

        internal static void CleanupOrphanedKeys()
        {
            var validKeys = DB.Core.GetAll().Select(vd => $"{vd.ModelName}_{vd.Plate}").ToHashSet();
            var keysToRemove = new List<string>();

            foreach (string key in Manager.RecoveredVehicleKeys)
            {
                var parts = key.Split('_');
                if (parts.Length != Constants.RECOVERY_KEY_PARTS)
                {
                    keysToRemove.Add(key);
                    continue;
                }
                string model = parts[Constants.FIRST_INDEX];
                string plate = parts[Constants.PLATE_INDEX];
                string modelPlateKey = $"{model}_{plate}";
                if (!validKeys.Contains(modelPlateKey))
                    keysToRemove.Add(key);
            }

            foreach (string key in keysToRemove)
            {
                Manager.RecoveredVehicleKeys.Remove(key);
                Logger.Debug($"[KeyManager] Clave huérfana eliminada: {key}");
            }
        }
    }
}