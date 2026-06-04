using MMI_SP.PatternMatching;
using System.Linq;

namespace MMI_SP.DB
{
    internal static class InsuredVehiclesData
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Option<VehicleData> FindVehicle(string vehicleId)
        {
            var vehicle = Core.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            return Option.from_nullable(vehicle);
        }
    }
}