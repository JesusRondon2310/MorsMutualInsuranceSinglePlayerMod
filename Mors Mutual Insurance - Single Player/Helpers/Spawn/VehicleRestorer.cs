using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Helpers.Spawn
{
    public static class VehicleRestorer
    {
        // ==========================================
        // BLOQUE: Restauración al cargar partida
        // ==========================================
        public static void ExecuteRestorationFrom(VehicleData data, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            if (data.IsDestroyed) return;

            Model model = new Model(data.ModelName);
            bool modelLoaded = model.Request(2000);

            if (modelLoaded)
            {
                model.MarkAsNoLongerNeeded();

                Vector3 garagePos = new Vector3(data.PosX, data.PosY, data.PosZ);
                Vehicle existing = World.GetClosestVehicle(garagePos, 30f);

                if (existing != null && existing.Exists() && VehiclesInGarage.IsDefaultGarage(existing))
                {
                    Function.Call(Hash.SET_VEHICLE_CAN_SAVE_IN_GARAGE, existing, false);
                    existing.Delete();
                }

                string searchKey = VehicleKey.ModelPlateKeyFrom(data);
                Vector3 playerPos = Game.Player.Character.Position;
                Vehicle other = World.GetAllVehicles()
                    .FirstOrDefault(v => v.Exists() && v != existing && $"{v.Model}_{v.Mods.LicensePlate}" == searchKey
                                         && v.Position.DistanceTo(playerPos) < 600f);

                if (other != null && other.Exists())
                {
                    Logger.Warning($"Vehículo duplicado detectado (KeyPair: {searchKey}) a <600m. Se usará ese en lugar de spawnear.");
                    insuredVehList.Add(other);
                    var blipResult = VehicleBlipHandler.Create(other);
                    if (blipResult is Ok<Blip> okBlip)
                        blipsToRemove[data.Id] = okBlip.Value;

                    var updatedData = data.With(d =>
                    {
                        d.PosX = other.Position.X;
                        d.PosY = other.Position.Y;
                        d.PosZ = other.Position.Z;
                        d.Heading = other.Heading;
                        d.IsInGarage = false;
                    });
                    DB.Core.Update(updatedData);
                    return;
                }

                // Spawn normal
                var spawnResult = VehicleSpawnManager.SpawnVehicle(data);
                spawnResult.match<bool>(
                    onOk: spawned =>
                    {
                        insuredVehList.Add(spawned);
                        var blipResult = VehicleBlipHandler.Create(spawned);
                        if (blipResult is Ok<Blip> okBlip)
                            blipsToRemove[data.Id] = okBlip.Value;
                        return true;
                    },
                    onErr: error =>
                    {
                        Logger.Error($"Error al restaurar vehículo {data.ModelName}: {error}");
                        return false;
                    }
                );
            }
            else
            {
                model.MarkAsNoLongerNeeded();
                Logger.Warning($"No se pudo cargar el modelo {data.ModelName} - {data.Plate}. Se mantiene el vehículo original.");

                Vector3 garagePos = new Vector3(data.PosX, data.PosY, data.PosZ);
                Vehicle existing = World.GetClosestVehicle(garagePos, 30f);
                if (existing != null && existing.Exists())
                {
                    insuredVehList.Add(existing);
                    var blipResult = VehicleBlipHandler.Create(existing);
                    if (blipResult is Ok<Blip> okBlip)
                        blipsToRemove[data.Id] = okBlip.Value;
                }
            }
        }
    }
}