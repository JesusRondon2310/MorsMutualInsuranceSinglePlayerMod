using GTA;
using GTA.Native;
using MMI_SP.Insurance.Delivery.Incoming;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Delivery
{
    internal static class Arrival
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void ForceIfNear(List<Data> incomingVehicles)
        {
            for (int i = incomingVehicles.Count - 1; i >= 0; i--)
            {
                Data t = incomingVehicles[i];
                if (t?.vehicle == null || !t.vehicle.Exists() || t.driver == null || !t.driver.Exists()) continue;

                // Fix: si el jugador ya está en el vehículo, limpiar el tracking.
                if (Game.Player.Character.IsInVehicle(t.vehicle))
                {
                    if (t.driver != null && t.driver.Exists())
                    {
                        t.driver.Task.ClearAllImmediately();
                        t.driver.IsPersistent = false;
                        t.driver.MarkAsNoLongerNeeded();
                    }
                    incomingVehicles.RemoveAt(i);
                    continue;
                }

                if (!t.driver.IsInVehicle(t.vehicle)) continue;

                float distance = t.vehicle.Position.DistanceTo(t.destination);
                if (distance <= 40.0f && t.vehicle.Speed < 3.0f)
                {
                    t.driver.Task.LeaveVehicle();
                    Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, t.driver, true);
                    Function.Call(Hash.SET_VEHICLE_DOOR_SHUT, t.vehicle, 0, false);
                    t.vehicle.LockStatus = VehicleLockStatus.CannotEnter;
                    t.driver.IsPersistent = false;
                    t.driver.MarkAsNoLongerNeeded();
                    incomingVehicles.RemoveAt(i);
                }
            }
        }
    }
}