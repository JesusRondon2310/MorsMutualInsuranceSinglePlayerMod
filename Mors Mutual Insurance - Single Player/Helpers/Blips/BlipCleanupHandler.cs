using System.Collections.Generic;
using System.Linq;
using GTA;

namespace MMI_SP.Helpers.Blips
{
    public static class BlipCleanupHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void RemoveByVehicle(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            if (veh == null) return;
            string key = VehicleIdentifier.Get(veh);
            RemoveByKey(blipsToRemove, key);
        }

        public static void RemoveByKey(Dictionary<string, Blip> blipsToRemove, string key)
        {
            if (blipsToRemove == null) return;
            blipsToRemove.TryGetValue(key, out Blip vehicleBlip);
            if (vehicleBlip != null)
            {
                vehicleBlip.Delete();
                blipsToRemove.Remove(key);
            }
        }

        public static void ClearAll(Dictionary<string, Blip> blipsToRemove)
        {
            if (blipsToRemove == null) return;
            for (int i = blipsToRemove.Count - 1; i >= 0; i--)
            {
                Blip toDel = blipsToRemove.ElementAt(i).Value;
                if (toDel != null && toDel.Exists()) toDel.Delete();
            }
        }
    }
}