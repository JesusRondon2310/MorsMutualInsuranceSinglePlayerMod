using GTA;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using MMI_SP.Helpers;

namespace MMI_SP.Insurance.Observer.LockVehicle
{
    internal static class StateChanger
    {
        internal static Vehicle FindNearestInsuredVehicle(Ped player, List<Vehicle> insuredVehList)
        {
            Vehicle nearest = null;
            float minDist = Constants.LOCK_MAX_DISTANCE;

            foreach (Vehicle veh in insuredVehList)
            {
                if (veh == null || !veh.Exists() || veh.IsDead) continue;
                float dist = player.Position.DistanceTo(veh.Position);
                if (dist < minDist) {
                    minDist = dist;
                    nearest = veh;
                }
            }
            return nearest;
        }

        internal static bool IsVehicleLocked(string vehicleId)
        {
            return DB.Core.FindVehicle(vehicleId).match(
                onSome: data => data.IsLocked,
                onNone: () => false
            );
        }
        internal static void ToggleLock(Vehicle vehicle, string vehicleId)
        {
            var dataOption = DB.Core.FindVehicle(vehicleId);
            bool currentlyLocked = dataOption.match(
                onSome: data => data.IsLocked,
                onNone: () => false
            );
            bool willBeLocked = !currentlyLocked;

            vehicle.LockStatus = willBeLocked ? VehicleLockStatus.CannotEnter : VehicleLockStatus.Unlocked;
            if (willBeLocked) vehicle.IsAlarmSet = true;

            DataPersistence.Persist(vehicleId, willBeLocked);
        }

    }
}