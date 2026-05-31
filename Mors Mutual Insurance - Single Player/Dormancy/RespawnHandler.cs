using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Spawn;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.Helpers.Teleport;
using MMI_SP.PatternMatching;

namespace MMI_SP.Dormancy
{
    internal static class RespawnHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<Vehicle> Execute(string vehId, List<DormantVehicle> dormantVehicles)
        {
            DormantVehicle dormant = dormantVehicles.FirstOrDefault(d => d.Data?.Id == vehId);
            if (dormant == null) return new Err<Vehicle>("Vehículo dormante no encontrado.");

            Vector3 savedPos = new Vector3(dormant.Data.PosX, dormant.Data.PosY, dormant.Data.PosZ);
            float distance = Game.Player.Character.Position.DistanceTo(savedPos);

            Vector3 spawnPos;
            float spawnHeading;

            if (distance <= 200f)
            {
                spawnPos = savedPos;
                spawnHeading = dormant.Data.Heading;
            }
            else
            {
                Vector3 behindPos = DriverDeliverySpawn.GetPosition(150f);
                var roadResult = RoadSpawnHandler.FindNode(behindPos.X, behindPos.Y, behindPos.Z);
                spawnPos = roadResult.unwrap_or(behindPos);
                spawnHeading = dormant.Data.Heading;
            }

            Model model = new Model(dormant.Data.ModelName);
            bool alreadyInWorld = World.GetAllVehicles()
                .Any(v => v.Mods.LicensePlate == dormant.Data.Plate && v.Model == model);
            if (alreadyInWorld)
                return new Err<Vehicle>("El vehículo ya existe en el mundo, no se puede duplicar.");

            var updatedData = new VehicleData(dormant.Data.Id, dormant.Data.ModelName, dormant.Data.Plate, dormant.Data.PrimaryColor,
                dormant.Data.SecondaryColor, false, windowTint: dormant.Data.WindowTint, wheelType: dormant.Data.WheelType,
                wheelColor: dormant.Data.WheelColor, tireSmokeColor: dormant.Data.TireSmokeColor, bulletproofTires: dormant.Data.BulletproofTires,
                neonLeft: dormant.Data.NeonLeft, neonRight: dormant.Data.NeonRight, neonFront: dormant.Data.NeonFront, neonBack: dormant.Data.NeonBack,
                neonColor: dormant.Data.NeonColor, posX: spawnPos.X, posY: spawnPos.Y, posZ: spawnPos.Z, heading: spawnHeading, mods: dormant.Data.Mods,
                destroyedAt: dormant.Data.DestroyedAt, isLocked: false, plateStyle: dormant.Data.PlateStyle, customTires: dormant.Data.CustomTires,
                isDormant: false, isInGarage: false, vehicleType: dormant.Data.VehicleType
            );

            DB.Core.Update(updatedData);

            var spawnResult = VehicleSpawnManager.SpawnVehicle(updatedData, validatePosition: false);
            if (spawnResult.is_err()) return spawnResult;
            Vehicle spawned = spawnResult.unwrap_or(null);

            ToRoad.Execute(spawned);

            Insurance.Observer.Manager.InsuredVehList.Add(spawned);

            VehicleBlipHandler.Create(spawned).match<bool>(
                onOk: blip =>
                {
                    Insurance.Observer.Manager.BlipsToRemove[vehId] = blip;
                    return true;
                },
                onErr: _ => false
            );

            dormantVehicles.Remove(dormant);
            return new Ok<Vehicle>(spawned);
        }
    }
}