using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Policies
{
    internal class Insurer
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static Insurer Instance { get; private set; }
        internal Insurer() { Instance = this; }

        // ==========================================
        // BLOQUE 2: Validaciones y asegurabilidad
        // ==========================================
        internal static bool IsInsurable(Vehicle veh)
        {
            if (veh == null || !veh.IsAlive) return false;
            return !veh.Model.IsTrain &&
                   !veh.Model.IsHelicopter &&
                   !veh.Model.IsPlane &&
                   !veh.Model.IsBoat;
        }

        internal bool IsInsured(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;

            // Buscar por ID único
            string id = VehicleIdentifier.Get(veh);
            if (DB.Core.FindVehicle(id).is_some()) return true;

            // Buscar por modelo + placa (vehículos sin ID por si acaso)
            string modelName = veh.Model.ToString();
            string plate = veh.Mods.LicensePlate;
            return DB.Core.GetAll().Any(v => v.Plate == plate && v.ModelName == modelName && !v.IsDestroyed);
        }

        // ==========================================
        // BLOQUE 3: Operaciones de negocio
        // ==========================================
        internal Result<bool> Insure(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");

            if (DB.Core.GetAll().Count >= Constants.MAX_INSURED_VEHICLES) return new Err<bool>("Has alcanzado el límite de vehículos asegurables.");

            string id = VehicleIdentifier.Get(veh);
            if (DB.Core.FindVehicle(id).is_some()) return new Err<bool>("El vehículo ya está asegurado.");

            return DB.VehicleDataBuilder.CreateFrom(veh, id).and_then<bool>(data => DB.Core.Add(data));
        }

        internal Result<bool> Cancel(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");
            return DB.Core.Remove(vehicleId);
        }

        internal Result<bool> MarkAsDestroyed(string vehicleId) => InsurerOperations.MarkAsDestroyed(vehicleId);

        internal Result<DB.VehicleData> RecoverVehicle(string vehicleId) => InsurerOperations.RecoverVehicle(vehicleId);

        internal Result<bool> UpdateVehicleData(Vehicle veh) => InsurerOperations.UpdateVehicleData(veh);

        // ==========================================
        // BLOQUE 4: Consultas
        // ==========================================
        internal List<string> GetInsuredList() => DB.Core.GetInsuredIds();
        internal List<string> GetDestroyedList() => DB.Core.GetDestroyedIds();
        internal List<DB.VehicleData> GetInsuredListFull() => DB.Core.GetAll();
    }
}