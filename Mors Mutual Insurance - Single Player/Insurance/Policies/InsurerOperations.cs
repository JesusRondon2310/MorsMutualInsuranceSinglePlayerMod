using GTA;
using MMI_SP.DB;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System;
using System.Linq;

namespace MMI_SP.Insurance.Policies
{
    internal static class InsurerOperations
    {
        // ==========================================
        // BLOQUE: Métodos privados auxiliares
        // ==========================================
        private static void TrimDestroyedVehicles()
        {
            var destroyed = DB.Core.GetAll()
                .Where(v => v.IsDestroyed)
                .OrderBy(v => v.DestroyedAt)
                .ToList();

            if (destroyed.Count <= Constants.MAX_CLAIMABLE_VEHICLES) return;

            var toRemove = destroyed.Take(destroyed.Count - Constants.MAX_CLAIMABLE_VEHICLES);
            foreach (var v in toRemove) DB.Core.Remove(v.Id);
        }

        // ==========================================
        // BLOQUE: Operaciones públicas
        // ==========================================
        internal static Result<bool> MarkAsDestroyed(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");

            return DB.Core.FindVehicle(vehicleId).match<Result<bool>>(
                onSome: vehicle => {
                    var updated = vehicle.With(d => {
                        d.IsDestroyed = true;
                        d.DestroyedAt = DateTime.UtcNow;
                        d.IsLocked = false;
                    });

                    var updateResult = DB.Core.Update(updated);
                    if (updateResult.is_err()) return updateResult;

                    TrimDestroyedVehicles();
                    return DB.Core.Save();
                },
                onNone: () => new Err<bool>("Vehículo no encontrado.")
            );
        }

        internal static Result<DB.VehicleData> RecoverVehicle(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId))
                return new Err<DB.VehicleData>("ID de vehículo no válido.");

            return DB.Core.FindVehicle(vehicleId).match<Result<DB.VehicleData>>(
                onSome: vehicle => {
                    if (!vehicle.IsDestroyed) return new Err<DB.VehicleData>("El vehículo no está destruido.");
                    return new Ok<DB.VehicleData>(vehicle);
                },
                onNone: () => new Err<DB.VehicleData>("Vehículo no encontrado.")
            );
        }

        internal static Result<bool> UpdateVehicleData(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");

            string id = VehicleIdentifier.Get(veh);
            var existingOption = DB.Core.FindVehicle(id);
            if (existingOption.is_none()) return new Err<bool>("Vehículo no encontrado.");

            VehicleData existing = ((Some<VehicleData>)existingOption).Value;

            var freshResult = DB.VehicleDataBuilder.CreateFrom(veh, id);
            if (freshResult.is_err()) return new Err<bool>(((Err<VehicleData>)freshResult).Message);

            VehicleData freshData = ((Ok<VehicleData>)freshResult).Value;

            var merged = freshData.With(d =>
            {
                d.IsDestroyed = existing.IsDestroyed;
                d.IsLocked = existing.IsLocked;
                d.IsDormant = existing.IsDormant;
                d.IsInGarage = existing.IsInGarage;
                d.DestroyedAt = existing.DestroyedAt;
                d.VehicleType = existing.VehicleType;
            });

            return DB.Core.Update(merged);
        }
    }
}