using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.DB;
using MMI_SP.Helpers.Spawn;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Helpers
{
    public static class VehiclesInGarage
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static readonly List<string> GarageNames = new List<string>
        {
            "Michael - Beverly Hills",
            "Franklin - Aunt",
            "Trevor - Sandy Shores",
            "Franklin - Grove Street",
            "Michael - Blaine County",
            "Franklin - Vinewood Hills",
            "Trevor - Pillbox Hill",
            "Michael - Los Santos",
            "Franklin - Downtown",
            "Trevor - Vespucci"
        };

        private static readonly List<Vector3> GaragePositions = new List<Vector3>
        {
            new Vector3(-802.5f, 174.9f, 72.3f),   // Michael - Beverly Hills
            new Vector3(-16.8f, -1438.4f, 30.7f),  // Franklin - Aunt
            new Vector3(1192.4f, 2694.9f, 38.9f),  // Trevor - Sandy Shores
            new Vector3(88.5f, -1960.4f, 20.9f),   // Franklin - Grove Street
            new Vector3(-1521.0f, 899.0f, 7.0f),   // Michael - Blaine County
            new Vector3(-622.0f, 36.0f, 43.0f),    // Franklin - Vinewood Hills
            new Vector3(-156.0f, -604.0f, 168.0f), // Trevor - Pillbox Hill
            new Vector3(-802.0f, 174.0f, 72.0f),   // Michael - Los Santos
            new Vector3(-622.0f, 36.0f, 43.0f),    // Franklin - Downtown
            new Vector3(-1152.0f, -1520.0f, 10.0f) // Trevor - Vespucci
        };

        private const float GarageRadius = 1f;

        // Depósitos policiales
        private static readonly List<Vector3> PoliceImpoundPositions = new List<Vector3>
        {
            new Vector3(-443.0f, -1690.0f, 19.0f),   // Los Santos (La Mesa)
            new Vector3(1525.0f, 3780.0f, 34.0f)     // Blaine County
        };

        private const float PoliceImpoundRadius = 80f;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static bool IsInAnyGarage(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;

            foreach (string garageName in GarageNames) if (Function.Call<bool>(Hash.IS_VEHICLE_IN_GARAGE_AREA, garageName, veh)) return true;

            return false;
        }

        public static void WakeUpDormantInGarages()
        {
            var dormantList = Dormancy.Core.DormantVehicles;
            if (dormantList.Count == 0) return;

            for (int i = dormantList.Count - 1; i >= 0; i--)
            {
                var dormant = dormantList[i];
                if (dormant?.Data == null) continue;

                Vector3 dormantPos = new Vector3(dormant.Data.PosX, dormant.Data.PosY, dormant.Data.PosZ);
                float distance = Game.Player.Character.Position.DistanceTo(dormantPos);

                if (distance > 50f) continue;
                if (!IsPositionInAnyGarage(dormantPos)) continue;

                var spawnResult = VehicleSpawnManager.SpawnVehicle(dormant.Data, validatePosition: false);
                if (spawnResult.is_err()) continue;

                Vehicle spawned = ((Ok<Vehicle>)spawnResult).Value;
                VehiclePersistence.SetPersistence(spawned, true);
                spawned.AttachedBlip?.Delete();

                Insurance.Observer.Manager.InsuredVehList.Add(spawned);

                var updatedData = new VehicleData(dormant.Data.Id, dormant.Data.ModelName, dormant.Data.Plate, dormant.Data.PrimaryColor, 
                    dormant.Data.SecondaryColor, false, windowTint: dormant.Data.WindowTint, wheelType: dormant.Data.WheelType, 
                    wheelColor: dormant.Data.WheelColor, tireSmokeColor: dormant.Data.TireSmokeColor, bulletproofTires: dormant.Data.BulletproofTires,
                    neonLeft: dormant.Data.NeonLeft, neonRight: dormant.Data.NeonRight, neonFront: dormant.Data.NeonFront, 
                    neonBack: dormant.Data.NeonBack, neonColor: dormant.Data.NeonColor, posX: dormant.Data.PosX, posY: dormant.Data.PosY, 
                    posZ: dormant.Data.PosZ, heading: dormant.Data.Heading, mods: dormant.Data.Mods, destroyedAt: dormant.Data.DestroyedAt,
                    isLocked: false, plateStyle: dormant.Data.PlateStyle, customTires: dormant.Data.CustomTires, isDormant: false,
                    vehicleType: dormant.Data.VehicleType
                );
                DB.Core.Update(updatedData);

                dormantList.RemoveAt(i);
            }
        }

        public static bool IsPositionInAnyGarage(Vector3 position)
        {
            foreach (var garagePos in GaragePositions) { if (position.DistanceTo(garagePos) <= GarageRadius) return true; }
            return false;
        }

        public static bool IsPositionInPoliceImpound(Vector3 position)
        {
            foreach (var impoundPos in PoliceImpoundPositions) { if (position.DistanceTo(impoundPos) <= PoliceImpoundRadius) return true; }
            return false;
        }
    }
}