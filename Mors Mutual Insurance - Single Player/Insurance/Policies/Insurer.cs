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

        internal Insurer()
        {
            Instance = this;
        }

        // ==========================================
        // BLOQUE 2: Inicialización
        // ==========================================
        internal void LoadFrom(List<DB.VehicleData> list)
        {
            DB.Core.Vehicles.Clear();
            DB.Core.Vehicles.AddRange(list);
        }

        // ==========================================
        // BLOQUE 3: Validaciones y asegurabilidad
        // ==========================================
        public static bool IsInsurable(Vehicle veh)
        {
            if (veh == null || !veh.IsAlive) return false;
            return !veh.Model.IsTrain &&
                   !veh.Model.IsHelicopter &&
                   !veh.Model.IsPlane &&
                   !veh.Model.IsBoat;
        }

        internal bool IsInsured(Vehicle veh) => DB.Core.IsInsured(veh);

        // ==========================================
        // BLOQUE 4: Operaciones de negocio
        // ==========================================
        internal Result<bool> Insure(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");
            if (DB.Core.Vehicles.Count >= 30) return new Err<bool>("Has alcanzado el límite de vehículos asegurables.");

            string id = VehicleIdentifier.Get(veh);
            if (DB.Core.Vehicles.Any(v => v.Id == id)) return new Err<bool>("El vehículo ya está asegurado.");

            return DB.VehicleDataBuilder.CreateFrom(veh, id)
                .and_then<bool>(data => DB.Core.Add(data));
        }

        internal Result<bool> Cancel(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<bool>("ID de vehículo no válido.");
            return DB.Core.Remove(vehicleId);
        }

        internal Result<bool> MarkAsDestroyed(string vehicleId)
            => InsurerOperations.MarkAsDestroyed(vehicleId);

        internal Result<DB.VehicleData> RecoverVehicle(string vehicleId)
            => InsurerOperations.RecoverVehicle(vehicleId);

        internal Result<bool> UpdateVehicleData(Vehicle veh)
            => InsurerOperations.UpdateVehicleData(veh);

        // ==========================================
        // BLOQUE 5: Consultas
        // ==========================================
        internal List<string> GetInsuredList() => DB.Core.GetInsuredIds();
        internal List<string> GetDestroyedList() => DB.Core.GetDestroyedIds();
        internal List<DB.VehicleData> GetInsuredListFull() => DB.Core.GetAll();
    }
}