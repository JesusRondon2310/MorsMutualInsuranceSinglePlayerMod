using System.Collections.Generic;
using GTA;
using GTA.Native;
using MMI_SP.Config;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Delivery.Incoming;
using MMI_SP.Insurance.Policies;

namespace MMI_SP.Insurance.Delivery
{
    internal static class TrackVehicleState
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        private static string NearKey(int baseKey) => $"near_arrival_{baseKey}";

        internal static void Execute(List<Data> incomingVehicles)
        {
            for (int i = incomingVehicles.Count - 1; i >= 0; i--)
            {
                Data incoming = incomingVehicles[i];
                int key = incoming.calledTime;
                string nearKey = NearKey(key);

                if (Game.Player.Character.IsInVehicle(incoming.vehicle))
                {
                    if (incoming.driver != null && incoming.driver.Exists()) {
                        incoming.driver.Task.ClearAllImmediately();
                        incoming.driver.IsPersistent = false;
                        incoming.driver.MarkAsNoLongerNeeded();
                    }
                    incomingVehicles.RemoveAt(i);
                    Timers.RemoveCustomTimer(nearKey);
                    continue;
                }

                if (incoming.vehicle.IsDead) {
                    Manager.CannotBringVehicle(incoming, Calculator.GetCost(incoming.vehicle));
                    Timers.RemoveCustomTimer(nearKey);
                    continue;
                }

                if (HasDriverArrived(incoming)) {
                    ForceLeaveVehicle(incoming, incomingVehicles, i);
                    Timers.RemoveCustomTimer(nearKey);
                    continue;
                }

                float distanceToDest = incoming.driver.Position.DistanceTo(incoming.destination);
                float distanceToPlayer = incoming.driver.Position.DistanceTo(Game.Player.Character.Position);
                bool isNear = distanceToDest <= 80f || distanceToPlayer <= 80f;
                bool isStopped = incoming.vehicle.Speed <= 1.0f && incoming.driver.IsInVehicle(incoming.vehicle);

                if (isNear && isStopped)
                {
                    if (!Timers.IsCustomTimerExpired(nearKey)) { Timers.StartCustomTimer(nearKey, 3000); 
                    } 
                    else {
                        ForceLeaveVehicle(incoming, incomingVehicles, i);
                        Timers.RemoveCustomTimer(nearKey);
                        continue;
                    }
                }
                else {
                    Timers.RemoveCustomTimer(nearKey);
                }

                if (Game.GameTime - incoming.calledTime > (ModSettings.BringVehicleTimeout * 60000)) {
                    Manager.CannotBringVehicle(incoming);
                    Timers.RemoveCustomTimer(nearKey);
                }
            }
        }

        private static bool HasDriverArrived(Data incoming)
        {
            if (!incoming.driver.IsInVehicle(incoming.vehicle)) return false;
            return IsNearDestination(incoming);
        }

        private static bool IsNearDestination(Data incoming)
        {
            float distanceToDest = incoming.driver.Position.DistanceTo(incoming.destination);
            float altitudeDiffDest = incoming.driver.Position.Z - incoming.destination.Z;
            float distanceToPlayer = incoming.driver.Position.DistanceTo(Game.Player.Character.Position);
            float altitudeDiffPlayer = incoming.driver.Position.Z - Game.Player.Character.Position.Z;
            return (distanceToDest <= 15.0f && altitudeDiffDest <= 2.0f) || (distanceToPlayer <= 15.0f && altitudeDiffPlayer <= 2.0f);
        }

        private static void ForceLeaveVehicle(Data incoming, List<Data> incomingVehicles, int index)
        {
            incoming.driver.Task.LeaveVehicle();
            Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, incoming.driver, true);
            Function.Call(Hash.RESET_PED_LAST_VEHICLE, incoming.driver);
            RemoveDriver(incoming, incomingVehicles, index);
        }

        private static void RemoveDriver(Data incoming, List<Data> incomingVehicles, int index)
        {
            incoming.driver.IsPersistent = false;
            incoming.driver.MarkAsNoLongerNeeded();
            incoming.driver.Task.Wander();
            Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, incoming.driver, true);
            incomingVehicles.RemoveAt(index);
        }
    }
}