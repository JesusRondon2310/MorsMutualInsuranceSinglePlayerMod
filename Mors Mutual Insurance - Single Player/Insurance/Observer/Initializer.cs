using GTA;
using GTA.Math;
using MMI_SP.DB;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Spawn;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer
{
    internal static class Initializer
    {
        // ==========================================
        // Métodos privados auxiliares
        // ==========================================
        private static void AddVehicleToListAndBlip(Vehicle veh, VehicleData data, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            insuredVehList.Add(veh);
            var blipResult = VehicleBlipHandler.Create(veh);
            if (blipResult is Ok<Blip> okBlip)
                blipsToRemove[data.Id] = okBlip.Value;
        }

        private static bool TryAddVehicleFromData(VehicleData data, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            var spawnResult = VehicleSpawnManager.SpawnVehicle(data);
            if (spawnResult is Ok<Vehicle> ok)
            {
                AddVehicleToListAndBlip(ok.Value, data, insuredVehList, blipsToRemove);
                return true;
            }
            Logger.Error($"Error al restaurar vehículo {data.Id}: {((Err<Vehicle>)spawnResult).Message}");
            return false;
        }

        // ==========================================
        // Función principal
        // ==========================================
        internal static void RestoreVehiclesFromDatabase(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            List<VehicleData> allVehicles = Policies.Manager.GetInsuredListFull();
            if (allVehicles == null || allVehicles.Count == Constants.ZERO) return;

            foreach (VehicleData data in allVehicles)
            {
                if (data.IsDestroyed) continue;

                // 1. Vehículos en garaje
                if (data.IsInNativeGarage)
                {
                    Vector3 lastPos = new Vector3(data.PosX, data.PosY, data.PosZ);
                    float dx = lastPos.X - VehiclesInGarage.GaragePosition[Constants.FIRST_INDEX].X;
                    float dy = lastPos.Y - VehiclesInGarage.GaragePosition[Constants.FIRST_INDEX].Y;
                    bool isInteriorGarage = (dx * dx + dy * dy) <= Constants.INTERIOR_GARAGE_RADIUS * Constants.INTERIOR_GARAGE_RADIUS;

                    if (isInteriorGarage && data.IsInInteriorGarage) continue; // Se restaurará bajo demanda en Garage.Entry
                    
                    else VehicleRestorer.ExecuteRestorationFrom(data, insuredVehList, blipsToRemove);

                    continue;
                }

                // 2. Vehículos dormantes
                if (data.IsDormant)
                {
                    Dormancy.Core.MarkAsDormant(data.Id);  continue;
                }

                // 3. Buscar si ya existe un vehículo con la misma placa y modelo en el mundo
                Model model = new Model(data.ModelName);
                Vehicle existing = World.GetAllVehicles().FirstOrDefault(v => v.Mods.LicensePlate == data.Plate && v.Model == model);
                if (existing != null)
                {
                    AddVehicleToListAndBlip(existing, data, insuredVehList, blipsToRemove);
                    continue;
                }

                // 4. Calcular distancia a la posición guardada
                Vector3 savedPos = new Vector3(data.PosX, data.PosY, data.PosZ);
                float distanceToPlayer = Game.Player.Character.Position.DistanceTo(savedPos);

                // 5. Si está lejos (umbral de dormancia)
                if (distanceToPlayer >= Constants.DORMANCY_THRESHOLD)
                {
                    if (data.IsLocked)
                        TryAddVehicleFromData(data, insuredVehList, blipsToRemove);
                    else
                        Dormancy.Core.MarkAsDormant(data.Id); // ← corregido
                    continue;
                }

                // 6. Spawn normal (cerca del jugador)
                TryAddVehicleFromData(data, insuredVehList, blipsToRemove);
            }
        }
    }
}