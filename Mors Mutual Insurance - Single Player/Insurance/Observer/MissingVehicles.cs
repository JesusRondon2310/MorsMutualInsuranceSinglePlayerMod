using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class MissingVehicles
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Handle(List<Vehicle> insuredVehList, int i, Vehicle currentVeh)
        {
            // 1. Obtener ID del vehículo
            string vehId = VehicleIdentifier.Get(currentVeh);

            // 2. Buscar sus datos en la BD
            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none())
            {
                insuredVehList.RemoveAt(i);
                return;
            }

            // Cast directo después de is_some() (excepción controlada)
            VehicleData data = ((Some<VehicleData>)dataOption).Value;

            // 3. Si está destruido o dormante, eliminarlo de la lista
            if (data.IsDestroyed || data.IsDormant)
            {
                insuredVehList.RemoveAt(i);
                return;
            }

            // 4. Si está bloqueado, despertarlo (evitar que se duerma)
            if (data.IsLocked)
            {
                DB.Core.SetDormant(vehId, false);
                return;
            }

            // 5. Verificar posición actual en BD (si es válida)
            Vector3 lastPos = new Vector3(data.PosX, data.PosY, data.PosZ);

            // 6. Determinar si es un vehículo recién recuperado
            string recoveryKey = VehicleKey.FullKeyFrom(data);
            bool isRecoveredVehicle = Manager.RecoveredVehicleKeys.Contains(recoveryKey);

            // 7. Calcular distancia a la última posición guardada
            float distanceToPlayer = Game.Player.Character.Position.DistanceTo(lastPos);

            // 8. Si está cerca del jugador o es recuperado y no está siendo conducido, despertarlo
            if (distanceToPlayer < Constants.DORMANCY_THRESHOLD || (isRecoveredVehicle && !Game.Player.Character.IsInVehicle(currentVeh)))
            {
                DB.Core.SetDormant(vehId, false);
                return;
            }

            // 9. En caso contrario, marcar como dormante y eliminarlo de la lista
            Dormancy.Core.MarkAsDormant(vehId);
            insuredVehList.RemoveAt(i);
            Logger.Warning($"[VehicleMonitor] Despawn involuntario del engine: {vehId}");
        }
    }
}