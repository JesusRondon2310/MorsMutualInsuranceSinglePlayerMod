using GTA;
using GTA.Math;
using GTA.Native;
using System.Linq;

namespace MMI_SP.Helpers
{
    public static class VehicleRecoveryHelper
    {
        public static void RecoverLostVehicles()
        {
            // Vehículos asegurados que deberían estar en el mundo
            var lostVehicles = DB.Core.GetAll()
                .Where(v => !v.IsDestroyed && !v.IsDormant && !v.IsInGarage)
                .ToList();

            if (lostVehicles.Count == 0)
            {
                Notification.ShowiFruit("Sin novedades", "No se encontraron vehículos perdidos.");
                return;
            }

            int recoveredCount = 0;
            int impoundedCount = 0;

            foreach (var vd in lostVehicles)
            {
                var matches = World.GetAllVehicles()
                    .Where(v => v.Mods.LicensePlate == vd.Plate && v.Model == new Model(vd.ModelName))
                    .ToList();

                if (matches.Count == 0)
                {
                    Dormancy.Core.MarkAsDormant(vd.Id);
                    recoveredCount++;
                    continue;
                }

                int inImpound = matches.Count(v => VehiclesInGarage.IsPositionInPoliceImpound(v.Position));
                if (inImpound > 0)
                    impoundedCount++;
            }

            if (impoundedCount > 0)
                Notification.ShowiFruit("Depósito policial", $"Tienes {impoundedCount} vehículo(s) en el depósito. Pagá la multa, moroso.");
            if (recoveredCount > 0)
                Notification.ShowiFruit("Vehículos recuperados", $"Se marcaron {recoveredCount} vehículo(s) perdido(s) como dormantes. Llama al mecánico.");
            if (impoundedCount == 0 && recoveredCount == 0)
                Notification.ShowiFruit("Todo en orden", "¿Tenías ganas de presionar el botón o qué?");
        }

        // Única función para limpiar duplicados en el depósito policial (se activa con el botón)
        public static void CleanDuplicatesInImpound()
        {
            Vector3 impoundCenter = VehiclesInGarage.PoliceImpoundPositions[0];
            float radius = VehiclesInGarage.PoliceImpoundRadius;

            int deleted = 0;
            var vehiclesInArea = World.GetAllVehicles()
                .Where(v => v.Position.DistanceTo(impoundCenter) <= radius)
                .ToList();

            foreach (var v in vehiclesInArea)
            {
                // Obtener el nombre interno del modelo (igual que en bug #2)
                string modelName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, v.Model.Hash);
                string plate = v.Mods.LicensePlate;

                // Buscar en la BD por modelo y placa
                var dataOption = DB.Core.GetAll().FirstOrDefault(d => d.ModelName == modelName && d.Plate == plate);

                if (dataOption != null)
                {
                    v.Delete();
                    deleted++;
                }
            }

            if (deleted > 0)
                Notification.ShowiFruit("Depósito limpiado", $"Se eliminaron {deleted} vehículo(s) duplicado(s).");
            else
                Notification.ShowiFruit("Sin duplicados", "No se encontraron vehículos duplicados en el depósito.");
        }
    }
}