using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Config;
using MMI_SP.Helpers;

namespace MMI_SP.Insurance.Delivery
{
    internal static class TrackVehicleState
    {
        // ==========================================
        // BLOQUE: Funciones auxiliares privadas
        // ==========================================
        private static void CleanupDriver(Data incoming)
        {
            if (incoming.driver == null || !incoming.driver.Exists()) return;
            incoming.driver.IsPersistent = false;
            incoming.driver.MarkAsNoLongerNeeded();
            incoming.driver.Task.ClearAllImmediately();
            incoming.driver.Task.Wander();
            Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, incoming.driver, true);
        }

        // ==========================================
        // BLOQUE: Función principal
        // ==========================================
        internal static void Execute(List<Data> incomingVehicles)
        {
            for (int i = incomingVehicles.Count - Constants.ONE; i >= Constants.ZERO; i--)
            {
                Data incoming = incomingVehicles[i];
                if (incoming.driver == null || !incoming.driver.Exists())
                {
                    incomingVehicles.RemoveAt(i);
                    continue;
                }

                // Destino dinámico
                incoming.destination = Game.Player.Character.Position;

                // 1. El jugador ya está dentro del vehículo → entregado
                if (Game.Player.Character.IsInVehicle(incoming.vehicle))
                {
                    CleanupDriver(incoming);
                    incomingVehicles.RemoveAt(i);
                    continue;
                }

                // 2. El vehículo está destruido → entrega fallida
                if (incoming.vehicle.IsDead)
                {
                    int cost = Policies.Manager.GetCost(incoming.vehicle);
                    Manager.CannotBringVehicle(incoming, cost);
                    incomingVehicles.RemoveAt(i);
                    continue;
                }

                // 3. Timeout global de la entrega
                int timeoutMs = ModSettings.BringVehicleTimeout * Constants.MINUTE_MS;
                if (Game.GameTime - incoming.calledTime > timeoutMs)
                {
                    Manager.CannotBringVehicle(incoming);
                    incomingVehicles.RemoveAt(i);
                    continue;
                }
            }
        }
    }
}