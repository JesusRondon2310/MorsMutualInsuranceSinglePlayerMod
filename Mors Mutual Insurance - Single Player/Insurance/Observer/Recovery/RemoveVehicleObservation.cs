using GTA;
using MMI_SP.DB;
using MMI_SP.Dormancy;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
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
            if (insuredVeh != null)
            {
                BlipCleanupHandler.RemoveByVehicle(insuredVeh, blipsToRemove);
                insuredVehList.Remove(insuredVeh);
            }

            // 2. Eliminar de la lista de recuperados y su blip
            Vehicle recoveredVeh = recoveredVehList.FirstOrDefault(v => VehicleIdentifier.Get(v) == vehicleId);
            if (recoveredVeh != null)
            {
                BlipCleanupHandler.RemoveByVehicle(recoveredVeh, blipsToRemove);
                recoveredVehList.Remove(recoveredVeh);
            }

            // 3. Eliminar de durmientes usando VehicleKey
            var dataOption = DB.Core.FindVehicle(vehicleId);
            string targetKey = dataOption.match(
                onSome: vd => VehicleKey.From(vd),
                onNone: () => string.Empty
            );

            if (!string.IsNullOrEmpty(targetKey))
            {
                var dormant = Dormancy.Core.DormantVehicles.FirstOrDefault(d =>
                    d.Data != null && VehicleKey.From(d.Data) == targetKey);
                if (dormant != null) Dormancy.Core.DormantVehicles.Remove(dormant);
            }

            // 4. Eliminar clave de protección temporal
            dataOption.match<bool>(
                onSome: vd =>
                {
                    string recoveryKey = VehicleKey.From(vd);
                    Manager.RecoveredVehicleKeys.Remove(recoveryKey);
                    return true;
                },
                onNone: () => false
            );
        }
    }
}