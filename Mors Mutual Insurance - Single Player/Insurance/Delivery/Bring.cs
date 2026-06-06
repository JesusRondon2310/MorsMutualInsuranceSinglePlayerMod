using GTA;
using GTA.Math;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Teleport;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Delivery
{
    internal static class Bring
    {
        // ==========================================
        // BLOQUE: Funciones de entrega
        // ==========================================
        internal static void Execute(Vehicle veh, int cost, bool instant, bool recoveredVehicle,
            Dictionary<string, Blip> blipsToRemove, List<Data> incomingVehicles)
        {
            if (!veh.Exists()) return;

            CancelIfIncoming(veh, incomingVehicles, blipsToRemove);

            if (instant) {
                DeliverInstant(veh, recoveredVehicle, blipsToRemove);
                return;
            }

            // Obtener distancia al jugador
            float distance = Game.Player.Character.Position.DistanceTo(veh.Position);
            bool isInImpound = VehiclesInGarage.IsPositionInPoliceImpound(veh.Position);
            string vehId = VehicleIdentifier.Get(veh);
            var dataOption = DB.Core.FindVehicle(vehId);
            bool isInGarage = dataOption.match(
                onSome: d => d.IsInGarage,
                onNone: () => false
            );

            // Caso 3: distancia entre 200m y 600m o -> TELEPORT
            if (distance >= Constants.MIN_DISTANCE_FOR_DELIVERY && distance < Constants.DORMANCY_THRESHOLD && !isInGarage && !isInImpound) {
                VehicleTeleport.Execute(veh, cost, recoveredVehicle, blipsToRemove, incomingVehicles);
                return;
            }
            // Caso 1 y 2: distancia < 200m o >= 600m -> flujo normal
            ScheduleDelivery(veh, cost, recoveredVehicle, incomingVehicles, blipsToRemove);
        }

        private static void CancelIfIncoming(Vehicle veh, List<Data> incomingVehicles, Dictionary<string, Blip> blipsToRemove)
        {
            Data toRemove = null;
            foreach (Data incoming in incomingVehicles) {
                if (incoming.vehicle != veh) continue;

                if (incoming.driver != null && incoming.driver.Exists()) {
                    incoming.driver.Task.ClearAllImmediately();
                    incoming.driver.IsPersistent = false;
                    incoming.driver.Delete();
                }

                if (!incoming.recovered) BlipCleanupHandler.RemoveByVehicle(incoming.vehicle, blipsToRemove);
                else incoming.vehicle.Repair();

                toRemove = incoming;
                break;
            }
            if (toRemove != null) incomingVehicles.Remove(toRemove);
        }

        private static void DeliverInstant(Vehicle veh, bool recoveredVehicle, Dictionary<string, Blip> blipsToRemove) {
            Teleport.InFrontOfPlayer(veh);
            if (!recoveredVehicle) AddBlip(veh, blipsToRemove);
        }

        private static void ScheduleDelivery(Vehicle veh, int cost, bool recoveredVehicle, List<Data> incomingVehicles,
            Dictionary<string, Blip> blipsToRemove)
        {
            var result = Incoming.BringVehicle(veh, cost, recoveredVehicle);

            result.match<bool>(
                onOk: ok => {
                    incomingVehicles.Add(ok);
                    if (!recoveredVehicle) AddBlip(veh, blipsToRemove);
                    return true;
                },
                onErr: error => {
                    Logger.Error($"Error al programar entrega del vehículo: {error}");
                    return false;
                }
            );
        }

        internal static void AddBlip(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            string key = VehicleIdentifier.Get(veh);
            if (blipsToRemove.TryGetValue(key, out Blip old) && old != null && old.Exists()) old.Delete();

            var result = VehicleBlipHandler.Create(veh);
            if (result is Ok<Blip> ok) blipsToRemove[key] = ok.Value;
        }

        // ==========================================
        // BLOQUE: Obtener vehículos que se pueden traer
        // ==========================================
        internal static List<Vehicle> GetBringableVehicles(List<Vehicle> recoveredVehList, List<Vehicle> insuredVehList)
        {
            List<Vehicle> vehiclesToBring = new List<Vehicle>(recoveredVehList);
            foreach (Vehicle v in insuredVehList) if (!vehiclesToBring.Contains(v)) vehiclesToBring.Add(v);
            return vehiclesToBring;
        }
    }
}