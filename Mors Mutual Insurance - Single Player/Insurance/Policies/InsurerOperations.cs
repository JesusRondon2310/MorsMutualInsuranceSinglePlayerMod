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

            var updated = new DB.VehicleData(
                vehicle.Id, vehicle.ModelName, vehicle.Plate,
                vehicle.PrimaryColor, vehicle.SecondaryColor, true,
                windowTint: vehicle.WindowTint,
                wheelType: vehicle.WheelType,
                wheelColor: vehicle.WheelColor,
                tireSmokeColor: vehicle.TireSmokeColor,
                neonLeft: vehicle.NeonLeft,
                neonRight: vehicle.NeonRight,
                neonFront: vehicle.NeonFront,
                neonBack: vehicle.NeonBack,
                neonColor: vehicle.NeonColor,
                bulletproofTires: vehicle.BulletproofTires,
                posX: vehicle.PosX, posY: vehicle.PosY, posZ: vehicle.PosZ, heading: vehicle.Heading,
                mods: vehicle.Mods,
                destroyedAt: DateTime.UtcNow,
                isLocked: false,
                plateStyle: vehicle.PlateStyle,
                customTires: vehicle.CustomTires,
                isDormant: vehicle.IsDormant,
                isInGarage: vehicle.IsInGarage,
                vehicleType: vehicle.VehicleType);

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
                    var preserved = new DB.VehicleData(
                        updated.Id, updated.ModelName, updated.Plate,
                        updated.PrimaryColor, updated.SecondaryColor, existing.IsDestroyed,
                        windowTint: updated.WindowTint,
                        wheelType: updated.WheelType,
                        wheelColor: updated.WheelColor,
                        tireSmokeColor: updated.TireSmokeColor,
                        neonLeft: updated.NeonLeft,
                        neonRight: updated.NeonRight,
                        neonFront: updated.NeonFront,
                        neonBack: updated.NeonBack,
                        neonColor: updated.NeonColor,
                        bulletproofTires: updated.BulletproofTires,
                        posX: updated.PosX, posY: updated.PosY, posZ: updated.PosZ, heading: updated.Heading,
                        mods: updated.Mods,
                        destroyedAt: existing.DestroyedAt,
                        isLocked: existing.IsLocked,
                        plateStyle: updated.PlateStyle,
                        customTires: updated.CustomTires,
                        isDormant: existing.IsDormant,
                        isInGarage: existing.IsInGarage,
                        vehicleType: existing.VehicleType);

                    DB.Core.Vehicles.Remove(existing);
                    DB.Core.Vehicles.Add(preserved);
                    return DB.JSONHandler.Save(DB.Core.Vehicles);
                });
        }
    }
}