using GTA;
using GTA.Math;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Spawn;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.Helpers.Teleport;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

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

            var updatedData = dormant.Data.With(d => {
                d.PosX = spawnPos.X;
                d.PosY = spawnPos.Y;
                d.PosZ = spawnPos.Z;
                d.Heading = spawnHeading;
                d.IsLocked = false;
                d.IsDormant = false;
                d.IsInGarage = false;
                d.IsDestroyed = false;
            });

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