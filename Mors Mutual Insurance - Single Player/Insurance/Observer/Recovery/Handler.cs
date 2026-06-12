using GTA;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer.Recovery
{
    public static class Handler
    {
        public static void OnVehicleRecovered(Vehicle veh, Blip blip, List<Vehicle> recoveredVehList, Dictionary<string, Blip> blipsToRemove)
            => VehicleRegister.RegisterRecoveredVehicle(veh, blip, recoveredVehList, blipsToRemove);

        public static void UpdateRecoveredVehicles(List<Vehicle> recoveredVehList) => VehicleRegister.UpdateRecoveredVehicles(recoveredVehList);

        public static void RemoveRecoveryKey(Vehicle playerVeh) => KeyManager.RemoveRecoveryKey(playerVeh);

        public static bool IsRecoveredAndNotDriven(Vehicle veh) => KeyManager.IsRecoveredAndNotDriven(veh);

        public static void CleanupOrphanedKeys() => KeyManager.CleanupOrphanedKeys();
    }
}