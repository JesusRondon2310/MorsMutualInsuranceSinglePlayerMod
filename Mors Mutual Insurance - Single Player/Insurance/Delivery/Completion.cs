using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Spawn.Coordinates;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Delivery
{
    internal static class Completion
    {
        internal static void ForceIfNear(List<Data> incomingVehicles)
        {
            for (int i = incomingVehicles.Count - 1; i >= 0; i--)
            {
                Data t = incomingVehicles[i];
                if (t?.vehicle == null || !t.vehicle.Exists() || t.driver == null || !t.driver.Exists()) continue;

                if (Game.Player.Character.IsInVehicle(t.vehicle))
                {
                    t.driver.Task.ClearAllImmediately();
                    t.driver.IsPersistent = false;
                    t.driver.MarkAsNoLongerNeeded();
                    incomingVehicles.RemoveAt(i);
                    continue;
                }

                if (!t.driver.IsInVehicle(t.vehicle)) continue;

                float distance = t.vehicle.Position.DistanceTo(t.destination);
                if (distance <= Constants.ARRIVAL_DISTANCE && t.vehicle.Speed < Constants.ARRIVAL_SPEED_THRESHOLD)
                {
                    // Frenar el vehículo para evitar inercia (mejora añadida, no estaba en original pero ayuda)
                    t.vehicle.Velocity = Vector3.Zero;
                    t.vehicle.WorldRotationVelocity = Vector3.Zero;
                    Function.Call(Hash.SET_VEHICLE_FORWARD_SPEED, t.vehicle, 0f);

                    t.driver.Task.LeaveVehicle();
                    Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, t.driver, true);
                    Function.Call(Hash.SET_VEHICLE_DOOR_SHUT, t.vehicle, Constants.ZERO, false);
                    t.vehicle.LockStatus = VehicleLockStatus.CannotEnter;

                    t.driver.IsPersistent = false;
                    t.driver.MarkAsNoLongerNeeded();
                    incomingVehicles.RemoveAt(i);
                }
            }
        }

        // ==========================================
        // BLOQUE: Fallo en la entrega
        // ==========================================
        internal static void Execute(Data incoming, int refund, List<Data> incomingVehicles)
        {
            NotifyFailure();
            Refund(incoming, refund);
            incoming.driver.Delete();
            RepairIfAlive(incoming);
            incomingVehicles.Remove(incoming);
        }

        private static void NotifyFailure() => Notification.ShowMechanic("Información", "No se pudo traer el vehículo.");

        private static void Refund(Data incoming, int refund)
        {
            Game.Player.Money += (refund == Constants.NONE) ? incoming.price : refund + incoming.price;
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
                EntityPosition pos = SpawnHandler.GetPlayerReferencePosition();
                incoming.vehicle.Position = pos.Position;
                incoming.vehicle.Heading = pos.Heading;
            }

            incoming.vehicle.IsEngineRunning = false;
            incoming.vehicle.Repair();
        }
    }
}