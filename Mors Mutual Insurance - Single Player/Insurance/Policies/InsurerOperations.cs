using System;
using System.Linq;
using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Policies
{
    internal static class InsurerOperations
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<bool> MarkAsDestroyed(string vehicleId)
        {
            var vehicleOption = DB.InsuredVehiclesData.FindVehicle(vehicleId);
            if (vehicleOption is None<DB.VehicleData>) return new Err<bool>("Vehículo no encontrado.");

            DB.VehicleData vehicle = ((Some<DB.VehicleData>)vehicleOption).Value;

            var updated = vehicle.With(d => {
                d.IsDestroyed = true;
                d.DestroyedAt = DateTime.UtcNow;
                d.IsLocked = false;
            });

            DB.Core.Vehicles.Remove(vehicle);
            DB.Core.Vehicles.Add(updated);

            var destroyed = DB.Core.Vehicles.Where(v => v.IsDestroyed).OrderBy(v => v.DestroyedAt).ToList();
            if (destroyed.Count > 5)
            {
                var oldest = destroyed.First();
                DB.Core.Vehicles.Remove(oldest);
            }

            return DB.JSONHandler.Save(DB.Core.Vehicles);
        }

        internal static Result<DB.VehicleData> RecoverVehicle(string vehicleId)
        {
            var vehicleOption = DB.InsuredVehiclesData.FindVehicle(vehicleId);
            if (vehicleOption is None<DB.VehicleData>) return new Err<DB.VehicleData>("Vehículo no encontrado.");

            DB.VehicleData vehicle = ((Some<DB.VehicleData>)vehicleOption).Value;
            if (!vehicle.IsDestroyed) return new Err<DB.VehicleData>("El vehículo no está destruido.");

            return new Ok<DB.VehicleData>(vehicle);
        }

        internal static Result<bool> UpdateVehicleData(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");
            string id = VehicleIdentifier.Get(veh);

            var existingOption = DB.InsuredVehiclesData.FindVehicle(id);
            if (existingOption is None<DB.VehicleData>) return new Err<bool>("Vehículo no encontrado.");
            DB.VehicleData existing = ((Some<DB.VehicleData>)existingOption).Value;

            return DB.VehicleDataBuilder.CreateFrom(veh, id)
                .and_then<bool>(updated =>
                {
                    var preserved = updated.With(d => {
                        d.IsDestroyed = existing.IsDestroyed;
                        d.IsLocked = existing.IsLocked;
                        d.IsDormant = existing.IsDormant;
                        d.IsInGarage = existing.IsInGarage;
                        d.DestroyedAt = existing.DestroyedAt;
                        d.VehicleType = existing.VehicleType;
                    });

                    DB.Core.Vehicles.Remove(existing);
                    DB.Core.Vehicles.Add(preserved);
                    return DB.JSONHandler.Save(DB.Core.Vehicles);
                });
        }
    }
}