using GTA;
using MMI_SP.Debug;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Helpers.Blips
{
    public static class RecoveryBlipHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void CreateFlashingBlip(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            veh.AttachedBlip?.Delete();
            VehicleBlipHandler.Create(veh, flashing: true).match<bool>(
                onOk: blip =>
                {
                    string key = VehicleIdentifier.Get(veh);
                    blipsToRemove[key] = blip;
                    return true;
                },
                onErr: error =>
                {
                    Logger.Error($"Error al crear blip titilante: {error}");
                    return false;
                }
            );
        }

        public static void RemoveBlip(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            BlipCleanupHandler.RemoveByVehicle(veh, blipsToRemove);
        }

        public static void RestoreBlip(Vehicle vehicle, Dictionary<string, Blip> blipsToRemove)
        {
            string vehId = VehicleIdentifier.Get(vehicle);
            // Forzar eliminación del blip anterior (titilante o normal) del diccionario
            if (blipsToRemove.TryGetValue(vehId, out Blip oldBlip) && oldBlip != null && oldBlip.Exists()) oldBlip.Delete();
            blipsToRemove.Remove(vehId);

            // Crear un blip nuevo sin flashing
            VehicleBlipHandler.Create(vehicle).match<bool>(
                onOk: blip =>
                {
                    blipsToRemove[vehId] = blip;
                    return true;
                },
                onErr: error =>
                {
                    Logger.Error($"Error al restaurar blip del vehículo: {error}");
                    return false;
                }
            );
        }
    }
}