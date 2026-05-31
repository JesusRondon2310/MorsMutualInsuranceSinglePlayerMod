using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using MMI_SP.Config;

namespace MMI_SP.Helpers
{
    public static class VehiclePersistence
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void SetPersistence(Vehicle veh, bool persistent)
        {
            if (!ModSettings.PersistentVehicles) return;
            if (!persistent) return;
            Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, veh, true, false);
        }

        public static void RemovePersistence(List<Vehicle> recoveredVehList)
        {
            if (ModSettings.PersistentVehicles) return;
            for (int i = recoveredVehList.Count - 1; i >= 0; i--)
                recoveredVehList.ElementAt(i).IsPersistent = false;
        }
    }
}