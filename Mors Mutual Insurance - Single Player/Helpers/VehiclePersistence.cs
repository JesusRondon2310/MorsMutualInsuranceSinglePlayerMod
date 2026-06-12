using GTA;
using GTA.Native;
using MMI_SP.Config;
using System.Collections.Generic;
using System.Linq;

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
         veh.IsPersistent = persistent;
      }

      public static void RemovePersistence(List<Vehicle> recoveredVehList)
      {
         if (ModSettings.PersistentVehicles) return;
         for (int i = recoveredVehList.Count - 1; i >= 0; i--) recoveredVehList.ElementAt(i).IsPersistent = false;
      }
   }
}