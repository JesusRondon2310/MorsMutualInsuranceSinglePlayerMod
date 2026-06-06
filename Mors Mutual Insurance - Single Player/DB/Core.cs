using GTA;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.DB
{
    public static class Core
    {
        // ==========================================
        // BLOQUE: API pública (delegación)
        // ==========================================
        public static Result<bool> Load() => Queries.Load();

        internal static Result<bool> Save() => Queries.Save();

        public static Option<VehicleData> FindVehicle(string vehicleId)
            => Queries.FindVehicle(vehicleId);

        public static Result<bool> Add(VehicleData data)
            => Queries.Add(data);

        public static Result<bool> Remove(string vehicleId)
            => Queries.Remove(vehicleId);

        public static Result<bool> Update(VehicleData updatedData)
            => Queries.Update(updatedData);

        public static Result<bool> SetDormant(string vehicleId, bool dormant)
            => Queries.SetDormant(vehicleId, dormant);

        internal static List<VehicleData> GetAll()
            => Queries.GetAll();

        internal static List<string> GetInsuredIds()
            => Queries.GetInsuredIds();

        internal static List<string> GetActiveInsuredIds()
            => Queries.GetActiveInsuredIds();

        internal static List<string> GetDestroyedIds()
            => Queries.GetDestroyedIds();
    }
}