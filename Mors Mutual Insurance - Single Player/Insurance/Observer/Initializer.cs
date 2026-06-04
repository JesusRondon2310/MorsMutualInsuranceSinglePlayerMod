using GTA;
using GTA.Math;
using MMI_SP.Debug;
using MMI_SP.Dormancy;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Spawn;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer
{
    internal static class Initializer
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void RestoreVehiclesFromDatabase(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            List<DB.VehicleData> allVehicles = Insurer.Instance.GetInsuredListFull();
            if (allVehicles == null || allVehicles.Count == 0) return;

            foreach (DB.VehicleData data in allVehicles)
            {
                if (data.IsDestroyed) continue;

                if (data.IsInGarage)
                {
                    // Comprobar si es un garaje interior (comprable) por distancia al centro
                    Vector3 lastPos = new Vector3(data.PosX, data.PosY, data.PosZ);
                    float dx = lastPos.X - VehiclesInGarage.GaragePosition[0].X;
                    float dy = lastPos.Y - VehiclesInGarage.GaragePosition[0].Y;
                    bool isInteriorGarage = (dx * dx + dy * dy) <= VehiclesInGarage.GarageRadius * VehiclesInGarage.GarageRadius;

                    if (isInteriorGarage)
                        continue; // No restaurar ahora, se hará bajo demanda en Garage.Entry

                    // Garaje nativo: restaurar normalmente
                    VehicleRestorer.ExecuteRestorationFrom(data, insuredVehList, blipsToRemove);
                    continue;
                }

                if (data.IsDormant) {
                    var dormant = new DormantVehicle { Data = data };
                    Dormancy.Core.DormantVehicles.Add(dormant);
                    continue;
                }

                Model model = new Model(data.ModelName);
                Vehicle existing = World.GetAllVehicles().FirstOrDefault(v => v.Mods.LicensePlate == data.Plate && v.Model == model);

                if (existing != null) {
                    insuredVehList.Add(existing);

                    var blipResult = VehicleBlipHandler.Create(existing);
                    if (blipResult is Ok<Blip> okBlip) blipsToRemove[data.Id] = okBlip.Value;
                    continue;
                }

                Vector3 savedPos = new Vector3(data.PosX, data.PosY, data.PosZ);
                float distanceToPlayer = Game.Player.Character.Position.DistanceTo(savedPos);

                if (distanceToPlayer >= 600f)
                {
                    if (data.IsLocked)
                    {
                        var lockedSpawnResult = VehicleSpawnManager.SpawnVehicle(data);
                        if (lockedSpawnResult is Err<Vehicle> lockedSpawnErr)
                        {
                            Logger.Error($"Error al restaurar vehículo bloqueado {data.Id}: {lockedSpawnErr.Message}");
                            continue;
                        }

                        Vehicle lockedSpawned = ((Ok<Vehicle>)lockedSpawnResult).Value;
                        insuredVehList.Add(lockedSpawned);

                        var lockedBlipResult = VehicleBlipHandler.Create(lockedSpawned);
                        if (lockedBlipResult is Ok<Blip> lockedOkBlip) blipsToRemove[data.Id] = lockedOkBlip.Value;
                        continue;
                    }

                    var distantDormant = new DormantVehicle { Data = data };
                    Dormancy.Core.DormantVehicles.Add(distantDormant);
                    continue;
                }

                var spawnResult = VehicleSpawnManager.SpawnVehicle(data);
                if (spawnResult is Err<Vehicle> spawnErr)
                {
                    Logger.Error($"Error al restaurar vehículo {data.Id}: {spawnErr.Message}");
                    continue;
                }

                Vehicle spawned = ((Ok<Vehicle>)spawnResult).Value;
                insuredVehList.Add(spawned);

                var blipResult2 = VehicleBlipHandler.Create(spawned);
                if (blipResult2 is Ok<Blip> okBlip2) blipsToRemove[data.Id] = okBlip2.Value;
            }
        }
    }
}