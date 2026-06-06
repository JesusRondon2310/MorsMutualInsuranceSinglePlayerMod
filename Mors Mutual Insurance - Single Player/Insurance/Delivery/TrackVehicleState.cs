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

        // IsArrived original (con distancia al jugador)
        private static bool IsArrived(Data incoming)
        {
            float distToDest = incoming.driver.Position.DistanceTo(incoming.destination);
            float altDiffDest = incoming.driver.Position.Z - incoming.destination.Z;
            float distToPlayer = incoming.driver.Position.DistanceTo(Game.Player.Character.Position);
            float altDiffPlayer = incoming.driver.Position.Z - Game.Player.Character.Position.Z;

            return (distToDest <= Constants.ARRIVAL_PRECISION_DIST && Math.Abs(altDiffDest) <= Constants.ARRIVAL_PRECISION_ALT) ||
                   (distToPlayer <= Constants.ARRIVAL_PRECISION_DIST && Math.Abs(altDiffPlayer) <= Constants.ARRIVAL_PRECISION_ALT);
        }

        private static bool WaitForArrivalConfirmation(Data incoming)
        {
            string nearKey = $"near_arrival_{incoming.calledTime}";
            if (!Timers.IsCustomTimerExpired(nearKey))
            {
                Timers.StartCustomTimer(nearKey, Constants.LONG_TIMEOUT_MS);
                return false;
            }
            Timers.RemoveCustomTimer(nearKey);
            return true;
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

                // Destino dinámico (opcional, ayuda a que el conductor siga al jugador)
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

                // 3. El conductor ya ha llegado (destino dinámico) → esperar confirmación y verificar parada
                if (IsArrived(incoming))
                {
                    // Verificar que el vehículo esté detenido y el conductor dentro
                    bool isVehicleStopped = incoming.vehicle.Speed <= Constants.CLEAR_AREA_RADIUS && incoming.driver.IsInVehicle(incoming.vehicle);
                    if (isVehicleStopped && WaitForArrivalConfirmation(incoming))
                    {
                        // Frenar el vehículo antes de forzar la salida
                        incoming.vehicle.Velocity = Vector3.Zero;
                        incoming.vehicle.WorldRotationVelocity = Vector3.Zero;
                        Function.Call(Hash.SET_VEHICLE_FORWARD_SPEED, incoming.vehicle, 0f);

                        incoming.driver.Task.LeaveVehicle();
                        Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, incoming.driver, true);
                        Function.Call(Hash.RESET_PED_LAST_VEHICLE, incoming.driver);
                        CleanupDriver(incoming);
                        incomingVehicles.RemoveAt(i);
                        continue;
                    }
                }

                // 4. Control de proximidad y parada
                float distToDest = incoming.driver.Position.DistanceTo(incoming.destination);
                float distToPlayer = incoming.driver.Position.DistanceTo(Game.Player.Character.Position);
                bool isNear = distToDest <= Constants.DELIVERY_COMPLETION_RADIUS || distToPlayer <= Constants.DELIVERY_COMPLETION_RADIUS;
                bool isStopped = incoming.vehicle.Speed <= Constants.CLEAR_AREA_RADIUS && incoming.driver.IsInVehicle(incoming.vehicle);

                if (isNear && isStopped)
                {
                    if (WaitForArrivalConfirmation(incoming))
                    {
                        // Frenar el vehículo antes de forzar la salida
                        incoming.vehicle.Velocity = Vector3.Zero;
                        incoming.vehicle.WorldRotationVelocity = Vector3.Zero;
                        Function.Call(Hash.SET_VEHICLE_FORWARD_SPEED, incoming.vehicle, 0f);

                        incoming.driver.Task.LeaveVehicle();
                        Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, incoming.driver, true);
                        Function.Call(Hash.RESET_PED_LAST_VEHICLE, incoming.driver);
                        CleanupDriver(incoming);
                        incomingVehicles.RemoveAt(i);
                    }
                }
                else
                {
                    string nearKey = $"near_arrival_{incoming.calledTime}";
                    Timers.RemoveCustomTimer(nearKey);
                }

                // 5. Timeout global de la entrega
                int timeoutMs = ModSettings.BringVehicleTimeout * Constants.MINUTE_MS;
                if (Game.GameTime - incoming.calledTime > timeoutMs)
                {
                    Manager.CannotBringVehicle(incoming);
                    incomingVehicles.RemoveAt(i);
                }
            }
        }
    }
}