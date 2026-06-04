using GTA;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Helpers.Spawn
{
    public static class InteriorVehicleRestorer
    {
        public static void OnEntry(Vehicle currentVeh, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            // Si entro conduciendo, guardar posición y marcar IsInGarage = true
            if (currentVeh != null && currentVeh.Exists() && currentVeh.Driver == Game.Player.Character)
            {
                string vehId = VehicleIdentifier.Get(currentVeh);
                var dataOption = DB.Core.FindVehicle(vehId);
                if (dataOption.is_some())
                {
                    var data = ((Some<VehicleData>)dataOption).Value;
                    var updatedData = data.With(d =>
                    {
                        d.PosX = currentVeh.Position.X;
                        d.PosY = currentVeh.Position.Y;
                        d.PosZ = currentVeh.Position.Z;
                        d.Heading = currentVeh.Heading;
                        d.IsInGarage = true;
                        d.IsLocked = false;
                        d.IsDormant = false;
                    });
                    DB.Core.Update(updatedData);
                }
            }

            // Spawnear todos los vehículos con IsInGarage = true
            var garageVehicles = DB.Core.GetAll().Where(v => v.IsInGarage && !v.IsDestroyed).ToList();
            foreach (var vd in garageVehicles)
            {
                var existing = World.GetAllVehicles()
                    .FirstOrDefault(v => v.Mods.LicensePlate == vd.Plate && v.Model == new Model(vd.ModelName));
                if (existing != null && existing.Exists())
                {
                    if (!insuredVehList.Contains(existing))
                        insuredVehList.Add(existing);
                    BlipCleanupHandler.RemoveByVehicle(existing, blipsToRemove);
                }
                else
                {
                    var spawnResult = VehicleSpawnManager.SpawnVehicle(vd);
                    if (spawnResult is Ok<Vehicle> ok)
                    {
                        insuredVehList.Add(ok.Value);
                    }
                    else
                    {
                        Logger.Error($"Error al spawnear {vd.ModelName}: {((Err<Vehicle>)spawnResult).Message}");
                    }
                }
            }
        }

        // Se llama desde Garage.Exit cuando el jugador sale conduciendo un vehículo
        public static void OnExitVehicle(Vehicle currentVeh, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            // Borrar todos los vehículos del garaje excepto el que está usando
            var toDelete = insuredVehList.Where(v => VehiclesInGarage.IsAnInteriorGarage(v) && v != currentVeh).ToList();
            foreach (var v in toDelete)
            {
                BlipCleanupHandler.RemoveByVehicle(v, blipsToRemove);
                insuredVehList.Remove(v);
                v.Delete();
            }

            // Marcar el vehículo que está usando como fuera del garaje
            if (currentVeh != null && currentVeh.Exists())
            {
                string vehId = VehicleIdentifier.Get(currentVeh);
                var dataOption = DB.Core.FindVehicle(vehId);
                if (dataOption.is_some())
                {
                    var data = ((Some<VehicleData>)dataOption).Value;
                    var updatedData = data.With(d => d.IsInGarage = false);
                    DB.Core.Update(updatedData);
                }
            }
        }

        // Se llama desde VehicleMonitor (observador) cuando el jugador sale a pie o se aleja
        public static void OnExitFoot(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            var toDelete = insuredVehList.Where(v => VehiclesInGarage.IsAnInteriorGarage(v)).ToList();
            foreach (var v in toDelete)
            {
                BlipCleanupHandler.RemoveByVehicle(v, blipsToRemove);
                insuredVehList.Remove(v);
                v.Delete();
            }
        }
    }
}