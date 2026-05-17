using System.Collections.Generic;
using System.Linq;
using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Observer
{
    internal static class BlipManager
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void RemoveRecoverBlip(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            string key = VehicleIdentifier.Get(veh);
            blipsToRemove.TryGetValue(key, out Blip vehicleBlip);
            if (vehicleBlip != null)
            {
                vehicleBlip.Delete();
                blipsToRemove.Remove(key);
            }
        }

        internal static void ClearAllBlips(Dictionary<string, Blip> blipsToRemove)
        {
            for (int i = blipsToRemove.Count - 1; i >= 0; i--)
            {
                Blip toDel = blipsToRemove.ElementAt(i).Value;
                if (toDel != null && toDel.Exists()) toDel.Delete();
            }
        }

        internal static Result<Blip> AddVehicleBlip(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<Blip>("El vehículo no existe.");

            Blip blip = veh.AddBlip();
            if (blip == null || !blip.Exists()) return new Err<Blip>("No se pudo crear el blip del vehículo.");

            blip.Sprite = BlipSprite.PersonalVehicleCar;
            blip.Name = "Vehículo asegurado";
            blip.IsShortRange = false;
            return new Ok<Blip>(blip);
        }
    }
}
