using GTA;
using MMI_SP.DB;
using MMI_SP.Helpers;
using MMI_SP.Helpers.Blips;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal class Garage
    {
        internal static bool Entry(Vehicle currentVeh, Dictionary<string, Blip> blipsToRemove)
        {
            bool inGarage = VehiclesInGarage.IsDefaultGarage(currentVeh) || VehiclesInGarage.IsAnInteriorGarage(currentVeh);
            if (!inGarage) return false;

            string vehId = VehicleIdentifier.Get(currentVeh);
            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) return false;
            var data = ((Some<VehicleData>)dataOption).Value;

            // Actualizar BD siempre (IsInGarage = true)
            var updatedData = data.With(d => {
                d.IsInGarage = true;
                d.IsLocked = false;
                d.IsDormant = false;
            });
            DB.Core.Update(updatedData);

            if (VehiclesInGarage.IsAnInteriorGarage(currentVeh))
            {
                // Restaurar todos los vehículos del garaje (spawnear)
                Helpers.Spawn.InteriorVehicleRestorer.OnEntry(currentVeh, Manager.InsuredVehList, blipsToRemove);
                return true;
            }

            // Garajes nativos
            VehiclePersistence.SetPersistence(currentVeh, false);
            BlipCleanupHandler.RemoveByVehicle(currentVeh, blipsToRemove);
            return true;
        }

        internal static bool Exit(Vehicle currentVeh)
        {
            // Si el vehículo todavía está dentro de un garaje, no salir
            if (VehiclesInGarage.IsDefaultGarage(currentVeh) || VehiclesInGarage.IsAnInteriorGarage(currentVeh))
                return false;

            string vehId = VehicleIdentifier.Get(currentVeh);
            var dataOption = DB.Core.FindVehicle(vehId);
            if (dataOption.is_none()) return false;
            var data = ((Some<VehicleData>)dataOption).Value;

            if (data.IsInGarage)
            {
                // El vehículo pertenecía a un garaje interior: manejar salida con vehículo
                Helpers.Spawn.InteriorVehicleRestorer.OnExitVehicle(currentVeh, Manager.InsuredVehList, Manager.BlipsToRemove);
            }
            else
            {
                // Garaje nativo: solo actualizar flag y persistencia
                var updatedData = data.With(d => d.IsInGarage = false);
                DB.Core.Update(updatedData);
                VehiclePersistence.SetPersistence(currentVeh, true);
            }
            return true;
        }
    }
}