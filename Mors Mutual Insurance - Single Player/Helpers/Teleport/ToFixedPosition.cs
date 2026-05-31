using GTA;
using GTA.Math;
using MMI_SP.Helpers.Spawn;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.PatternMatching;

namespace MMI_SP.Helpers.Teleport
{
    public static class ToFixedPosition
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<Vehicle> Execute(DB.VehicleData data, EntityPosition spawnPos)
        {
            var fixedPos = FixedSpawnHandler.FixGround(spawnPos.Position);
            if (fixedPos.is_err()) return new Err<Vehicle>(((Err<Vector3>)fixedPos).Message);

            Vector3 finalPos = ((Ok<Vector3>)fixedPos).Value;

            var updatedData = new DB.VehicleData(data.Id, data.ModelName, data.Plate, data.PrimaryColor, data.SecondaryColor, false,
                windowTint: data.WindowTint, wheelType: data.WheelType, wheelColor: data.WheelColor, tireSmokeColor: data.TireSmokeColor,
                neonLeft: data.NeonLeft, neonRight: data.NeonRight, neonFront: data.NeonFront, neonBack: data.NeonBack, neonColor: data.NeonColor,
                bulletproofTires: data.BulletproofTires, posX: finalPos.X, posY: finalPos.Y, posZ: finalPos.Z, heading: spawnPos.Heading,
                mods: data.Mods, destroyedAt: data.DestroyedAt, isLocked: data.IsLocked, plateStyle: data.PlateStyle, customTires: data.CustomTires,
                isDormant: data.IsDormant, isInGarage: data.IsInGarage, vehicleType: data.VehicleType
                );

            var spawnResult = VehicleDeliverySpawner.SpawnVehicleForDelivery(updatedData, validatePosition: false);
            if (spawnResult.is_err()) return spawnResult;
            Vehicle veh = spawnResult.unwrap_or(null);

            ToRoad.Execute(veh);

            return new Ok<Vehicle>(veh);
        }
    }
}