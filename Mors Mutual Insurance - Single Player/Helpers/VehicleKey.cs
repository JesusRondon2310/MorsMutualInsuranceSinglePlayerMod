using MMI_SP.DB;

namespace MMI_SP.Helpers
{
    public static class VehicleKey
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static string FullKeyFrom(VehicleData data) => $"{data.ModelName}_{data.Plate}_{data.Id}";

        public static string ModelPlateKeyFrom(VehicleData data) => $"{data.ModelName}_{data.Plate}";
    }
}