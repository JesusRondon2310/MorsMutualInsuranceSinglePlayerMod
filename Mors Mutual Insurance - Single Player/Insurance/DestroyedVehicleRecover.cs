using GTA;
using GTA.Math;
using MMI_SP.Debug;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Spawn;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal static class DestroyedVehicleRecover
    {
        internal static Result<Vehicle> RecoverVehicle(string vehicleId, Insurer insurer)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<Vehicle>("ID de vehículo no válido.");

            var recoverResult = insurer.RecoverVehicle(vehicleId);
            if (recoverResult is Err<DB.VehicleData> err) return new Err<Vehicle>(err.Message);
            DB.VehicleData data = recoverResult.unwrap_or(null);

            var nodeResult = RecoverNodeSelector.GetNode(data);
            if (nodeResult.is_err()) return new Err<Vehicle>(((Err<EntityPosition>)nodeResult).Message);
            EntityPosition spawnPos = nodeResult.unwrap_or(default(EntityPosition));

            var fixedPos = FixedSpawnHandler.FixGround(spawnPos.Position);
            if (fixedPos.is_err()) return new Err<Vehicle>(((Err<Vector3>)fixedPos).Message);
            Vector3 finalPos = fixedPos.unwrap_or(Vector3.Zero);

            var updatedData = new DB.VehicleData(
                data.Id, data.ModelName, data.Plate,
                data.PrimaryColor, data.SecondaryColor, false,
                windowTint: data.WindowTint,
                wheelType: data.WheelType,
                wheelColor: data.WheelColor,
                tireSmokeColor: data.TireSmokeColor,
                neonLeft: data.NeonLeft,
                neonRight: data.NeonRight,
                neonFront: data.NeonFront,
                neonBack: data.NeonBack,
                neonColor: data.NeonColor,
                bulletproofTires: data.BulletproofTires,
                posX: finalPos.X, posY: finalPos.Y,
                posZ: finalPos.Z, heading: spawnPos.Heading,
                mods: data.Mods,
                destroyedAt: data.DestroyedAt,
                isLocked: true,
                plateStyle: data.PlateStyle,
                customTires: data.CustomTires,
                isDormant: false,
                isInGarage: false,
                vehicleType: data.VehicleType);

            // Primero intentamos spawnear (sin tocar la DB)
            var spawnResult = VehicleDeliverySpawner.SpawnVehicleForDelivery(updatedData, validatePosition: false);
            if (spawnResult is Err<Vehicle> spawnErr) return spawnErr;
            Vehicle spawned = spawnResult.unwrap_or(null);

            // Solo si el spawn fue exitoso, actualizamos la DB y añadimos protección
            string recoveryKey = Helpers.VehicleKey.From(updatedData);
            Observer.Manager.RecoveredVehicleKeys.Add(recoveryKey);

            DB.Core.Update(updatedData).match<bool>(
                onOk: _ => true,
                onErr: error =>
                {
                    Logger.Error($"Error al actualizar posición de spawn en DB: {error}");
                    return false;
                }
            );

            Observer.Recovery.Handler.OnVehicleRecovered(spawned, null, Observer.Manager.RecoveredVehList, Observer.Manager.BlipsToRemove);
            RecoveryBlipHandler.CreateFlashingBlip(spawned, Observer.Manager.BlipsToRemove);

            return new Ok<Vehicle>(spawned);
        }
    }
}