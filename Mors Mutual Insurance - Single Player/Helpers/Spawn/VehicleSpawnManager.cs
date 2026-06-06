using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.PatternMatching;

namespace MMI_SP.Helpers.Spawn
{
    public static class VehicleSpawnManager
    {
        // ==========================================
        // Métodos privados
        // ==========================================

        private static Result<Model> LoadModel(string modelName, int timeoutMs, bool retryOnce)
        {
            Model model = new Model(modelName);
            bool loaded = retryOnce
                ? model.Request(timeoutMs) || model.Request(timeoutMs)
                : model.Request(timeoutMs);

            if (!loaded) {
                model.MarkAsNoLongerNeeded();
                return new Err<Model>($"No se pudo cargar el modelo {modelName}. Dale las gracias a la kk de C#.");
            }

            return new Ok<Model>(model);
        }

        private static (Vector3 Position, float Heading) ResolveSpawnPosition(VehicleData data, bool adjustToRoadNode)
        {
            Vector3 spawnPos = new Vector3(data.PosX, data.PosY, data.PosZ);
            float heading = data.Heading;

            if (adjustToRoadNode) {
                var nodeResult = SpawnHandler.GetValidRoad(spawnPos.X, spawnPos.Y, spawnPos.Z);
                if (nodeResult.is_ok()) spawnPos = ((Ok<Vector3>)nodeResult).Value;

                heading = SpawnHandler.GetValidOrientation(spawnPos, data.Heading);
            }

            return (spawnPos, heading);
        }

        private static Result<Vehicle> CreateVehicleInternal(Model model, Vector3 position, float heading)
        {
            Vehicle veh = World.CreateVehicle(model, position, heading);
            if (veh == null || !veh.Exists()) return new Err<Vehicle>("No se pudo crear el vehículo.");

            return new Ok<Vehicle>(veh);
        }

        // ==========================================
        // API pública
        // ==========================================

        // Spawn para vehículos persistentes (garajes, restauración)
        public static Result<Vehicle> SpawnVehicle(VehicleData data, bool adjustToRoadNode = true)
        {
            if (data == null)
                return new Err<Vehicle>("Datos del vehículo no válidos.");

            var loadResult = LoadModel(data.ModelName, Constants.SHORT_TIMEOUT_MS, retryOnce: false);
            if (loadResult.is_err())
                return new Err<Vehicle>(((Err<Model>)loadResult).Message);

            Model model = ((Ok<Model>)loadResult).Value;
            var (spawnPos, heading) = ResolveSpawnPosition(data, adjustToRoadNode);

            var createResult = CreateVehicleInternal(model, spawnPos, heading);
            if (createResult.is_err()) return createResult;

            Vehicle veh = ((Ok<Vehicle>)createResult).Value;
            VehicleCustomizer.ApplyAll(veh, data);

            if (data.IsLocked)
            {
                veh.LockStatus = VehicleLockStatus.CannotEnter;
                veh.IsAlarmSet = true;
            }

            veh.Heading = data.Heading;
            VehiclePersistence.SetPersistence(veh, true);

            return new Ok<Vehicle>(veh);
        }

        // Spawn para entregas temporales (recuperación, mecánico)
        public static Result<Vehicle> SpawnVehicleForDelivery(VehicleData data, bool adjustToRoadNode = true)
        {
            if (data == null)
                return new Err<Vehicle>("Datos del vehículo no válidos.");

            var loadResult = LoadModel(data.ModelName, Constants.LONG_TIMEOUT_MS, retryOnce: true);
            if (loadResult.is_err())
                return new Err<Vehicle>(((Err<Model>)loadResult).Message);

            Model model = ((Ok<Model>)loadResult).Value;
            var (spawnPos, heading) = ResolveSpawnPosition(data, adjustToRoadNode);

            var createResult = CreateVehicleInternal(model, spawnPos, heading);
            if (createResult.is_err()) return createResult;

            Vehicle veh = ((Ok<Vehicle>)createResult).Value;
            VehicleCustomizer.ApplyAll(veh, data);

            if (data.IsLocked)
                veh.LockStatus = VehicleLockStatus.CannotEnter;

            VehiclePersistence.SetPersistence(veh, true);

            return new Ok<Vehicle>(veh);
        }
    }
}