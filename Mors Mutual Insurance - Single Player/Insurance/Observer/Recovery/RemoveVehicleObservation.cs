using GTA;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer.Recovery
{
    internal static class RemoveVehicleObservation
    {
        public static void Execute(string vehicleId,
            List<Vehicle> insuredVehList,
            List<Vehicle> recoveredVehList,
            Dictionary<string, Blip> blipsToRemove)
        {
            // 1. Eliminar de la lista de asegurados y su blip
            Vehicle insuredVeh = insuredVehList.FirstOrDefault(v => VehicleIdentifier.Get(v) == vehicleId);
            if (insuredVeh != null) {
                BlipCleanupHandler.RemoveByVehicle(insuredVeh, blipsToRemove);
                insuredVehList.Remove(insuredVeh);
                Logger.Debug($"[RemoveVehicleObservation] Vehículo asegurado eliminado: {vehicleId}");
            }

            // 2. Eliminar de la lista de recuperados y su blip
            Vehicle recoveredVeh = recoveredVehList.FirstOrDefault(v => VehicleIdentifier.Get(v) == vehicleId);
            if (recoveredVeh != null) {
                BlipCleanupHandler.RemoveByVehicle(recoveredVeh, blipsToRemove);
                recoveredVehList.Remove(recoveredVeh);
                Logger.Debug($"[RemoveVehicleObservation] Vehículo recuperado eliminado: {vehicleId}");
            }

            // 3. Obtener datos del vehículo (si existe)
            var dataOption = DB.Core.FindVehicle(vehicleId);
            if (dataOption.is_some())
            {
                var vd = ((Some<VehicleData>)dataOption).Value;
                string fullKey = VehicleKey.FullKeyFrom(vd);

                // 4. Eliminar de durmientes usando la clave completa
                if (Dormancy.Core.RemoveDormantByKey(fullKey)) Logger.Debug($"[RemoveVehicleObservation] Vehículo durmiente eliminado: {fullKey}");

                // 5. Eliminar clave de protección temporal
                if (Manager.RecoveredVehicleKeys.Remove(fullKey))
                    Logger.Debug($"[RemoveVehicleObservation] Clave de recuperación eliminada: {fullKey}");
                else
                    Logger.Debug($"[RemoveVehicleObservation] Clave de recuperación no encontrada: {fullKey}");
            }
            else {
                Logger.Warning($"[RemoveVehicleObservation] No se encontró VehicleData para ID: {vehicleId}");
            }
        }
    }
}