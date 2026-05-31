using MMI_SP.DB;

namespace MMI_SP.Helpers
{
    public static class VehicleKey
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static string From(VehicleData data)
        {
            return $"{data.ModelName}_{data.Plate}";
        }
    }
}