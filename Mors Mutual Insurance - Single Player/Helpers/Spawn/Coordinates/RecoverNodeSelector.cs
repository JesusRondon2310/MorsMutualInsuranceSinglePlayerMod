using GTA;
using GTA.Native;
using MMI_SP.DB;
using MMI_SP.PatternMatching;
using System;
using System.Collections.Generic;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    internal static class RecoverNodeSelector
    {
        private static readonly Random _random = new Random();

        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<EntityPosition> GetNode(VehicleData data)
        {
            if (data == null || string.IsNullOrEmpty(data.VehicleType)) return new Err<EntityPosition>("Datos del vehículo no válidos.");

            if (!Enum.TryParse(data.VehicleType, out VehicleType vehicleType))
                return new Err<EntityPosition>($"Tipo de vehículo desconocido: {data.VehicleType}");

            List<EntityPosition> tempList = new List<EntityPosition>();

            switch (vehicleType)
            {
                default:
                    tempList.AddRange(MMIWarehouse.SpawnListVehicle);
                    tempList.AddRange(MMIWarehouse.SpawnListVehicleLong);
                    break;
            }

            while (tempList.Count > 0)
            {
                int n = _random.Next(0, tempList.Count);
                EntityPosition spawn = tempList[n];

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, spawn.Position.X, spawn.Position.Y, spawn.Position.Z,
                    Constants.DISTANCE_IN_FRONT_OF_PLAYER, Constants.DISTANCE_IN_FRONT_OF_PLAYER, Constants.DISTANCE_IN_FRONT_OF_PLAYER, 0))
                    return new Ok<EntityPosition>(spawn);
                else
                    tempList.Remove(spawn);
            }

            EntityPosition clearSpawn = MMIWarehouse.SpawnListVehicle[_random.Next(0, MMIWarehouse.SpawnListVehicle.Count)];
            Function.Call(Hash.CLEAR_AREA_OF_VEHICLES, clearSpawn.Position.X, clearSpawn.Position.Y, clearSpawn.Position.Z, Constants.VEHICLE_STOP_MIN_SPEED,
                false, false, false, false, false);
            return new Ok<EntityPosition>(clearSpawn);
        }
    }
}