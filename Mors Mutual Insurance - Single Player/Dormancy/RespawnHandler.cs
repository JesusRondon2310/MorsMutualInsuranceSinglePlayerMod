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
        // BLOQUE: Funciones auxiliares
        // ==========================================

        private static DormantVehicle FindDormant(string vehId, List<DormantVehicle> dormantVehicles)
        {
            return dormantVehicles.FirstOrDefault(d => d.Data?.Id == vehId);
        }

        private static (Vector3 Position, float Heading) CalculateSpawnPosition(DormantVehicle dormant)
        {
            Vector3 savedPos = new Vector3(dormant.Data.PosX, dormant.Data.PosY, dormant.Data.PosZ);
            float distance = Game.Player.Character.Position.DistanceTo(savedPos);

            if (distance <= Constants.MIN_DISTANCE_FOR_DELIVERY) {
                return (savedPos, dormant.Data.Heading);
            }
            else {
                Vector3 behindPos = SpawnHandler.DriverDeliverySpawn(Constants.DELIVERY_DISTANCE_BEHIND_PLAYER);
                var roadResult = SpawnHandler.GetValidRoad(behindPos.X, behindPos.Y, behindPos.Z);
                Vector3 spawnPos = roadResult.unwrap_or(behindPos);
                return (spawnPos, dormant.Data.Heading);
            }
        }

        private static bool AlreadyExistsInWorld(DormantVehicle dormant)
        {
            Model model = new Model(dormant.Data.ModelName);
            return World.GetAllVehicles()
                .Any(v => v.Mods.LicensePlate == dormant.Data.Plate && v.Model == model);
        }

        // ==========================================
        // BLOQUE: Función principal
        // ==========================================

        internal static Result<Vehicle> Execute(string vehId, List<DormantVehicle> dormantVehicles)
        {
            // 1. Buscar el vehículo dormante
            DormantVehicle dormant = FindDormant(vehId, dormantVehicles);
            if (dormant == null) return new Err<Vehicle>("Vehículo dormante no encontrado.");

            // 2. Calcular posición de spawn
            var (spawnPos, spawnHeading) = CalculateSpawnPosition(dormant);

            // 3. Verificar que no exista ya un vehículo con la misma placa en el mundo
            if (AlreadyExistsInWorld(dormant)) return new Err<Vehicle>("El vehículo ya existe en el mundo, no se puede duplicar.");

            // 4. Actualizar datos en la BD (orquestador DB.Core)
            var updatedData = dormant.Data.With(d =>
            {
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

            // 5. Spawnear el vehículo
            var spawnResult = VehicleSpawnManager.SpawnVehicle(updatedData, adjustToRoadNode: false);
            if (spawnResult.is_err()) return new Err<Vehicle>(((Err<Vehicle>)spawnResult).Message);
            Vehicle spawned = ((Ok<Vehicle>)spawnResult).Value;

            // 6. Ajustar a la carretera
            Teleport.ToRoad(spawned);

            // 7. Registrar en las listas del Observer (orquestador público)
            Insurance.Observer.Manager.InsuredVehList.Add(spawned);
            VehicleBlipHandler.Create(spawned).match<bool>(
                onOk: blip => {
                    Insurance.Observer.Manager.BlipsToRemove[vehId] = blip;
                    return true;
                },
                onErr: _ => false
            );

            // 8. Eliminar de la lista de dormantes
            dormantVehicles.Remove(dormant);
            return new Ok<Vehicle>(spawned);
        }
    }
}