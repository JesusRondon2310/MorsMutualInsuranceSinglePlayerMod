using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.PatternMatching;
using System;
using System.Collections.Generic;

namespace MMI_SP.Helpers.Spawn.Coordinates
{
    internal static class MMIWarehouse
    {
        private static readonly Random _random = new Random();

        // ==========================================
        // BLOQUE 1: Datos — Listas de nodos por tipo
        // ==========================================
        public static readonly List<EntityPosition> SpawnListVehicle = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-225.2716f, -1182.783f, 22.49698f), 2.3600f),
            new EntityPosition(new Vector3(-229.9406f, -1182.361f, 22.49209f), 6.1440f),
            new EntityPosition(new Vector3(-234.6615f, -1182.197f, 22.48984f), 355.5509f),
            new EntityPosition(new Vector3(-244.1168f, -1179.623f, 22.5177f), 308.1156f),
            new EntityPosition(new Vector3(-243.4413f, -1173.07f, 22.53329f), 271.4005f),
            new EntityPosition(new Vector3(-243.5148f, -1166.511f, 22.56954f), 242.3607f),
            new EntityPosition(new Vector3(-237.2427f, -1162.784f, 22.47172f), 183.7536f),
            new EntityPosition(new Vector3(-232.8058f, -1162.548f, 22.44885f), 182.2262f),
            new EntityPosition(new Vector3(-228.4865f, -1162.615f, 22.45386f), 181.4573f),
            new EntityPosition(new Vector3(-150.4142f, -1166.01f, 24.73805f), 177.0276f),
            new EntityPosition(new Vector3(-143.6111f, -1163.825f, 24.76486f), 160.3781f),
            new EntityPosition(new Vector3(-136.2873f, -1183.153f, 24.7363f), 78.20843f),
            new EntityPosition(new Vector3(-136.9411f, -1177.181f, 24.72224f), 102.267f),
            new EntityPosition(new Vector3(-246.5937f, -1150.561f, 22.62461f), 269.4836f),
            new EntityPosition(new Vector3(-238.7069f, -1150.786f, 22.62887f), 269.3971f),
            new EntityPosition(new Vector3(-232.8114f, -1150.434f, 22.54277f), 272.1211f),
            new EntityPosition(new Vector3(-211.5235f, -1150.303f, 22.55123f), 268.1985f),
            new EntityPosition(new Vector3(-198.5835f, -1150.331f, 22.54078f), 269.7671f)
        };

        public static readonly List<EntityPosition> SpawnListVehicleLong = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-157.9389f, -1162.761f, 24.11157f), 0.6600574f),
            new EntityPosition(new Vector3(-236.0531f, -1149.395f, 23.04231f), 269.1866f),
            new EntityPosition(new Vector3(-174.2821f, -1149.661f, 23.17635f), 269.3501f),
            new EntityPosition(new Vector3(-200.4261f, -1182.882f, 23.1067f), 90.51575f)
        };

        public static readonly List<EntityPosition> SpawnListMilitary = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-1594.426f, 3185.479f, 30.40495f), 147.6925f),
            new EntityPosition(new Vector3(-1603.479f, 3203.978f, 30.41406f), 171.5964f),
            new EntityPosition(new Vector3(-1615.621f, 3169.568f, 29.66991f), 223.9812f),
            new EntityPosition(new Vector3(-1580.501f, 3156.202f, 30.64534f), 154.0106f),
            new EntityPosition(new Vector3(-1565.606f, 3131.228f, 32.23048f), 142.0033f),
            new EntityPosition(new Vector3(-1630.897f, 2980.762f, 32.45866f), 251.8481f),
            new EntityPosition(new Vector3(-1565.328f, 3020.8f, 32.43408f), 121.0561f),
            new EntityPosition(new Vector3(-1668.656f, 3081.12f, 30.85717f), 231.5131f)
        };

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static Result<EntityPosition> GetRecoverNode(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<EntityPosition>("El vehículo no es válido.");

            List<EntityPosition> tempList = new List<EntityPosition>();

            if (veh.ClassType == VehicleClass.Military) {
                tempList.AddRange(SpawnListMilitary);
            }
            else
            {
                veh.Model.GetDimensions(out Vector3 min, out Vector3 max);
                Vector3 vehDimension = max - min;
                if (vehDimension.Y > Constants.VEHICLE_LONG_THRESHOLD)
                    tempList.AddRange(SpawnListVehicleLong);
                else
                    tempList.AddRange(SpawnListVehicle);
            }

            while (tempList.Count > 0)
            {
                int n = _random.Next(0, tempList.Count);
                EntityPosition spawn = tempList[n];

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY,
                    spawn.Position.X, spawn.Position.Y, spawn.Position.Z,
                    Constants.DISTANCE_IN_FRONT_OF_PLAYER,
                    Constants.DISTANCE_IN_FRONT_OF_PLAYER,
                    Constants.DISTANCE_IN_FRONT_OF_PLAYER, 0))
                    return new Ok<EntityPosition>(spawn);
                else
                    tempList.Remove(spawn);
            }

            // Si no hay ningún punto libre, limpiamos uno aleatorio de la lista genérica
            EntityPosition clearSpawn = SpawnListVehicle[_random.Next(0, SpawnListVehicle.Count)];
            Function.Call(Hash.CLEAR_AREA_OF_VEHICLES, clearSpawn.Position.X, clearSpawn.Position.Y, clearSpawn.Position.Z,
                Constants.CLEAR_AREA_RADIUS, false, false, false, false, false);
            return new Ok<EntityPosition>(clearSpawn);
        }
    }
}