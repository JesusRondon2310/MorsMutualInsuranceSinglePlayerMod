using System.Collections.Generic;
using GTA;
using GTA.Math;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Spawn.Coordinates;
using MMI_SP.Insurance.Delivery.Incoming;

namespace MMI_SP.Insurance.Delivery
{
    internal static class Fail
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Execute(Data incoming, int refund, List<Data> incomingVehicles)
        {
            NotifyFailure();
            Refund(incoming, refund);
            incoming.driver.Delete();
            RepairIfAlive(incoming);
            incomingVehicles.Remove(incoming);
        }

        private static void NotifyFailure()
        {
            Notification.ShowMechanic("Información", "No se pudo traer el vehículo.");
        }

        private static void Refund(Data incoming, int refund)
        {
            Game.Player.Money += (refund == 0) ? incoming.price : refund + incoming.price;
        }

        private static void RepairIfAlive(Data incoming)
        {
            if (incoming.vehicle.IsDead) return;

            if (incoming.originalPosition.Position != Vector3.Zero)
            {
                incoming.vehicle.Position = incoming.originalPosition.Position;
                incoming.vehicle.Heading = incoming.originalPosition.Heading;
            }
            else
            {
                EntityPosition pos = FixedSpawnHandler.GetRecoverNode();
                incoming.vehicle.Position = pos.Position;
                incoming.vehicle.Heading = pos.Heading;
            }

            incoming.vehicle.IsEngineRunning = false;
            incoming.vehicle.Repair();
        }
    }
}