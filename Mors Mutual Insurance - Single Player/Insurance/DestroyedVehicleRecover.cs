using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Spawn;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal static class DestroyedVehicleRecover
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<Vehicle> RecoverVehicle(string vehicleId, Insurer insurer)
        {
            // 1. Validar ID
            if (string.IsNullOrEmpty(vehicleId)) return new Err<Vehicle>("ID de vehículo no válido.");

            // 2. Obtener datos del vehículo desde el seguro
            var recoverResult = insurer.RecoverVehicle(vehicleId);
            if (recoverResult.is_err()) return new Err<Vehicle>(((Err<VehicleData>)recoverResult).Message);
            VehicleData data = ((Ok<VehicleData>)recoverResult).Value;

            // 3. Elegir nodo de recuperación (posición y heading)
            var nodeResult = RecoverNodeSelector.GetNode(data);
            if (nodeResult.is_err()) return new Err<Vehicle>(((Err<EntityPosition>)nodeResult).Message);
            EntityPosition spawnPos = ((Ok<EntityPosition>)nodeResult).Value;

            // 4. Ajustar altura del suelo en la posición elegida
            var fixedPosResult = SpawnHandler.FixGround(spawnPos.Position);
            if (fixedPosResult.is_err()) return new Err<Vehicle>(((Err<Vector3>)fixedPosResult).Message);
            Vector3 finalPos = ((Ok<Vector3>)fixedPosResult).Value;

            // 5. Actualizar datos del vehículo (posición, heading, flags)
            var updatedData = data.With(d =>
            {
                d.PosX = finalPos.X;
                d.PosY = finalPos.Y;
                d.PosZ = finalPos.Z;
                d.Heading = spawnPos.Heading;
                d.IsLocked = true;
                d.IsDormant = false;
                d.IsInGarage = false;
                d.IsDestroyed = false;
            });

            // 6. Spawnear el vehículo (sin revalidar carretera, ya tenemos posición corregida)
            var spawnResult = VehicleSpawnManager.SpawnVehicleForDelivery(updatedData, adjustToRoadNode: false);
            if (spawnResult.is_err()) return new Err<Vehicle>(((Err<Vehicle>)spawnResult).Message);
            Vehicle spawned = ((Ok<Vehicle>)spawnResult).Value;

            // 7. Registrar clave del vehículo recuperado (evita duplicados)
            string recoveryKey = VehicleKey.FullKeyFrom(updatedData);
            Observer.Manager.RecoveredVehicleKeys.Add(recoveryKey);

            // 8. Actualizar la base de datos con la nueva posición
            DB.Core.Update(updatedData).match<bool>(
                onOk: _ => true,
                onErr: error => {
                    Logger.Error($"Error al actualizar posición de spawn en DB: {error}");
                    return false;
                }
            );

            // 9. Delegar la gestión de blips y listas al módulo de recuperación
            Observer.Recovery.Handler.OnVehicleRecovered(spawned, null, Observer.Manager.RecoveredVehList, Observer.Manager.BlipsToRemove);
            RecoveryBlipHandler.CreateFlashingBlip(spawned, Observer.Manager.BlipsToRemove);

            return new Ok<Vehicle>(spawned);
        }
    }
}