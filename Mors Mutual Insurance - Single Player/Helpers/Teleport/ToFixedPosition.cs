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

            var updatedData = data.With(d => {
                d.PosX = finalPos.X;
                d.PosY = finalPos.Y;
                d.PosZ = finalPos.Z;
                d.Heading = spawnPos.Heading;
                d.IsDestroyed = false;
            });

            var spawnResult = VehicleDeliverySpawner.SpawnVehicleForDelivery(updatedData, validatePosition: false);
            if (spawnResult.is_err()) return spawnResult;
            Vehicle veh = spawnResult.unwrap_or(null);

            ToRoad.Execute(veh);

            return new Ok<Vehicle>(veh);
        }
    }
}