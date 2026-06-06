using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MMI_SP.Config;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using Newtonsoft.Json;

namespace MMI_SP.DB
{
    internal static class Queries
    {
        // ==========================================
        // BLOQUE 1: Datos privados
        // ==========================================
        private static List<VehicleData> Vehicles = new List<VehicleData>();
        private static string DatabasePath => Path.Combine(ModSettings.BaseDir, "db.json");

        // ==========================================
        // BLOQUE 2: Persistencia privada
        // ==========================================
        private static Result<List<VehicleData>> LoadFromDisk()
        {
            try {
                if (File.Exists(DatabasePath)) {
                    string json = File.ReadAllText(DatabasePath);
                    var list = JsonConvert.DeserializeObject<List<VehicleData>>(json);
                    return new Ok<List<VehicleData>>(list ?? new List<VehicleData>());
                }
                return new Ok<List<VehicleData>>(new List<VehicleData>());
            }
            catch (Exception ex) {
                return new Err<List<VehicleData>>(ex.Message);
            }
        }

        private static Result<bool> SaveToDisk()
        {
            try {
                string json = JsonConvert.SerializeObject(Vehicles, Formatting.Indented);
                File.WriteAllText(DatabasePath, json);
                Logger.Debug("Insured database saved.");
                return new Ok<bool>(true);
            }
            catch (Exception ex) {
                return new Err<bool>(ex.Message);
            }
        }

        // ==========================================
        // BLOQUE 3: API pública interna
        // ==========================================
        internal static Result<bool> Load()
        {
            return LoadFromDisk().match<Result<bool>>(
                onOk: data => {
                    Vehicles = data;
                    return new Ok<bool>(true);
                },
                onErr: error => new Err<bool>(error)
            );
        }

        internal static Result<bool> Save() => SaveToDisk();

        internal static Option<VehicleData> FindVehicle(string vehicleId) {
            var vehicle = Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            return Option.from_nullable(vehicle);
        }

        internal static Result<bool> Add(VehicleData data)
        {
            if (Vehicles.Any(v => v.ModelName == data.ModelName && v.Plate == data.Plate))
                return new Err<bool>("Ya existe un vehículo asegurado con este modelo y placa.");

            Vehicles.Add(data);
            return Save();
        }

        internal static Result<bool> Remove(string vehicleId) {
            var vehicle = Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null) return new Err<bool>("Vehículo no encontrado.");
            Vehicles.Remove(vehicle);
            return Save();
        }

        internal static Result<bool> Update(VehicleData updatedData)
        {
            var existing = Vehicles.FirstOrDefault(v => v.Id == updatedData.Id);
            if (existing == null) return new Err<bool>("Vehículo no encontrado.");
            Vehicles.Remove(existing);
            Vehicles.Add(updatedData);
            return Save();
        }

        internal static Result<bool> SetDormant(string vehicleId, bool dormant)
        {
            var updated = Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (updated == null) return new Err<bool>("Vehículo no encontrado.");
            var data = updated.With(d => d.IsDormant = dormant);
            Vehicles.Remove(updated);
            Vehicles.Add(data);
            return Save();
        }

        internal static List<VehicleData> GetAll() => new List<VehicleData>(Vehicles);

        internal static List<string> GetInsuredIds() => Vehicles.Where(v => !v.IsDestroyed).Select(v => v.Id).ToList();

        internal static List<string> GetActiveInsuredIds() => Vehicles.Where(v => !v.IsDestroyed && !v.IsDormant).Select(v => v.Id).ToList();

        internal static List<string> GetDestroyedIds()
            => Vehicles.Where(v => v.IsDestroyed)
                .OrderByDescending(v => v.DestroyedAt)
                .Take(Constants.MAX_CLAIMABLE_VEHICLES)
                .Select(v => v.Id).ToList();
    }
}