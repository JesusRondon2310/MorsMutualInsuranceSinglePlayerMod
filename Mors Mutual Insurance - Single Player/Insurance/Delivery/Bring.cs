using GTA;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.Helpers.Teleport;
using MMI_SP.Insurance.Delivery.Incoming;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Delivery
{
    internal static class Bring
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Execute(
            Vehicle veh, int cost, bool instant, bool recoveredVehicle,
            List<Vehicle> recoveredVehList, Dictionary<string, Blip> blipsToRemove,
            List<Data> incomingVehicles)
        {
            if (!veh.Exists()) return;

            CancelIfIncoming(veh, incomingVehicles, blipsToRemove);

            if (instant)
                DeliverInstant(veh, recoveredVehicle, blipsToRemove);
            else
                ScheduleDelivery(veh, cost, recoveredVehicle, incomingVehicles, blipsToRemove);
        }

        private static void CancelIfIncoming(Vehicle veh, List<Data> incomingVehicles, Dictionary<string, Blip> blipsToRemove)
        {
            foreach (Data incoming in incomingVehicles)
            {
                if (incoming.vehicle != veh) continue;

                incoming.driver.Task.ClearAllImmediately();
                incoming.driver.IsPersistent = false;
                incoming.driver.Delete();

                if (!incoming.recovered)
                    BlipCleanupHandler.RemoveByVehicle(incoming.vehicle, blipsToRemove);
                else
                    incoming.vehicle.Repair();
            }
        }

        private static void DeliverInstant(Vehicle veh, bool recoveredVehicle, Dictionary<string, Blip> blipsToRemove)
        {
            InFrontOfPlayer.Execute(veh);

            if (!recoveredVehicle) AddBlip(veh, blipsToRemove);
        }

        private static void ScheduleDelivery(Vehicle veh, int cost, bool recoveredVehicle, List<Data> incomingVehicles, 
            Dictionary<string, Blip> blipsToRemove)
        {
            var result = Incoming.Handler.BringVehicle(veh, cost, recoveredVehicle);

            result.match<bool>(
                onOk: ok => { incomingVehicles.Add(ok); return true; },
                onErr: error => {
                    Logger.Error($"Error al programar entrega del vehículo: {error}");
                    return false;
                }
            );

            if (!recoveredVehicle) AddBlip(veh, blipsToRemove);
        }

        private static void AddBlip(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            string key = VehicleIdentifier.Get(veh);
            if (blipsToRemove.TryGetValue(key, out Blip old) && old != null && old.Exists()) old.Delete();

            var result = VehicleBlipHandler.Create(veh);
            if (result is Ok<Blip> ok) blipsToRemove[key] = ok.Value;
        }
    }
}